using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour, ISaveData, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Customization")]
    [SerializeField] private Skill skill;
    [SerializeField] private SkillUI requiredUnlockedSkill;
    [Space(20), SerializeField] private float hoverSpeed = 4f;
    [SerializeField] private float hoverSizeIncrease = 1.25f;
    [SerializeField] private float hoverOutlineSpeed = 0.5f;
    [SerializeField] private float hoverOutlineCatchupSpeed = 4f;
    [Space(20), SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Sprite lockedBackgroundSprite;
    [SerializeField] private Sprite unlockedBackgroundSprite;
    [Header("References")]
    [SerializeField] private SkillInformation skillInformation;
    [SerializeField] private UnlockInformation unlockInformation;
    [SerializeField] private RectTransform lockedCover;
    [SerializeField] private RectTransform backgroundOutline;
    [SerializeField] private Image backgroundOutlineImage;

    private bool unlockable;
    public bool unlocked { get; private set; }
    private bool hovered;

    private Vector2 initialSize;
    private Vector2 initialBackgroundOutlineSize;
    private float targetOutlineRotation = 45f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.sizeDelta;
        initialBackgroundOutlineSize = backgroundOutline.sizeDelta;
    }

    private void Update()
    {
        UpdateTransform();
        UpdateOutline();
        UpdateUnlockedColor();
        UpdateUnlockableState();
    }

    private void UpdateTransform()
    {
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, !hovered ? initialSize : (initialSize * hoverSizeIncrease), Time.deltaTime * hoverSpeed);
    }

    private void UpdateOutline()
    {
        if (hovered)
        {
            targetOutlineRotation += hoverOutlineSpeed * Time.deltaTime;
            targetOutlineRotation = targetOutlineRotation % 360;
        }

        Quaternion newRotation = Quaternion.Euler(backgroundOutline.eulerAngles.x, backgroundOutline.eulerAngles.y, targetOutlineRotation);
        backgroundOutline.rotation = Quaternion.Slerp(backgroundOutline.rotation, newRotation, Time.deltaTime * hoverOutlineCatchupSpeed);
    }

    private void SnapOutlineRotation()
    {
        int clampedValue = Mathf.RoundToInt(Mathf.Round(targetOutlineRotation / 45f) * 45);
        if (clampedValue % 90 == 0)
        {
            if (targetOutlineRotation >= clampedValue)
                clampedValue += 45;
            else
                clampedValue -= 45;
        }

        targetOutlineRotation = clampedValue;
    }

    private void UpdateUnlockedColor()
    {
        backgroundOutlineImage.color = Color.Lerp(backgroundOutlineImage.color, unlocked ? unlockedColor : lockedColor, Time.deltaTime * hoverOutlineSpeed);
        backgroundOutlineImage.sprite = unlocked ? unlockedBackgroundSprite : lockedBackgroundSprite;
        backgroundOutline.sizeDelta = Vector2.Lerp(backgroundOutline.sizeDelta, initialBackgroundOutlineSize, Time.deltaTime * hoverSpeed / 3f);
    }

    private void UpdateUnlockableState()
    {
        unlockable = requiredUnlockedSkill != null ? requiredUnlockedSkill.unlocked : true;
        lockedCover.gameObject.SetActive(!unlockable);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!unlockable) return;

        hovered = true;

        skillInformation.Open(skill.visibleName, skill.displayDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        SnapOutlineRotation();

        skillInformation.Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!unlockable || unlocked) return;

        if (unlockInformation.selectedSkill != this || unlockInformation.selectedSkill == null)
            unlockInformation.Open(this, skill);
        else
            unlockInformation.Close();
    }

    public void UnlockSkill()
    {
        if (!unlockable || unlocked) return;

        unlocked = true;
        backgroundOutline.sizeDelta += Vector2.one * 35f;
        SoundManager.Instance.PlayAudio("SkillUnlock", true, 1f);

        foreach (SkillModifier modifier in skill.modifiers)
        {
            Stat selectedStat = RunManager.Instance.statManager.stats[modifier.statType];
            StatModifier statModifier = new StatModifier(selectedStat, modifier.statChange, modifier.isMultiplicative);
            RunManager.Instance.statManager.ApplyModifier(statModifier);
        }

        foreach (SkillStatCost statCost in skill.skillStatCosts)
        {
            if (!statCost.removesAmount) continue;

            Stat stat = RunManager.Instance.statManager.stats[statCost.statType];
            float percentageSubtraction = (statCost.requiredAmount / 100f) * stat.maxValue;

            if (!statCost.removesFromMaxAmount)
            {
                if (statCost.isPercentage)
                    stat.currentValue -= percentageSubtraction;
                else
                    stat.currentValue -= statCost.requiredAmount;
            }
            else
                RunManager.Instance.statManager.ApplyModifier(new StatModifier(stat, statCost.requiredAmount, statCost.isPercentage));
        }

        foreach (SkillItemCost itemCost in skill.skillItemCosts)
        {
            if (!itemCost.removesAmount) continue;

            Inventory.Instance.TakeItem(itemCost.item, itemCost.requiredAmount);
        }
    }

    public string GetSaveData()
    {
        return JsonConvert.SerializeObject(unlocked);
    }

    public void PutSaveData(string data)
    {
        unlocked = JsonConvert.DeserializeObject<bool>(data);
    }
}
