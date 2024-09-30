using UnityEngine;

public class SkillsUIButton : MonoBehaviour
{
    public void OnClick() => SkillsUI.Instance.Toggle();
}
