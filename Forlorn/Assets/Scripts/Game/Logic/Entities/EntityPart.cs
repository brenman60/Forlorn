using UnityEngine;

[CreateAssetMenu(menuName = "Entities/New Entity Part", fileName = "New Entity Part")]
public class EntityPart : ScriptableObject
{
    public Sprite[] sprites;
    public Color mainColor = Color.white;
}
