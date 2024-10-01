using UnityEngine;
using UnityEngine.UI;

public class ObjectivesList : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup mainVerticalLayoutGroup;
    [SerializeField] private VerticalLayoutGroup contentVerticalLayoutGroup;
    [SerializeField] private ContentSizeFitter objectivesSizeFitter;
    [Space(20), SerializeField] private Transform objectivesList;

    private int lastObjectivesCount = 1;

    private void Start()
    {
        ResetListView();
    }

    private void Update()
    {
        if (objectivesList.childCount != lastObjectivesCount)
        {
            lastObjectivesCount = objectivesList.childCount;
            ResetListView();
        }
    }

    public void ResetListView()
    {
        Canvas.ForceUpdateCanvases();
        objectivesSizeFitter.SetLayoutVertical();
        mainVerticalLayoutGroup.SetLayoutVertical();
        contentVerticalLayoutGroup.SetLayoutVertical();
    }
}
