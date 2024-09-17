#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Skills))]
public class SkillsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Skills skills = (Skills)target;
        if (GUILayout.Button("AutoFill Skills"))
            AutoFillSkills(skills);
    }

    private void AutoFillSkills(Skills skills)
    {
        string searchFolder = "Assets/Scripts";
        skills.skills.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Skill", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Skill skill = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Skill)) as Skill;

            if (skill != null)
                skills.skills.Add(skill);
        }

        EditorUtility.SetDirty(skills);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
