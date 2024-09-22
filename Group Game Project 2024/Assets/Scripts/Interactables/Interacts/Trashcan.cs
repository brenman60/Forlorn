using UnityEngine;

public class Trashcan : Interactable, ISaveData
{
    [SerializeField] private DropLootTable[] drops;
    [Space(15), SerializeField] private Vector2 minDropVelocty;
    [SerializeField] private Vector2 maxDropVelocity;
    [Space(15), SerializeField] private Rigidbody2D lidRigidbody;

    public override void Interact()
    {
        foreach (DropLootTable drop in drops)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= drop.chance)
                ItemDropManager.Instance.CreateDrop(drop.item, Random.Range(drop.minAmount, drop.maxAmount + 1), transform.position, new Vector2(Random.Range(minDropVelocty.x, maxDropVelocity.x), Random.Range(minDropVelocty.y, maxDropVelocity.y)));
        }

        lidRigidbody.constraints = RigidbodyConstraints2D.None;
        lidRigidbody.velocity = new Vector2(Random.Range(minDropVelocty.x, maxDropVelocity.x), 1f);
        lidRigidbody.angularVelocity = Mathf.Atan2(lidRigidbody.velocity.y, lidRigidbody.velocity.x) * Mathf.Rad2Deg;

        SoundManager.Instance.PlayAudio("TrashcanOpen", false, 0.5f);

        interactable = false;
    }

    public string GetSaveData()
    {
        return interactable.ToString();
    }

    public void PutSaveData(string data)
    {
        interactable = bool.Parse(data);
        if (!interactable) Destroy(lidRigidbody.gameObject);
    }
}
