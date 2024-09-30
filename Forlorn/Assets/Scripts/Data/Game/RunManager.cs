using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;

public class RunManager : MonoBehaviour, ISaveData
{
    public static RunManager Instance { get; private set; }

    public StatManager statManager = new StatManager();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/RunManager"));
            managerObject.name = "RunManager";

            RunManager manager = managerObject.GetComponent<RunManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(TickStatManager), 1f, 1f);
    }

    private void TickStatManager()
    {
        if (GameManager.Instance.gameActive && TransitionUI.doneLoading) statManager.TickEffects();
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[3]
        {
            statManager.GetSaveData(),
            Inventory.Instance != null ? Inventory.Instance.GetSaveData() : string.Empty,
            SkillsUI.Instance != null ? SkillsUI.Instance.GetSaveData() : string.Empty,
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        statManager.ClearAll();

        statManager.ApplyEffect(new HungerEffect(false));
        statManager.ApplyEffect(new ThirstEffect(false));
        statManager.ApplyEffect(new HealthEffect(false));

        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        statManager.PutSaveData(dataPoints[0]);
        if (!string.IsNullOrEmpty(dataPoints[1])) StartCoroutine(WaitForInventory(dataPoints[1]));
        if (!string.IsNullOrEmpty(dataPoints[2])) StartCoroutine(WaitForSkillsUI(dataPoints[2]));
    }

    private IEnumerator WaitForInventory(string inventoryData)
    {
        yield return new WaitUntil(() => Inventory.Instance != null);
        Inventory.Instance.PutSaveData(inventoryData);
    }

    private IEnumerator WaitForSkillsUI(string skillsData)
    {
        yield return new WaitUntil(() => SkillsUI.Instance != null);
        SkillsUI.Instance.PutSaveData(skillsData);
    }
}

[Serializable]
public struct EmploymentInformation
{

}
