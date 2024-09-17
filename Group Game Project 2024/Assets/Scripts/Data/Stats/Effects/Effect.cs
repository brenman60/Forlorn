public abstract class Effect : ISaveData
{
    protected StatManager statManager;
    public bool saveable { get; protected set; }

    public Effect(StatManager statManager, bool saveable)
    {
        this.statManager = statManager;
        this.saveable = saveable;
    }

    public abstract void Tick();

    public abstract string GetSaveData();

    public abstract void PutSaveData(string data);
}
