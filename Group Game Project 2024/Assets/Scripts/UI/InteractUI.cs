using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    public static InteractUI Instance { get; private set; }

    [SerializeField] private float openSpeed = 5f;
    [Space(15)]
    [SerializeField] private TextMeshProUGUI interactKey;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color flashColor;

    private CanvasGroup canvasGroup;
    private Outline backgroundImageOutline;
    private Color originalOutlineColor;
    private Color originalColor;

    private List<Interactable> interactables = new List<Interactable>();

    private bool open;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject uiObject = Instantiate(Resources.Load<GameObject>("UI/InteractUI"));
            uiObject.name = "InteractUI";

            InteractUI ui = uiObject.GetComponent<InteractUI>();
            Instance = ui;

            DontDestroyOnLoad(uiObject);
        }
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        backgroundImageOutline = backgroundImage.GetComponent<Outline>();
        originalColor = backgroundImage.color;
        originalOutlineColor = backgroundImageOutline.effectColor;
    }

    private void Update()
    {
        open = interactables.Count > 0;

        UpdateInteractText();
        UpdateCanvasGroup();
        UpdateBackgroundColor();

        if (Input.GetKeyDown(Keybinds.GetKeybind(KeyType.Interact)))
            DoInteraction();
    }

    public void DoInteraction()
    {
        backgroundImage.color = flashColor;
        backgroundImageOutline.effectColor = flashColor;

        interactables[0].Interact();
        RemoveInteraction(interactables[0]);
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.deltaTime * openSpeed);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }

    private void UpdateBackgroundColor()
    {
        backgroundImage.color = Color.Lerp(backgroundImage.color, originalColor, Time.deltaTime * openSpeed);
        backgroundImageOutline.effectColor = Color.Lerp(backgroundImageOutline.effectColor, originalOutlineColor, Time.deltaTime * openSpeed);
    }

    private void UpdateInteractText()
    {
        if (interactables.Count > 0)
            interactText.text = interactables[0].interactText;
    }

    public void AddInteraction(Interactable interactable)
    {
        interactables.Add(interactable);

        interactKey.text = Keybinds.GetKeybind(KeyType.Interact).ToString();
    }

    public void RemoveInteraction(Interactable interactable)
    {
        if (interactables.Contains(interactable))
            interactables.Remove(interactable);
    }
}
