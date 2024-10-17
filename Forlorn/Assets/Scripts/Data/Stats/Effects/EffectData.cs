using Newtonsoft.Json;
using System;

[Serializable]
public class EffectData
{
    public string effectTypeName;
    public string effectData;
    public string effectSpecificData;

    public EffectData(Effect effect)
    {
        if (effect == null) return;

        effectTypeName = effect.GetType().AssemblyQualifiedName;

        string[] effectDataPoints = new string[4]
        {
            effect.identifier,
            effect.timeRemoval.ToString(),
            effect.timeLeft.ToString(),
            effect.showsIcon.ToString(),
        };

        effectData = JsonConvert.SerializeObject(effectDataPoints);
        effectSpecificData = effect.GetSaveData();
    }

    public Effect CreateEffect()
    {
        Type effectType = Type.GetType(effectTypeName);
        if (effectType != null)
        {
            string[] effectDataPoints = JsonConvert.DeserializeObject<string[]>(effectData);

            Effect effect = (Effect)Activator.CreateInstance(effectType, new object[5] 
            {
                effectDataPoints[0],
                true, 
                bool.Parse(effectDataPoints[1]), 
                int.Parse(effectDataPoints[2]), 
                bool.Parse(effectDataPoints[3]) 
            });
            if (!string.IsNullOrEmpty(effectSpecificData)) effect.PutSaveData(effectSpecificData);
            return effect;
        }

        throw new Exception("Effect type not found.");
    }
}