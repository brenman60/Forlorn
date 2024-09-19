using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TextMeshProUGUI nameText;

    private Item selectedItem;
    private int quantity;

    public void ChangeItem(Item newItem, int newQuantity)
    {
        selectedItem = newItem;
        quantity = newQuantity;
        itemImage.sprite = newItem.icon;
        itemAmount.text = newQuantity.ToString();
        nameText.text = newItem.visibleName;
    }

    public void ToggleNameView(bool toggle) => namePanel.SetActive(toggle);
}
