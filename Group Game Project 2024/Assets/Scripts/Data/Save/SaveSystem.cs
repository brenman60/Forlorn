using System.IO;
using UnityEngine;
using AESEncryption;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class SaveSystem
{
    public static bool initialized { get; private set; } = false;

    private static readonly string mainPath = Application.persistentDataPath;
    private static readonly string gamesPath = Path.Combine(mainPath, "runs");
    private static string globalDataPath
    {
        get { return Path.Combine(mainPath, "global.ggp"); }
    }
    private static bool globalDataExists
    {
        get { return File.Exists(globalDataPath); }
    }
    private static string runPath
    {
        get { if (!Directory.Exists(gamesPath)) Directory.CreateDirectory(gamesPath); return GetRunPath(GameManager.runId); }
    }

    public readonly static string runDataFile = "run.data";
    private static string runDataPath 
    { 
        get { if (!Directory.Exists(runPath)) Directory.CreateDirectory(runPath); return Path.Combine(runPath, runDataFile); }
    }
    private static string runWorldPath 
    { 
        get { if (!Directory.Exists(runPath)) Directory.CreateDirectory(runPath); return Path.Combine(runPath, "map"); } 
    }

#if !UNITY_WEBGL
    // File structure is going to be in an array for general organization (will prevent having to make lists of strings and adding the values during the save process which is messy)
    // Each component in need of saving can just have their "GetSaveData" method in the string array (will need to make checks during loading process for missing values when values are added while previous save data exists)
    public static async void SaveGlobal()
    {
        if (!initialized) return;

        string[] globalData = new string[2]
        {
            Keybinds.Instance.GetSaveData(),
            GameSettings.Instance.GetSaveData(),
        };

        await WriteToFile(globalDataPath, JsonConvert.SerializeObject(globalData));
    }

    public static async void SaveRunData()
    {
        string[] runData = new string[3]
        {
            RunManager.Instance.GetSaveData(),
            WorldGeneration.Instance.GetSaveData(),
            GameManager.Instance.GetSaveData(),
        };

        await WriteToFile(runDataPath, JsonConvert.SerializeObject(runData));
    }

    public static async void SaveRunMap(string worldSection, string section, string dataJson)
    {
        await WriteToFile(Path.Combine(runWorldPath, worldSection, section + ".ggp"), dataJson);
    }

    // Loading needs to be in the exact same order as saving, once it's been written it should almost always stay that exact way
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static async Task LoadGlobal()
    {
        if (initialized) return;
        if (!globalDataExists)
        {
            initialized = true;
            return;
        }

        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(await ReadFromFile(globalDataPath));
        if (dataPoints.Length >= 1) Keybinds.Instance.PutSaveData(dataPoints[0]);
        if (dataPoints.Length >= 2) GameSettings.Instance.PutSaveData(dataPoints[1]);

        initialized = true;
    }

    public static async Task LoadRunData()
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(await ReadFromFile(runDataPath));
        if (dataPoints.Length >= 1) RunManager.Instance.PutSaveData(dataPoints[0]);
        if (dataPoints.Length >= 2) WorldGeneration.Instance.PutSaveData(dataPoints[1]);
        if (dataPoints.Length >= 3) GameManager.Instance.PutSaveData(dataPoints[2]);
    }

    public static async Task<string> LoadRunMap(string worldSection, string section)
    {
        string mapPath = Path.Combine(runWorldPath, worldSection, section + ".ggp");
        if (File.Exists(mapPath))
            return await ReadFromFile(mapPath);
        else
            return string.Empty;
    }

    public static List<string> GetAllRuns()
    {
        List<string> runs = new List<string>();

        if (Directory.Exists(gamesPath))
        {
            DirectoryInfo gamesInfo = new DirectoryInfo(gamesPath);
            foreach (DirectoryInfo gameInfo in gamesInfo.GetDirectories())
                runs.Add(gameInfo.Name);
        }

        return runs;
    }
#elif UNITY_WEBGL // I will be putting some basic WebGL support in regards to save data, but it will probably be limited to this depth (mostly the fault of WebGL's limits with save data, that being, the 1 MB limit)
    public static void SaveGlobal()
    {
        try
        {
            PlayerPrefs.SetString("Keybinds", Keybinds.Instance.GetSaveData());
            PlayerPrefs.SetString("Settings", GameSettings.Instance.GetSaveData());
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError("WebGL save data encoutered an error while putting global data: " + e.GetBaseException());
        }
    }

    public static void SaveRun() 
    {
        try
        {
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError("WebGL save data encoutered an error while putting run data: " + e.GetBaseException());
        }
    }

    public static void LoadGlobal()
    {
        Keybinds.Instance.PutSaveData(PlayerPrefs.GetString("Keybinds"));
        GameSettings.Instance.PutSaveData(PlayerPrefs.GetString("Settings"));
    }

    public static void LoadRun()
    {
        
    }
#endif

    public static string GetRunPath(string runId)
    {
        if (!Directory.Exists(gamesPath)) 
            Directory.CreateDirectory(gamesPath);
        
        return Path.Combine(gamesPath, runId);
    }

    // Encrypting save data can just happen in these individual methods so we don't have to deal with it at all in the main methods (will help keep everything consistent and organized)
    public static async Task<bool> WriteToFile(string filePath, string data)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string encrypedData = AesOperation.EncryptString(data);
                    await writer.WriteAsync(encrypedData);

                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to file with path '" + filePath + "': " + e.GetBaseException());
            return false;
        }
    }

    public static async Task<string> ReadFromFile(string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string rawData = await reader.ReadToEndAsync();
                    string unencrypted = await AesOperation.DecryptString(rawData);

                    return unencrypted;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading from file '" + filePath + "': " + e.GetBaseException());
            return null;
        }
    }
}
