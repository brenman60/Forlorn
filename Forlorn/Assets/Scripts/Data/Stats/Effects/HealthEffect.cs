public class HealthEffect : Effect
{
    public HealthEffect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(identifier, saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    public override void Tick()
    {
        base.Tick();

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
        if (RunManager.Instance.statManager.stats[StatType.Health].currentValue <= 0 && !GameEndingUI.gameFinished)
            GameEndingUI.Instance.FinishGame("Died");
    }

    public override string GetSaveData()
    {
        return string.Empty;
    }

    public override void PutSaveData(string data)
    {
        
    }
}