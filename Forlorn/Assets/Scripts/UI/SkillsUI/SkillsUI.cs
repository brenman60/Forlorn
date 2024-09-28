using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
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

    private Camera mainCam;
    private CanvasGroup canvasGroup;

    private bool dragging;
    private Vector3 targetSkillsCollectionPosition;

    private float scaleFactor = 1f;

    private bool open;

    private void Start()
    {
        mainCam = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();
        targetSkillsCollectionPosition = skillsCollection.position;

        scrollSlider.minValue = minScrollSize;
        scrollSlider.maxValue = maxScrollSize;
        scrollSlider.value = scaleFactor;
    }

    private void Update()
    {
        UpdateCanvasGroup();
        CheckDragging();
        UpdateScaling();

        skillsCollection.position = Vector2.Lerp(skillsCollection.position, targetSkillsCollectionPosition, Time.deltaTime * dragSpeed);
    }

    private void UpdateCanvasGroup()
    {
        if (Input.GetKeyDown(Keybinds.GetKeybind(KeyType.SkillsUIOpen)))
            Toggle();

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.deltaTime * openSpeed);
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
            scaleFactor += Time.deltaTime * Input.mouseScrollDelta.y * scrollIntensity;
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
        if (open) SoundManager.Instance.PlayAudio("SkillsOpen", false, 0.5f);
    }
}
