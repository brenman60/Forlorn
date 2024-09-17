using UnityEngine;

public class CameraOscillate : MonoBehaviour
{
    [SerializeField] private float shakeMagnitude = 10f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 randomShake = new Vector3(
            Random.Range(-1f, 1f) * shakeMagnitude,
            Random.Range(-1f, 1f) * shakeMagnitude,
            Random.Range(-1f, 1f) * shakeMagnitude
        );

        transform.position = originalPosition + randomShake;
    }
}
