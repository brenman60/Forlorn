#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shops))]
public class ShopsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Shops shops = (Shops)target;
        if (GUILayout.Button("AutoFill Shops"))
            AutoFillShops(shops);
    }

    private void AutoFillShops(Shops shops)
    {
        string searchFolder = "Assets/Scripts";
        shops.shops.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Shop", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Shop shop = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Shop)) as Shop;

            if (shop != null)
                shops.shops.Add(shop);
        }

        EditorUtility.SetDirty(shops);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
