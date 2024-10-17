using TMPro;
using UnityEngine;

public class SkillInformation : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float canvasGroupSpeed = 6f;
    [SerializeField] private float mouseMoveSpeed = 4f;
    [SerializeField] private Vector2 mouseOffset;
    [Header("References")]
    [SerializeField] private Canvas skillsCanvas;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    private CanvasGroup canvasGroup;
    private bool open;

    private Vector2 mousePosition;
    private Vector2 initialMouseOffset;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        initialMouseOffset = mouseOffset;
    }

    private void Update()
    {
        UpdateCanvasGroup();
        UpdateMousePosition();
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.unscaledDeltaTime * canvasGroupSpeed);
    }

    private void UpdateMousePosition()
    {
        float widthRatio = Screen.width / 1920f;
        float heightRatio = Screen.height / 1080f;
        mouseOffset = initialMouseOffset * new Vector2(widthRatio, heightRatio) * transform.localScale;

        //if (open)
        mousePosition = Input.mousePosition;

        transform.position = Vector2.Lerp(transform.position, mousePosition + mouseOffset, Time.unscaledDeltaTime * mouseMoveSpeed);
    }

    public void Open(string skillName, string skillDescription)
    {
        open = true;

        skillNameText.text = skillName;
        skillDescriptionText.text = skillDescription;

        mousePosition = Input.mousePosition;
        transform.position = mousePosition + mouseOffset;
    }

    public void Close()
    {
        open = false;
    }
}
