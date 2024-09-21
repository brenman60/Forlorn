#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CitySpawnables))]
public class CitySpawnablesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CitySpawnables spawnables = (CitySpawnables)target;
        if (GUILayout.Button("AutoFill Spawnables"))
            AutoFillSections(spawnables);
    }

    private void AutoFillSections(CitySpawnables sections)
    {
        string searchFolder = "Assets/Scripts";
        sections.spawnables.Clear();

        string[] guids = AssetDatabase.FindAssets("t:CitySpawnable", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CitySpawnable spawnable = AssetDatabase.LoadAssetAtPath(assetPath, typeof(CitySpawnable)) as CitySpawnable;

            if (spawnable != null)
                sections.spawnables.Add(spawnable);
        }

        EditorUtility.SetDirty(sections);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
