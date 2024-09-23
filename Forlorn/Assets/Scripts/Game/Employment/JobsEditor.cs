#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Jobs))]
public class JobsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Jobs jobs = (Jobs)target;
        if (GUILayout.Button("AutoFill Jobs"))
            AutoFillJobs(jobs);
    }

    private void AutoFillJobs(Jobs jobs)
    {
        string searchFolder = "Assets/Scripts";
        jobs.jobs.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Job", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Job job = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Job)) as Job;

            if (job != null)
                jobs.jobs.Add(job);
        }

        EditorUtility.SetDirty(jobs);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
