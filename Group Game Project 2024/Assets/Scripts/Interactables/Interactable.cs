using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactText;

    public abstract void Interact();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            InteractUI.Instance.AddInteraction(this);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            InteractUI.Instance.RemoveInteraction(this);
    }
}