#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillsUI))]
public class SkillsUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillsUI skills = (SkillsUI)target;
        if (GUILayout.Button("Add Missing Skills"))
            AddMissingSkills(skills);
    }

    private void AddMissingSkills(SkillsUI skills)
    {
        SkillUI[] skillObjects = FindObjectsOfType<SkillUI>();
        foreach (SkillUI skillUI in skillObjects)
            if (!skills.skills.Contains(skillUI))
                skills.skills.Add(skillUI);

        EditorUtility.SetDirty(skills);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif