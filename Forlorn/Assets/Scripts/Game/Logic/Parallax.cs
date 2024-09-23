using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform target;
    public float intensity;

    private float startPos;

    private void Start()
    {
        startPos = transform.position.x;
    }

    private void Update()
    {
        float dist = target.position.x * intensity;

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}