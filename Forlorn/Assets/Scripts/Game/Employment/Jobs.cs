using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/Jobs List", fileName = "Jobs List")]
public class Jobs : ScriptableObject
{
    public List<Job> jobs = new List<Job>();
}
