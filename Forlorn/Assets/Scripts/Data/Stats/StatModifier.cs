using Newtonsoft.Json;

public class StatModifier : ISaveData
{
    public string identifier { get; private set; }
    private Stat stat;
    private float modifier;
    private bool isMulti;

    public StatModifier(string identifier, Stat stat, float modifier, bool isMulti)
    {
        this.identifier = identifier;
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
        string[] data = new string[4]
        {
            identifier,
            JsonConvert.SerializeObject(stat.type),
            modifier.ToString(),
            isMulti.ToString(),
        };

        return JsonConvert.SerializeObject(data);
    }

    public void PutSaveData(string data)
    {
        string[] savedData = JsonConvert.DeserializeObject<string[]>(data);
        identifier = savedData[0];
        stat.type = JsonConvert.DeserializeObject<StatType>(savedData[1]);
        modifier = float.Parse(savedData[2]);
        isMulti = bool.Parse(savedData[3]);
    }
}
