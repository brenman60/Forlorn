using Newtonsoft.Json;
using UnityEngine;

public class Stat : ISaveData
{
    public StatType type;
    public float baseValue { get; private set; }
    public float currentValue
    {
        get
        {
            return currentValue_;
        }
        set
        {
            if (value > maxValue)
                currentValue_ = maxValue;
            else
                currentValue_ = value;

            currentValue_ = Mathf.Clamp(currentValue_, 0, float.MaxValue);
        }
    }
    private float currentValue_;
    private float valueAddition;
    private float valueMultiplier;

    public Stat(float baseValue, StatType type, bool hasNoMax)
    {
        this.baseValue = baseValue;
        this.type = type;
        this.currentValue_ = baseValue;
        Recalculate();

        if (hasNoMax)
            this.baseValue = float.MaxValue;
    }

    public float maxValue
    {
        get { return (baseValue + valueAddition) * valueMultiplier; }
    }

    public void AddModifier(float value, bool isMultiplicative)
    {
        if (isMultiplicative)
            valueMultiplier *= value;
        else
            valueAddition += value;
    }

    public void RemoveModifier(float value, bool isMultiplicative)
    {
        if (isMultiplicative)
            valueMultiplier /= value;
        else
            valueAddition -= value;
    }

    public void Recalculate()
    {
        valueAddition = 0f;
        valueMultiplier = 1f;
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[1]
        {
            currentValue_.ToString(),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        currentValue_ = float.Parse(dataPoints[0]);
    }
}
