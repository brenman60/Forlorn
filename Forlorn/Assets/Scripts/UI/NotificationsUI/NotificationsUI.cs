using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsUI : MonoBehaviour
{
    public static NotificationsUI Instance { get; private set; }

    [SerializeField] private GameObject taskTemplate;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNotification(string notificationText)
    {
        StartCoroutine(CreateNotificationUI(notificationText));
    }

    private IEnumerator CreateNotificationUI(string notificationText)
    {
        GameObject notifObject = Instantiate(taskTemplate, taskTemplate.transform.parent);
        TextMeshProUGUI notifText = notifObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        notifText.text = notificationText;

        notifObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.SetLayoutVertical();
    }
}
