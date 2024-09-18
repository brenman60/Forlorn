#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomButtonMenu : MonoBehaviour
{
    [MenuItem("GameObject/UI/Forlorn Button", false, 10)]
    private static void CreateForlornUIButton(MenuCommand menuCommand)
    {
        GameObject customButtonObj = new GameObject("Forlorn Button");
        RectTransform customButtonObjRectTransform = customButtonObj.AddComponent<RectTransform>();
        customButtonObjRectTransform.sizeDelta = new Vector2(160f, 30f);
        customButtonObj.AddComponent<Image>();
        customButtonObj.AddComponent<ForlornButton>();

        GameObject textObj = new GameObject("Text");
        RectTransform textObjRectTransform = textObj.AddComponent<RectTransform>();
        textObjRectTransform.anchorMin = Vector2.zero;
        textObjRectTransform.anchorMax = Vector2.one;
        textObjRectTransform.sizeDelta = Vector2.zero;
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Button";
        text.alignment = TextAlignmentOptions.Center;
        GameObjectUtility.SetParentAndAlign(textObj, customButtonObj);

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas = canvasObj.GetComponent<Canvas>();

            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        GameObjectUtility.SetParentAndAlign(customButtonObj, canvas.gameObject);

        Undo.RegisterCreatedObjectUndo(customButtonObj, "Create " + customButtonObj.name);

        Selection.activeObject = customButtonObj;
    }
}
#endif