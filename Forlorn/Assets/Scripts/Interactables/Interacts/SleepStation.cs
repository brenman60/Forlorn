using UnityEngine;

public class SleepStation : Interactable
{
    [SerializeField] private string sleepText = "Sleep?";
    [SerializeField] private string wakeText = "Wake Up?";

    private int randomIdentifier;
    private bool playerSleeping;

    private void Start()
    {
        randomIdentifier = Random.Range(0, 9999);
        interactText = sleepText;
    }

    public override void Interact()
    {
        if (!playerSleeping)
        {
            Player.Instance.movementLocked = true;
            interactText = wakeText;
            InteractUI.Instance.AddInteraction(this);

            TimeScaleManager.AddInfluence(name + randomIdentifier, 3f);
        }
        else
        {
            Player.Instance.movementLocked = false;
            interactText = sleepText;

            TimeScaleManager.RemoveInfluence(name + randomIdentifier);
        }

        playerSleeping = !playerSleeping;
    }
}
