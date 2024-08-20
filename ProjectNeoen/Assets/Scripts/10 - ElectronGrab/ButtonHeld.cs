using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHeld : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool _pressed { get; private set; }

    //public EventSystem eventSystem;
    //public GraphicRaycaster graphicRaycaster;

    [SerializeField] private RectTransform _buttonRect;
    [SerializeField] private float _buttonStart = 10.0f;
    [SerializeField] private float _animateTime;
    [SerializeField] private bool _touching;

    private void Awake()
    {
        _touching = false;
    }

    //private void Update()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            if (IsPointerOverUIObject(touch))
    //            {
    //                _pressed = true;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        _pressed = false;
    //    }
    //}

    //private bool IsPointerOverUIObject(Touch touch)
    //{
    //    PointerEventData pointerEventData = new PointerEventData(eventSystem);
    //    pointerEventData.position = new Vector2(touch.position.x, touch.position.y);

    //    List<RaycastResult> results = new List<RaycastResult>();
    //    graphicRaycaster.Raycast(pointerEventData, results);

    //    foreach (RaycastResult result in results)
    //    {
    //        if (result.gameObject == this.gameObject)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        //_pressed = true;
        if (!_touching)
        {
            AnimateButton(0);
            _touching = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //_pressed = false;
        if (_touching)
        {
            AnimateButton(_buttonStart);
            _touching = false;
        }
    }

    private void AnimateButton(float dist)
    {
        _buttonRect.DOAnchorPosY(dist, _animateTime).SetUpdate(true);
    }
}
