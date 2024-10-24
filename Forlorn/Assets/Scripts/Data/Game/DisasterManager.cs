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
        WorldGeneration.SectionRotted += SectionRotted;
        WorldGeneration.SectionChanged += SectionChanged;
    }

    private void SectionChanged(string arg1, DisasterEvent disaster)
    {
        ReloadDisaster(string.Empty, DisasterEvent.None);
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

        if (disaster == DisasterEvent.None || section != WorldGeneration.section) return;

        currentDisaster = MonoBehaviour.Instantiate(Resources.Load<GameObject>(disasterResourcePaths[disaster])).GetComponent<Disaster>();
        currentDisaster.StartDisaster();
    }

    ~DisasterManager()
    {
        WorldGeneration.SectionRotted -= SectionRotted;
    }
}
