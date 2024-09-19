using System;

[Serializable]
public class EffectData
{
    public string EffectTypeName;
    public string effectData;

    public EffectData(Effect effect)
    {
        if (effect == null) return;

        EffectTypeName = effect.GetType().AssemblyQualifiedName;
        effectData = effect.GetSaveData();
    }

    public Effect CreateEffect()
    {
        Type effectType = Type.GetType(EffectTypeName);
        if (effectType != null)
        {
            Effect effect = (Effect)Activator.CreateInstance(effectType, RunManager.Instance.statManager, true);
            effect.PutSaveData(effectData);
            return effect;
        }

        throw new Exception("Effect type not found.");
    }
}