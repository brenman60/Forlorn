using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class WorldGeneration : MonoBehaviour, ISaveData
{
    public static WorldGeneration Instance { get; private set; }
    public static event Action<string, DisasterEvent> SectionChanged;
    public static event Action<string, DisasterEvent> SectionRotted;

    private static readonly int sectionBorderSize = 3;

    public static string worldSection = "City1";
    private static string section_ = originSection;
    public static string section 
    {
        get { return section_; }
        set
        {
            SectionChanged?.Invoke(value, Instance.rottenSections.TryGetValue(value, out DisasterEvent disaster) ? disaster : DisasterEvent.None);
            section_ = value;
        }
    }
    private string currentWorldSection;

    public static bool worldLoaded { get; private set; }
    public static (float, float) worldBounds;

    private bool saveDataLoaded;

    private readonly static string originSection = "1";
    private static bool isOriginSection { get { return section == originSection; } }

    [SerializeField] private CitySections citySections;

    private List<CityBlock> currentSections = new List<CityBlock>();
    private int playerSpawnIndex = -1;

    private Dictionary<string, int> cityWideSections = new Dictionary<string, int>();
    private Dictionary<string, float> rottingSections = new Dictionary<string, float>();
    private Dictionary<string, DisasterEvent> rottenSections = new Dictionary<string, DisasterEvent>();

    private float baseRottingFactor = 0.75f;
    private float rottingFactor
    {
        get
        {
            return baseRottingFactor * RunManager.Instance.statManager.stats[StatType.WorldRottingSpeed].currentValue;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject generationObject = Instantiate(Resources.Load<GameObject>("Utils/WorldGeneration"));
            generationObject.name = "WorldGeneration";

            WorldGeneration generator = generationObject.GetComponent<WorldGeneration>();
            Instance = generator;

            DontDestroyOnLoad(generationObject);
        }
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += OpenMap;
    }

    private void Update()
    {
        foreach (KeyValuePair<string, float> rottingSection in rottingSections.ToArray())
        {
            float rottingValue = rottingSection.Value;
            if (rottingValue <= 0)
            {
                rottingSections.Remove(rottingSection.Key);

                List<DisasterEvent> disasters = new List<DisasterEvent>();
                foreach (DisasterEvent disaster_ in Enum.GetValues(typeof(DisasterEvent)))
                    if (disaster_ != DisasterEvent.None) disasters.Add(disaster_);

                DisasterEvent disaster = disasters[Random.Range(0, disasters.Count)];
                rottenSections.Add(rottingSection.Key, disaster);
                SectionRotted?.Invoke(rottingSection.Key, disaster);
            }
            else
                rottingSections[rottingSection.Key] -= rottingFactor * Time.deltaTime;
        }
    }

    private async void OpenMap(Scene arg0, Scene arg1)
    {
        if (!GameManager.validGameScenes.Contains(SceneManager.GetActiveScene().name)) return;
        TransitionUI.openPrevention.Add("Generating City");

        // Generate or load world section
        // World is split into World Section (like City 1) -> and in each World Section are Sections (basically horizontal strips, so probably 1.ggp, 2.ggp, 3.ggp, etc)
        // On every Start in this class, we should just load the current Section in the "section" string variable.

        //
        // World Section {
        //   Section 1,
        //   Section 2,
        //   etc...
        // }

        if (worldSection != currentWorldSection) // ok so this was written for when the player goes to another city
        {                                        // but that isnt a feature yet and it breaks shit when quitting and reloading and stuff so its getting commented out for now
            //print("Clearing sections");
            //cityWideSections.Clear();
        }

        currentSections.Clear();

        currentWorldSection = worldSection;

        string sectionData = await SaveSystem.LoadRunMap(worldSection, section);
        if (sectionData != string.Empty)
            LoadMap(sectionData);
        else
            GenerateMap();

        if (!rottingSections.ContainsKey(section) && !rottenSections.ContainsKey(section))
            rottingSections.Add(section, Random.Range(240, 480));
        else if (rottenSections.TryGetValue(section, out DisasterEvent disaster))
            SectionRotted?.Invoke(section, disaster);
    }

    private void LoadMap(string mapData)
    {
        string[] sectionDataPoints = JsonConvert.DeserializeObject<string[]>(mapData);
        Player.Instance.PutSaveData(sectionDataPoints[0]);

        GameObject cityRoot = GameObject.FindGameObjectWithTag("CityRoot");
        Dictionary<string, List<string>> compiledBlocks = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(sectionDataPoints[1]);
        foreach (KeyValuePair<string, List<string>> blockType in compiledBlocks)
        {
            foreach (string block in blockType.Value)
            {
                CitySection section = citySections.GetSectionByName(blockType.Key);
                GameObject sectionObject = Instantiate(section.prefabObject, cityRoot.transform);
                sectionObject.name = blockType.Key;
                CityBlock cityBlock = sectionObject.GetComponent<CityBlock>();
                if (cityBlock is TrainStation) playerSpawnIndex = currentSections.Count;
                cityBlock.PutSaveData(block);

                ChangeBounds(sectionObject);

                currentSections.Add(cityBlock);
            }
        }

        if (!isOriginSection)
            playerSpawnIndex = sectionBorderSize;

        ItemDropManager.Instance.PutSaveData(sectionDataPoints[2]);

        worldLoaded = true;
        TransitionUI.openPrevention.Remove("Generating City");
    }

    private async void GenerateMap()
    {
        GameObject cityRoot = GameObject.FindGameObjectWithTag("CityRoot");
        List<CitySection> sections = await SelectSections();
        float xOffsets = 0;
        CityBlock lastPlaced = null;
        foreach (CitySection section in sections)
        {
            GameObject sectionObject = Instantiate(section.prefabObject, cityRoot.transform);
            sectionObject.name = section.prefabObject.name;
            CityBlock cityBlock = sectionObject.GetComponent<CityBlock>();
            if (cityBlock is TrainStation) playerSpawnIndex = currentSections.Count;

            // Position formula
            // x = (lastPlaced.length / 2 + currentPlacing.length / 2) + total

            sectionObject.transform.position = new Vector3(((lastPlaced != null ? lastPlaced.length / 2 : 0) + (cityBlock.length / 2)) + xOffsets, 0, 0);
            xOffsets += cityBlock.transform.position.x - (lastPlaced != null ? lastPlaced.transform.position.x : 0);

            ChangeBounds(sectionObject);

            cityBlock.PutSaveData(string.Empty);
            lastPlaced = cityBlock;
            currentSections.Add(cityBlock);
        }

        if (!isOriginSection)
            playerSpawnIndex = sectionBorderSize;

        // Teleport player to train station (if on the first section (which will be true if the train station exists))
        if (playerSpawnIndex != -1)
        {
            Vector3 stationPosition = currentSections[playerSpawnIndex].transform.position;
            Vector3 playerPosition = Player.Instance.transform.position;
            Player.Instance.transform.position = new Vector3(stationPosition.x, playerPosition.y, playerPosition.z);
        }

        SaveSection();
        worldLoaded = true;
        TransitionUI.openPrevention.Remove("Generating City");
    }

    private void ChangeBounds(GameObject cityBlockObject)
    {
        if (cityBlockObject.transform.position.x < worldBounds.Item1)
            worldBounds.Item1 = cityBlockObject.transform.position.x;
        else if (cityBlockObject.transform.position.x > worldBounds.Item2)
            worldBounds.Item2 = cityBlockObject.transform.position.x;
    }

    private async Task<List<CitySection>> SelectSections()
    {
        int sectionLength = Random.Range(30, 41);

        List<CitySection> selectedSections = new List<CitySection>();
        foreach (CitySection section in citySections.sections)
        {
            var count = selectedSections.Count(item => item == section);
            bool belowMaxSectionAmount = section.maxPerSection <= 0 || count < section.maxPerSection;
            bool belowMaxCityAmount = section.maxPerCity <= 0 || GetCityWideAmount(section) < section.maxPerCity;
            if ((section.requiredInSections.Contains(WorldGeneration.section) || section.requiredInAnySection) &&
                belowMaxSectionAmount && belowMaxCityAmount)
            {
                if (section.maxPerCity > 0)
                {
                    if (cityWideSections.ContainsKey(section.name))
                        cityWideSections[section.name]++;
                    else
                        cityWideSections.Add(section.name, 1);
                }

                selectedSections.Add(section);
            }
        }

        while (selectedSections.Count < sectionLength)
        {
            CitySection currentSection = citySections.sections[Random.Range(0, citySections.sections.Count)];
            var count = selectedSections.Count(item => item == currentSection);
            float random = Random.Range(0f, 1f);
            bool withinRandom = random <= currentSection.rarity;
            bool belowMaxAmount = currentSection.maxPerSection <= 0 || count < currentSection.maxPerSection;
            bool belowMaxCityAmount = currentSection.maxPerCity <= 0 || GetCityWideAmount(currentSection) < currentSection.maxPerCity;
            if (withinRandom && belowMaxAmount && belowMaxCityAmount && !currentSection.cannotNaturallySpawn)
            {
                selectedSections.Add(currentSection);

                if (currentSection.maxPerCity > 0)
                {
                    if (cityWideSections.ContainsKey(currentSection.name))
                        cityWideSections[currentSection.name]++;
                    else
                        cityWideSections.Add(currentSection.name, 1);
                }
            }

            await Task.Yield();
        }

        int GetCityWideAmount(CitySection currentSection)
        {
            bool hasAmount = cityWideSections.TryGetValue(currentSection.name, out int amount);
            if (hasAmount) return amount;
            else return -1;
        }

        selectedSections.Shuffle();

        // Create section border
        for (int border = 0; border < sectionBorderSize; border++)
        {
            selectedSections.Insert(0, isOriginSection ? citySections.GetSectionByName("WorldBorder") : citySections.GetSectionByName("PreviousSectionRoad"));
            selectedSections.Add(citySections.GetSectionByName("WorldBorder"));
        }

        return selectedSections;
    }

    public void EnterNewSection(string newSection)
    {
        section = newSection;
        TransitionUI.Instance.TransitionTo("Game");
    }

    public void SaveSection()
    {
        string playerData = Player.Instance.GetSaveData();
        Dictionary<string, List<string>> cityBlocksCompiled = new Dictionary<string, List<string>>();
        foreach (CityBlock block in currentSections)
            if (!cityBlocksCompiled.ContainsKey(block.name))
                cityBlocksCompiled.Add(block.name, new List<string>() { block.GetSaveData() });
            else
                cityBlocksCompiled[block.name].Add(block.GetSaveData());

        string[] dataPoints = new string[3]
        {
            playerData,
            JsonConvert.SerializeObject(cityBlocksCompiled),
            ItemDropManager.Instance.GetSaveData()
        };

        SaveSystem.SaveRunMap(worldSection, section, JsonConvert.SerializeObject(dataPoints));
    }

    public string GetSaveData()
    {
        if (GameManager.validGameScenes.Contains(SceneManager.GetActiveScene().name))
            SaveSection();

        string[] dataPoints = new string[5]
        {
            worldSection,
            section,
            JsonConvert.SerializeObject(cityWideSections),
            JsonConvert.SerializeObject(rottingSections),
            JsonConvert.SerializeObject(rottenSections),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        worldSection = dataPoints[0];
        section = dataPoints[1];
        cityWideSections = JsonConvert.DeserializeObject<Dictionary<string, int>>(dataPoints[2]);
        rottingSections = JsonConvert.DeserializeObject<Dictionary<string, float>>(dataPoints[3]);
        rottenSections = JsonConvert.DeserializeObject<Dictionary<string, DisasterEvent>>(dataPoints[4]);
    }

    private IEnumerator WaitForItemDrop(string dropData)
    {
        yield return new WaitUntil(() => ItemDropManager.Instance != null);
        ItemDropManager.Instance.PutSaveData(dropData);
    }
}

public enum DisasterEvent
{
    None,
    //PolicePatrols,
    //ThunderStorm,
    //Inflation,
    //RabidDogs,
    //Flooding,
    //Blackout,
    SnowStorm,
    //CrimeWave,
    //AirQuality,
}

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rand = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string ToJSON(this Vector3 vector)
    {
        return vector.x + " " + vector.y + " " + vector.z;
    }

    public static Vector3 ToVector3(this string json)
    {
        string[] positions = json.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        return new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
    }

    public static string ToJSON(this Quaternion rotation)
    {
        return rotation.eulerAngles.x + " " + rotation.eulerAngles.y + " " + rotation.eulerAngles.z;
    }

    public static Quaternion ToQuaternion(this string json)
    {
        string[] rotations = json.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        return Quaternion.Euler(float.Parse(rotations[0]), float.Parse(rotations[1]), float.Parse(rotations[2]));
    }
}
