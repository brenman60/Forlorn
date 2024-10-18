using UnityEngine;

public class ImportantIcon : MonoBehaviour
{
    public bool important { get; private set; }

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, important ? 1f : 0f, Time.unscaledDeltaTime * 4f);
    }

    public void SetImportance(bool important)
    {
        this.important = important;
    }
}
