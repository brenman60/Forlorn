using TypeReferences;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/New Effect UI Data", fileName = "New Effect UI Data")]
public class EffectUIData : ScriptableObject
{
    [Inherits(typeof(Effect))]
    public TypeReference effectType = null;

    public string visibleName = "Example Name";
    public Sprite icon = null;
    [TextArea(5, int.MaxValue)] public string description = "Example Description";
}
