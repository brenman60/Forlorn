using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public DialogueOption option;

    public TextMeshProUGUI optionText;
    public CanvasGroup canvasGroup;
    [SerializeField] private GameObject infoButton;
    [SerializeField] private DialogueOptionInfoUI optionInfoUI;

    private void Update()
    {
        infoButton.SetActive(GameManager.isMobile);
    }

    public void OnClick()
    {
        optionInfoUI.ViewInfo(option);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (option.optionRequirements.Count > 0)
            optionInfoUI.ViewInfo(option);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        optionInfoUI.Toggle(false);
    }
}
