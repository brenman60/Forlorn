using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    public static TransitionUI Instance { get; private set; }

    public static List<string> openPrevention = new List<string>();
    public static bool doneLoading
    {
        get
        {
            return !Instance.transitioning && openPrevention.Count == 0;
        }
    }

    [SerializeField] private GameObject gridSquarePrefab;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI progressInfoText;
    [SerializeField] private CanvasGroup loadingPlayerIcon;
    [Space(15), SerializeField] private float delayBetweenSquares = 0.005f;
    [SerializeField] private float transitionDuration = 0.15f;

    private int gridWidth;
    private int gridHeight;
    private float gridSquareSize = 100f;
    private RectTransform[,] gridSquares;

    private bool transitioning;

    private CanvasGroup progressTextCanvasGroup;
    private CanvasGroup progressInfoTextCanvasGroup;

    private float loadingInfoDotsTimer;
    private int loadingInfoDots;

    private Coroutine waitForSceneFinishCoroutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject uiObject = Instantiate(Resources.Load<GameObject>("UI/TransitionUI"));
            uiObject.name = "TransitionUI";

            TransitionUI ui = uiObject.GetComponent<TransitionUI>();
            Instance = ui;

            DontDestroyOnLoad(uiObject);
        }
    }

    private void Start()
    {
        progressTextCanvasGroup = progressText.GetComponent<CanvasGroup>();
        progressInfoTextCanvasGroup = progressInfoText.GetComponent<CanvasGroup>();

        CreateGrid();
        StartCoroutine(ShrinkGrid());
        SceneManager.sceneLoaded += NewSceneLoaded;
    }

    private void Update()
    {
        progressTextCanvasGroup.alpha = Mathf.MoveTowards(progressTextCanvasGroup.alpha, transitioning ? 1f : 0f, Time.unscaledDeltaTime * 2f);
        progressInfoTextCanvasGroup.alpha = Mathf.MoveTowards(progressInfoTextCanvasGroup.alpha, transitioning ? 1f : 0f, Time.unscaledDeltaTime * 2f);
        loadingPlayerIcon.alpha = Mathf.MoveTowards(loadingPlayerIcon.alpha, transitioning ? 1f : 0f, Time.unscaledDeltaTime * 2f);

        ChangeProgressInfo();
    }

    private void ChangeProgressInfo()
    {
        loadingInfoDotsTimer += Time.unscaledDeltaTime;
        if (loadingInfoDotsTimer >= .25f)
        {
            loadingInfoDotsTimer = 0f;
            if (loadingInfoDots == 3)
                loadingInfoDots = 1;
            else
                loadingInfoDots++;
        }

        progressInfoText.text = openPrevention.Count > 0 ? openPrevention[0] : string.Empty;
        if (progressInfoText.text.Length != 0)
            for (int dot = 0; dot < loadingInfoDots; dot++)
                progressInfoText.text += ".";
    }

    private void CreateGrid()
    {
        float widthRatio = 1920f / Screen.width;
        float heightRatio = 1080f / Screen.height;

        int width = Mathf.CeilToInt(Screen.width / gridSquareSize) * Mathf.CeilToInt(widthRatio);
        int height = Mathf.CeilToInt(Screen.height / gridSquareSize) * Mathf.CeilToInt(heightRatio);

        gridWidth = width;
        gridHeight = height;

        gridSquares = new RectTransform[height, width];

        gridLayout.cellSize = new Vector2(gridSquareSize, gridSquareSize);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                RectTransform newSquare = Instantiate(gridSquarePrefab, gridLayout.transform).GetComponent<RectTransform>();
                newSquare.name = "GridSquare" + x + "_" + y;
                newSquare.sizeDelta = new Vector2(gridSquareSize, gridSquareSize);

                Image squareImage = newSquare.GetComponent<Image>();
                squareImage.color = Color.black;

                newSquare.gameObject.SetActive(true);
                gridSquares[y, x] = newSquare;
            }
        }
    }

    private IEnumerator ShrinkGrid()
    {
        yield return StartCoroutine(ResetGridLayout());
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                float delay = (y + x) * delayBetweenSquares;
                StartCoroutine(ResizeSquare(gridSquares[y, x], Vector2.zero, delay));
            }
        }
    }

    private void GrowGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                float delay = (y + x) * delayBetweenSquares;
                StartCoroutine(ResizeSquare(gridSquares[y, x], Vector2.one * gridSquareSize, delay));
            }
        }
    }

    private IEnumerator ResizeSquare(RectTransform square, Vector2 targetSize, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 initialSize = square.sizeDelta;
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            square.sizeDelta = Vector2.Lerp(initialSize, targetSize, time / transitionDuration);
            yield return new WaitForEndOfFrame();
        }

        square.sizeDelta = targetSize;
    }

    private IEnumerator ResetGridLayout()
    {
        gridLayout.enabled = true;
        gridLayout.SetLayoutHorizontal();
        gridLayout.SetLayoutVertical();

        yield return new WaitForEndOfFrame();

        gridLayout.enabled = false;
    }

    private void NewSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (waitForSceneFinishCoroutine != null) StopCoroutine(WaitForSceneFinishing());
        waitForSceneFinishCoroutine = StartCoroutine(WaitForSceneFinishing());
    }

    private IEnumerator WaitForSceneFinishing()
    {
        yield return new WaitForSeconds(.1f);
        yield return new WaitUntil(() => openPrevention.Count == 0);
        StartCoroutine(ShrinkGrid());
        transitioning = false;
    }

    public void TransitionTo(string sceneName)
    {
        StartCoroutine(InternalTransition(sceneName));
    }

    private IEnumerator InternalTransition(string sceneName)
    {
        transitioning = true;
        progressText.text = "0%";

        GrowGrid();

        yield return new WaitForSeconds(1.75f);

        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneLoading.allowSceneActivation = true;
        do
        {
            progressText.text = Mathf.RoundToInt(sceneLoading.progress * 100) + "%";
            yield return new WaitForEndOfFrame();
        }
        while (!sceneLoading.isDone);
        progressText.text = "100%";
    }
}
