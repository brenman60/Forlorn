using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityPartAnimator : MonoBehaviour
{
    [SerializeField] private List<EntityPartState> entityParts = new List<EntityPartState>();

    private SpriteRenderer spriteRenderer;

    public void UpdatePartSprite(EntityState state, int index)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (EntityPartState entityPart in entityParts)
            if (entityPart.state == state && index <= entityPart.entityPart.sprites.Length - 1)
            {
                spriteRenderer.sprite = entityPart.entityPart.sprites[index];
                spriteRenderer.color = entityPart.entityPart.mainColor;
                break;
            }
    }

    [Serializable]
    private struct EntityPartState
    {
        public EntityState state;
        public EntityPart entityPart;
    }
}
