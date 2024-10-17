public abstract class Effect : ISaveData
{
    public string identifier { get; private set; }

    public bool timePaused;

    public bool timeRemoval { get; private set; }
    public int timeLeft { get; private set; }
    public bool showsIcon { get; private set; }

    public bool saveable { get; protected set; }

    public Effect(string identifier, bool saveable, bool timeRemoval, int timeLeft, bool showsIcon)
    {
        this.identifier = identifier;
        this.saveable = saveable;
        this.timeRemoval = timeRemoval;
        this.timeLeft = timeLeft;
        this.showsIcon = showsIcon;
    }

    public virtual void OnApply()
    {

    }

    public virtual void Tick()
    {
        if (!timePaused)
            timeLeft--;
    }

    public virtual void OnRemoval()
    {

    }

    public virtual string GetSaveData()
    {
        return string.Empty;
    }

    public virtual void PutSaveData(string data)
    {

    }
}
