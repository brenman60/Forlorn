using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/City Sections", fileName = "City Sections")]
public class CitySections : ScriptableObject
{
    public List<CitySection> sections;

    public CitySection GetSectionByName(string name)
    {
        foreach (CitySection section in sections)
            if (section.name == name)
                return section;

        Debug.LogError("Section with name '" + name + "' does not exist.");
        return null;
    }
}
