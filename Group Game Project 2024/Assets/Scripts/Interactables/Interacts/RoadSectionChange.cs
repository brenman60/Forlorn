using UnityEngine;

public class RoadSectionChange : Interactable
{
    [SerializeField] private bool entering;

    public override void Interact()
    {
        WorldGeneration.Instance.SaveSection();

        int currentSectionNum = int.Parse(WorldGeneration.section);
        WorldGeneration.section = (currentSectionNum + (entering ? 1 : -1)).ToString();
        TransitionUI.Instance.TransitionTo("Game");
    }
}
