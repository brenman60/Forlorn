using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockInformation : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float openSpeed = 6f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Vector3 baseOffset;
    [SerializeField] private float defaultHeight = 63.2557f;
    [SerializeField] private float defaultRequirementHeight = 28.2557f;
    [Header("References")]
    [SerializeField] private RectTransform skillsCollection;
    [SerializeField] private TextMeshProUGUI requirementPrice;
    [SerializeField] private Button learnButton;

    public SkillUI selectedSkill { get; private set; }
    private Skill selectedSkill_;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 newBaseOffset;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        newBaseOffset = baseOffset;
    }

    private void Update()
    {
        UpdateCanvasGroup();
        UpdatePosition();
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, selectedSkill != null ? 1f : 0f, Time.unscaledDeltaTime * openSpeed);
        canvasGroup.interactable = selectedSkill != null && !selectedSkill.unlocked;
        canvasGroup.blocksRaycasts = selectedSkill != null;
    }

    private void UpdatePosition()
    {
        float widthRatio = Screen.width / 1920f;
        float heightRatio = Screen.height / 1080f;
        float selfHeightRatio = rectTransform.rect.height / defaultHeight;
        float collectionRatio = skillsCollection.localScale.x;
        //Vector2 listSizeAdjust = new Vector2(0, (requirementsList.childCount - 1) * defaultRequirementHeight) * new Vector2(widthRatio, heightRatio) * collectionRatio;
        newBaseOffset = (baseOffset * rectTransform.sizeDelta * new Vector2(widthRatio, heightRatio) * collectionRatio / selfHeightRatio); // - listSizeAdjust;

        if (selectedSkill != null)
            transform.position = Vector3.Lerp(transform.position, selectedSkill.transform.position + newBaseOffset, Time.unscaledDeltaTime * moveSpeed);
    }

    public void Open(SkillUI skillUI, Skill skill)
    {
        selectedSkill = skillUI;
        selectedSkill_ = skill;
        transform.position = skillUI.transform.position + newBaseOffset;
        ReloadRequirements();
    }

    private void ReloadRequirements()
    {
        requirementPrice.text = $"<color=#7fd9fa>{selectedSkill_.skillPointCost}</color> {(selectedSkill_.skillPointCost == 1 ? "Point" : "Points")}";
        learnButton.interactable = RunManager.Instance.statManager.stats[StatType.SkillPoints].currentValue >= selectedSkill_.skillPointCost;
    }

    public void UnlockSkill()
    {
        if (selectedSkill == null) return;

        if (!selectedSkill.unlocked)
            selectedSkill.UnlockSkill();

        Close();
    }

    public void Close()
    {
        selectedSkill = null;
    }
}
