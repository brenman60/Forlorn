public class HungerEffect : Effect
{
    public HungerEffect(StatManager statManager, bool saveable) : base(statManager, saveable)
    {
    }

    public override void Tick()
    {
        float hungerDegrade = statManager.stats[StatType.HungerDegradation].maxValue;
        float hungerSuppression = statManager.stats[StatType.HungerSuppression].maxValue;
        statManager.stats[StatType.Hunger].currentValue -= hungerDegrade / hungerSuppression;
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        throw new System.NotImplementedException();
    }
}
