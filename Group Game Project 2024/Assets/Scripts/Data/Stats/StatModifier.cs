using Newtonsoft.Json;

public class StatModifier : ISaveData
{
    private Stat stat;
    private float modifier;
    private bool isMulti;

    public StatModifier(Stat stat, float modifier, bool isMulti)
    {
        this.stat = stat;
        this.modifier = modifier;
        this.isMulti = isMulti;
    }

    public void Apply()
    {
        stat.AddModifier(modifier, isMulti);
    }

    public void Remove()
    {
        stat.RemoveModifier(modifier, isMulti);
    }

    public string GetSaveData()
    {
        string[] data = new string[3]
        {
            JsonConvert.SerializeObject(stat.type),
            modifier.ToString(),
            isMulti.ToString(),
        };

        return JsonConvert.SerializeObject(data);
    }

    public void PutSaveData(string data)
    {
        string[] savedData = JsonConvert.DeserializeObject<string[]>(data);
        stat.type = JsonConvert.DeserializeObject<StatType>(savedData[0]);
        modifier = float.Parse(savedData[1]);
        isMulti = bool.Parse(savedData[2]);
    }
}
