using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void PlayClickSound(ForlornButton button)
    {
        SoundManager.Instance.PlayAudio(button.clickSound, false);
    }

    public void AddButton(ForlornButton button)
    {
        button.onClick.AddListener(() => PlayClickSound(button));

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }

    public static List<RaycastResult> GetUIsOnMouse()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        List<RaycastResult> finalResults = new List<RaycastResult>();
        foreach (RaycastResult result in results)
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                finalResults.Add(result);

        return finalResults;
    }
}
