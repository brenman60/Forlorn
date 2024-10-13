#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectUIDatas))]
public class EffectUIDatasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectUIDatas datas = (EffectUIDatas)target;
        if (GUILayout.Button("AutoFill Data"))
            AutoFillDatas(datas);
    }

    private void AutoFillDatas(EffectUIDatas datas)
    {
        string searchFolder = "Assets/Scripts";
        datas.datas.Clear();

        string[] guids = AssetDatabase.FindAssets("t:EffectUIData", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EffectUIData item = AssetDatabase.LoadAssetAtPath(assetPath, typeof(EffectUIData)) as EffectUIData;

            if (item != null)
                datas.datas.Add(item);
        }

        EditorUtility.SetDirty(datas);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
