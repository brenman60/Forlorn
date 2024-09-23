using UnityEngine;

public class TrashcanLid : MonoBehaviour
{
    private bool impacted;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (impacted) return;
        impacted = true;

        SoundManager.Instance.PlayAudio("TrashcanLidImpact", true, 0.25f);
    }
}
