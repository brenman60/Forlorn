using UnityEngine;

public class CameraTimeVisuals : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        float gameTime = GameManager.Instance.gameTime;
        Color32 timeColor = Color.black;

        switch (GameManager.Instance.dayStatus)
        {
            case DayStatus.Morning:
                timeColor = new Color32(255, 218, 191, 255);
                break;
            case DayStatus.Midday:
                timeColor = new Color32(191, 244, 255, 255);
                break;
            case DayStatus.Night:
                timeColor = new Color32(5, 0, 22, 255);
                break;
        }

        mainCam.backgroundColor = MoveTowardsColor(mainCam.backgroundColor, timeColor, Time.deltaTime * 0.1f);
    }

    private Color MoveTowardsColor(Color current, Color target, float maxDelta)
    {
        float r = Mathf.MoveTowards(current.r, target.r, maxDelta);
        float g = Mathf.MoveTowards(current.g, target.g, maxDelta);
        float b = Mathf.MoveTowards(current.b, target.b, maxDelta);

        return new Color(r, g, b, 1);
    }
}
