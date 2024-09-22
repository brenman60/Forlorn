public class ThirstEffect : Effect
{
    public ThirstEffect(bool saveable) : base(saveable)
    {
    }

    public override void Tick()
    {
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
