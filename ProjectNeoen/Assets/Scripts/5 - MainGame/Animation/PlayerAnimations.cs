using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private CharacterSelectionManager _charSelectManager;
    [SerializeField] private NPCDialogueManager _dialogueManager;
    [SerializeField] private ClickController _clickController;
    [SerializeField] private AudioManager _soundManager;
    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    public CurrentAnimationState State;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [field: SerializeField] public string _playerPersona {  get; private set; }
    [SerializeField] private string _currentState;
    [SerializeField] private float _specialIdleCounter = 0;
    [SerializeField] private float _specialIdleTimer = 5f;
    [SerializeField] private float _animTime;
    private const string Idle = "Idle";
    private const string Idle_Special = "IdleSpecial";
    private const string Running = "Running";
    private const string Transition = "Transition";
    [SerializeField] private float _direction;

    public void SetPersonaAvatar()
    {
        _animator.runtimeAnimatorController = _charSelectManager.AnimatorController;
        _playerPersona = _charSelectManager.PlayerPersona.PlayerPersona.ToString();
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _clickController = GetComponent<ClickController>();
        _soundManager = FindObjectOfType<AudioManager>();
        State = CurrentAnimationState.Idle;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Futureville) return;
        PlayerAnimatorState();
        AdjustPlayerDirection();
    }

    private void PlayerAnimatorState()
    {
        PlaySpecialIdleAnimation();
        PlayIdleAnimation();
        PlayTransitionAnimation();
        PlayRunningAnimation();
    }

    private void PlaySpecialIdleAnimation() //play special idle animation once the special idle timer has finished
    {
        if (State != CurrentAnimationState.SpecialIdle) return;

        ChangeAnimatorState(Idle_Special);

        if (_animTime == 0) _animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        _animTime -= Time.deltaTime;

        if (_animTime <= 0)
        {
            _specialIdleCounter = 0;
            _animTime = 0;
            State = CurrentAnimationState.Idle;
        }
    }

    private void PlayIdleAnimation()
    {
        if (State != CurrentAnimationState.Idle) return;

        ChangeAnimatorState(Idle);
        IdleTimer();
    }

    private void PlayTransitionAnimation()
    {
        if (State != CurrentAnimationState.Transition) return;

        ChangeAnimatorState(Transition);

        if (_animTime == 0) _animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        _animTime -= Time.deltaTime;

        if (_animTime <= 0)
        {
            _specialIdleCounter = 0;
            _animTime = 0;
            State = CurrentAnimationState.Idle;
        }
    }

    private void PlayRunningAnimation()
    {
        if (State != CurrentAnimationState.Running) return;

        ChangeAnimatorState(Running);
        _clickController.PlayFootstepAudio();
        _clickController.DestinationReached(); //check if destination reached
    }

    private void ChangeAnimatorState(string newState) //change animation to new state
    {
        if (newState == _currentState) return;
        _animator.CrossFadeInFixedTime(newState, 0.1f, 0);
        _currentState = newState;

        if (State == CurrentAnimationState.Transition)
        {
            _clickController.StopFootstepAudio();
            _soundManager.Play("walkingStopping");
        }
        else if (State == CurrentAnimationState.Idle)
        {
            _clickController.StopFootstepAudio();
        }
        //else if (State == CurrentAnimationState.Running)
        //{
        //    _clickController.PlayFootstepAudio();
        //}
    }

    private bool IsAnimationPlaying(Animator animator, string stateName) //check if animation has finished (only use this to check non looping aniamtions)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f ||
            animator.IsInTransition(0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void IdleTimer()
    {
        if (_clickController._inWater) return; //disable special idle in water
        if (_dialogueManager.DialogueOpen) return; //disable special idle in dialogue
        if (_specialIdleCounter >= _specialIdleTimer) State = CurrentAnimationState.SpecialIdle;
        _specialIdleCounter += Time.deltaTime; //start timer for special idle animation
    }

    private void AdjustPlayerDirection() //flip sprite in direction player is moving
    {
        Vector3 normalizedMovement = _agent.desiredVelocity.normalized;

        Vector3 forwardVector = Vector3.Project(normalizedMovement, transform.forward);

        Vector3 rightVector = Vector3.Project(normalizedMovement, transform.right);

        float forwardVelocity = forwardVector.magnitude * Vector3.Dot(forwardVector, transform.forward); //used to check what direction the player is moving towards up and down

        float rightVelocity = rightVector.magnitude * Vector3.Dot(rightVector, transform.right); //used to check what direction the player is moving towards left and right


        if (rightVelocity > 0.01f)
        {
            PlayRightAnimation();
        }
        else if (rightVelocity < -0.01f)
        {
            PlayLeftAnimation();
        }
    }

    private void PlayRightAnimation()
    {
        if (_charSelectManager.PlayerAvatar.Symetrical) _spriteRenderer.flipX = true;
        else
        {
            if (_direction == 0)
            {
                _animator.runtimeAnimatorController = _charSelectManager.AnimatorControllerRight;
                _animator.CrossFadeInFixedTime(_currentState, 0.1f, 0);
                _direction++;
            }
        }
    }

    private void PlayLeftAnimation()
    {
        if (_charSelectManager.PlayerAvatar.Symetrical) _spriteRenderer.flipX = false;
        else
        {
            if (_direction == 1)
            {
                _animator.runtimeAnimatorController = _charSelectManager.AnimatorController;
                _animator.CrossFadeInFixedTime(_currentState, 0.1f, 0);
                _direction--;
            }
        }
    }

    public void FaceNPC(Vector3 npcPos)
    {
        if (npcPos.x < transform.position.x)
        {
            PlayLeftAnimation();
        }
        if (npcPos.x > transform.position.x)
        {
            PlayRightAnimation();
        }
    }
}

public enum CurrentAnimationState
{
    Idle,
    SpecialIdle,
    Transition,
    Running
}
