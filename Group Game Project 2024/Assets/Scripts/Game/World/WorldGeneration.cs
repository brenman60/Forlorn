using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGeneration : MonoBehaviour, ISaveData
{
    public static WorldGeneration Instance { get; private set; }

    public static string worldSection = "City1";
    public static string section = originSection;
    private string currentWorldSection;

    private readonly static string originSection = "1";
    private static bool isOriginSection { get { return section == originSection; } }

    [SerializeField] private CitySections citySections;

    private List<CityBlock> currentSections = new List<CityBlock>();
    private int trainStationIndex = -1;

    private Dictionary<string, int> cityWideSections = new Dictionary<string, int>();

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

        if (worldSection != currentWorldSection)
            cityWideSections.Clear();

        currentWorldSection = worldSection;

        string sectionData = await SaveSystem.LoadRunMap(worldSection, section);
        if (sectionData != string.Empty)
            LoadMap(sectionData);
        else
            GenerateMap();
    }

    private void LoadMap(string mapData)
    {
        GameObject cityRoot = GameObject.FindGameObjectWithTag("CityRoot");
        Dictionary<string, List<string>> compiledBlocks = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(mapData);
        foreach (KeyValuePair<string, List<string>> blockType in compiledBlocks)
        {
            foreach (string block in blockType.Value)
            {
                CitySection section = citySections.GetSectionByName(blockType.Key);
                GameObject sectionObject = Instantiate(section.prefabObject, cityRoot.transform);
                sectionObject.name = blockType.Key;
                CityBlock cityBlock = sectionObject.GetComponent<CityBlock>();
                if (cityBlock is TrainStation) trainStationIndex = currentSections.Count;
                cityBlock.PutSaveData(block);

                currentSections.Add(cityBlock);
            }
        }

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
            if (cityBlock is TrainStation) trainStationIndex = currentSections.Count;

            // Position formula
            // x = (lastPlaced.length / 2 + currentPlacing.length / 2) + total

            sectionObject.transform.position = new Vector3(((lastPlaced != null ? lastPlaced.length / 2 : 0) + (cityBlock.length / 2)) + xOffsets, 0, 0);
            xOffsets += cityBlock.transform.position.x - (lastPlaced != null ? lastPlaced.transform.position.x : 0);

            cityBlock.PutSaveData(string.Empty);
            lastPlaced = cityBlock;
            currentSections.Add(cityBlock);
        }

        // Teleport player to train station (if on the first section (which will be true if the train station exists))
        if (trainStationIndex != -1)
        {
            Vector3 stationPosition = currentSections[trainStationIndex].transform.position;
            Vector3 playerPosition = Player.Instance.transform.position;
            Player.Instance.transform.position = new Vector3(stationPosition.x, playerPosition.y, playerPosition.z);
        }

        SaveSection();
        TransitionUI.openPrevention.Remove("Generating City");
    }

    private async Task<List<CitySection>> SelectSections()
    {
        int sectionLength = Random.Range(30, 41);
        int currentSectionNumber = int.Parse(section);

        List<CitySection> selectedSections = new List<CitySection>();
        foreach (CitySection section in citySections.sections)
            if (section.requiredInSections.Contains(WorldGeneration.section) || section.requiredInAnySection)
                selectedSections.Add(section);

        while (selectedSections.Count < sectionLength)
        {
            CitySection currentSection = citySections.sections[Random.Range(0, citySections.sections.Count)];
            var count = selectedSections.Count(item => item == currentSection);
            float random = Random.Range(0f, 1f);
            bool withinRandom = random <= currentSection.rarity;
            bool belowMaxAmount = currentSection.maxPerSection <= 0 || count < currentSection.maxPerSection;
            bool belowMaxCityAmount = currentSection.maxPerCity <= 0 || cityWideSections.TryGetValue(currentSection.name, out int cityCount) && cityCount < currentSection.maxPerCity;
            if (withinRandom && belowMaxAmount && belowMaxCityAmount)
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

        selectedSections.Shuffle();
        return selectedSections;
    }

    public void EnterNewSection(string newSection)
    {
        section = newSection;
        TransitionUI.Instance.TransitionTo("Game");
    }

    public void SaveSection()
    {
        Dictionary<string, List<string>> cityBlocksCompiled = new Dictionary<string, List<string>>();
        foreach (CityBlock block in currentSections)
            if (!cityBlocksCompiled.ContainsKey(block.name))
                cityBlocksCompiled.Add(block.name, new List<string>() { block.GetSaveData() });
            else
                cityBlocksCompiled[block.name].Add(block.GetSaveData());

        SaveSystem.SaveRunMap(worldSection, section, JsonConvert.SerializeObject(cityBlocksCompiled));
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[4]
        {
            worldSection,
            section,
            JsonConvert.SerializeObject(cityWideSections),

            Player.Instance != null ? Player.Instance.GetSaveData() : string.Empty,
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        worldSection = dataPoints[0];
        section = dataPoints[1];
        cityWideSections = JsonConvert.DeserializeObject<Dictionary<string, int>>(dataPoints[2]);

        if (!string.IsNullOrEmpty(dataPoints[3]))
            StartCoroutine(WaitForPlayer(dataPoints[3]));
    }

    private IEnumerator WaitForPlayer(string playerData)
    {
        yield return new WaitUntil(() => Player.Instance != null);
        Player.Instance.PutSaveData(playerData);
    }
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
