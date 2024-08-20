using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Data")]
    public GameState State;
    public ControlType Controls;
    public bool GamePaused;
    public PlayerControls PlayerControls;

    [Header("Important Game References")]
    [SerializeField] private GameObject _mainGameWorld;
    [SerializeField] private GameObject _gameUIButtons;
    [SerializeField] private GameObject _camConfiner;
    [SerializeField] private GameObject _npcDialogue;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private ClickController _playerController;
    [SerializeField] private PlayerAnimations _playerAnimations;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private NPCDialogueManager _npcDialogueManager;
    [SerializeField] private TradingCardManager _tradingCardManager;
    [SerializeField] private ActivateDialogue _mayorStartup;
    [SerializeField] private CameraZoom _camZoom;

    [Header("NPCs")]
    [SerializeField] public List<MiniGameNPC> MiniGameNPC;
    [SerializeField] public List<TradingCardNPC> WorldNPC;
    [SerializeField] public MiniGameNPC ObjectiveNPC;
    [SerializeField] private bool _setObjectiveNPC;
    [SerializeField] public string ObjectiveName;
    [SerializeField] public string ObjectiveLocation;

    public static event Action<GameState> OnGameStateChanged;
    public bool Mute;

    private void OnApplicationQuit()
    {
        Instance = null;
        DOTween.KillAll();
        Destroy(gameObject);
    }
    
    private void Awake()
    {
        Instance = this;
        PlayerControls = new PlayerControls();
        _setObjectiveNPC = false;
        Mute = false;
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
    }

    private void Start()
    {
        AudioManager.Instance.Play("IntroAudio");
        UpdateGameState(GameState.StartMenu);
    }

    private void Update()
    {
        if (Controls == ControlType.NotYetSet) ControlCheck();
    }

    private void ControlCheck()
    {
        if (PlayerControls.Movement.ClickMove.WasPressedThisFrame()) //mouse and keyboard controls
        {
            Controls = ControlType.KeyboardAndMouse; //set control type
            _infoText.text = "Click to move";
        }
        else if (PlayerControls.Movement.TouchMove.phase == InputActionPhase.Performed) //Touch controls
        {
            Controls = ControlType.TouchControls; //set control type
            _infoText.text = "Touch to move";
        }
    }

    public void UpdateGameState(GameState newState) //change state of game
    {
        if (State == newState) return;
        State = newState;

        switch (newState)
        {
            case GameState.StartMenu:
                HandleMainMenu();
                break;
            case GameState.WorldZoom:
                HandleCamZoom();
                break;
            case GameState.Futureville:
                HandleFuturevilleWorld();
                break;
            case GameState.InGameMenus:
                HandleGameUI();
                break;
            case GameState.MiniGame:
                HandleMiniGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleMainMenu() //set the start of the game
    {
        Time.timeScale = 1.0f;
        _mainGameWorld.SetActive(true);
    }

    private void HandleCamZoom()
    {
        Time.timeScale = 1.0f;
        GamePaused = false;
        _mainGameWorld?.SetActive(true);
        AudioManager.Instance.FadeOut("IntroAudio");
        AudioManager.Instance.FadeIn("ambiance", 1);
        _playerAnimations.SetPersonaAvatar();
        SetObjectiveNPC();
        _camZoom.ZoomIn();
    }

    private void HandleFuturevilleWorld() //take player to the main game world and set character avatar/persona
    {
        Time.timeScale = 1.0f;
        GamePaused = false;
        _mainGameWorld?.SetActive(true);
        _gameUIButtons.SetActive(true);
        _npcDialogue.SetActive(true);
        _camConfiner.SetActive(true);
        _playerController.GetComponent<SpriteRenderer>().enabled = true;
        AudioManager.Instance.FadeOut("IntroAudio");
        AudioManager.Instance.FadeIn("ambiance", 1);
        if (_playerController._inWater) _playerController.WalkInWater(_playerController._inWater);
    }

    private void HandleGameUI() //display the in-game UI(trading crads and game objective)
    {
        _playerController.StopFootstepAudio();
        Time.timeScale = 0;
        _gameUIButtons.SetActive(false);
        GamePaused = true;
    }

    private void HandleMiniGame() //transition player to mini game
    {
        _mainGameWorld.SetActive(false);
        _gameUIButtons.SetActive(false);
        _npcDialogue.SetActive(false);
        _camConfiner.SetActive(false);
        _playerController.GetComponent<SpriteRenderer>().enabled = false;
        AudioManager.Instance.Stop("ambiance");
    }

    private void SetObjectiveNPC()
    {
        if (_setObjectiveNPC) return;
        string persona = _playerAnimations._playerPersona;

        switch (persona)
        {
            case "Social":
                CheckObjective(global::MiniGameNPC.SceneName.CommunityChaos);
                _tradingCardManager.LoadCardSet(_tradingCardManager.WorkingWithPeople);
                break;
            case "Investigative":
                CheckObjective(global::MiniGameNPC.SceneName.MaximumPower);
                _tradingCardManager.LoadCardSet(_tradingCardManager.SolvingProblems);
                break;
            case "Realistic":
                CheckObjective(global::MiniGameNPC.SceneName.ElectronGrab);
                _tradingCardManager.LoadCardSet(_tradingCardManager.WorkingWithHands);
                break;
            case "Artistic":
                CheckObjective(global::MiniGameNPC.SceneName.Scrambled);
                _tradingCardManager.LoadCardSet(_tradingCardManager.BeingCreative);
                break;
            case "Enterprising":
                CheckObjective(global::MiniGameNPC.SceneName.BossBattle);
                _tradingCardManager.LoadCardSet(_tradingCardManager.LeadingAndGuiding);
                break;
            case "Conventional":
                CheckObjective(global::MiniGameNPC.SceneName.PickinTeams);
                _tradingCardManager.LoadCardSet(_tradingCardManager.DataAndDetails);
                break;
            default:
                Debug.Log("bugged");
                break;
        }
    }

    private void CheckObjective(MiniGameNPC.SceneName scene)
    {
        foreach(MiniGameNPC npc in MiniGameNPC)
        {
            if (npc.SceneToLoad == scene)
            {
                npc.SetObjectiveNPC();
                ObjectiveNPC = npc;
                ObjectiveName = npc.Name;
                ObjectiveLocation = npc.Location;
                _npcDialogueManager.UpdateObjectiveLocation();
                _setObjectiveNPC = true;
            }
        }
        _mayorStartup.OpeningConvo();
    }
}

public enum GameState
{
    StartMenu,
    WorldZoom,
    Futureville,
    InGameMenus,
    MiniGame
}

public enum ControlType
{
    NotYetSet,
    TouchControls,
    KeyboardAndMouse
}
