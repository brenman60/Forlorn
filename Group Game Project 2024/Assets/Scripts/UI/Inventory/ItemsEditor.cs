#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Items))]
public class ItemsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Items items = (Items)target;
        if (GUILayout.Button("AutoFill Items"))
            AutoFillItems(items);
    }

    private void AutoFillItems(Items items)
    {
        string searchFolder = "Assets/Scripts";
        items.items.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Item)) as Item;

            if (item != null)
                items.items.Add(item);
        }

        EditorUtility.SetDirty(items);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
