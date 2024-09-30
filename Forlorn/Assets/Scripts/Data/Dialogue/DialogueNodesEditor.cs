#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueNodes))]
public class DialogueNodesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueNodes dialogueNodes = (DialogueNodes)target;
        if (GUILayout.Button("AutoFill Nodes"))
            AutoFillNodes(dialogueNodes);
    }

    private void AutoFillNodes(DialogueNodes dialogueNodes)
    {
        string searchFolder = "Assets/Scripts";
        dialogueNodes.nodes.Clear();

        string[] guids = AssetDatabase.FindAssets("t:DialogueNode", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogueNode)) as DialogueNode;

            if (node != null)
                dialogueNodes.nodes.Add(node);
        }

        EditorUtility.SetDirty(dialogueNodes);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
