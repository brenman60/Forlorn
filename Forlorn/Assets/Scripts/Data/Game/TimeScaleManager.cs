using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeScaleManager : MonoBehaviour
{
    private static TimeScaleManager Instance;

    private static List<string> timePauses = new List<string>();
    private static Dictionary<string, float> timeInfluences = new Dictionary<string, float>();

    public static bool paused
    {
        get
        {
            return timePauses.Count > 0;
        }
    }

    public static float currentTime
    {
        get
        {
            if (timePauses.Count > 0) return 0f;

            float startTime = 1f;
            foreach (float influence in timeInfluences.Values)
                startTime += influence;

            return startTime;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/TimeScaleManager"));
            managerObject.name = "TimeScaleManager";

            TimeScaleManager manager = managerObject.GetComponent<TimeScaleManager>();
            Instance = manager;

            SceneManager.activeSceneChanged += ActiveSceneChanged;

            DontDestroyOnLoad(managerObject);
        }
    }

    private static void ActiveSceneChanged(Scene arg0, Scene arg1)
    {
        timePauses.Clear();
        timeInfluences.Clear();
    }

    private void Update()
    {
        if (timePauses.Count > 0)
        {
            Time.timeScale = 0;
            return;
        }

        Time.timeScale = currentTime;
    }

    public static void AddPause(string name)
    {
        timePauses.Add(name);
    }

    public static bool HasPause(string name) => timePauses.Contains(name);

    public static void RemovePause(string name)
    {
        if (timePauses.Contains(name))
            timePauses.Remove(name);
    }

    public static void AddInfluence(string name, float influence)
    {
        timeInfluences.Add(name, influence);
    }

    public static bool HasInfluence(string name) => timeInfluences.ContainsKey(name);

    public static void RemoveInfluence(string name)
    {
        if (timeInfluences.ContainsKey(name))
            timeInfluences.Remove(name);
    }
}
