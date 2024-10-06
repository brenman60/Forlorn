using System.Collections.Generic;
using UnityEngine;

public class DiscoveryObjective : MonoBehaviour
{
    [SerializeField] private string mainDiscoveryObjective;
    [SerializeField] private List<Objective> rewardingObjects = new List<Objective>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ObjectivesList.Instance.HasOnGoingObjective(mainDiscoveryObjective))
        {
            ObjectivesList.Instance.TryCompleteObjective(mainDiscoveryObjective);
            foreach (Objective rewardingObjective in rewardingObjects)
                ObjectivesList.Instance.CreateNewObjective(rewardingObjective);
        }
    }
}
