using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Entity : MonoBehaviour
{
    [Header("Customization")]
    public EntityState state;
    public int frame;

    [Header("References")]
    [SerializeField] private List<EntityPartAnimator> parts = new List<EntityPartAnimator>();

    private EntityState lastState;
    private int lastFrame;

    private SortingGroup sortingGroup;

    private void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();

        foreach (EntityPartAnimator part in parts)
            part.UpdatePartSprite(state, frame);
    }

    private void Update()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(transform.position.x * 100f);

        if (lastState != state || lastFrame != frame)
        {
            lastState = state;
            lastFrame = Mathf.Clamp(frame, 0, int.MaxValue);

            foreach (EntityPartAnimator part in parts)
                part.UpdatePartSprite(state, frame);
        }
    }
    
    public void PlaySound(string soundName)
    {
        SoundManager.Instance.PlayAudio(soundName, true, 0.25f, transform);
    }
}

public enum EntityState
{
    Idle,
    Walking,
    Running,
}
