using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : ISaveData
{
    public Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>()
    {
        [StatType.Health] = new Stat(100f, StatType.Health),
        [StatType.HealthRegeneration] = new Stat(1f, StatType.HealthRegeneration),
        [StatType.Hunger] = new Stat(100f, StatType.Hunger),
        [StatType.HungerDegradation] = new Stat(1f, StatType.HungerDegradation),
        [StatType.HungerSuppression] = new Stat(1f, StatType.HungerSuppression),
        [StatType.Thirst] = new Stat(100f, StatType.Thirst),
        [StatType.ThirstDegradation] = new Stat(1f, StatType.ThirstDegradation),
        [StatType.ThirstSuppression] = new Stat(1f, StatType.ThirstSuppression),
        [StatType.Luck] = new Stat(1f, StatType.Luck),
        [StatType.MovementSpeed] = new Stat(1f, StatType.MovementSpeed),

        [StatType.Money] = new Stat(25f, StatType.Money),
    };

    private List<StatModifier> modifiers = new List<StatModifier>(); // Modifiers are objects that change a stat's max value
    private List<Effect> effects = new List<Effect>(); // Effects are objects that tick every x seconds and can change a stat's value

    public void ApplyModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        modifier.Apply();
        RecalculateAllMax();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        modifier.Remove();
        RecalculateAllMax();
    }

    public void ApplyEffect(Effect effect)
    {
        effects.Add(effect);
    }

    public void RemoveEffect(Effect effect)
    {
        effects.Remove(effect);
    }

    public void TickEffects()
    {
        foreach (Effect effect in effects)
            effect.Tick();
    }

    private void RecalculateAllMax()
    {
        foreach (Stat stat in stats.Values)
            stat.Recalculate();

        foreach (var modifier in modifiers)
            modifier.Apply();
    }

    // Saving data is sort of weird for StatManager, but I think we can basically just take note of the modifiers that are active, save them to a JSON,
    // and then when loading them we can just reconstruct them back, and that should work theoretically.
    public string GetSaveData()
    {
        List<string> compiledData = new List<string>();

        List<string> compiledModifiers = new List<string>();
        foreach (StatModifier modifier in modifiers)
            compiledModifiers.Add(modifier.GetSaveData());

        List<EffectData> compiledEffects = new List<EffectData>();
        foreach (Effect effect in effects)
            if (effect.saveable) compiledEffects.Add(new EffectData(effect));

        Dictionary<StatType, string> compiledStats = new Dictionary<StatType, string>();
        foreach (Stat stat in stats.Values)
            compiledStats.Add(stat.type, stat.GetSaveData());

        compiledData.Add(JsonConvert.SerializeObject(compiledStats));
        compiledData.Add(JsonConvert.SerializeObject(compiledModifiers));
        compiledData.Add(JsonConvert.SerializeObject(compiledEffects));
        return JsonConvert.SerializeObject(compiledData);
    }

    public void PutSaveData(string data)
    {
        List<string> compiledData = JsonConvert.DeserializeObject<List<string>>(data);
        Dictionary<StatType, string> compiledStats = JsonConvert.DeserializeObject<Dictionary<StatType, string>>(compiledData[0]);
        List<string> compiledModifiers = JsonConvert.DeserializeObject<List<string>>(compiledData[1]);
        List<EffectData> compiledEffects = JsonConvert.DeserializeObject<List<EffectData>>(compiledData[2]);

        foreach (KeyValuePair<StatType, string> statData in compiledStats)
            stats[statData.Key].PutSaveData(statData.Value);

        foreach (string modifierDataRaw in compiledModifiers)
        {
            string[] modifierData = JsonConvert.DeserializeObject<string[]>(modifierDataRaw);

            // This sucks
            StatModifier modifier = new StatModifier(stats[JsonConvert.DeserializeObject<StatType>(modifierData[0])], 0, false);
            modifier.PutSaveData(modifierDataRaw);
            ApplyModifier(modifier);
        }

        foreach (EffectData effectData in compiledEffects)
        {
            Effect effect = effectData.CreateEffect();
            effects.Add(effect);
        }

        RecalculateAllMax();
    }
}

public enum StatType
{
    Health,
    HealthRegeneration,
    Hunger,
    HungerDegradation,
    Thirst,
    ThirstDegradation,
    Luck,
    MovementSpeed,
    HungerSuppression,
    ThirstSuppression,
    Money,
}
