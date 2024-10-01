using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemDrop : MonoBehaviour, ISaveData
{
    [SerializeField] private Items items;

    [Header("Light Customization")]
    [SerializeField] private float lightSpeed = 2.5f;
    [SerializeField] private float maxIntensity = 0.75f;
    [SerializeField] private float minIntensity = 0.25f;

    [Header("Hover Customization")]
    [SerializeField] private float hoverSpeed = 2.5f;

    [Space(15), SerializeField] private SpriteRenderer itemSprite;

    private Item item;
    private int amount;

    private float collectDebounce = 1f;
    private float waveOffsets;

    private Rigidbody2D rigidBody2D;
    private new Light2D light;

    private void Start()
    {
        light = GetComponent<Light2D>();
        waveOffsets = Random.Range(-180f, 180f);
    }

    private void Update()
    {
        collectDebounce -= Time.deltaTime;

        UpdateLightIntensity();
        UpdateHover();
    }

    private void UpdateLightIntensity()
    {
        float waveValue = (Mathf.Sin((Time.time + waveOffsets) * lightSpeed) + 1) / 2;
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, waveValue);
    }
    
    private void UpdateHover()
    {
        float waveValue = (Mathf.Sin((Time.time + waveOffsets) * hoverSpeed) + 1) / 2;
        itemSprite.transform.eulerAngles = new Vector3(itemSprite.transform.eulerAngles.x, Mathf.Lerp(0f, 180f, waveValue), itemSprite.transform.eulerAngles.z);
        itemSprite.transform.localPosition = new Vector3(itemSprite.transform.localPosition.x, Mathf.Lerp(-0.05f, 0.05f, waveValue), itemSprite.transform.localPosition.z);
    }

    public void UpdateItem(Item newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;

        itemSprite.sprite = item.icon;
    }

    public void UpdateVelocity(Vector2 velocity)
    {
        if (rigidBody2D == null) rigidBody2D = GetComponent<Rigidbody2D>();
        rigidBody2D.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision) => TryCollect(collision);

    private void OnTriggerStay2D(Collider2D collision) => TryCollect(collision);

    private void TryCollect(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collectDebounce <= 0 && Inventory.Instance.HasAnySpaceLeft(item))
        {
            Inventory.Instance.PutItem(item, amount);
            ItemDropManager.Instance.DestroyDrop(this);
            SoundManager.Instance.PlayAudio("ItemPickup", true, Random.Range(0.5f, 0.75f));
        }
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[3]
        {
            transform.position.ToJSON(),
            item.name,
            amount.ToString(),
        };

        return JsonConvert.SerializeObject(dataPoints);

    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        transform.position = dataPoints[0].ToVector3();
        UpdateItem(items.GetItemByName(dataPoints[1]), int.Parse(dataPoints[2]));
    }
}
