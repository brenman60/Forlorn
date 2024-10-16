using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float openSpeed = 5f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private GameObject slotTemplate;
    [SerializeField] private Transform slotsList;
    [Space(25)]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private CanvasGroup purchaseButton;

    private List<Coroutine> typingCoroutines = new List<Coroutine>();
    private Coroutine itemImageFade;

    private SlotUI selectedSlot;
    private CanvasGroup canvasGroup;
    private bool open;

    private bool purchaseButtonEnabled;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.unscaledDeltaTime * openSpeed);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;

        purchaseButton.alpha = purchaseButtonEnabled ? 1f : 0.2f;
        purchaseButton.interactable = purchaseButtonEnabled;
        purchaseButton.blocksRaycasts = purchaseButtonEnabled;
    }

    public void SelectItem(SlotUI slot)
    {
        foreach (Coroutine typingCoroutine in typingCoroutines)
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (itemImageFade != null) StopCoroutine(itemImageFade);

        Item item = slot.selectedItem;
        typingCoroutines.Add(StartCoroutine(TypewriteText(itemName, item.visibleName)));
        typingCoroutines.Add(StartCoroutine(TypewriteText(itemDescription, item.visibleDescription)));
        typingCoroutines.Add(StartCoroutine(TypewriteText(itemPrice, "$" + item.shopCost)));

        itemImageFade = StartCoroutine(FadeItemImage(item.icon));

        float currentMoney = RunManager.Instance.statManager.stats[StatType.Money].currentValue;
        purchaseButtonEnabled = currentMoney >= item.shopCost;

        selectedSlot = slot;
    }

    public void PurchaseItem()
    {
        Item item = selectedSlot.selectedItem;
        RunManager.Instance.statManager.stats[StatType.Money].currentValue -= item.shopCost;

        float currentMoney = RunManager.Instance.statManager.stats[StatType.Money].currentValue;
        purchaseButtonEnabled = currentMoney >= item.shopCost;

        Inventory.Instance.PutItem(item, 1);
    }

    public void OpenShop(Shop shop)
    {
        foreach (Transform previousSlot in slotsList)
            if (previousSlot.gameObject != slotTemplate)
                Destroy(previousSlot.gameObject);

        shopNameText.text = shop.visibleName;
        foreach (Item shopItem in shop.shopItems)
        {
            GameObject newSlot = Instantiate(slotTemplate, slotsList);
            newSlot.name = shopItem.name;

            SlotUI slotUI = newSlot.GetComponent<SlotUI>();
            slotUI.ChangeItem(shopItem);

            newSlot.SetActive(true);
        }

        Toggle();
    }

    public void Toggle()
    {
        open = !open;

        if (open)
        {
            //TimeScaleManager.AddPause(name);
        }
        else
        {
            Player.Instance.gameObject.SetActive(true);
            //TimeScaleManager.RemovePause(name);
        }
    }

    private IEnumerator FadeItemImage(Sprite newItem)
    {
        while (itemImage.color.a != 0)
        {
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, itemImage.color.a - (0.1f * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(0.025f);
        }

        itemImage.sprite = newItem;
        yield return new WaitForSeconds(0.2f);

        while (itemImage.color.a != 1)
        {
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, itemImage.color.a + (0.1f * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(0.025f);
        }
    }

    private IEnumerator TypewriteText(TextMeshProUGUI textField, string text)
    {
        while (textField.text.Length > 0)
        {
            textField.text = textField.text.Remove(textField.text.Length - 1);
            yield return new WaitForSeconds(0.025f);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (char character in text)
        {
            textField.text += character;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
