using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    public static Camera mainCam { get; private set; }

    [SerializeField] private float relocateSpeed;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    private Transform followTarget;
    private float moveSpeed;
    private Vector2 minPos;
    private Vector2 maxPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;
        ResetFollowing();
    }

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z) + positionOffset;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveSpeed);
        if (minPos != Vector2.zero && maxPos != Vector2.zero)
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), transform.position.z);
    }

    public void SetNewFollowing(Transform target, Vector2 minPosition = new(), Vector2 maxPosition = new(), float relocateSpeed = 0)
    {
        followTarget = target;

        moveSpeed = relocateSpeed;
        minPos = minPosition;
        maxPos = maxPosition;
    }

    public void ResetFollowing()
    {
        followTarget = player;

        moveSpeed = relocateSpeed;
        minPos = minPosition;
        maxPos = maxPosition;
    }

    public static bool IsPositionOnScreen(Vector3 worldPosition)
    {
        Vector3 screenPoint = mainCam.WorldToScreenPoint(worldPosition);

        if (screenPoint.z > 0 &&
            screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
            screenPoint.y >= 0 && screenPoint.y <= Screen.height)
        {
            return true;
        }
        else
            return false;
    }
}
