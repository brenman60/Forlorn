public class ThirstEffect : Effect
{
    public ThirstEffect(StatManager statManager, bool saveable) : base(statManager, saveable)
    {
    }

    public override void Tick()
    {
        float thirstDegrade = statManager.stats[StatType.ThirstDegradation].maxValue;
        float thirstSuppression = statManager.stats[StatType.ThirstSuppression].maxValue;
        statManager.stats[StatType.Thirst].currentValue -= thirstDegrade / thirstSuppression;
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        
    }
}
