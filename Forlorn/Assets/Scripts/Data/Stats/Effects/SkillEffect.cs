public class SkillEffect : Effect
{
    public SkillEffect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon) : base(identifier, saveable, timeRemoval, timeLeft, showsIcon)
    {
    }

    public override void Tick()
    {
        base.Tick();

        Stat pointsProgress = RunManager.Instance.statManager.stats[StatType.SkillPointsProgress];
        pointsProgress.currentValue++;
        if (pointsProgress.currentValue >= StatManager.skillPointProgressMax)
        {
            pointsProgress.currentValue -= StatManager.skillPointProgressMax;
            RunManager.Instance.statManager.stats[StatType.SkillPoints].currentValue++;
        }
    }
}
