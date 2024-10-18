using Newtonsoft.Json;
using System;
using UnityEngine;

public class Stat : ISaveData
{
    public StatType type;
    public event Action<float> valueChanged;
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
            valueChanged?.Invoke(currentValue_);
        }
    }
    private float currentValue_;
    private float valueAddition;
    private float valueMultiplier;

    private bool hasNoMax;

    public Stat(float baseValue, StatType type, bool hasNoMax)
    {
        this.baseValue = baseValue;
        this.type = type;
        this.currentValue_ = baseValue;
        this.hasNoMax = hasNoMax;
        Recalculate();
    }

    public float maxValue { get { if (!hasNoMax) return (baseValue + valueAddition) * valueMultiplier; else return float.MaxValue; } }

    public void AddModifier(float value, bool isMultiplicative)
    {
        if (isMultiplicative)
            valueMultiplier *= value;
        else
            valueAddition += value;

        if (hasNoMax)
            currentValue = (baseValue + valueAddition) * valueMultiplier;
    }

    public void RemoveModifier(float value, bool isMultiplicative)
    {
        if (isMultiplicative)
            valueMultiplier /= value;
        else
            valueAddition -= value;

        if (!hasNoMax)
            currentValue = (baseValue + valueAddition) * valueMultiplier;
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
