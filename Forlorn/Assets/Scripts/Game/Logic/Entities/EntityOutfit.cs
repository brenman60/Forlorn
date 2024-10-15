using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entities/New Entity Outfit", fileName = "New Entity Outfit")]
public class EntityOutfit : ScriptableObject
{
    public List<EntityOutfitState> outfitStates = new List<EntityOutfitState>();

    public EntityOutfitState GetOutfitState(EntityState state)
    {
        foreach (EntityOutfitState outfitState in outfitStates)
            if (outfitState.state == state)
                return outfitState;

        return new EntityOutfitState();
    }

    public Dictionary<EntityState, EntityPart> GetAllParts(EntityPartType type)
    {
        Dictionary<EntityState, EntityPart> parts = new Dictionary<EntityState, EntityPart>();
        foreach (EntityOutfitState outfitState in outfitStates)
            foreach (EntityPart part in outfitState.outfitParts)
                if (part.type == type)
                {
                    parts.Add(outfitState.state, part);
                    break;
                }

        return parts;
    }

    [Serializable]
    public struct EntityOutfitState
    {
        public EntityState state;
        public List<EntityPart> outfitParts;
    }
}
