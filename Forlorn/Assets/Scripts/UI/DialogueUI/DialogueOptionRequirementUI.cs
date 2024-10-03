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

    public void Init(DialogueOptionRequirement requirement)
    {
        switch (requirement.type)
        {
            case DialogueOptionRequirementType.Item:
                Item requiredItem = items.GetItemByName(requirement.requirement);
                icon.sprite = requiredItem.icon;
                infoText.text = $"<color={subtractionColor}>{(requirement.removesAmount ? "-" : string.Empty)}{requirement.requiredAmount}</color> {requiredItem.visibleName}";
                break;
            case DialogueOptionRequirementType.Stat:
                icon.transform.parent.gameObject.SetActive(false);
                infoText.text = $"<color={subtractionColor}>{(requirement.removesAmount ? "-" : string.Empty)}{requirement.requiredAmount}</color> {requirement.requirement}";
                infoText.rectTransform.anchorMin = Vector3.zero;
                infoText.rectTransform.anchorMax = Vector3.one;
                infoText.rectTransform.sizeDelta = Vector2.zero;
                break;
        }
    }
}
