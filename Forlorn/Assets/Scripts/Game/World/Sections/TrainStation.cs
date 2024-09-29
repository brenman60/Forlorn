using System.Collections;
using UnityEngine;

public class TrainStation : CityBlock
{
    [SerializeField] private float carriageStartPosition;
    [SerializeField] private float carriageCenterPosition;
    [SerializeField] private float carriageEndPosition;
    [SerializeField] private Transform carriageHolder;
    [SerializeField] private ParticleSystem stopSmoke;

    private float carriageTimer;

    protected override void InitSpawnables()
    {
        base.InitSpawnables();

        carriageTimer = 0f;

        StartCoroutine(StartingCutscene());
    }

    private IEnumerator StartingCutscene()
    {
        Player.Instance.gameObject.SetActive(false);
        PlayerCamera.Instance.SetNewFollowing(carriageHolder.GetChild(0), transform.position - new Vector3(length / 4, 0), transform.position, 1f);
        yield return StartCoroutine(MoveCarriage(1.5f, carriageStartPosition, carriageCenterPosition, 0.35f, 0.01f, true));
        Player.Instance.gameObject.SetActive(true);
        PlayerCamera.Instance.ResetFollowing();
    }

    private IEnumerator MoveCarriage(float delay, float startPos, float endPos, float startSpeed, float endSpeed, bool useStopSmoke)
    {
        yield return new WaitUntil(() => TransitionUI.doneLoading);

        carriageHolder.transform.localPosition = new Vector3(startPos, carriageHolder.transform.localPosition.y, carriageHolder.transform.localPosition.z);
        yield return new WaitForSeconds(delay);

        while (carriageTimer < 1f)
        {
            carriageTimer += Time.deltaTime * Mathf.Lerp(startSpeed, endSpeed, carriageTimer);
            float newX = Mathf.Lerp(startPos, endPos, carriageTimer);
            carriageHolder.transform.localPosition = new Vector3(newX, carriageHolder.transform.localPosition.y, carriageHolder.transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }

        carriageHolder.transform.localPosition = new Vector3(endPos, carriageHolder.transform.localPosition.y, carriageHolder.transform.localPosition.z);

        if (useStopSmoke) stopSmoke.Play();
    }
}
