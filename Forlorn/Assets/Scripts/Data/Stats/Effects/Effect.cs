public abstract class Effect : ISaveData
{
    public bool timeRemoval { get; private set; }
    public int timeLeft { get; private set; }
    public bool showsIcon { get; private set; }

    public bool saveable { get; protected set; }

    public Effect(bool saveable, bool timeRemoval, int timeLeft, bool showsIcon)
    {
        this.saveable = saveable;
        this.timeRemoval = timeRemoval;
        this.timeLeft = timeLeft;
        this.showsIcon = showsIcon;
    }

    public virtual void Tick()
    {
        timeLeft--;
    }

    public abstract string GetSaveData();

    public abstract void PutSaveData(string data);
}
