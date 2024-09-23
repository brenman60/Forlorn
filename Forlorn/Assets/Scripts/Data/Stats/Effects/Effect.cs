public abstract class Effect : ISaveData
{
    public bool saveable { get; protected set; }

    public Effect(bool saveable)
    {
        this.saveable = saveable;
    }

    public abstract void Tick();

    public abstract string GetSaveData();

    public abstract void PutSaveData(string data);
}
