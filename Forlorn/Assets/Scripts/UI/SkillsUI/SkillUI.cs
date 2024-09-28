using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Customization")]
    [SerializeField] private Skill skill;
    [Space(20), SerializeField] private float hoverSpeed = 4f;
    [SerializeField] private float hoverSizeIncrease = 1.25f;
    [SerializeField] private float hoverOutlineSpeed = 0.5f;
    [SerializeField] private float hoverOutlineCatchupSpeed = 4f;
    [Header("References")]
    [SerializeField] private SkillInformation skillInformation;
    [SerializeField] private RectTransform backgroundOutline;

    private bool hovered;

    private Vector2 initialSize;
    private float targetOutlineRotation = 45f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.sizeDelta;
    }

    private void Update()
    {
        UpdateTransform();
        UpdateOutline();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        skillInformation.Open(skill.visibleName, skill.displayDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        SnapOutlineRotation();

        skillInformation.Close();
    }
}
