using UnityEngine;

public class Trashcan : Interactable, ISaveData
{
    [SerializeField] private DropLootTable[] drops;
    [Space(15)]
    [SerializeField] private Vector2 minDropVelocty;
    [SerializeField] private Vector2 maxDropVelocity;

    private bool opened;

    public override void Interact()
    {
        foreach (DropLootTable drop in drops)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= drop.chance)
                ItemDropManager.Instance.CreateDrop(drop.item, Random.Range(drop.minAmount, drop.maxAmount + 1), transform.position, new Vector2(Random.Range(minDropVelocty.x, maxDropVelocity.x), Random.Range(minDropVelocty.y, maxDropVelocity.y)));
        }

        opened = true;
        enabled = false;
    }

    public string GetSaveData()
    {
        return opened.ToString();
    }

    public void PutSaveData(string data)
    {
        opened = bool.Parse(data);
        if (opened) enabled = false;
    }
}
