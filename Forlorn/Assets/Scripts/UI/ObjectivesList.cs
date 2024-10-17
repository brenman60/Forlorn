using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesList : MonoBehaviour, ISaveData
{
    public static ObjectivesList Instance { get; private set; }

    [Header("Customization")]
    [SerializeField] private Color objectiveCompleteFlash;

    [Header("References")]
    [SerializeField] private VerticalLayoutGroup mainVerticalLayoutGroup;
    [SerializeField] private VerticalLayoutGroup contentVerticalLayoutGroup;
    [SerializeField] private ContentSizeFitter objectivesSizeFitter;
    [Space(20), SerializeField] private Transform objectivesList;
    [SerializeField] private GameObject objectiveTemplate;

    private static List<Objective> objectives = new List<Objective>();
    private Dictionary<string, GameObject> objectivesUI = new Dictionary<string, GameObject>();
    private int lastObjectivesCount = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Objective objective in objectives)
            CreateObjectiveUI(objective);

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

    private void CreateObjectiveUI(Objective objective)
    {
        if (objectiveTemplate == null || objectivesList == null || objectivesUI.ContainsKey(objective.identifier)) return;

        GameObject objectiveUI = Instantiate(objectiveTemplate, objectivesList);
        TextMeshProUGUI objectiveText = objectiveUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        objectiveText.text = objective.visibleText;

        objectiveUI.SetActive(true);
        objectivesUI.Add(objective.identifier, objectiveUI);
    }

    public void ResetListView()
    {
        Canvas.ForceUpdateCanvases();
        objectivesSizeFitter.SetLayoutVertical();
        mainVerticalLayoutGroup.SetLayoutVertical();
        contentVerticalLayoutGroup.SetLayoutVertical();
    }

    public void CreateNewObjective(Objective newObjective)
    {
        objectives.Add(newObjective);
        CreateObjectiveUI(newObjective);
    }

    public bool HasOnGoingObjective(string identifier)
    {
        bool hasObjective = false;
        foreach (Objective objective in objectives)
            if (objective.identifier == identifier)
            {
                hasObjective = true;
                break;
            }

        return hasObjective;
    }

    public void TryCompleteObjective(string identifier, bool failed = false)
    {
        foreach (Objective objective in objectives.ToArray())
            if (objective.identifier == identifier && HasOnGoingObjective(objective.identifier))
            {
                objectives.Remove(objective);
                StartCoroutine(CompleteObjectiveUI(identifier, failed));
            }
    }

    private IEnumerator CompleteObjectiveUI(string identifier, bool failed)
    {
        GameObject objectiveUI = objectivesUI[identifier];
        CanvasGroup canvasGroup = objectiveUI.GetComponent<CanvasGroup>();
        Toggle toggle = objectiveUI.transform.GetChild(1).GetComponent<Toggle>();
        GameObject crossout = objectiveUI.transform.GetChild(2).gameObject;
        crossout.SetActive(failed);
        Color originalToggleColor = toggle.graphic.color;

        toggle.isOn = true;
        toggle.graphic.color = objectiveCompleteFlash;

        while (toggle.graphic.color != originalToggleColor)
        {
            Color currentColor = toggle.graphic.color;
            toggle.graphic.color = new Color(
                Mathf.MoveTowards(currentColor.r, originalToggleColor.r, Time.deltaTime * 15f),
                Mathf.MoveTowards(currentColor.g, originalToggleColor.g, Time.deltaTime * 15f),
                Mathf.MoveTowards(currentColor.b, originalToggleColor.b, Time.deltaTime * 15f)
                );
            yield return new WaitForEndOfFrame();
        }

        while (canvasGroup.alpha != 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 0.25f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(objectiveUI);
        objectivesUI.Remove(identifier);
    }

    public void ClearAll()
    {
        foreach (Transform previousObjectiveUI in objectivesList)
            if (previousObjectiveUI.gameObject != objectiveTemplate)
                Destroy(previousObjectiveUI.gameObject);

        objectivesUI.Clear();

        objectives.Clear();
    }

    public string GetSaveData()
    {
        return JsonConvert.SerializeObject(objectives);
    }

    public void PutSaveData(string data)
    {
        foreach (Transform previousObjectiveUI in objectivesList)
            if (previousObjectiveUI.gameObject != objectiveTemplate)
                Destroy(previousObjectiveUI.gameObject);

        objectivesUI.Clear();

        objectives = JsonConvert.DeserializeObject<List<Objective>>(data);
        foreach (Objective objective in objectives)
            CreateObjectiveUI(objective);
    }
}

[Serializable]
public struct Objective
{
    public string identifier;
    public string visibleText;

    public Objective(string identifier, string visibleText)
    {
        this.identifier = identifier;
        this.visibleText = visibleText;
    }
}
