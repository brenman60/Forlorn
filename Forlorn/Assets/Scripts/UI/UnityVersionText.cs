using TMPro;
using UnityEngine;

public class UnityVersionText : MonoBehaviour
{
    [SerializeField] private string prefix;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = prefix + Application.unityVersion;
    }
}
