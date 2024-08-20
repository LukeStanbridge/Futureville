using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;


public class ClickController : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Vector2 _clickedPos;
    private NavMeshAgent _agent;

    [SerializeField] private NPCDialogueManager _dialogueManager;
    [SerializeField] private CharacterSelectionManager _charSelectManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PlayerAnimations _playerAnimations;

    private Vector3 _startPos;

    public bool OnGrass;
    [SerializeField] private string _footstepAudio;
    //[SerializeField] private Canvas _canvas;
    //[SerializeField] private bool _firstClick;
    [SerializeField] private SceneTransitionManager _sceneTransition;
    private Tween _clickText;

    [Header("Walking In Water")]
    [SerializeField] private GameObject _ripple;
    [SerializeField] private ParticleSystem _rippleParticles;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _spriteMask;
    [field: SerializeField] public bool _inWater { get; private set; } = false;


    private void Awake()
    {
        _startPos = transform.position;
        _playerAnimations = GetComponent<PlayerAnimations>();
        _playerControls = GameManager.Instance.PlayerControls;
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        //_firstClick = false;
        _clickedPos = new Vector2(transform.position.x, transform.position.y);
        _audioManager = FindObjectOfType<AudioManager>();
        //_canvas = GetComponentInChildren<Canvas>();
        _footstepAudio = "walkingConcrete";
        _spriteMask.SetActive(false);
        _rippleParticles = _ripple.GetComponent<ParticleSystem>();
        _rippleParticles.Stop();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Futureville) return; //pause game if not in futureville
        PlayerInput();
        Move();
    }

    private void PlayerInput() //get input from mouse or touch to move player
    {
        if (_sceneTransition.Transitioning || _dialogueManager.DialogueOpen) return; //kill movement will transitioning scenes or dialogue is happening

        ClickMove();

        TouchMove();
    }

    private void ClickMove() //mouse and keyboard controls
    {
        if (GameManager.Instance.Controls != ControlType.KeyboardAndMouse) return;
        if (!_playerControls.Movement.ClickMove.WasPressedThisFrame()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        _clickedPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        StartRunning();
    }

    private void TouchMove() //touch controls
    {
        if (GameManager.Instance.Controls != ControlType.TouchControls) return;
        if (!_playerControls.Movement.TouchMove.WasPerformedThisFrame()) return;
        if (_playerControls.Movement.TouchMove.phase != InputActionPhase.Performed) return;
        if (IsPointerOverUIObject()) return;
        _clickedPos = Camera.main.ScreenToWorldPoint(_playerControls.Movement.TouchMove.ReadValue<Vector2>());
        StartRunning();
    }

    private void StartRunning()
    {
        if (_playerAnimations.State != CurrentAnimationState.Running)
        {
            _playerAnimations.State = CurrentAnimationState.Running;
            if (_inWater)
            {
                _audioManager.Play("splashing");
            }
        }
    }

    private bool IsPointerOverUIObject() //check if mouse is over UI object
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void Move() //move agent
    {
        _agent.SetDestination(_clickedPos);
    }

    public void StopPlayer()
    {
        _clickedPos = transform.position;
        _agent.SetDestination(_clickedPos);
    }

    public void DestinationReached() //check if destination reached
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    if (_playerAnimations.State != CurrentAnimationState.Transition) _playerAnimations.State = CurrentAnimationState.Transition;
                    if (_inWater)
                    {
                        _audioManager.Stop("splashing");
                    }
                }
            }
        }
    }

    public void PlayFootstepAudio()
    {
        if (GameManager.Instance.State == GameState.MiniGame) return;
        if (!_audioManager.IsAudioPlaying("skippingConcrete") && _charSelectManager.PlayerAvatarName == "Char2" && OnGrass == false)
        {
            _audioManager.Stop(_footstepAudio);
            _audioManager.Play("skippingConcrete");
            _footstepAudio = "skippingConcrete";
        }
        else if (!_audioManager.IsAudioPlaying("walkingConcrete") && OnGrass == false && _charSelectManager.PlayerAvatarName != "Char2")
        {
            _audioManager.Stop(_footstepAudio);
            _audioManager.Play("walkingConcrete");
            _footstepAudio = "walkingConcrete";
        }
        else if (!_audioManager.IsAudioPlaying("walkingGrass") && OnGrass == true && _playerAnimations.State == CurrentAnimationState.Running)
        {
            _audioManager.Stop(_footstepAudio);
            _audioManager.Play("walkingGrass");
            _footstepAudio = "walkingGrass";
        }
    }

    public void StopFootstepAudio()
    {
        if (_audioManager.IsAudioPlaying("walkingConcrete")) _audioManager.Stop("walkingConcrete");
        if (_audioManager.IsAudioPlaying("walkingGrass")) _audioManager.Stop("walkingGrass");
        if (_audioManager.IsAudioPlaying("skippingConcrete")) _audioManager.Stop("skippingConcrete");
        if (_audioManager.IsAudioPlaying("splashing")) _audioManager.Stop("splashing");
    }

    public void WalkInWater(bool inWater)
    {
        _inWater = inWater;

        if (_inWater)
        {
            _spriteMask.SetActive(true);
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            if (_playerAnimations.State == CurrentAnimationState.Running)
            {
                _audioManager.Play("splashing");
            }
            _rippleParticles.Play();
        }
        else
        {
            _audioManager.Stop("splashing");
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            _spriteMask.SetActive(false);
            _rippleParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    //private void InfoText()
    //{
    //    if (_firstClick) return;

    //    _clickText.Kill();
    //    _canvas.gameObject.transform.DOScale(0, 0.5f);
    //    _firstClick = true;
    //}
}
