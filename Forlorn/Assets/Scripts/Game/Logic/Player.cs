using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, ISaveData
{
    public static Player Instance { get; private set; }

    [SerializeField] private float baseMovementSpeed = 1f;

    private Animator animator;

    private PlayerInput playerControls;
    private InputAction controlMove;

    private void Awake()
    {
        Instance = this;

        playerControls = new PlayerInput();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!DeathUI.PlayerDead)
            Movement();
    }

    private void Movement()
    {
        Vector3 finalVelocity = controlMove.ReadValue<Vector2>();
        finalVelocity.y = 0;

        bool walking = finalVelocity.x != 0;
        bool sprinting = Input.GetKey(Keybinds.GetKeybind(KeyType.Sprint));
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

    public void PlayFootStepAudio()
    {
        SoundManager.Instance.PlayAudio("Footstep", true, 0.25f);
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

    private void OnEnable()
    {
        controlMove = playerControls.Player.Movement;
        controlMove.Enable();
    }

    private void OnDisable()
    {
        controlMove.Disable();
    }
}
