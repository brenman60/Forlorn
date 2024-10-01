using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [SerializeField] private Items items;
    [Space(20), SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField, Space(20)] private float showSpeed;
    [SerializeField] private RectTransform dialogueRect;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBackground;
    [Space(20), SerializeField] private float textFadeInTime;
    [SerializeField] private float openHeightIncrease, closedHeightDecrease;
    [Space(20), SerializeField] private GameObject optionTemplate;
    [SerializeField] private Transform optionsList;

    private List<DialogueNode> queuedDialogues = new List<DialogueNode>();

    private bool dialogueInProgress;
    private float initialDialogueY;
    private Coroutine currentDialogue;
    private Dictionary<GameObject, DialogueOption> optionButtons = new Dictionary<GameObject, DialogueOption>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject uiObject = Instantiate(Resources.Load<GameObject>("UI/DialogueUI"));
            uiObject.name = "DialogueUI";

            DialogueUI ui = uiObject.GetComponent<DialogueUI>();
            Instance = ui;

            DontDestroyOnLoad(uiObject);
        }
    }

    private void Start()
    {
        initialDialogueY = dialogueRect.position.y;

        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void Update()
    {
        dialogueCanvasGroup.alpha = Mathf.Lerp(dialogueCanvasGroup.alpha, dialogueInProgress ? 1f : 0f, Time.deltaTime * showSpeed);
        dialogueCanvasGroup.interactable = dialogueInProgress;
        dialogueCanvasGroup.blocksRaycasts = dialogueInProgress;

        dialogueRect.position = Vector3.Lerp(dialogueRect.position, new Vector3(dialogueRect.position.x, initialDialogueY + (dialogueInProgress ? openHeightIncrease : closedHeightDecrease), dialogueRect.position.z), Time.deltaTime * showSpeed);

        if (!dialogueInProgress && queuedDialogues.Count > 0)
        {
            DialogueNode nextDialogue = queuedDialogues[0];
            currentDialogue = StartCoroutine(ShowDialogue(nextDialogue));
            queuedDialogues.Remove(nextDialogue);
        }
    }

    private IEnumerator ShowDialogue(DialogueNode node)
    {
        foreach (Transform previousButton in optionsList) if (previousButton.gameObject != optionTemplate) Destroy(previousButton.gameObject);
        optionButtons.Clear();

        dialogueInProgress = true;

        dialogueText.color = node.textColor;
        dialogueBackground.color = node.backgroundColor;

        if (!node.useTypewriter)
            dialogueText.text = node.dialogue;
        else
        {
            dialogueText.text = string.Empty;
            for (int i = 0; i < node.dialogue.Length; i++)
            {
                dialogueText.text += node.dialogue[i];
                SoundManager.Instance.PlayAudio("DialogueType", false);
                yield return new WaitForSeconds(node.typewriterSpeed);
            }
        }

        yield return new WaitForSeconds(node.displayTime);

        // Either show all options if any, or if no options then just close UI

        if (node.options.Count == 0)
        {
            dialogueInProgress = false;
            StartCoroutine(CloseDialogue(node));
        }
        else
            foreach (DialogueOption option in node.options)
                CreateOption(option);
    }

    private void CreateOption(DialogueOption option)
    {
        GameObject button = Instantiate(optionTemplate, optionsList);
        DialogueOptionUI optionUI = button.GetComponent<DialogueOptionUI>();
        optionUI.optionText.text = option.optionText;

        bool hasRequirements = true;
        foreach (DialogueOptionRequirement requirement in option.optionRequirements)
        {
            switch (requirement.type)
            {
                case DialogueOptionRequirementType.Stat:
                    StatType statType = (StatType)Enum.Parse(typeof(StatType), requirement.requirement);
                    Stat stat = RunManager.Instance.statManager.stats[statType];
                    float currentPercentage = stat.maxValue * (requirement.requiredAmount / 100);
                    if (requirement.amountIsPercentage && currentPercentage < requirement.requiredAmount)
                        hasRequirements = false;
                    else if (stat.currentValue < requirement.requiredAmount)
                        hasRequirements = false;
                    break;
                case DialogueOptionRequirementType.Item:
                    Item item = items.GetItemByName(requirement.requirement);
                    int itemAmount = Inventory.Instance.HasItem(item);
                    if (itemAmount < requirement.requiredAmount)
                        hasRequirements = false;
                    break;
            }
        }

        optionUI.canvasGroup.interactable = hasRequirements;
        optionUI.canvasGroup.alpha = hasRequirements ? 1f : 0.25f;

        button.SetActive(true);
        optionButtons.Add(button, option);
    }

    private IEnumerator CloseDialogue(DialogueNode node)
    {
        while (dialogueText.text.Length > 0 && !dialogueInProgress)
        {
            dialogueText.text = dialogueText.text.Remove(dialogueText.text.Length - 1);
            yield return new WaitForSeconds(node != null ? node.typewriterSpeed / dialogueText.text.Length : 0.025f / 4f);
        }
    }

    public void ChooseOption(GameObject optionButton)
    {
        DialogueOption option = optionButtons[optionButton];

        // Call specified class
        if (!string.IsNullOrEmpty(option.onSelectClass))
        {
            Type targetType = Type.GetType(option.onSelectClass);
            if (targetType == null)
            {
                Debug.LogError("Type '" + option.onSelectClass + "' not found.");
                return;
            }

            MethodInfo targetMethod = targetType.GetMethod(option.onSelectMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (targetMethod == null)
            {
                Debug.LogError("Method '" + option.onSelectMethod + "' not found.");
                return;
            }

            if (targetMethod.IsStatic)
                targetMethod.Invoke(null, ConvertSelectArguments(option.onSelectArguments));
            else
            {
                var targetObject = FindObjectOfType(targetType);
                if (targetObject != null)
                    targetMethod.Invoke(targetObject, ConvertSelectArguments(option.onSelectArguments));
            }
        }

        // Remove required items
        foreach (DialogueOptionRequirement requirement in option.optionRequirements)
        {
            if (!requirement.removesAmount) continue;

            switch (requirement.type)
            {
                case DialogueOptionRequirementType.Stat:
                    StatType statType = (StatType)Enum.Parse(typeof(StatType), requirement.requirement);
                    Stat stat = RunManager.Instance.statManager.stats[statType];
                    if (requirement.amountIsPercentage)
                        stat.currentValue -= stat.maxValue * (requirement.requiredAmount / 100);
                    else
                        stat.currentValue -= requirement.requiredAmount;
                    break;
                case DialogueOptionRequirementType.Item:
                    Item item = items.GetItemByName(requirement.requirement);
                    Inventory.Instance.TakeItem(item, requirement.requiredAmount);
                    break;
            }
        }

        if (option.nextNode == null)
        {
            dialogueInProgress = false;
            StartCoroutine(CloseDialogue(null));
        }
        else
            StartCoroutine(ShowDialogue(option.nextNode));
    }

    private object[] ConvertSelectArguments(List<DialogueOnSelectArgument> arguments)
    {
        object[] newArguments = new object[arguments.Count];
        for (int i = 0; i < arguments.Count; i++)
        {
            switch (arguments[i].type)
            {
                case ArgumentType.Int:
                    newArguments[i] = int.Parse(arguments[i].argument);
                    break;
                case ArgumentType.Float:
                    newArguments[i] = float.Parse(arguments[i].argument);
                    break;
                case ArgumentType.Bool:
                    newArguments[i] = bool.Parse(arguments[i].argument);
                    break;
                case ArgumentType.Item:
                    newArguments[i] = items.GetItemByName(arguments[i].argument);
                    break;
                default:
                    newArguments[i] = arguments[i].argument;
                    break;
            }
        }

        return newArguments;
    }

    public async Task AddDialogue(DialogueNode node)
    {
        queuedDialogues.Add(node);
        while (queuedDialogues.Contains(node))
            await Task.Yield();
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        queuedDialogues.Clear();
        if (currentDialogue != null) StopCoroutine(currentDialogue);
        dialogueInProgress = false;
        dialogueText.text = string.Empty;
    }
}
