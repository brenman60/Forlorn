using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private float movementTime;
    private float movingDirection;
    private bool walking;
    private bool running;

    private float movementDebounce;

    private void Update()
    {
        movementDebounce -= Time.deltaTime;
        if (movementDebounce <= 0)
        {
            running = Random.Range(0, 2) == 0;

            movementTime = Random.Range(10f, 25f);
            movingDirection = (Random.Range(0, 2) == 0 ? -0.5f : 0.5f) * Random.Range(0.9f, 1.2f);
            if (running) movingDirection *= 1.75f;

            movementDebounce = movementTime + Random.Range(1f, 5f);
        }

        if (movementTime > 0)
        {
            movementTime -= Time.deltaTime;

            transform.position += new Vector3(movingDirection, 0f) * Time.deltaTime;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, movingDirection == 0 ?
                transform.rotation.eulerAngles.y :
                (movingDirection >= 0 ?
                0 : 180), transform.rotation.eulerAngles.z);
        }

        if (transform.position.x < WorldGeneration.worldBounds.Item1 || transform.position.x > WorldGeneration.worldBounds.Item2)
            movingDirection = -movingDirection;

        UpdateMovementStatus();
    }

    private void UpdateMovementStatus()
    {
        walking = movementTime > 0;

        animator.speed = Mathf.Abs(movingDirection);
        animator.SetBool("walking", walking);
        animator.SetBool("running", walking && running);
    }
}

public class PersonNode
{

}
