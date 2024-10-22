public class DisasterManager
{
    public void Init()
    {
        WorldGeneration.SectionChanged += ReloadDisaster;
    }

    private void ReloadDisaster()
    {
        
    }

    ~DisasterManager()
    {
        WorldGeneration.SectionChanged -= ReloadDisaster;
    }
}
