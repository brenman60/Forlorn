using TMPro;
using UnityEngine;

public class MoneyInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Update()
    {
        moneyText.text = RunManager.Instance.statManager.stats[StatType.Money].currentValue.ToString();
    }
}
