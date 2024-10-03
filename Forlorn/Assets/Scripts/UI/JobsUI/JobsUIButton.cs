using UnityEngine;

public class JobsUIButton : MonoBehaviour
{
    public void OnClick() => JobsUI.Instance.Toggle();
}
