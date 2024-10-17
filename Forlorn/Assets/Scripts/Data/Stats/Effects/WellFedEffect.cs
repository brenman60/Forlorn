public class WellFedEffect : Effect
{
    public WellFedEffect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(identifier, saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    private const string speedModIdentifier = "wellFedSpeedMod";
    private const string hungerModIdentifier = "wellFedHungerMod";

    public override void OnApply()
    {
        base.OnApply();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.ApplyModifier(new StatModifier(speedModIdentifier, statManager.stats[StatType.MovementSpeed], 1.5f, true));
        statManager.ApplyModifier(new StatModifier(hungerModIdentifier, statManager.stats[StatType.HungerSuppression], 2f, true));
    }

    public override void OnRemoval()
    {
        base.OnRemoval();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.RemoveModifier(speedModIdentifier);
        statManager.RemoveModifier(hungerModIdentifier);
    }
}
