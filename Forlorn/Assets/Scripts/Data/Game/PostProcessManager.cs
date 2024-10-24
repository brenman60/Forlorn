using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance { get; private set; }

    [SerializeField] private List<ScenePostProcess> sceneSpecificVolumes = new List<ScenePostProcess>();

    private List<VolumeProfile> volumesInLine = new List<VolumeProfile>();

    private Volume volume;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/PostProcessManager"));
            managerObject.name = "PostProcessManager";

            PostProcessManager manager = managerObject.GetComponent<PostProcessManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        volume = GetComponent<Volume>();
        SceneManager.activeSceneChanged += ActiveSceneChanged;
        ActiveSceneChanged(new Scene(), SceneManager.GetActiveScene());
    }

    private void Update()
    {
        if (volumesInLine.Count == 0) return;

        VolumeProfile currentProfile = volumesInLine[volumesInLine.Count - 1];
        if (volume.profile != currentProfile)
        {
            volume.weight -= Time.deltaTime * 5f;
            if (volume.weight <= 0) volume.profile = currentProfile;
        }
        else
            volume.weight += Time.deltaTime * 5f;

        volume.weight = Mathf.Clamp01(volume.weight);
    }

    private void ActiveSceneChanged(Scene previous, Scene current)
    {
        volumesInLine.Clear();
        foreach (ScenePostProcess scenePostProcess in sceneSpecificVolumes)
            if (scenePostProcess.scene == current.name)
            {
                volumesInLine.Add(scenePostProcess.volume);
                break;
            }
    }

    public void AddProfile(VolumeProfile profile)
    {
        if (!volumesInLine.Contains(profile))
            volumesInLine.Add(profile);
    }

    public void RemoveProfile(VolumeProfile profile)
    {
        if (volumesInLine.Contains(profile))
            volumesInLine.Remove(profile);
    }

    [Serializable]
    private struct ScenePostProcess
    {
        public string scene;
        public VolumeProfile volume;
    }
}
