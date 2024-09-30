using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Node", fileName = "New Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("Customization")]
    [TextArea(8, int.MaxValue)] public string dialogue = "New Dialogue";
    public Color textColor = Color.white;
    public Color backgroundColor = Color.black;
    [Tooltip("Amount of time before options appear, or if no options, the time it shows until disappearing.")] public float displayTime = 2.5f;
    public bool useTypewriter = true;
    public float typewriterSpeed = 0.025f;

    [Header("Options")]
    public List<DialogueOption> options = new List<DialogueOption>();
}

[Serializable]
public struct DialogueOption
{
    [Header("Customization")]
    public string optionText;
    public DialogueNode nextNode;
    [Header("Selection")]
    public string onSelectClass;
    public string onSelectMethod;
    public string[] onSelectArguments;
}
