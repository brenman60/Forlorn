using UnityEngine;

public class TrainExit : Interactable
{
    [SerializeField] private DialogueNode startingNode;

    public override async void Interact()
    {
        await DialogueUI.Instance.AddDialogue(startingNode);
    }
}
