using UnityEngine;
using UnityEngine.UI;

public class ForlornButton : Button
{
    protected override void Start()
    {
        base.Start();

        if (Application.isPlaying)
            UIManager.Instance.AddButton(this);
    }
}
