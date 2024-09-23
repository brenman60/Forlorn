using TMPro;
using UnityEngine;

public class DayInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;

    private void Update()
    {
        dayText.text = "Day " + (GameManager.Instance.gameDays + 1);
    }
}
