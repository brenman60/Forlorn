using Newtonsoft.Json;
using System;
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
        statManager.ApplyEffect(new HungerEffect(statManager, false));
        statManager.ApplyEffect(new ThirstEffect(statManager, false));
        statManager.ApplyEffect(new HealthEffect(statManager, false));

        InvokeRepeating(nameof(TickStatManager), 1f, 1f);
    }

    private void TickStatManager()
    {
        if (GameManager.Instance.gameActive) statManager.TickEffects();
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[1]
        {
            statManager.GetSaveData(),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        statManager.PutSaveData(dataPoints[0]);
    }
}

[Serializable]
public struct EmploymentInformation
{

}
