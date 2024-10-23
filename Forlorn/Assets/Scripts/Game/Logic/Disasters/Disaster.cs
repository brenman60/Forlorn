using UnityEngine;

[RequireComponent(typeof(Parallax))]
public abstract class Disaster : MonoBehaviour
{
    protected Parallax parallax;

    public abstract void StartDisaster();

    public virtual void Start()
    {
        parallax = GetComponent<Parallax>();
        parallax.target = Player.Instance.transform;
    }

    public abstract void EndDisaster();
}
