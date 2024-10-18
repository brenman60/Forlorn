using Newtonsoft.Json;
using System.Collections.Generic;

public class ApartmentManager : ISaveData
{
    public List<string> apartments = new List<string>();

    private const string restIdentifier = "apartmentSleep";

    public void SetRestStatus(bool status)
    {
        StatManager statManager = RunManager.Instance.statManager;
        if (status)
        {
            TimeScaleManager.AddInfluence(restIdentifier, 3f);

            Effect sleepEffect = statManager.GetEffect(restIdentifier);
            if (sleepEffect != null)
            {
                sleepEffect.ReapplyTimer(90);
                sleepEffect.timePaused = true;
            }
            else
            {
                sleepEffect = new ComfortingSleepEffect(restIdentifier, true, true, 90, true);
                sleepEffect.timePaused = true;
                statManager.ApplyEffect(sleepEffect);
            }
        }
        else
        {
            TimeScaleManager.RemoveInfluence(restIdentifier);

            Effect sleepEffect = statManager.GetEffect(restIdentifier);
            if (sleepEffect != null)
                sleepEffect.timePaused = false;
        }
    }

    public bool HasApartment(string name)
    {
        return apartments.Contains(name);
    }

    public void PurchaseApartment(string name)
    {
        apartments.Add(name);
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[1]
        {
            JsonConvert.SerializeObject(apartments),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        apartments = JsonConvert.DeserializeObject<List<string>>(dataPoints[0]);
    }
}
