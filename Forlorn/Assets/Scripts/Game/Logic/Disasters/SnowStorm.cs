public class SnowStorm : Disaster
{
    public override void EndDisaster()
    {
        base.EndDisaster();

        Effect freezingEffect = RunManager.Instance.statManager.GetEffect("snowStormFreezing");
        if (freezingEffect != null)
            freezingEffect.timePaused = true;
    }

    public override void StartDisaster()
    {
        base.StartDisaster();

        if (!RunManager.Instance.statManager.HasEffect(typeof(FreezingEffect)))
        {
            FreezingEffect freezingEffect = new FreezingEffect("snowStormFreezing", true, true, 15, true);
            freezingEffect.timePaused = true;
            RunManager.Instance.statManager.ApplyEffect(freezingEffect);
        }
        else
        {
            Effect freezingEffect = RunManager.Instance.statManager.GetEffect("snowStormFreezing");
            freezingEffect.timePaused = true;
            freezingEffect.ReapplyTimer(15);
        }
    }
}
