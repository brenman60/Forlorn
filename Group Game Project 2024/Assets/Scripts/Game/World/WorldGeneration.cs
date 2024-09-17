using Newtonsoft.Json;
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

    private readonly static string originSection = "1";
    private static bool isOriginSection { get { return section == originSection; } }

    [SerializeField] private CitySections citySections;

    private TrainStation trainStation;

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

        string sectionData = await SaveSystem.LoadRunMap(worldSection, section);
        if (sectionData != string.Empty)
            LoadMap(sectionData);
        else
            GenerateMap();
    }

    private void LoadMap(string mapData)
    {
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
            CityBlock cityBlock = sectionObject.GetComponent<CityBlock>();
            if (cityBlock is TrainStation station) trainStation = station;

            // Position formula
            // x = (lastPlaced.length / 2 + currentPlacing.length / 2) + total

            sectionObject.transform.position = new Vector3(((lastPlaced != null ? lastPlaced.length / 2 : 0) + (cityBlock.length / 2)) + xOffsets, 0, 0);
            xOffsets += cityBlock.transform.position.x - (lastPlaced != null ? lastPlaced.transform.position.x : 0);

            cityBlock.PutSaveData(string.Empty);
            lastPlaced = cityBlock;
        }

        // Teleport player to train station (if on the first section (which will be true if the train station exists))
        if (trainStation != null)
        {
            Vector3 stationPosition = trainStation.transform.position;
            Vector3 playerPosition = Player.Instance.transform.position;
            Player.Instance.transform.position = new Vector3(stationPosition.x, playerPosition.y, playerPosition.z);
        }

        //SaveSystem.SaveRunMap(worldSection, section, );
        TransitionUI.openPrevention.Remove("Generating City");
    }

    private async Task<List<CitySection>> SelectSections()
    {
        int sectionLength = Random.Range(30, 41);
        int currentSectionNumber = int.Parse(section);

        List<CitySection> selectedSections = new List<CitySection>();
        foreach (CitySection section in citySections.sections)
            if (section.requiredUpTo <= currentSectionNumber)
                selectedSections.Add(section);

        while (selectedSections.Count < sectionLength)
        {
            CitySection currentSection = citySections.sections[Random.Range(0, citySections.sections.Count)];
            var count = selectedSections.Count(item => item == currentSection);
            float random = Random.Range(0f, 1f);
            bool withinRandom = random <= currentSection.rarity;
            bool belowMaxAmount = currentSection.maxPerSection <= 0 || count < currentSection.maxPerSection;
            if (withinRandom && belowMaxAmount)
                selectedSections.Add(currentSection);

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
        string sectionData = string.Empty;

        SaveSystem.SaveRunMap(worldSection, section, sectionData);
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[2]
        {
            worldSection,
            section,
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        worldSection = dataPoints[0];
        section = dataPoints[1];
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
}
