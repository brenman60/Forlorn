using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksUI : MonoBehaviour
{
    [SerializeField] private GameObject taskTemplate;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private Dictionary<TimeTask, GameObject> taskUIs = new Dictionary<TimeTask, GameObject>();

    private void Start()
    {
        TaskManager.tasksChanged += TasksChanged;

        if (RunManager.Instance.taskManager.tasks.Count != 0)
            foreach (TimeTask task in RunManager.Instance.taskManager.tasks)
                StartCoroutine(CreateTaskUI(task));
    }

    private void TasksChanged(TimeTask task, bool active)
    {
        if (!active) return;
        StartCoroutine(CreateTaskUI(task));
    }

    private IEnumerator CreateTaskUI(TimeTask task)
    {
        GameObject taskObject = Instantiate(taskTemplate, taskTemplate.transform.parent);
        TaskUI taskUI = taskObject.GetComponent<TaskUI>();
        taskObject.name = task.taskName;
        taskUI.Setup(task);

        taskObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.SetLayoutVertical();
    }
}
