using Newtonsoft.Json;
using UnityEngine;

public class Player : MonoBehaviour, ISaveData
{
    public static Player Instance { get; private set; }
    [HideInInspector] public bool movementLocked;

    [SerializeField] private float baseMovementSpeed = 1f;

    private Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!GameEndingUI.gameFinished && !movementLocked)
            Movement();
    }

    private void Movement()
    {
        Vector3 finalVelocity = Keybinds.Instance.controlMove.ReadValue<Vector2>();
        if (finalVelocity.x != 0) finalVelocity.x = Mathf.RoundToInt(finalVelocity.x);
        finalVelocity.y = 0;

        bool walking = finalVelocity.x != 0;
        bool sprinting = Keybinds.Instance.controlRun.ReadValue<float>() != 0;
        if (sprinting)
            finalVelocity.x *= 1.5f;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, finalVelocity.x == 0 ?
            transform.rotation.eulerAngles.y :
            (finalVelocity.x >= 0 ?
            0 : 180), transform.rotation.eulerAngles.z);

        float movementStat = RunManager.Instance.statManager.stats[StatType.MovementSpeed].currentValue;
        animator.speed = baseMovementSpeed * movementStat;
        finalVelocity *= baseMovementSpeed;
        finalVelocity *= movementStat;
        transform.position += finalVelocity * Time.deltaTime;

        animator.SetBool("walking", walking);
        animator.SetBool("running", sprinting && walking);
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[3]
        {
            transform.position.ToJSON(),
            transform.localScale.ToJSON(),
            transform.rotation.ToJSON(),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        transform.position = dataPoints[0].ToVector3();
        transform.localScale = dataPoints[1].ToVector3();
        transform.rotation = dataPoints[2].ToQuaternion();
    }
}
