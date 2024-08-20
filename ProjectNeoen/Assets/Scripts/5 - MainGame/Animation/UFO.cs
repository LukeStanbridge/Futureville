using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UFO : MonoBehaviour
{
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private float _flyTime;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private string _currentState;
    [SerializeField] private Transform _abductee;

    [SerializeField] private Transform _alienHead;
    [SerializeField] private float _headMovement;
    [SerializeField] private float _upTimer;
    [SerializeField] private float _downTimer;
    [SerializeField] private float _headBobTimer;
    [SerializeField] private float _headPeekTimer;


    private const string RayIn = "UFORayIn";
    private const string RayLoop = "UFORayLoop";
    private const string RayOut = "UFORayOut";

    private bool _ufoSequence = false;

    private void Awake()
    {
        _startPoint = transform.position;
        _spriteRenderer.color = new Color(1, 1, 1, 0);
        GetComponent<AudioSource>().mute = true;
    }

    private void Start()
    {
        InvokeRepeating("PeakingHead", 0, _headBobTimer);
    }

    private void PeakingHead()
    {
        _alienHead.DOMoveY(_alienHead.position.y + _headMovement, _upTimer).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _alienHead.DOMoveY(_alienHead.position.y, _headPeekTimer).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _alienHead.DOMoveY(_alienHead.position.y - _headMovement, _downTimer).SetEase(Ease.InOutSine);
            });
        });
    }

    public void TriggrUFO()
    {
        if (_ufoSequence) return;
        GetComponent<AudioSource>().mute = false;
        StartCoroutine(UFOSequence());
    }

    private IEnumerator UFOSequence()
    {
        _ufoSequence = true;
        Tween myTween = transform.DOMove(_endPoint.position, _flyTime); // move ufo into position
        yield return myTween.WaitForCompletion();
        _spriteRenderer.color = new Color(1, 1, 1, 1); //enable transparency
        _animator.Play(RayIn); //play ray coming in
        float animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);
        _animator.Play(RayLoop); //start UFO ray loop
        _abductee.GetComponent<SpriteRenderer>().sortingOrder = 2;
        _abductee.transform.DOMove(new Vector3(_endPoint.position.x, _endPoint.position.y - 0.5f, 0), 3); //move abductee into ship
        _abductee.transform.DOScale(0.5f, 3);
        yield return new WaitForSeconds(3);
        _animator.Play(RayOut); //play UFO ray going up
        animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);
        _spriteRenderer.color = new Color(1, 1, 1, 0); 
        transform.DOMove(_startPoint, _flyTime); //move UFO and abductee out of screen
        _abductee.transform.DOMove(_startPoint, _flyTime);
        yield return new WaitForSeconds(_flyTime);
        GetComponent<AudioSource>().mute = true;
    }
}
