using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour, ISaveData
{
    public static ItemDropManager Instance { get; private set; }

    [SerializeField] private GameObject dropTemplate;

    private List<ItemDrop> drops = new List<ItemDrop>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreateDrop(Item item, int amount, Vector2 spawnPosition, Vector2 spawnVelocity)
    {
        ItemDrop drop = CreateDropObject();
        drop.transform.position = spawnPosition;
        drop.UpdateItem(item, amount);
        drop.UpdateVelocity(spawnVelocity);
    }

    public void DestroyDrop(ItemDrop drop)
    {
        drops.Remove(drop);
        Destroy(drop.gameObject);
    }

    private ItemDrop CreateDropObject()
    {
        GameObject dropObject = Instantiate(dropTemplate, transform);
        ItemDrop itemDrop = dropObject.GetComponent<ItemDrop>();
        dropObject.SetActive(true);

        drops.Add(itemDrop);
        return itemDrop;
    }

    public string GetSaveData()
    {
        print("Getting drops");
        List<string> dropsData = new List<string>();
        foreach (ItemDrop itemDrop in drops)
            dropsData.Add(itemDrop.GetSaveData());

        return JsonConvert.SerializeObject(dropsData);
    }

    public void PutSaveData(string data)
    {
        print("Putting drops");
        List<string> dropsData = JsonConvert.DeserializeObject<List<string>>(data);
        foreach (string dropData in dropsData)
        {
            ItemDrop dropObject = CreateDropObject();
            dropObject.PutSaveData(dropData);
        }
    }
}

[Serializable]
public struct DropLootTable
{
    public Item item;
    public int minAmount;
    public int maxAmount;
    public float chance;
}
