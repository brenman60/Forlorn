using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UILineRenderer : MonoBehaviour
{
    private Image lineImage;
    private RectTransform rectTransform;

    public void DrawLine(Vector2 start, Vector2 end)
    {
        if (lineImage == null) lineImage = GetComponent<Image>();
        if (rectTransform == null) rectTransform = lineImage.GetComponent<RectTransform>();

        lineImage.enabled = true;

        print(start + " : " + end);
        float distance = Vector2.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.position = start;

        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ClearLine()
    {
        lineImage.enabled = false;
    }
}
