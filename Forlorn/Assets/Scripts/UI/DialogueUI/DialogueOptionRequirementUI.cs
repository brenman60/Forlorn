using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptionRequirementUI : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private string subtractionColor = "#d62d2d";
    [Header("References")]
    [SerializeField] private Items items;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI infoText;

    public void Init(DialogueRequirement requirement)
    {
        icon.transform.parent.gameObject.SetActive(false);
        infoText.text = requirement.requirementText;
        infoText.rectTransform.anchorMin = Vector3.zero;
        infoText.rectTransform.anchorMax = Vector3.one;
        infoText.rectTransform.sizeDelta = Vector2.one * -40;
        infoText.rectTransform.anchoredPosition = Vector3.zero;

        /*

            Item requiredItem = items.GetItemByName(requirement.requirement);
            icon.sprite = requiredItem.icon;
            infoText.text = $"<color={subtractionColor}>{(requirement.removesAmount ? "-" : string.Empty)}{requirement.requiredAmount}</color> {requiredItem.visibleName}";

         */
    }
}
