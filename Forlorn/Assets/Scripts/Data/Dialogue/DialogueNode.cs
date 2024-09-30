using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Node", fileName = "New Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public string dialogue = "New Dialogue";
    public List<DialogueOption> options = new List<DialogueOption>();
}

[Serializable]
public struct DialogueOption
{
    public string option;
    public DialogueNode nextNode;
    public UnityEvent onSelect;
}
