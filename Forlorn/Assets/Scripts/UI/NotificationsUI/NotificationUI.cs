using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private float displayTime = 5f;
    [SerializeField] private float disappearSlowdown = 5f;

    private float existanceTime;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        existanceTime += Time.deltaTime;

        if (existanceTime > displayTime)
        {
            float difference = (existanceTime - displayTime) / disappearSlowdown;
            float newAlpha = Mathf.Lerp(1f, 0f, difference);
            canvasGroup.alpha = newAlpha;

            if (newAlpha <= 0)
                Destroy(gameObject);
        }
    }
}
