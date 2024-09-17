using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/Sounds List", fileName = "Sounds List")]
public class Sounds : ScriptableObject
{
    public List<Sound> sounds;

    public Sound GetSoundByName(string name)
    {
        foreach (Sound sound in sounds)
            if (sound.name == name) 
                return sound;

        Debug.LogError("Sound with name '" + name + "' not found. Returning null.");
        return null;
    }
}
