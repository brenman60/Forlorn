using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    public string interactText;
    public Sound interactSound;
    public bool disappearsPlayer;
    public bool interactable { get; protected set; } = true;

    public abstract void Interact();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && interactable)
            InteractUI.Instance.AddInteraction(this);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            InteractUI.Instance.RemoveInteraction(this);
    }
}
