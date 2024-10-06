using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private Slider percentageSlider;
    [SerializeField] private TextMeshProUGUI percentageText;

    private TimeTask task;

    public void Setup(TimeTask task)
    {
        this.task = task;
        TaskManager.tasksChanged += TasksChanged;

        taskText.text = task.taskName;
        UpdatePercentage();
    }

    private void Update()
    {
        UpdatePercentage();
    }

    private void UpdatePercentage()
    {
        float percentageTime = task.taskTime - task.currentTime;

        percentageSlider.maxValue = task.taskTime;
        percentageSlider.value = percentageTime;

        int currentPercentage = Mathf.RoundToInt((percentageTime / task.taskTime) * 100f);
        percentageText.text = currentPercentage + "%";
    }

    private void TasksChanged(TimeTask task, bool active)
    {
        if (!active && task == this.task)
            Destroy(gameObject);
    }
}
