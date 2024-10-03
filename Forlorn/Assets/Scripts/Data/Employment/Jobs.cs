using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/Jobs List", fileName = "Jobs List")]
public class Jobs : ScriptableObject
{
    public List<Job> jobs = new List<Job>();

    public Job GetJobByName(string name)
    {
        foreach (Job job in jobs)
            if (job.name == name)
                return job;

        Debug.LogError("Job with name '" + name + "' not found. Returning null.");
        return null;
    }
}
