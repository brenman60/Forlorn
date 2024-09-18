using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/UIManager"));
            managerObject.name = "UIManager";

            UIManager manager = managerObject.GetComponent<UIManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);
        }
    }

    private void PlayClickSound()
    {
        SoundManager.Instance.PlayAudio("UIClick", false);
    }

    public void AddButton(Button button)
    {
        button.onClick.AddListener(() => PlayClickSound());

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
