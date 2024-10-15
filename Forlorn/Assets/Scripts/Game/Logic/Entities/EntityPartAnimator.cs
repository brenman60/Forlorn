using System.Collections.Generic;
using UnityEngine;

public class EntityPartAnimator : MonoBehaviour
{
    public EntityPartType type;
    [HideInInspector] public Dictionary<EntityState, EntityPart> entityParts = new Dictionary<EntityState, EntityPart>();

    private SpriteRenderer spriteRenderer;

    public void UpdatePartSprite(EntityState state, int index)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (KeyValuePair<EntityState, EntityPart> entityPart in entityParts)
            if (entityPart.Key == state && index <= entityPart.Value.sprites.Length - 1)
            {
                spriteRenderer.sprite = entityPart.Value.sprites[index];
                spriteRenderer.color = entityPart.Value.mainColor;
                break;
            }
    }
}
