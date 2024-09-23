using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Item selectedItem { get; private set; }

    [SerializeField] private Image itemImage;

    private int quantity;

    public void ChangeItem(Item newItem)
    {
        selectedItem = newItem;

        UpdateItemImage();
    }

    private void UpdateItemImage()
    {
        if (selectedItem != null)
        {
            itemImage.sprite = selectedItem.icon;
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.sprite = null;
            itemImage.color = new Color(0, 0, 0, 0);
        }
    }
}
