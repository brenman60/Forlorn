using Newtonsoft.Json;
using UnityEngine;

public class StatModifier : ISaveData
{
    public string identifier { get; private set; }
    public bool saveable { get; private set; }
    private Stat stat;
    private float modifier;
    private bool isMulti;

    public StatModifier(string identifier, Stat stat, float modifier, bool isMulti, bool saveable)
    {
        this.identifier = identifier;
        this.stat = stat;
        this.modifier = modifier;
        this.isMulti = isMulti;
        this.saveable = saveable;
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
        string[] data = new string[5]
        {
            identifier,
            ((int)stat.type).ToString(),
            modifier.ToString(),
            isMulti.ToString(),
            saveable.ToString(),
        };

        return JsonConvert.SerializeObject(data);
    }

    public void PutSaveData(string data)
    {
        throw new System.Exception("StatModifier.PutSaveData should not need be to called. Create an empty modifier using the StatModifier Constructor instead.");
    }
}
