public class ComfortingSleepEffect : Effect
{
    public ComfortingSleepEffect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(identifier, saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    private const string healthModIdentifier = "comfortingSleepHealthMod";
    private const string hungerModIdentifier = "comfortingSleepHungerMod";
    private const string thirstModIdentifier = "comfortingSleepThirstMod";

    public override void OnApply()
    {
        base.OnApply();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.ApplyModifier(new StatModifier(healthModIdentifier, statManager.stats[StatType.HealthRegeneration], 2.25f, true));
        statManager.ApplyModifier(new StatModifier(hungerModIdentifier, statManager.stats[StatType.HungerDegradation], 0.5f, true));
        statManager.ApplyModifier(new StatModifier(thirstModIdentifier, statManager.stats[StatType.ThirstDegradation], 0.5f, true));
    }

    public override void OnRemoval()
    {
        base.OnRemoval();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.RemoveModifier(healthModIdentifier);
        statManager.RemoveModifier(hungerModIdentifier);
        statManager.RemoveModifier(thirstModIdentifier);
    }
}
