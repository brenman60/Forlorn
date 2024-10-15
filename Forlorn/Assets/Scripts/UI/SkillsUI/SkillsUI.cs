using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour, ISaveData
{
    public static SkillsUI Instance { get; private set; }

    [Header("Customization")]
    [SerializeField] private float openSpeed = 10f;
    [SerializeField] private float dragSpeed = 4f;
    [SerializeField] private float scrollIntensity = 15f;
    [SerializeField] private float minScrollSize = 0.25f;
    [SerializeField] private float maxScrollSize = 5f;
    [Header("References")]
    [SerializeField] private RectTransform skillsCollection;
    [SerializeField] private List<RectTransform> scalableRects;
    [SerializeField] private Slider scrollSlider;
    public List<SkillUI> skills;

    private Camera mainCam;
    private CanvasGroup canvasGroup;

    private bool dragging;
    private Vector3 targetSkillsCollectionPosition;

    private float scaleFactor = 1f;

    private bool open;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mainCam = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();
        targetSkillsCollectionPosition = skillsCollection.position;

        scrollSlider.minValue = minScrollSize;
        scrollSlider.maxValue = maxScrollSize;
        scrollSlider.value = scaleFactor;

        SceneManager.sceneLoaded += NewSceneLoaded;
    }

    private void NewSceneLoaded(Scene newScene, LoadSceneMode sceneLoadMode)
    {
        if (!GameManager.validGameScenes.Contains(newScene.name))
        {
            Instance = null;
            //if (gameObject != null) Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateCanvasGroup();

        if (open)
        {
            CheckDragging();
            UpdateScaling();
        }

        skillsCollection.position = Vector2.Lerp(skillsCollection.position, targetSkillsCollectionPosition, Time.unscaledDeltaTime * dragSpeed);
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.unscaledDeltaTime * openSpeed);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }

    private void CheckDragging()
    {
        KeyCode dragKey = Keybinds.GetKeybind(KeyType.SkillsUIDrag);
        if (!dragging && Input.GetKeyDown(dragKey))
            StartCoroutine(DragCamera(dragKey));

        dragging = Input.GetKey(dragKey);
    }

    private void UpdateScaling()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            scaleFactor += Time.unscaledDeltaTime * Input.mouseScrollDelta.y * scrollIntensity;
            scaleFactor = Mathf.Clamp(scaleFactor, minScrollSize, maxScrollSize);

            ResizeUI();
            scrollSlider.value = scaleFactor;
        }
    }

    private void ResizeUI()
    {
        foreach (RectTransform scalable in scalableRects)
            scalable.transform.localScale = Vector3.one * scaleFactor;
    }

    public void UpdateSlider()
    {
        scaleFactor = scrollSlider.value;
        ResizeUI();
    }

    private IEnumerator DragCamera(KeyCode dragKey)
    {
        Vector3 initialMousePosition = Input.mousePosition;
        while (Input.GetKey(dragKey))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 travel = currentMousePosition - initialMousePosition;
            targetSkillsCollectionPosition += travel;
            initialMousePosition = currentMousePosition;

            yield return null;
        }
    }

    public void Toggle()
    {
        open = !open;

        if (open)
        {
            TimeScaleManager.AddPause(name);
            SoundManager.Instance.PlayAudio("SkillsOpen", false, 0.5f);
        }
        else
            TimeScaleManager.RemovePause(name);
    }

    public string GetSaveData()
    {
        List<string> skillsData = new List<string>();
        foreach (SkillUI skill in skills)
            skillsData.Add(skill.GetSaveData());

        return JsonConvert.SerializeObject(skillsData);
    }

    public void PutSaveData(string data)
    {
        List<string> skillsData = JsonConvert.DeserializeObject<List<string>>(data);
        for (int i = 0; i < skillsData.Count; i++)
            skills[i].PutSaveData(skillsData[i]);
    }
}
