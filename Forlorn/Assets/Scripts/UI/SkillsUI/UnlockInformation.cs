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
    [SerializeField] private ContentSizeFitter requirementsCSF;
    [SerializeField] private Transform requirementsList;
    [SerializeField] private GameObject requirementTemplate;

    public SkillUI selectedSkill { get; private set; }

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
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, selectedSkill != null ? 1f : 0f, Time.deltaTime * openSpeed);
    }

    private void UpdatePosition()
    {
        float widthRatio = Screen.width / 1920f;
        float heightRatio = Screen.height / 1080f;
        float selfHeightRatio = rectTransform.rect.height / defaultHeight;
        float collectionRatio = skillsCollection.localScale.x;
        Vector2 listSizeAdjust = new Vector2(0, (requirementsList.childCount - 1) * defaultRequirementHeight) * new Vector2(widthRatio, heightRatio) * collectionRatio;
        newBaseOffset = (baseOffset * rectTransform.sizeDelta * new Vector2(widthRatio, heightRatio) * collectionRatio / selfHeightRatio) - listSizeAdjust;

        if (selectedSkill != null)
            transform.position = Vector3.Lerp(transform.position, selectedSkill.transform.position + newBaseOffset, Time.deltaTime * moveSpeed);
    }

    public void Open(SkillUI skill)
    {
        selectedSkill = skill;
        transform.position = skill.transform.position + newBaseOffset;
        ReloadRequirements();
    }

    private void ReloadRequirements()
    {
        foreach (Transform previousRequirement in requirementsList) if (previousRequirement.gameObject != requirementTemplate) Destroy(previousRequirement.gameObject);

        requirementsCSF.SetLayoutVertical();
    }

    public void Close()
    {
        selectedSkill = null;
    }
}
