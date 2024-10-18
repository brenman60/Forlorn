using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TypeReferences;

public class StatManager : ISaveData
{
    public static event Action<StatModifier, bool> modifiersChanged;
    public static event Action<Effect, bool> effectsChanged;

    public const int skillPointProgressMax = 100;

    public Dictionary<StatType, Stat> defaultStats = new Dictionary<StatType, Stat>()
    {
        [StatType.DayLength] = new Stat(1f, StatType.DayLength, true),

        [StatType.Health] = new Stat(100f, StatType.Health, false),
        [StatType.HealthRegeneration] = new Stat(1f, StatType.HealthRegeneration, false),
        [StatType.Hunger] = new Stat(100f, StatType.Hunger, false),
        [StatType.HungerDegradation] = new Stat(1f, StatType.HungerDegradation, false),
        [StatType.HungerSuppression] = new Stat(1f, StatType.HungerSuppression, false),
        [StatType.Thirst] = new Stat(100f, StatType.Thirst, false),
        [StatType.ThirstDegradation] = new Stat(1f, StatType.ThirstDegradation, false),
        [StatType.ThirstSuppression] = new Stat(2f, StatType.ThirstSuppression, false),
        [StatType.Luck] = new Stat(1f, StatType.Luck, true), // until i can make a "luck manager" type thing this will probably go unused
        [StatType.MovementSpeed] = new Stat(1f, StatType.MovementSpeed, true),

        [StatType.Money] = new Stat(25f, StatType.Money, true),
        [StatType.JobPointMultiplier] = new Stat(1f, StatType.JobPointMultiplier, true),
        [StatType.InventoryMax] = new Stat(16f, StatType.InventoryMax, true),

        // Specific skills for job applications
        [StatType.Communication] = new Stat(1f, StatType.Communication, true),
        [StatType.Cooking] = new Stat(1f, StatType.Cooking, true),

        // Skill points
        [StatType.SkillPoints] = new Stat(0f, StatType.SkillPoints, true),
        [StatType.SkillPointsProgress] = new Stat(0f, StatType.SkillPointsProgress, true),
    };

    public Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    public List<StatModifier> modifiers { get; private set; } = new List<StatModifier>(); // Modifiers are objects that change a stat's max value
    public List<Effect> effects { get; private set; } = new List<Effect>(); // Effects are objects that tick every x seconds and can change a stat's value

    private List<StatModifier> defaultModifiers = new List<StatModifier>();

    private List<Effect> defaultEffects = new List<Effect>()
    {
        new HealthEffect("defaultHealth", false, false, 0, false),
        new HungerEffect("defaultHunger", false, false, 0, false),
        new ThirstEffect("defaultThirst", false, false, 0, false),
        new SkillEffect("defaultSkill", false, false, 0, false),
    };

    public void ApplyModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        modifier.Apply();
        RecalculateAllMax();

        modifiersChanged?.Invoke(modifier, true);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        modifier.Remove();
        RecalculateAllMax();

        modifiersChanged?.Invoke(modifier, false);
    }

    public void RemoveModifier(string identifier)
    {
        foreach (StatModifier modifier in modifiers.ToArray())
            if (modifier.identifier == identifier)
            {
                modifiers.Remove(modifier);
                modifier.Remove();
                RecalculateAllMax();

                modifiersChanged?.Invoke(modifier, false);
                break;
            }
    }

    public void ApplyEffect(Effect effect)
    {
        effects.Add(effect);
        effectsChanged?.Invoke(effect, true);

        effect.OnApply();
    }

    public bool HasEffect(TypeReference effectType)
    {
        foreach (Effect effect in effects)
            if (effect.GetType() == effectType.Type)
                return true;

        return false;
    }

    public Effect GetEffect(string identifier)
    {
        foreach (Effect effect in effects)
            if (effect.identifier == identifier)
                return effect;

        return null;
    }

    public void RemoveEffect(Effect effect)
    {
        effects.Remove(effect);
        effectsChanged?.Invoke(effect, false);

        effect.OnRemoval();
    }

    public void RemoveEffect(string identifier)
    {
        foreach (Effect effect in effects.ToArray())
            if (effect.identifier == identifier)
            {
                effects.Remove(effect);
                effectsChanged?.Invoke(effect, false);

                effect.OnRemoval();
                break;
            }
    }

    public void Tick()
    {
        foreach (Effect effect in effects.ToArray())
        {
            effect.Tick();

            if (effect.timeLeft <= 0 && effect.timeRemoval)
                RemoveEffect(effect);
        }
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
            if (modifier.saveable)
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
        ClearAll();

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
            StatModifier modifier = new StatModifier(modifierData[0], stats[(StatType)int.Parse(modifierData[1])], float.Parse(modifierData[2]), bool.Parse(modifierData[3]), bool.Parse(modifierData[4]));
            ApplyModifier(modifier);
        }

        foreach (EffectData effectData in compiledEffects)
        {
            Effect effect = effectData.CreateEffect();
            ApplyEffect(effect);
        }
    }

    public void ClearAll()
    {
        stats = defaultStats;
        modifiers = defaultModifiers;
        effects = defaultEffects;
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
    JobPointMultiplier,
    InventoryMax,
    Communication,
    Cooking,
    DayLength,
    SkillPoints,
    SkillPointsProgress,
}
