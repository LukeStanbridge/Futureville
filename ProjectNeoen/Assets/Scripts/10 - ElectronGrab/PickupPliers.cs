using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickupPliers : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Dragging variables and references
    [SerializeField] private GrabberMovement _grabberMovement;
    public bool IsDragging { get; private set; }
    private RectTransform _rectTransform;
    private Vector2 _startPos;
    private Image _image;

    [Header("MALFUNCTION TWEEN")]
    [SerializeField] private float _angle;
    [SerializeField] private float _tweenWiggleTimer;
    private Tween _wiggleTween;

    [Header("FIXING TIMER")]
    [SerializeField] private Transform _fixingTimerContainer;
    [SerializeField] private Image _countdownImage;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _fixingTimer;
    [SerializeField] private float _malfunctionTimeReset;

    //Setters
    public void SetDisplayContainer(bool display) => _fixingTimerContainer.gameObject?.SetActive(display);
    public void SetMalfunctionTimer(float time)
    {
        _malfunctionTimeReset = time;
        _fixingTimer = _malfunctionTimeReset;
        _countdownImage.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPos = _rectTransform.anchoredPosition;
        _image = GetComponent<Image>();
        SetDisplayContainer(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        KillWiggleTween();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
        IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _fixingTimer = _malfunctionTimeReset;
        SetDisplayContainer(false);
        _grabberMovement.ResetFixTimer();
        _grabberMovement.ResetMouse();
        _rectTransform.anchoredPosition = _startPos;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        IsDragging = false;
    }

    public void CountDownTimer()
    {
        _fixingTimer -= Time.deltaTime;
        _timerText.text = $"{(int)_fixingTimer % 60:D1}";
    }

    public void RotateFixingTimer()
    {
        _countdownImage.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart)
        .SetRelative()
        .SetEase(Ease.Linear);
    }

    public void WigglePliers()
    {
        if (IsDragging) return;
        _wiggleTween = transform.DORotate(new Vector3(0, 0, _angle), _tweenWiggleTimer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void KillWiggleTween()
    {
        _wiggleTween.Kill();
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
