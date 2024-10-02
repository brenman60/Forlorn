using TMPro;
using UnityEngine;

public class DialogueOptionUI : MonoBehaviour
{
    public TextMeshProUGUI optionText;
    public CanvasGroup canvasGroup;
    [SerializeField] private GameObject infoButton;

    private void Update()
    {
        infoButton.SetActive(GameManager.isMobile);
    }
}
