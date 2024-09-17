public class HealthEffect : Effect
{
    public HealthEffect(StatManager statManager, bool saveable) : base(statManager, saveable)
    {
    }

    public override void Tick()
    {
        HealthRegeneration();
        HealthRequirements();
    }

    private void HealthRegeneration()
    {
        statManager.stats[StatType.Health].currentValue += statManager.stats[StatType.HealthRegeneration].maxValue;
    }

    private void HealthRequirements()
    {
        float currentHunger = statManager.stats[StatType.Hunger].currentValue;
        float currentThirst = statManager.stats[StatType.Thirst].currentValue;

        if (currentHunger <= 10f || currentThirst <= 10f)
            statManager.stats[StatType.Health].currentValue -= statManager.stats[StatType.HealthRegeneration].maxValue * 2.5f;
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