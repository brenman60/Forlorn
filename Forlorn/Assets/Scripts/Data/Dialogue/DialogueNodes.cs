using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node List", fileName = "Node List")]
public class DialogueNodes : ScriptableObject
{
    public List<DialogueNode> nodes = new List<DialogueNode>();

    public DialogueNode GetDialogueNodeByName(string name)
    {
        foreach (DialogueNode node in nodes)
            if (node.name == name)
                return node;

        Debug.LogError("DialogueNode with name '" + name + "' not found. Returning null.");
        return null;
    }
}
