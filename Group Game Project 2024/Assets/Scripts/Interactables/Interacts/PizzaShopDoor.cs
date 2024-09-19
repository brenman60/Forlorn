using UnityEngine;

public class PizzaShopDoor : Interactable
{
    [SerializeField] private bool exterior = true;
    [SerializeField] private GameObject exteriorTeleportPoint;
    [SerializeField] private GameObject interiorTeleportPoint;

    public override void Interact()
    {
        if (exterior)
            Player.Instance.transform.position = interiorTeleportPoint.transform.position;
        else
            Player.Instance.transform.position = exteriorTeleportPoint.transform.position;
    }
}
