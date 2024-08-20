using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ElectronGrabManager : MonoBehaviour
{
    [Header("Important References")]
    [SerializeField] private MiniGameManager _miniGameManager;
    [SerializeField] private SceneTransitionManager _transition;
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private PickupPliers _pliers;
    [SerializeField] private Canvas _backGroundCanvas;
    [SerializeField] private Canvas _canvasGame;
    public RectTransform GameCanvas;
    public Transform Environmnet;
    [SerializeField] private Transform _gameTransform;
    [SerializeField] private GameObject _electron;
    [SerializeField] private GameObject _player;
    public GameObject GameBounds;
    public Transform SpawnContainer;
    [SerializeField] private float _startingTime = 120f;
    private CinemachineVirtualCamera Cam;
    private PlayerControls _playerControls;
    private float _timeRemaining;
    private Vector3 _startPos;
    private bool _playerWins;
    private float _gameScale;
    public bool GameOver { get; private set; }

    [Header("Malfunction Event Variables")]
    [SerializeField] private float _malfunctionEventTimer;
    [SerializeField] private float _randomMalfunctionTimeLow;
    [SerializeField] private float _randomMalfunctionTimeHigh;
    [field:SerializeField] public bool Malfunction { get; private set; }
    public bool MalfunctionComplete { get; private set; }
    

    [Header("Escalation Variables")]
    [SerializeField] private float _escalationTimerReset = 30;
    [field: SerializeField] public int GameQuarter { get; private set; } = 0;
    private float _escalationTimer;

    [Header("Electron Spawner Variables")]
    [SerializeField] private float _spawnerTime = 0.7f;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;
    private float _minSpawnTimeReset;
    private float _maxSpawnTimeReset;
    private float _electronSpawnTimer = 1;

    [Header("UI Objects and References")]
    [SerializeField] private GameObject _leftButton;
    [SerializeField] private GameObject _rightButton;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private AlarmClockRinging _clock;
    [SerializeField] private Image _progressFill;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private int _electronsGrabbed;
    [SerializeField] private int _electronsMissed;
    [SerializeField] private float _chargedPercent;
    private string _gameOverText;

    //Setters
    public bool SetMalfunction(bool value) { return Malfunction = value; }
    public bool SetMalfunctionComplete(bool value) { return MalfunctionComplete = value; }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _clock = GetComponent<AlarmClockRinging>();
        if (!_miniGameManager._debugTesting)
        {
            _transition = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransitionManager>();
            Cam = _transition.Cam;
            Cam.m_Lens.OrthographicSize = 5f;
            _backGroundCanvas.worldCamera = Camera.main;
            _canvasGame.worldCamera = Camera.main;
            CheckControls();
        }
        _startPos = new Vector3(0, -4, 0);

        _minSpawnTimeReset = _minSpawnTime;
        _maxSpawnTimeReset = _maxSpawnTime;
        _playerWins = true;
        _gameOverText = _miniGameManager._win.Description;
    }

    private void CheckControls()
    {
        if (GameManager.Instance.Controls == ControlType.TouchControls) 
        {
            _leftButton.SetActive(true);
            _rightButton.SetActive(true);
        }
        else
        {
            _leftButton.SetActive(false);
            _rightButton.SetActive(false);
        }
    }

    public void ResetGame()
    {
        //hide UI elements
        _gameUI.SetActive(true);
        _pliers.KillWiggleTween();
        _player.GetComponent<GrabberMovement>().ResetMalfunction();
        _clock.ResetClockRotation();
        GameOver = false;
        GameQuarter = 0;
        Malfunction = false;
        MalfunctionComplete = false;
        _electronsGrabbed = 0;
        _electronsMissed = 0;
        _chargedPercent = 0;
        _minSpawnTime = _minSpawnTimeReset;
        _maxSpawnTime = _maxSpawnTimeReset;
        _malfunctionEventTimer = UnityEngine.Random.Range(_randomMalfunctionTimeLow, _randomMalfunctionTimeHigh);
        _escalationTimer = _escalationTimerReset;
        _progressFill.fillAmount = 0;
        _player.transform.localPosition = _startPos;
        _timeRemaining = _startingTime;
        DestroyElectrons();
        _miniGameManager.SetReset(false);
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        if (_miniGameManager.ResetGame)
        {
            ResetGame();
        }

        if (_clock.FinishedRinging)
        {
            _soundManager.Stop("Alarm");
            LeaveGame();
            _clock.ResetAlarm();
        }

        if(_miniGameManager.Playing)
        {
            _timeRemaining -= Time.deltaTime;
            ElectronGenerator();
            SpawnEscalationTimer();
            if (!MalfunctionComplete) MalfunctionEvent();

            if (_timeRemaining <= 6) _soundManager.Play("5sec");

            if (_timeRemaining <= 0) //no time left game over
            {
                _soundManager.FadeOut("MiniGameMenuTheme");
                _soundManager.Stop("5sec");
                _timeRemaining = 0;
                _clock.TriggerClockRinging();
                _soundManager.Play("Alarm");
                DestroyElectrons();
                _miniGameManager.SetPlaying(false);
            }

            _timeText.text = $"{(int)_timeRemaining / 60}:{(int)_timeRemaining % 60:D2}";
        }
    }

    private void MalfunctionEvent()
    {
        if (Malfunction) return;
        if(_malfunctionEventTimer > 0)
        {
            _malfunctionEventTimer -= Time.deltaTime;
        }
        else
        {
            //trigger malfunction event
            _player.GetComponent<GrabberMovement>().MalfunctionIcon(); //display malfunction sprite
            _player.GetComponent<GrabberMovement>().PulseIcon();
            _soundManager.Play("DeadBattery");
            Malfunction = true;
        }
    }

    private void SpawnEscalationTimer()
    {
        if (_escalationTimer > 0)
        {
            _escalationTimer -= Time.deltaTime;
        }
        else
        {
            GameQuarter++;
            _minSpawnTime = _minSpawnTime - 0.1f;
            _minSpawnTime = Mathf.Round(_minSpawnTime * 10) * 0.1f;
            _maxSpawnTime = _maxSpawnTime - 0.1f;
            _maxSpawnTime = Mathf.Round(_maxSpawnTime * 10) * 0.1f;
            _escalationTimer = _escalationTimerReset;
        }
    }

    private void ElectronGenerator() //function for spawning electrons at randomised time
    {
        if (_electronSpawnTimer > 0)
        {
            _electronSpawnTimer -= Time.deltaTime;
        }
        else
        {
            float xOffset = GameBounds.GetComponent<RectTransform>().sizeDelta.x / 2;
            float electronWidth = _electron.GetComponent<CapsuleCollider2D>().size.x / 2;
            float posX = Random.Range(-xOffset + electronWidth, xOffset - electronWidth);
            float posY = GameBounds.GetComponent<RectTransform>().sizeDelta.x / 4 - 2;

            GameObject electron = Instantiate(_electron);
            electron.GetComponent<TrailRenderer>().widthMultiplier *= (GameCanvas.localScale.x * Environmnet.localScale.x);
            electron.GetComponent<Electron>().SetAudioManager(_soundManager);
            electron.transform.SetParent(SpawnContainer.transform, false);
            electron.transform.localPosition = new Vector3(posX, posY , 0);

            _spawnerTime = RandomSpawnTimer();
            _electronSpawnTimer = _spawnerTime;
        }
    }

    private float RandomSpawnTimer() //randomise 
    {
        float spawnTime = UnityEngine.Random.Range(_minSpawnTime, _maxSpawnTime);
        return spawnTime;   
    }

    public void AddScore(float value)
    {
        //add and update score to progress bart
        if (value > 0) _electronsGrabbed++;
        else _electronsMissed++;

        if (_progressFill.fillAmount < 1 || _progressFill.fillAmount > 0)
        {
            _progressFill.fillAmount += value;
        }
        
        _chargedPercent = _progressFill.fillAmount;
        _chargedPercent = Mathf.Round(_chargedPercent * 100);
    }

    private string AppendString(string text, string append1, string append2, string append3)
    {
        text = text.Replace("{1}", append1);
        text = text.Replace("{2}", append2);
        text = text.Replace("{3}", append3);
        return text;
    }

    public void LeaveGame()
    {
        //return to mini game title screen
        _miniGameManager._win.Description = AppendString(_gameOverText, _electronsGrabbed.ToString(), _electronsMissed.ToString(), _chargedPercent.ToString());
        _player.transform.localPosition = _startPos;
        _miniGameManager.EndGame(_playerWins);
    }

    public Vector2 ScreenBounds()
    {
        Vector2 screenBounds = new Vector2(GameBounds.transform.position.x, GameBounds.transform.position.y);
        return screenBounds;
    }

    private void DestroyElectrons()
    {
        foreach (Transform child in SpawnContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
