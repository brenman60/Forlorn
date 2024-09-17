using System.Collections;
using System.Collections.Generic;
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

    private List<DialogueProperties> queuedDialogues = new List<DialogueProperties>();

    private bool dialogueInProgress;
    private float initialDialogueY;
    private Coroutine currentDialogue;

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
            DialogueProperties nextDialogue = queuedDialogues[0];
            currentDialogue = StartCoroutine(ShowDialogue(nextDialogue));
            queuedDialogues.Remove(nextDialogue);
        }
    }

    private IEnumerator ShowDialogue(DialogueProperties properties)
    {
        dialogueInProgress = true;

        dialogueText.color = properties.textColor;
        dialogueBackground.color = properties.backgroundColor;

        if (!properties.useTypewriter)
            dialogueText.text = properties.dialogue;
        else
        {
            dialogueText.text = string.Empty;
            for (int i = 0; i < properties.dialogue.Length; i++)
            {
                dialogueText.text += properties.dialogue[i];
                SoundManager.Instance.PlayAudio("DialogueType", false);
                yield return new WaitForSeconds(properties.specificCharacterSpeeds.TryGetValue(properties.dialogue[i], out float specificSpeed) ? specificSpeed : properties.typewriterSpeed);
            }
        }

        yield return new WaitForSeconds(properties.displayTime);

        dialogueInProgress = false;

        for (int i = 0; i < properties.dialogue.Length; i++)
        {
            dialogueText.text = dialogueText.text.Remove(dialogueText.text.Length - 1);
            yield return new WaitForSeconds(properties.typewriterSpeed / dialogueText.text.Length);
        }
    }

    public async Task AddDialogue(DialogueProperties properties)
    {
        queuedDialogues.Add(properties);
        while (queuedDialogues.Contains(properties))
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

public struct DialogueProperties
{
    public string dialogue;
    public Color textColor;
    public Color backgroundColor;
    public float displayTime;
    public bool useTypewriter;
    public float typewriterSpeed;
    public Dictionary<char, float> specificCharacterSpeeds;

    public DialogueProperties(
        string dialogue, 
        Color textColor, 
        Color backgroundColor, 
        float displayTime, 
        bool useTypewriter, 
        float typewriterSpeed, 
        Dictionary<char, float> specificCharacterSpeeds)
    {
        this.dialogue = dialogue;
        this.textColor = textColor;
        this.backgroundColor = backgroundColor;
        this.displayTime = displayTime;
        this.useTypewriter = useTypewriter;
        this.typewriterSpeed = typewriterSpeed;
        this.specificCharacterSpeeds = specificCharacterSpeeds;
    }
}
