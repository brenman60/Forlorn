using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour, ISaveData, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Customization")]
    [SerializeField] private Skill skill;
    [SerializeField] private SkillUI[] requiredUnlockedSkills;
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
    [SerializeField] private Transform lineRendererParent;
    [SerializeField] private GameObject lineRendererPrefab;

    private bool unlockable;
    public bool unlocked { get; private set; }
    private bool hovered;

    private Vector2 initialSize;
    private Vector2 initialBackgroundOutlineSize;
    private float targetOutlineRotation = 45f;

    private RectTransform rectTransform;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.sizeDelta;
        initialBackgroundOutlineSize = backgroundOutline.sizeDelta;

        foreach (SkillUI requiredSkill in requiredUnlockedSkills)
        {
            GameObject requiredLine = Instantiate(lineRendererPrefab, lineRendererParent);
            UILineRenderer uiLineRenderer = requiredLine.GetComponent<UILineRenderer>();
            uiLineRenderer.DrawLine(transform.position, requiredSkill.transform.position);
        }
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
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, !hovered ? initialSize : (initialSize * hoverSizeIncrease), Time.unscaledDeltaTime * hoverSpeed);
    }

    private void UpdateOutline()
    {
        if (hovered)
        {
            targetOutlineRotation += hoverOutlineSpeed * Time.unscaledDeltaTime;
            targetOutlineRotation = targetOutlineRotation % 360;
        }

        Quaternion newRotation = Quaternion.Euler(backgroundOutline.eulerAngles.x, backgroundOutline.eulerAngles.y, targetOutlineRotation);
        backgroundOutline.rotation = Quaternion.Slerp(backgroundOutline.rotation, newRotation, Time.unscaledDeltaTime * hoverOutlineCatchupSpeed);
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
        backgroundOutlineImage.color = Color.Lerp(backgroundOutlineImage.color, unlocked ? unlockedColor : lockedColor, Time.unscaledDeltaTime * hoverOutlineSpeed);
        backgroundOutlineImage.sprite = unlocked ? unlockedBackgroundSprite : lockedBackgroundSprite;
        backgroundOutline.sizeDelta = Vector2.Lerp(backgroundOutline.sizeDelta, initialBackgroundOutlineSize, Time.unscaledDeltaTime * hoverSpeed / 3f);
    }

    private void UpdateUnlockableState()
    {
        if (requiredUnlockedSkills.Length > 0)
        {
            bool allUnlocked = true;
            foreach (SkillUI requiredSkill in requiredUnlockedSkills)
                if (!requiredSkill.unlocked)
                {
                    allUnlocked = false;
                    break;
                }

            unlockable = allUnlocked;
        }
        else
            unlockable = true;

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
        if (!unlockable || unlocked || eventData.button != PointerEventData.InputButton.Left) return;

        if (unlockInformation.selectedSkill != this || unlockInformation.selectedSkill == null)
        {
            unlockInformation.Open(this, skill);

            targetOutlineRotation += 135f;
            targetOutlineRotation = targetOutlineRotation % 360;
        }
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
            if (modifier.changesMaxValue)
            {
                StatModifier statModifier = new StatModifier(modifier.modifierIdentifier, selectedStat, modifier.statChange, modifier.isMultiplicative, true);
                RunManager.Instance.statManager.ApplyModifier(statModifier);
            }
            else
            {
                selectedStat.currentValue += modifier.statChange;
            }
        }

        RunManager.Instance.statManager.stats[StatType.SkillPoints].currentValue -= skill.skillPointCost;
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
