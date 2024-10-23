using System.Collections.Generic;
using UnityEngine;

public class DisasterManager
{
    private Dictionary<DisasterEvent, string> disasterResourcePaths = new Dictionary<DisasterEvent, string>()
    {
        [DisasterEvent.SnowStorm] = "Disasters/SnowStorm",
    };

    private Disaster currentDisaster;
    
    public void Init()
    {
        WorldGeneration.SectionChanged += ReloadDisaster;
        WorldGeneration.SectionRotted += SectionRotted;
        ReloadDisaster(WorldGeneration.section, WorldGeneration.);
    }

    private void SectionRotted(string section, DisasterEvent disaster)
    {
        ReloadDisaster(section, disaster);
    }

    private void ReloadDisaster(string section, DisasterEvent disaster)
    {
        if (currentDisaster != null)
        {
            currentDisaster.EndDisaster();
            MonoBehaviour.Destroy(currentDisaster.gameObject);
        }

        if (disaster == DisasterEvent.None) return;

        currentDisaster = MonoBehaviour.Instantiate(Resources.Load<GameObject>(disasterResourcePaths[disaster])).GetComponent<Disaster>();
        currentDisaster.StartDisaster();
    }

    ~DisasterManager()
    {
        WorldGeneration.SectionChanged -= ReloadDisaster;
        WorldGeneration.SectionRotted -= SectionRotted;
    }
}
