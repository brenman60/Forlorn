using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Apartments", fileName = "Has Apartments")]
public class HasApartmentRequirement : DialogueRequirement
{
    [SerializeField] private List<ApartmentRequirement> requirements = new List<ApartmentRequirement>();

    public override bool MeetsRequirement()
    {
        bool isValid = true;

        foreach (ApartmentRequirement requirement in requirements)
        {
            bool hasApartment = RunManager.Instance.apartmentManager.HasApartment(requirement.apartment);

            switch (requirement.type)
            {
                case ApartmentRequirementType.CantHave:
                    if (hasApartment)
                        isValid = false;
                    break;
                case ApartmentRequirementType.NeedsToHave:
                    if (!hasApartment)
                        isValid = false;
                    break;
            }
        }

        return isValid;
    }

    [Serializable]
    protected struct ApartmentRequirement
    {
        public string apartment;
        public ApartmentRequirementType type;
    }

    protected enum ApartmentRequirementType
    {
        CantHave,
        NeedsToHave,
    }
}
