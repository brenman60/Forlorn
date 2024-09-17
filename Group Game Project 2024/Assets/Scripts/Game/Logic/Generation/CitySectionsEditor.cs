#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CitySections))]
public class CitySectionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CitySections sections = (CitySections)target;
        if (GUILayout.Button("AutoFill Sections"))
            AutoFillSections(sections);
    }

    private void AutoFillSections(CitySections sections)
    {
        string searchFolder = "Assets/Scripts";
        sections.sections.Clear();

        string[] guids = AssetDatabase.FindAssets("t:CitySection", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CitySection section = AssetDatabase.LoadAssetAtPath(assetPath, typeof(CitySection)) as CitySection;

            if (section != null)
                sections.sections.Add(section);
        }

        EditorUtility.SetDirty(sections);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
