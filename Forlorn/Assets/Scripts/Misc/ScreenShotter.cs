using UnityEngine;

public class ScreenShotter : MonoBehaviour
{
    private int screenshotIndex = 0;

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Period))
        {
            ScreenCapture.CaptureScreenshot($"screenshot{screenshotIndex}.png");
            screenshotIndex++;
        }
    }
}
