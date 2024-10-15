using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    protected PersonType aiType = PersonType.Wander;
    protected List<float> movementNodes = new List<float>();

    [SerializeField] private GameObject body;
    [SerializeField] private Animator animator;

    protected bool running;
    private bool walking;

    private float defaultMovementSpeed;
    private float movementDebounce;
    private float movementSpeed;
    private int movementNodeIndex;

    private Coroutine movementCoroutine;

    private void Start()
    {
        defaultMovementSpeed = Random.Range(0.4f, 0.8f);
    }

    private void Update()
    {
        movementDebounce -= Time.deltaTime;
        if (movementDebounce <= 0)
        {
            if (movementCoroutine != null) StopCoroutine(movementCoroutine);

            switch (aiType)
            {
                case PersonType.Wander:
                    movementCoroutine = StartCoroutine(MoveToPosition(Random.Range(WorldGeneration.worldBounds.Item1, WorldGeneration.worldBounds.Item2), Random.Range(15f, 25f), Random.Range(0, 5) == 0));
                    break;
                case PersonType.Patrol:
                    movementCoroutine = StartCoroutine(MoveToPosition(movementNodes[movementNodeIndex], Random.Range(10f, 12f), false));
                    break;
            }

            movementNodeIndex++;
            if (movementNodeIndex >= movementNodes.Count)
                movementNodeIndex = 0;
            else if (movementNodeIndex < 0)
                movementNodeIndex = movementNodes.Count - 1;
        }

        UpdateMovementStatus();
    }

    private void UpdateMovementStatus()
    {
        walking = movementSpeed != 0;

        animator.speed = Mathf.Abs(movementSpeed);
        animator.SetBool("walking", walking);
        animator.SetBool("running", walking && running);
    }

    public IEnumerator MoveToPosition(float xPos, float nextPosTime, bool run)
    {
        movementDebounce = nextPosTime;
        running = run;

        while (Mathf.Abs(transform.position.x - xPos) > 0.05f)
        {
            movementSpeed = transform.position.x < xPos ? defaultMovementSpeed : -defaultMovementSpeed;
            if (run) movementSpeed *= 1.75f;

            if (transform.position.x < WorldGeneration.worldBounds.Item1 || transform.position.x > WorldGeneration.worldBounds.Item2)
                break;

            transform.position += new Vector3(movementSpeed, 0) * Time.deltaTime;
            transform.eulerAngles = movementSpeed > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

            yield return new WaitForEndOfFrame();
        }

        movementSpeed = 0f;
    }
}

public enum PersonType
{
    Wander,
    Patrol,
}
