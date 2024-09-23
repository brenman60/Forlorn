using UnityEngine;

public abstract class Ticker : MonoBehaviour
{
    public virtual void Update()
    {
        Tick();
    }

    public abstract void Tick();
}
