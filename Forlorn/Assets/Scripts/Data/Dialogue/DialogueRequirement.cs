using UnityEngine;

public abstract class DialogueRequirement : ScriptableObject
{
    public string requirementText;

    public abstract bool MeetsRequirement();
}
