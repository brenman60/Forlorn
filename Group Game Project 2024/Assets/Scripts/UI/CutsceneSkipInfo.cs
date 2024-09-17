using UnityEngine;

public class CutsceneSkipInfo : MonoBehaviour
{
    [SerializeField] private float showTime;
    [SerializeField] private float fullOpacityTime;
    [SerializeField] private float decreaseOpacitySpeed;
    [Space(15), SerializeField] private float yIncrease;

    private CanvasGroup canvasGroup;

    private float elaspedTime;
    private float initialY;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        initialY = transform.position.y;
    }

    private void Update()
    {
        elaspedTime += Time.deltaTime * decreaseOpacitySpeed;

        float lerpProgress = Mathf.Clamp(elaspedTime - fullOpacityTime, 0, float.MaxValue) / showTime;
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(initialY, initialY + yIncrease, lerpProgress));
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, lerpProgress);
    }
}
