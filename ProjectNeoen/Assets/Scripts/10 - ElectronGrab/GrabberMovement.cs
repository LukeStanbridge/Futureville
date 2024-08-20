using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GrabberMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _linearDrag;
    [SerializeField] private bool _changingDirection;
    [SerializeField] private ElectronGrabManager _grabManager;
    [SerializeField] private MiniGameManager _miniGameManager;
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private PickupPliers _pliers;
    [SerializeField] private ButtonHeld _left;
    [SerializeField] private ButtonHeld _right;
    [SerializeField] private GameObject _gameBounds;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _mouseOver;
    [SerializeField] private float _fixingTimer = 0;
    [SerializeField] private float _fixingTime;
    public void ResetFixTimer() { _fixingTimer = 0; }   
    public void ResetMouse() { _mouseOver = false; }
    [SerializeField] private bool _turnLeft;
    [SerializeField] private bool _turnRight;
    [SerializeField] private Sprite _malfunctionSprite;
    [SerializeField] private Sprite _normalSprite;

    private float _speedMultiplier = 100;
    private float _playerMaxSpeed; 
    private PlayerControls _playerControls;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Vector3 _playerScale;

    private Color _playerColor;

    //pulse effects
    private Vector3 _minScale;
    private Vector3 _maxScale;
    private Vector3 _startScale;

    private void Awake()
    {
        _playerScale = transform.localScale;
        _startScale = transform.localScale;
        _minScale = transform.localScale * 0.9f;
        _maxScale = transform.localScale * 1.1f;

        _playerControls = new PlayerControls();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _normalSprite = _spriteRenderer.sprite;
        _playerColor = _spriteRenderer.color;
        _rb = GetComponent<Rigidbody2D>();
        _playerMaxSpeed = 6;
        GetComponent<TrailRenderer>().widthMultiplier *= (_grabManager.GameCanvas.localScale.x * _grabManager.Environmnet.localScale.x);
    }

    private void OnEnable()
    {
        _playerControls.ElectronGrab.Enable();
    }

    private void OnDisable()
    {
        _playerControls.ElectronGrab.Disable();
    }

    private void Update()
    {
        if (!_miniGameManager.Playing) return;
        PlayerInput();
        _changingDirection = (_rb.velocity.x > 0f && _movement.x < 0f) || (_rb.velocity.x < 0f && _movement.x > 0f);
        Turn();

        if (_mouseOver)
        {
            if (!_grabManager.Malfunction) return;
            MalfunctionFixing();
            _pliers.CountDownTimer();
        }
    }

    private void FixedUpdate()
    {
        if (!_miniGameManager.Playing) return;
        Move();
        ApplyLinearDrag(); //disable for ice skating
    }

    private void LateUpdate()
    {
        if (!_miniGameManager.Playing)
        {
            _rb.velocity = Vector3.zero;
            return;
        }
        Vector3 pos = transform.localPosition;
        float xOffset = _gameBounds.GetComponent<RectTransform>().sizeDelta.x / 2;
        float playerWidth = GetComponent<PolygonCollider2D>().points.ElementAt(2).y;
        pos.x = Mathf.Clamp(pos.x, -xOffset - playerWidth, xOffset + playerWidth);
        transform.localPosition = pos;
    }

    private void PlayerInput()
    {
        if (_grabManager.Malfunction) return;
        _movement = _playerControls.ElectronGrab.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (_grabManager.Malfunction || _grabManager.GameOver)
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        _rb.AddForce(_movement * _moveSpeed * _speedMultiplier * Time.deltaTime); //accel and deccel movement
        if (Mathf.Abs(_rb.velocity.x) > _playerMaxSpeed)
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _playerMaxSpeed, 0);
        }
    }

    private void ApplyLinearDrag() //Decelleration
    {
        if (Mathf.Abs(_movement.x) < 0.4f || _changingDirection)
        {
            _rb.drag = _linearDrag;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    private void Turn()
    {
        if (_grabManager.GameOver) return;
        if (_grabManager.Malfunction) return;

        if (_movement.x < 0f)
        {
            if (!_turnLeft) StartCoroutine(ChangeDir(7));
        }

        if ( _movement.x > 0f)
        {
            if (!_turnRight) StartCoroutine(ChangeDir(-7));
        }

        if (_movement.x == 0f)
        {
            if (_turnRight) _turnRight = false;
            if (_turnLeft) _turnLeft = false;
        }
    }

    private IEnumerator ChangeDir(float rotation)
    {
        _turnRight = true;
        _turnLeft = true;
        Tween myTween = transform.DORotate(new Vector3(0, 0, rotation), 0.2f, RotateMode.Fast);
        yield return myTween.WaitForCompletion();
        myTween = transform.DORotate(new Vector3(0, 0, 0), 0.2f, RotateMode.Fast);
        yield return myTween.WaitForCompletion();
    }

    public void MalfunctionIcon()
    {
        //_soundManager.Play("Malfunction");
        _spriteRenderer.sprite = _malfunctionSprite;
        _pliers.WigglePliers();
        _pliers.SetMalfunctionTimer(_fixingTime);
    }

    public void PulseIcon()
    {
        transform.DOScale(_maxScale, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.DOScale(_minScale, 0.5f).SetEase(Ease.InOutSine).OnComplete(PulseIcon);
        });
    }

    public void PulsePlayer()
    {
        transform.localScale = _playerScale;
        transform.DOScale(_playerScale * 1.2f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.DOScale(_playerScale * 1, 0.1f).SetEase(Ease.InOutSine);
        });
    }

    public void ColorChangePlayer()
    {
        _spriteRenderer.color = _playerColor;
        _spriteRenderer.DOColor(new Color(1, 1, 1, 1), 0.1f).OnComplete(() =>
        {
            _spriteRenderer.DOColor(_playerColor, 0.1f);
        });
    }

    private void MalfunctionFixing()
    {
        if (_grabManager.Malfunction && _pliers.IsDragging)
        {
            if (_fixingTimer <= _fixingTime)
            {
                _fixingTimer += Time.deltaTime;
            }
            else
            {
                ResetMalfunction();
            }
        }
    }

    public void ResetMalfunction()
    {
        _soundManager.Stop("Fixing");
        _soundManager.Stop("DeadBattery");
        _spriteRenderer.sprite = _normalSprite;
        _pliers.SetDisplayContainer(false);
        _pliers.SetMalfunctionTimer(_fixingTime);
        _grabManager.SetMalfunction(false);
        _grabManager.SetMalfunctionComplete(true);
        _fixingTimer = 0;
        DOTween.Kill(transform);
        transform.localScale = _startScale;
        _mouseOver = false;
    }

    private void OnMouseOver()
    {
        if (!_pliers.IsDragging) return;
        if (_mouseOver) return;
        if (!_grabManager.Malfunction) return;
        _soundManager.Play("Fixing");
        _pliers.SetDisplayContainer(true);
        _pliers.RotateFixingTimer();
        _mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (!_pliers.IsDragging) return;
        if (!_mouseOver) return;
        _soundManager.Stop("Fixing");
        _pliers.SetDisplayContainer(false);
        _pliers.SetMalfunctionTimer(_fixingTime);
        _fixingTimer = 0;
        _mouseOver = false;
    }
}
