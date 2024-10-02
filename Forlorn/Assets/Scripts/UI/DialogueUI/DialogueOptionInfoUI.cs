using UnityEngine;

public class DialogueOptionInfoUI : MonoBehaviour
{
    [SerializeField] private float startYPos;
    [SerializeField] private float openHeight = 350f;
    [SerializeField] private float openSpeed = 15f;

    private RectTransform rectTransform;
    private bool open;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.position = new Vector3(rectTransform.position.x, startYPos + rectTransform.sizeDelta.y / 2);
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(rectTransform.sizeDelta.x, open ? openHeight : 0f), Time.deltaTime * openSpeed);
    }

    public void ViewInfo(DialogueOption option)
    {
        Toggle();
    }

    public void Toggle(bool toggle)
    {
        open = toggle;
    }

    public void Toggle()
    {
        open = !open;
    }
}
