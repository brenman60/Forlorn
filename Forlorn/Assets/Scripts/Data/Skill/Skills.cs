using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill List", fileName = "Skill List")]
public class Skills : ScriptableObject
{
    public List<Skill> skills;

    public Skill GetSkillByName(string name)
    {
        foreach (Skill skill in skills)
            if (skill.name == name)
                return skill;

        Debug.LogError("Skill with the name '" + name + "' not found, returning null.");
        return null;
    }

    public Skill GetSkillBySaveName(string savename)
    {
        foreach (Skill skill in skills)
            if (skill.saveName == savename)
                return skill;

        Debug.LogError("Skill with the name '" + savename + "' not found, returning null.");
        return null;
    }
}
