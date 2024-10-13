using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/New Effect UI Data List", fileName = "New Effect UI Data List")]
public class EffectUIDatas : ScriptableObject
{
    public List<EffectUIData> datas;

    public EffectUIData GetEffectDataByType(TypeReference type)
    {
        foreach (EffectUIData effect in datas) 
            if (effect.effectType == type)
                return effect;

        Debug.LogError("Could not find effect data with type '" + type + "'. Returning null.");
        return null;
    }
}
