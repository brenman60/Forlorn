using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/New Sound", fileName = "New Sound")]
public class Sound : ScriptableObject
{
    public AudioClip soundClip;
    public SoundType soundType;
}
