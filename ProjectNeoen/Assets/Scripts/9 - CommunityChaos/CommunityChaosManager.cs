using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class CommunityChaosManager : MonoBehaviour
{
    [Header("General Gameplay Variables")]
    [SerializeField] private List<NPCEscalation> _npcs;
    [SerializeField] private List<Transform> _NPCPositions; 
    [SerializeField] public MiniGameManager _miniGameManager;
    [SerializeField] private AudioManager _audioManager;
    public CinemachineVirtualCamera Cam;    
    [SerializeField] private SceneTransitionManager _transition;
    [SerializeField] private Transform _gameTransform;
    [SerializeField] private Canvas _canvasBG;
    [SerializeField] private Canvas _canvasNPCs;

    [Header("UI Objects")]
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private float _startingTime = 120f;

    [Header("PowerUps")]
    [SerializeField] private FoodPowerUp _foodPowerUp;
    [SerializeField] private WindowPowerUp _windowPowerUp;
    [SerializeField] private EnergyDrinkPowerUp _energyDrinkPowerUp;
    [SerializeField] private float _windowOpenTimer;
    [SerializeField] private float _windowPowerUpTimer;
    [SerializeField] private float _energyDrinkTimer;
    [SerializeField] private int _energyQuestionLives;
    [SerializeField] private int _energyOpinionLives;
    [SerializeField] private int _energyTakeoverLives;

    [Header("Lives")]
    [SerializeField] private int _questionLives;
    [SerializeField] private int _opinionLives;
    [SerializeField] private int _takeOverLives;

    [Header("NPC Variables")]
    [SerializeField] private float _npcEscaltionTimer;
    [SerializeField] private float _npcResetTimer;

    //global variables
    [SerializeField] private AlarmClockRinging _clock;
    [SerializeField] private float _escalationTimer;
    [SerializeField] private float _escalationTimerReset = 30;
    [SerializeField] private int _gameQuarter;
    [SerializeField] private float _timeRemaining;
    [SerializeField] private float _triggerTimer = 5;
    [SerializeField] private float _triggerTimerReset;
    [SerializeField] private List<NPCEscalation> _currentNpcs;
    [SerializeField] private int _score;

    [SerializeField] private TextMeshProUGUI _takeoverText;
    [SerializeField] private int _takeOvers;
    [SerializeField] private float _takeOverTimer;
    [SerializeField] private string _originalWinText;
    public void TakeOverCounter() => _takeOvers++;
    public bool PlayerWins { get; private set; } = false;
    private bool _fullTakeOver;

    private void Awake()
    {
        _triggerTimerReset = _triggerTimer;
        _npcResetTimer = _npcEscaltionTimer;
        _originalWinText = _miniGameManager._win.Description;
        _clock = GetComponent<AlarmClockRinging>();
        foreach (NPCEscalation npc in _npcs)
        {
            npc.SetReferences(_audioManager, this);
        }
        if (!_miniGameManager._debugTesting)
        {
            _transition = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransitionManager>();
            _gameTransform.localPosition = new Vector3(_transition._camera.transform.position.x, _transition._camera.transform.position.y - 0.1f, 0);
            this.transform.position = new Vector3(_transition._camera.transform.position.x, _transition._camera.transform.position.y, 0);
            _canvasBG.worldCamera = Camera.main;
            _canvasNPCs.worldCamera = Camera.main;
            Cam = _transition.Cam;
            Cam.m_Lens.OrthographicSize = 5f;
        }
    }

    private void SetLives(int question, int opinion, int takeover)
    {
        foreach (NPCEscalation npc in _npcs)
        {
            npc.EnergyDrinkPowerUp(question, opinion, takeover);
        }
    }

    private void ResetGame()
    {
        StopAllCoroutines();
        StartCoroutine(TakeOver(0));
        _currentNpcs.Clear();
        SetLives(_questionLives, _opinionLives, _takeOverLives); //set click lives to original amount
        ResetFoodAndWindowPowerups();
        ResetEnergyDrink();
        _clock.ResetClockRotation();

        foreach(NPCEscalation npc in _npcs)
        {
            npc.ResetNPC();
        }

        _audioManager.Stop("5sec");
        _audioManager.Stop("FullTakeOver");
        _audioManager.Stop("Drink");
        _audioManager.Stop("Alarm");

        _takeoverText.text = "0";
        _fullTakeOver = false;
        _gameQuarter = 3;
        _takeOvers = 0;
        _escalationTimer = _escalationTimerReset;
        _triggerTimer = _triggerTimerReset;
        _timeRemaining = _startingTime;
        _timeText.text = $"{(int)_timeRemaining / 60}:{(int)_timeRemaining % 60:D2}";   
        PlayerWins = false;

        _miniGameManager.SetReset(false); //always call this last
    }

    private void Update()
    {
        if (_miniGameManager.ResetGame) ResetGame(); //reset game after on starting game

        if (_clock.FinishedRinging)
        {
            EndGame();
            _clock.ResetAlarm();
        }

        if (_miniGameManager.Playing)
        {
            _timeRemaining -= Time.deltaTime;
            _timeText.text = $"{(int)_timeRemaining / 60}:{(int)_timeRemaining % 60:D2}";
            SpawnEscalationTimer();

            if (_timeRemaining <= 6) _audioManager.Play("5sec");

            if (_timeRemaining <= 0) GameOver();

            _triggerTimer -= Time.deltaTime;
            if (_fullTakeOver) return;
            if (_triggerTimer <= 0) TriggerNPC();
        }
    }

    private void GameOver()
    {
        _timeRemaining = 0;
        _audioManager.FadeOut("MiniGameMenuTheme");
        _audioManager.Stop("5sec");
        _audioManager.FadeOut("FullTakeOver");
        _audioManager.FadeOut("Drink");
        _audioManager.Play("Alarm");
        PlayerWins = true;
        _miniGameManager._win.Description = AppendString(_miniGameManager._win.Description, _takeOvers.ToString());
        StopAllCoroutines();
        StartCoroutine(TakeOver(0));
        ResetEnergyDrink();
        _clock.TriggerClockRinging();
        _miniGameManager.SetPlaying(false);
    }

    public void TriggerAdjacent(NPCEscalation npc)
    {
        if (_fullTakeOver) return;

        int parentPos = _NPCPositions.IndexOf(npc.transform.parent);
        int NPCPosition = _npcs.IndexOf(npc);

        if (parentPos % 7 == 0 && _NPCPositions[parentPos + 1].gameObject.transform.childCount > 0) //start of row
        {
            AdjacentActivation(NPCPosition + 1);
        }
        else if (parentPos % 7 == 6 && _NPCPositions[parentPos - 1].gameObject.transform.childCount > 0) //end of row
        {
            AdjacentActivation(NPCPosition - 1);
        }
        else //escalte both sides
        {
            if (parentPos != 0 && _NPCPositions[parentPos - 1].gameObject.transform.childCount > 0) AdjacentActivation(NPCPosition - 1);
            if (parentPos != _NPCPositions.Count - 1 && _NPCPositions[parentPos + 1].gameObject.transform.childCount > 0) AdjacentActivation(NPCPosition + 1);
        }
    }

    private void AdjacentActivation(int index)
    {
        NPCEscalation adjacentNPC = _npcs[index].GetComponent<NPCEscalation>();
        if (_currentNpcs.Contains(adjacentNPC)) return;
        _currentNpcs.Add(adjacentNPC);
        adjacentNPC.Activate(_npcEscaltionTimer);
    }

    private void TriggerNPC() //choose a random NPC and activate them
    {
        if (_fullTakeOver) return;

        for (int i = 0; i < _gameQuarter; i++)
        {
            float randomTimer = UnityEngine.Random.Range(0, _triggerTimerReset);
            StartCoroutine(DelayActivate(randomTimer));
        }
        _triggerTimer = _triggerTimerReset;
    }

    private IEnumerator DelayActivate(float spawnTimer)
    {
        yield return new WaitForSeconds(spawnTimer);
        int index = UnityEngine.Random.Range(0, _npcs.Count);
        while (_currentNpcs.Contains(_npcs[index])) // make sure there isn't an index currently being used
        {
            if (_currentNpcs.Count >= _npcs.Count - 1) break; //IMPORTANT break out of while loop when all NPC's are activated, game will crash without this
            index = UnityEngine.Random.Range(0, _npcs.Count);
        }
        if (!_currentNpcs.Contains(_npcs[index]) && !_fullTakeOver)
        {
            _currentNpcs.Add(_npcs[index]);
            _npcs[index].Activate(_npcEscaltionTimer);
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
            _gameQuarter++;
            _escalationTimer = _escalationTimerReset;
        }
    }

    public void DeEscalateNPC(NPCEscalation npc)
    {
        _currentNpcs.Remove(npc);
    }

    public void FullTakeOverTrigger()
    {
        if (_fullTakeOver == false)
        {
            _fullTakeOver = true;
        }

        _audioManager.Play("FullTakeOver");
        _takeOvers++;
        _takeoverText.text = _takeOvers.ToString(); 

        foreach (NPCEscalation npc in _npcs)
        {
            if (npc._coroutine != null) StopCoroutine(npc._coroutine);
            npc.ResetNPC();
            npc.FullTakeover();
        }

        StartCoroutine(TakeOver(_takeOverTimer));
    }

    private IEnumerator TakeOver(float timer)
    {
        yield return new WaitForSeconds(timer);

        foreach (NPCEscalation npc in _npcs)
        {
            DeEscalateNPC(npc);
            npc.ResetNPC();
        }

        ResetFoodAndWindowPowerups();

        _fullTakeOver = false;
    }

    private void ResetFoodAndWindowPowerups()
    {
        _foodPowerUp.ResetFood();
        _windowPowerUp.ResetWindow();
        _npcEscaltionTimer = _npcResetTimer;
        _triggerTimerReset = _npcResetTimer;
        _triggerTimer = _npcResetTimer;
    }

    public void WindowPowerUp()
    {
        _audioManager.Play("Window");
        _npcEscaltionTimer = _windowPowerUpTimer;
        _triggerTimerReset = _windowPowerUpTimer;
    }

    public void FoodPowerUp()
    {
        _audioManager.Play("Chips");
        //_gameQuarter /= 2;
        _gameQuarter = _gameQuarter - 1;
        if (_gameQuarter < 1) _gameQuarter = 1;
    }

    public void EnergyDrinkPowerUp()
    {
        _audioManager.Mute("MiniGameMenuTheme");
        _audioManager.FadeIn("Drink", 1);
        StartCoroutine(DrinkEnergyDrink());
    }

    private IEnumerator DrinkEnergyDrink()
    {
        SetLives(_energyQuestionLives, _energyOpinionLives, _energyTakeoverLives);
        float timeRemaining = _energyDrinkTimer;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        _audioManager.Mute("MiniGameMenuTheme");
        SetLives(_questionLives, _opinionLives, _takeOverLives);
    }

    private void ResetEnergyDrink()
    {
        _energyDrinkPowerUp.ResetEnergyDrink();
    }

    private string AppendString(string text, string append1)
    {
        text = _originalWinText;
        text = text.Replace("{1}", append1);
        return text;
    }

    public void EndGame()
    {
        StopAllCoroutines();
        StartCoroutine(TakeOver(0));
        _miniGameManager.EndGame(PlayerWins);
    }
}