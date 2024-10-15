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

    private ForlornButton interactButton;

    private CanvasGroup canvasGroup;
    private Outline backgroundImageOutline;
    private Color originalOutlineColor;
    private Color originalColor;

    private List<Interactable> interactables = new List<Interactable>();

    private float interactCooldown;
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
        interactButton = backgroundImage.GetComponent<ForlornButton>();

        canvasGroup = GetComponent<CanvasGroup>();
        backgroundImageOutline = backgroundImage.GetComponent<Outline>();
        originalColor = backgroundImage.color;
        originalOutlineColor = backgroundImageOutline.effectColor;
    }

    private void Update()
    {
        interactCooldown -= Time.unscaledDeltaTime;
        open = interactables.Count > 0;

        UpdateInteractText();
        UpdateCanvasGroup();
        UpdateBackgroundColor();

        if (Keybinds.Instance.controlInteract.ReadValue<float>() != 0 && interactables.Count > 0 && open && interactCooldown <= 0f)
        {
            interactCooldown = 2f;
            DoInteraction();
        }
    }

    public void DoInteraction()
    {
        backgroundImage.color = flashColor;
        backgroundImageOutline.effectColor = flashColor;

        interactables[0].Interact();

        if (interactables[0].interactSound != null)
            SoundManager.Instance.PlayAudio(interactables[0].interactSound, true);

        if (interactables[0].disappearsPlayer)
            Player.Instance.gameObject.SetActive(false);
        else
            RemoveInteraction(interactables[0]);

        SoundManager.Instance.PlayAudio(interactButton.clickSound, false, 1f);
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
