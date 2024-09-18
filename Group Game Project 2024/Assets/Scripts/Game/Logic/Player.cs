using Newtonsoft.Json;
using UnityEngine;

public class Player : MonoBehaviour, ISaveData
{
    public static Player Instance { get; private set; }

    [SerializeField] private float baseMovementSpeed = 1f;
    [SerializeField] private float baseJumpHeight = 1f;

    private float jumpDebounce;
    private bool isGrounded = true;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Movement();
        Jump();
    }

    private void Movement()
    {
        Vector3 finalVelocity = new Vector2();
        if (Input.GetKey(Keybinds.GetKeybind(KeyType.Left)))
            finalVelocity.x += -1f;

        if (Input.GetKey(Keybinds.GetKeybind(KeyType.Right)))
            finalVelocity.x += 1f;

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

    private void Jump()
    {
        jumpDebounce -= Time.deltaTime;
        if (Input.GetKey(Keybinds.GetKeybind(KeyType.Jump)) && isGrounded && jumpDebounce <= 0f)
        {
            isGrounded = false;
            jumpDebounce = .25f;
            rigidbody2D.AddForce(new Vector2(0, baseJumpHeight));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = false;
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
