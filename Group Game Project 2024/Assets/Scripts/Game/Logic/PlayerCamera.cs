using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private bool instantRelocate;
    [SerializeField] private float relocateSpeed;
    [Space(15), SerializeField] private Transform player;
    [SerializeField] private Vector3 positionOffset;

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, transform.position.z) + positionOffset;
        transform.position = instantRelocate ? newPosition : Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * relocateSpeed);
        //transform.LookAt(player, Vector3.up);
    }
}
