using UnityEngine;

public class DialogueOptionInfoUI : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float openHeight = 350f;
    [SerializeField] private float openSpeed = 15f;
    [Header("References")]
    [SerializeField] private Transform requirementsList;
    [SerializeField] private GameObject requirementTemplate;
    [Space(20), SerializeField] private Transform rewardsList;
    [SerializeField] private GameObject rewardTemplate;

    private RectTransform rectTransform;
    private bool open;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(rectTransform.sizeDelta.x, open ? openHeight : 0f), Time.deltaTime * openSpeed);
    }

    public void ViewInfo(DialogueOption option)
    {
        Toggle();

        foreach (Transform previousRequirement in requirementsList) if (previousRequirement.gameObject != requirementTemplate) Destroy(previousRequirement.gameObject);
        foreach (Transform previousReward in rewardsList) if (previousReward.gameObject != rewardTemplate) Destroy(previousReward.gameObject);

        foreach (DialogueOptionRequirement requirement in option.optionRequirements)
        {
            GameObject requirementObject = Instantiate(requirementTemplate, requirementsList);
            requirementObject.GetComponent<DialogueOptionRequirementUI>().Init(requirement);
            requirementObject.SetActive(true);
        }
    }

    public void Toggle(bool toggle)
    {
        open = toggle;
    }

    public void Toggle()
    {
        open = !open;
    }
}
