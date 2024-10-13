public class ThirstEffect : Effect
{
    public ThirstEffect(bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    public override void Tick()
    {
        base.Tick();

        float thirstDegrade = RunManager.Instance.statManager.stats[StatType.ThirstDegradation].maxValue;
        float thirstSuppression = RunManager.Instance.statManager.stats[StatType.ThirstSuppression].maxValue;
        RunManager.Instance.statManager.stats[StatType.Thirst].currentValue -= thirstDegrade / thirstSuppression;
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        
    }
}
