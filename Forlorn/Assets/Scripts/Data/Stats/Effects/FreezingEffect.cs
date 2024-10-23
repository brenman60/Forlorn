public class FreezingEffect : Effect
{
    public FreezingEffect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(identifier, saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    private const string movementSpeedModIdentifier = "freezingMovementSpeedMod";

    public override void OnApply()
    {
        base.OnApply();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.ApplyModifier(new StatModifier(movementSpeedModIdentifier, statManager.stats[StatType.MovementSpeed], 0.75f, true, false));
    }

    public override void OnRemoval()
    {
        base.OnRemoval();

        StatManager statManager = RunManager.Instance.statManager;
        statManager.RemoveModifier(movementSpeedModIdentifier);
    }
}
