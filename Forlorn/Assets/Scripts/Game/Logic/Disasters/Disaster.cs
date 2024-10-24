using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Parallax))]
public abstract class Disaster : MonoBehaviour
{
    [SerializeField] private VolumeProfile seperateVolume;

    protected Parallax parallax;

    public virtual void StartDisaster()
    {
        if (seperateVolume != null) PostProcessManager.Instance.AddProfile(seperateVolume);
    }

    public virtual void Start()
    {
        parallax = GetComponent<Parallax>();
        parallax.target = Player.Instance.transform;
    }

    public virtual void EndDisaster()
    {
        if (seperateVolume != null) PostProcessManager.Instance.RemoveProfile(seperateVolume);
    }
}
