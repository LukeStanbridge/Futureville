using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class FoodPowerUp : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CommunityChaosManager _communityChaosManager;
    [SerializeField] private Transform _snacksTable;
    [SerializeField] private Vector2 _startPos;
    [SerializeField] private float _snacksOffset;
    [SerializeField] private Vector3 _snacksRotation;
    [SerializeField] private GameObject _emptyBowl;
    [SerializeField] private GameObject _fullBowl;
    [SerializeField] private float _rotateAngle;
    [SerializeField] private float _rotateTimer;
    public bool CanGiveFood;
    private RectTransform _rectTransform;
    private Image _image;
    private GameObject originalParent;
    private Tween _chipsRotate;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _startPos = _rectTransform.anchoredPosition;
        originalParent = this.transform.parent.gameObject;
    }

    private void TweenChips(float delay)
    {
        _chipsRotate = transform.DORotate(new Vector3(0, 0, _rotateAngle), _rotateTimer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(delay);
    }

    public void ResetFood()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        GetComponent<Image>().enabled = true;
        _fullBowl?.SetActive(false);
        _emptyBowl?.SetActive(true);
        _rectTransform.sizeDelta = new Vector2(100, 100);
        this.transform.SetParent(originalParent.transform);
        _rectTransform.DOAnchorPos(_startPos, 1);
        _rectTransform.rotation = Quaternion.Euler(Vector3.zero);
        _chipsRotate.Kill();
        TweenChips(3.0f);
    }

    public void DropFood()
    {
        _communityChaosManager.FoodPowerUp();
        _fullBowl?.SetActive(true);
        _emptyBowl?.SetActive(false);
        GetComponent<Image>().enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _chipsRotate.Kill();
        _rectTransform.rotation = Quaternion.Euler(Vector3.zero);
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanGiveFood)
        {
            _rectTransform.position = new Vector3(_snacksTable.transform.position.x, _snacksTable.transform.position.y + _snacksOffset, 0);
            _rectTransform.rotation = Quaternion.Euler(_snacksRotation);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            DropFood();
        }
        else
        {
            this.transform.SetParent(originalParent.transform);
            _rectTransform.anchoredPosition = _startPos;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            TweenChips(0);
        }

        CanGiveFood = false;
        _image.raycastTarget = true;
    }
}
