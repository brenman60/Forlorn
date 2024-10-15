using UnityEngine;

[CreateAssetMenu(menuName = "Entities/New Entity Part", fileName = "New Entity Part")]
public class EntityPart : ScriptableObject
{
    public EntityPartType type;
    public Sprite[] sprites;
    public Color mainColor = Color.white;
}

public enum EntityPartType
{
    Headwear,
    Head,
    Neckwear,
    Undershirt,
    Overshirt,
    Arms,
    Sleeves,
    Legs,
    Pants,
    Shoes
}
