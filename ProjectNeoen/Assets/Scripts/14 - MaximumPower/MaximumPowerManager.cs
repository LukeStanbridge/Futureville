using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaximumPowerManager : MonoBehaviour
{
    [Header("Important References")]
    [field: SerializeField] public MiniGameManager MiniGameUIManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AlarmClockRinging _clock;
    [SerializeField] private ToolManager _toolManager;
    public GenerateTileSet _generateTileSet;
    public List<GameObject> Tiles;
    public List<GameObject> _ignoredTiles;
    public List<GameObject> _placedTurbines;
    public List<GameObject> Trees;
    [SerializeField] private List<BlowingWind> _blowingWinds;
    public List<Image> _summaryTiles;
    [field: SerializeField] public GameObject _tileSelected { get; private set; }
    [SerializeField] private float _startingTime = 300f;

    private float _timeRemaining;
    [SerializeField] private SceneTransitionManager _transition;

    public Transform _gameTransform;

    [Header("Turbine Placement")]
     
    public Windmill CurrentTurbine;
    public void DetachTurbine() => CurrentTurbine = null;
    public GameObject TurbinePrefab;
    
    public void ReleaseTurbine() => PlacingTurbine = false;
    public int TurbineCount { get; private set; }
    [SerializeField] private TextMeshProUGUI _turbineNumber;

    public void AddTurbine(GameObject turbine, TileStats tile)
    {
        TurbineCount++;
        _turbineNumber.text = "x" + (10 - TurbineCount);
        _generateTileSet.AdjustTurbineWakeEffect(tile);
        _placedTurbines.Add(turbine);
        
    }
    public void RemoveTurbine(GameObject turbine, TileStats tile)
    {
        TurbineCount--;
        _turbineNumber.text = "x" + (10 - TurbineCount);
        _generateTileSet.AdjustTurbineWakeEffect(tile);
        _placedTurbines.Remove(turbine);
    }

    public void RemoveTree(GameObject tree)
    {
        Trees.Remove(tree);
    }

    public bool _statsDisplayed { get; private set; }

    [Header("Tool Bools")]
    public bool PlacingTurbine;
    public bool ClearTurbine;
    public bool PlaceTree;
    public bool ClearingSpace;
    public bool UndoingSpace;

    [Header("UI Objects and Variables")]
    [SerializeField] private TextMeshProUGUI _timeText;

    [SerializeField] private GameObject _windLegend;
    [SerializeField] private GameObject _enviroLegend;
    [SerializeField] private GameObject _defaultLegend;

    [SerializeField] private Sprite[] _backgrounds;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Transform _background;
    [SerializeField] private Transform _overlayBackground;
    [SerializeField] private Canvas _backgroundCanvas;
    [SerializeField] private Canvas _overlayCanvas;   
    [SerializeField] private Canvas _windCanvas;   

    [SerializeField] private float _simulationTime;
    [SerializeField] private GameObject _summaryOverlay;
    [SerializeField] private Transform _summaryBackground;
    [SerializeField] private Color _summaryTileColor;
    [SerializeField] private Color _highValColor;
    [SerializeField] private Color _midValColor;
    [SerializeField] private Color _lowValColor;
    private string _loseText;
    private string _winText;

    public CinemachineVirtualCamera Cam;

    public void SetTile(GameObject tile) => _tileSelected = tile;

    private void Awake()
    {
        _clock = GetComponent<AlarmClockRinging>();

        //if (Camera.main.aspect > 1.6) _generateTileSet.gameObject.transform.localScale = new Vector3(108, 108, 0);
        //else _generateTileSet.gameObject.transform.localScale = new Vector3(108, 120, 0);

        if (!MiniGameUIManager._debugTesting)
        {
            _transition = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransitionManager>();
            Cam = _transition.Cam;
            Cam.m_Lens.OrthographicSize = 5f;
            _backgroundCanvas.worldCamera = Camera.main;
            _overlayCanvas.worldCamera = Camera.main;
            _windCanvas.worldCamera = Camera.main;
            this.gameObject.transform.position = new Vector3(Cam.transform.position.x, Cam.transform.position.y, 0);
        }

        foreach (Transform tile in _summaryBackground)
        {
            _summaryTiles.Add(tile.GetComponent<Image>());
            if (tile.transform.childCount == 0)
            {
                tile.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }

        _enviroLegend?.SetActive(false);
        _windLegend?.SetActive(false);
        _loseText = MiniGameUIManager._lose.Description;
        _winText = MiniGameUIManager._win.Description;
    }

    private void Start()
    {
        _summaryOverlay?.SetActive(false);
    }

    private void ResetGame()
    {
        ResetTiles();
        _clock.ResetClockRotation();
        _backgroundImage.sprite = null;
        _statsDisplayed = false;
        ClearingSpace = false;
        UndoingSpace = false;
        PlacingTurbine = false;
        _defaultLegend.gameObject.SetActive(true);
        _windLegend.gameObject.SetActive(false);
        _enviroLegend.gameObject.SetActive(false);
        _summaryOverlay.gameObject.SetActive(false);
        DisplayNormal();
        TurbineCount = 0;
        _timeRemaining = _startingTime;
        _generateTileSet.GenerateTiles();
        MiniGameUIManager.SetReset(false);
    }

    private void Update()
    {
        if (MiniGameUIManager.ResetGame) ResetGame(); //reset game after on starting game

        if (_clock.FinishedRinging)
        {
            RunSimulation();
            _clock.ResetAlarm();
        }

        if (MiniGameUIManager.Playing)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 6) _audioManager.Play("5sec");

            if (_timeRemaining <= 0) //no time left game over
            {
                _timeRemaining = 0;
                MiniGameUIManager.DisableOptionsButton();
                DisplayNormal();
                _clock.TriggerClockRinging();
                _audioManager.FadeOut("Theme");
                _audioManager.Stop("5sec");
                _audioManager.Play("Alarm");
                //RunSimulation();
                MiniGameUIManager.SetPlaying(false);
            }

            _timeText.text = $"{(int)_timeRemaining / 60}:{(int)_timeRemaining % 60:D2}";
        }
    }

    private void ResetTiles()
    {
        foreach (GameObject tile in Tiles)
        {
            foreach (Transform child in tile.transform)
            {
                Destroy(child.gameObject);
            }
        }
        _placedTurbines.Clear();
        Trees.Clear();
        
        foreach (Image tile in _summaryTiles)
        {
            if (tile.transform.childCount == 0) continue;
            else if (tile.color != _summaryTileColor)
            {
                tile.color = _summaryTileColor;
                tile.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }

        Tiles.Clear();
    }

    public void Turbine()
    {
        if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        PlacingTurbine = true;
        ClearTurbine = false;
        PlaceTree = false;
        ClearingSpace = false;
        UndoingSpace = false;
    }

    public void RemoveTurbine()
    {
        if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        PlacingTurbine = false;
        ClearTurbine = true;
        PlaceTree = false;
        ClearingSpace = false;
        UndoingSpace = false;
    }

    public void PlantTree()
    {
        if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        PlacingTurbine = false;
        ClearTurbine = false;
        PlaceTree = true;
        ClearingSpace = false;
        UndoingSpace = false;
    }

    public void ClearSpace()
    {
        if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        PlacingTurbine = false;
        ClearTurbine = false;
        PlaceTree = false;
        ClearingSpace = true;
        UndoingSpace = false;
    }

    public void UndoSpace()
    {
        if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        PlacingTurbine = false;
        ClearTurbine = false;
        PlaceTree = false;
        ClearingSpace = false;
        UndoingSpace = true;
    }

    private void SetTurbineSpeeds()
    {
        foreach (GameObject turbine in _placedTurbines)
        {
            TileStats stats = turbine.GetComponentInParent<TileStats>();
            float spinTimer = 10.5f - (stats.WindGeneration * 0.1f);
            if (stats.WindGeneration == 0) spinTimer = 0;
            turbine.GetComponent<Windmill>().SetTurbineSpeed(spinTimer);
        }
    }

    public void RunSimulation()
    {
        //if (!MiniGameUIManager.Playing) return;
        DisplayNormal();
        MiniGameUIManager.SetPlaying(false);
        MiniGameUIManager.DisableOptionsButton();
        _timeText.text = "0:00";
        _toolManager.HideActiveTool();
        _generateTileSet.CalcTotalTurbulence(); //if turbine turbulence % >= 100% then it is excluded from further claculations
        _generateTileSet.CalcWindSpeed();
        _generateTileSet.SimulateWindGeneration();
        _backgroundImage.sprite = null;
        _audioManager.Play("Wind");
        _audioManager.Play("Whoosh");
        SetTurbineSpeeds();
        StartCoroutine(Simulation());
    }

    private IEnumerator Simulation()
    { 
        foreach(GameObject turbine in _placedTurbines) //spin turbines
        {
            turbine.GetComponent<Windmill>().SpinTurbine();
        }

        foreach(BlowingWind wind in _blowingWinds) //play wind
        {
            wind.PlayWindEffect();
        }

        yield return new WaitForSecondsRealtime(_simulationTime);

        foreach (GameObject turbine in _placedTurbines) //stop turbines
        {
            turbine.GetComponent<Windmill>().StopTurbine();
        }

        foreach (BlowingWind wind in _blowingWinds) //stop wind
        {
            wind.KillWindEffects();
        }

        _audioManager.FadeOut("Wind");
        _audioManager.Stop("Whoosh");

        if (Trees.Count >= 70 && Mathf.Round(_generateTileSet.TotalWindGeneration * 1000) >= 10000)
        {
            string results = AppendString(_winText,
                                      _placedTurbines.Count.ToString(),
                                      _generateTileSet.ProductiveTurbines.ToString(),
                                      Mathf.Round(_generateTileSet.TotalWindGeneration * 1000).ToString(),
                                      Trees.Count.ToString());

            MiniGameUIManager._win.Description = results;

            MiniGameUIManager.EndGame(true);
        }
        else
        {
            string results = AppendString(_loseText,
                                      _placedTurbines.Count.ToString(),
                                      _generateTileSet.ProductiveTurbines.ToString(),
                                      Mathf.Round(_generateTileSet.TotalWindGeneration * 1000).ToString(),
                                      Trees.Count.ToString());

            MiniGameUIManager._lose.Description = results;
            MiniGameUIManager.EndGame(false);
        }

    }

    public void DisplayDataButton()
    {
        MiniGameUIManager.CloseEndGamePanel();
        _summaryOverlay.SetActive(true);
        DisplayOverlayData();
    }

    private void DisplayOverlayData()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].transform.GetComponentInChildren<Windmill>())
            {
                TileStats stats = Tiles[i].GetComponentInChildren<TileStats>();
                TextMeshProUGUI dataText = _summaryTiles[i].GetComponentInChildren<TextMeshProUGUI>();
                dataText.text = stats.WindGeneration.ToString("0"); //display value to 2 decimal places
                dataText.fontSize = 28;
                dataText.fontStyle = FontStyles.Bold;
                SetSumaryColour(_summaryTiles[i], stats.WindGeneration);
            }
        }
    }

    private void SetSumaryColour(Image summaryTile, float percent)
    {
        if (percent < 1) summaryTile.color = _lowValColor;
        else if (percent >= 1 && percent < 50) summaryTile.color = _midValColor;
        else if (percent >= 50) summaryTile.color = _highValColor;
    }

    private string AppendString(string text, string append1, string append2, string append3, string append4)
    {
        text = text.Replace("{1}", append1);
        text = text.Replace("{2}", append2);
        text = text.Replace("{3}", append3);
        text = text.Replace("{4}", append4);
        return text;
    }

    private void DisplayNormal() //return grid to normal display
    {
        if (!MiniGameUIManager.Playing) return;
        AdjustTurbineSortLayer(1);
        _statsDisplayed = false;
        _backgroundImage.sprite = null;
        _backgroundImage.enabled = false;
        _defaultLegend.gameObject.SetActive(true);
        _windLegend.gameObject.SetActive(false);
        _enviroLegend.gameObject.SetActive(false);
    }

    public void DisplayWind()
    {
        if (!MiniGameUIManager.Playing) return;
        if (_backgroundImage.sprite == _backgrounds[0])
        {
            DisplayNormal();
            _toolManager.DeactivateTool();
            return;
        }
        AdjustTurbineSortLayer(3);
        _statsDisplayed = true;
        _backgroundImage.sprite = _backgrounds[0];
        _backgroundImage.enabled = true;
        _defaultLegend.gameObject.SetActive(false);
        _windLegend.gameObject.SetActive(true);
        _enviroLegend.gameObject.SetActive(false);
    }

    public void DisplayTerrain()
    {
        if (!MiniGameUIManager.Playing) return;
        if (_backgroundImage.sprite == _backgrounds[1])
        {
            DisplayNormal();
            _toolManager.DeactivateTool();
            return;
        }
        AdjustTurbineSortLayer(3);
        _statsDisplayed = true;
        _backgroundImage.sprite = _backgrounds[1];
        _backgroundImage.enabled = true;
        _defaultLegend.gameObject.SetActive(false);
        _windLegend.gameObject.SetActive(false);
        _enviroLegend.gameObject.SetActive(false);
    }

    public void DisplayEnviro()
    {
        if (!MiniGameUIManager.Playing) return;
        if (_backgroundImage.sprite == _backgrounds[2])
        {
            DisplayNormal();
            _toolManager.DeactivateTool();
            return;
        }
        AdjustTurbineSortLayer(3);
        _statsDisplayed = true;
        _backgroundImage.sprite = _backgrounds[2];
        _backgroundImage.enabled = true;
        _defaultLegend.gameObject.SetActive(false);
        _windLegend.gameObject.SetActive(false);
        _enviroLegend.gameObject.SetActive(true);
    }

    private void AdjustTurbineSortLayer(int layer)
    {
        foreach(GameObject turbine in _placedTurbines)
        {
            turbine.GetComponent<SpriteRenderer>().sortingOrder = layer;
        }
    }
}
