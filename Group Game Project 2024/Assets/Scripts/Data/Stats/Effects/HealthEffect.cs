public class HealthEffect : Effect
{
    public HealthEffect(bool saveable) : base(saveable)
    {
    }

    public override void Tick()
    {
        HealthRegeneration();
        HealthRequirements();
        HealthCheck();
    }

    private void HealthRegeneration()
    {
        RunManager.Instance.statManager.stats[StatType.Health].currentValue += RunManager.Instance.statManager.stats[StatType.HealthRegeneration].maxValue;
    }

    private void HealthRequirements()
    {
        float currentHunger = RunManager.Instance.statManager.stats[StatType.Hunger].currentValue;
        float currentThirst = RunManager.Instance.statManager.stats[StatType.Thirst].currentValue;

        if (currentHunger <= 10f || currentThirst <= 10f)
            RunManager.Instance.statManager.stats[StatType.Health].currentValue -= RunManager.Instance.statManager.stats[StatType.HealthRegeneration].maxValue * 2.5f;
    }

    private void HealthCheck()
    {
        if (RunManager.Instance.statManager.stats[StatType.Health].currentValue <= 0 && !DeathUI.PlayerDead)
            DeathUI.Instance.PlayerDeath();
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        
    }
}