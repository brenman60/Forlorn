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

    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField, Space(15)] private float showSpeed;
    [SerializeField] private RectTransform dialogueRect;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBackground;
    [Space(15), SerializeField] private float textFadeInTime;
    [Space(15), SerializeField] private float openHeightIncrease, closedHeightDecrease;
    [Space(15), SerializeField] private Transform topText;
    [SerializeField] private GameObject optionTemplate;
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
        dialogueRect.position = Vector3.Lerp(dialogueRect.position, new Vector3(dialogueRect.position.x, initialDialogueY + (dialogueInProgress ? openHeightIncrease : closedHeightDecrease), dialogueRect.position.z), Time.deltaTime * showSpeed);

        if (!dialogueInProgress && queuedDialogues.Count > 0)
        {
            DialogueNode nextDialogue = queuedDialogues[0];
            currentDialogue = StartCoroutine(ShowDialogue(nextDialogue));
            queuedDialogues.Remove(nextDialogue);
        }

        optionsList.transform.position = new Vector3(optionsList.transform.position.x, topText.transform.position.y + 30, optionsList.transform.position.z);
    }

    private IEnumerator ShowDialogue(DialogueNode node)
    {
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
        {
            foreach (Transform previousButton in optionsList) if (previousButton.gameObject != optionTemplate) Destroy(previousButton.gameObject);
            optionButtons.Clear();

            foreach (DialogueOption option in node.options)
            {
                GameObject button = Instantiate(optionTemplate, optionsList);
                button.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;
                button.SetActive(true);

                optionButtons.Add(button, option);
            }
        }
    }

    private IEnumerator CloseDialogue(DialogueNode node)
    {
        while (dialogueText.text.Length > 0 && !dialogueInProgress)
        {
            dialogueText.text = dialogueText.text.Remove(dialogueText.text.Length - 1);
            yield return new WaitForSeconds(node.typewriterSpeed / dialogueText.text.Length);
        }
    }

    public void ChooseOption(GameObject optionButton)
    {
        DialogueOption option = optionButtons[optionButton];

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
            targetMethod.Invoke(null, option.onSelectArguments);
        else
        {
            var targetObject = FindObjectOfType(targetType);
            if (targetObject != null)
                targetMethod.Invoke(targetObject, option.onSelectArguments);
        }

        print(targetType.Name);
        print(targetMethod.Name);

        if (option.nextNode == null)
        {
            dialogueInProgress = false;
            StartCoroutine(CloseDialogue(null));
        }
        else
            StartCoroutine(ShowDialogue(option.nextNode));
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
