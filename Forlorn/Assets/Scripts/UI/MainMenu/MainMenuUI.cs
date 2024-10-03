using System;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject continueRunsScreen;
    [SerializeField] private GameObject runTemplate;

    public void NewRun()
    {
        string randomId = string.Empty;
        for (int i = 0; i < 50; i++)
        {
            int randomValue = UnityEngine.Random.Range(0, 26);
            randomId += Convert.ToChar(randomValue + 65);
        }

        GameManager.runId = randomId;
        RunManager.Instance.statManager.ClearAll();
        SaveSystem.SaveRunData();
        TransitionUI.Instance.TransitionTo("StartingCutscene");
    }

    public void OpenSettings()
    {
        SettingsUI.Instance.Toggle(true);
    }

    public void ContinueRun()
    {
        LoadContinuableRuns();
        continueRunsScreen.SetActive(true);
    }

    private void LoadContinuableRuns()
    {
        foreach (Transform previousRun in runTemplate.transform.parent)
            if (previousRun.gameObject != runTemplate) Destroy(previousRun.gameObject);

        foreach (string runName in SaveSystem.GetAllRuns())
        {
            GameObject runObject = Instantiate(runTemplate, runTemplate.transform.parent);

            RunInfo info = runObject.GetComponent<RunInfo>();
            info.runId = runName;

            runObject.SetActive(true);
        }
    }

    public void Quit()
    {
#if !UNITY_WEBGL
        Application.Quit();
#endif
    }
}
