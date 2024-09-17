#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sounds))]
public class SoundsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Sounds sounds = (Sounds)target;
        if (GUILayout.Button("AutoFill Sounds"))
            AutoFillSounds(sounds);
    }

    private void AutoFillSounds(Sounds sounds)
    {
        string searchFolder = "Assets/Scripts";
        sounds.sounds.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Sound", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sound sound = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sound)) as Sound;

            if (sound != null)
                sounds.sounds.Add(sound);
        }

        EditorUtility.SetDirty(sounds);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
