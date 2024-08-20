using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimations : MonoBehaviour
{
    private Animator _animator;
    private MiniGameNPC _miniGameNPC;
    private ActivateDialogue _activateDialogue;
    public NPCAnimationState State;
    [SerializeField] private string _currentState;
    [SerializeField] private float _helpCounter = 0;
    [SerializeField] private float _helpTimer = 5f;
    [SerializeField] private float _animTime;
    private const string Idle = "Idle";
    private const string Help = "Help";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _miniGameNPC = GetComponent<MiniGameNPC>();
        _activateDialogue = GetComponent<ActivateDialogue>();
    }

    private void Update()
    {
        if (_animator == null) return;
        if (GameManager.Instance.State != GameState.Futureville) return;
        PlayerAnimatorState();
    }

    private void PlayerAnimatorState()
    {
        PlaySpecialIdleAnimation();
        PlayIdleAnimation();
    }

    private void PlaySpecialIdleAnimation() //play special idle animation once the special idle timer has finished
    {
        if (State != NPCAnimationState.Help) return;

        ChangeAnimatorState(Help);

        if (_animTime == 0) _animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        _animTime -= Time.deltaTime;

        if (_animTime <= 0)
        {
            _helpCounter = 0;
            _animTime = 0;
            State = NPCAnimationState.Idle;
        }
    }

    private void PlayIdleAnimation()
    {
        if (State != NPCAnimationState.Idle) return;

        ChangeAnimatorState(Idle);
        IdleTimer();
    }

    private void IdleTimer()
    {
        if (!_miniGameNPC.ObjectiveNPC) return;
        if (_activateDialogue.CanClickNPC) return;
        if (_helpCounter >= _helpTimer) State = NPCAnimationState.Help;
        _helpCounter += Time.deltaTime; //start timer for special idle animation
    }

    private void ChangeAnimatorState(string newState) //change animation to new state
    {
        if (newState == _currentState) return;
        _animator.CrossFadeInFixedTime(newState, 0.1f, 0);
        _currentState = newState;
    }
}

public enum NPCAnimationState
{
    Idle,
    Help
}
