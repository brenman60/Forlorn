public class HungerEffect : Effect
{
    public HungerEffect(bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    public override void Tick()
    {
        base.Tick();

        float hungerDegrade = RunManager.Instance.statManager.stats[StatType.HungerDegradation].maxValue;
        float hungerSuppression = RunManager.Instance.statManager.stats[StatType.HungerSuppression].maxValue;
        RunManager.Instance.statManager.stats[StatType.Hunger].currentValue -= hungerDegrade / hungerSuppression;
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        
    }
}
