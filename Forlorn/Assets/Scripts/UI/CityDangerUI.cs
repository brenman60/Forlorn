using UnityEngine;

public class CityDangerUI : MonoBehaviour
{
    private float showTimer;
    private bool visible;

    private float shakeTimer;

    private float initialX;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        initialX = transform.position.x;

        canvasGroup = GetComponent<CanvasGroup>();
        WorldGeneration.SectionRotted += SectionRotted;
    }

    private void SectionRotted(string section, DisasterEvent disaster)
    {
        if (section == WorldGeneration.section)
        {
            showTimer = 10f;

            SoundManager.Instance.PlayAudio("SafetyNotification", false, 1f);
        }
    }

    private void Update()
    {
        showTimer -= Time.deltaTime;
        shakeTimer -= Time.deltaTime;

        visible = showTimer > 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, visible ? 1f : 0f, Time.deltaTime * 2f);

        if (shakeTimer <= 0f)
        {
            transform.position = new Vector3(initialX + Random.Range(-4f, 4f), transform.position.y);
            shakeTimer = Random.Range(0.025f, 0.05f);
        }
    }

    private void OnDestroy()
    {
        WorldGeneration.SectionRotted -= SectionRotted;
    }
}
