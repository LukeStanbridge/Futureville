using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _buttonRect;
    [SerializeField] private float _buttonHoverUp = 10f;
    [SerializeField] private float _buttonStart = 5.0f;
    [SerializeField] private float _animateTime;
    [field: SerializeField] public bool CanClick { get; private set; }
    public bool DebugMode; //DEBUG ONLY: delete later, for testing mini games outside of Main Game, linked to mini game manager
    [SerializeField, Space(10f)]
    public UnityEvent _onClick;
    
    private Button _thisButton;
    private WaitForSecondsRealtime _delayTimer;
    private float _touchDelayTimer;
    private int _buttonLayer;
    private bool _buttonUp;
    private Tween _buttonTween;
    private bool _touchControls;

    public void SetCanClick(bool canClick) {  CanClick = canClick; }
    public void ResetButton() { PointerExit(); }

    private void Awake()
    {
        _thisButton = GetComponent<Button>();
        _touchDelayTimer = _animateTime * 2;
        _delayTimer = new WaitForSecondsRealtime(_touchDelayTimer); // cache the yield instruction for performance
        CanClick = true;
        _buttonUp = false;
    }

    private void Start()
    {
        _thisButton.onClick.AddListener(() => StartCoroutine(DelayedClickRoutine()));
        _buttonLayer = LayerMask.NameToLayer("Button");
        if (!DebugMode && GameManager.Instance.Controls == ControlType.TouchControls) _touchControls = true;
    }

    private void PointerEnter() //move button label up when mouse is hovering over object
    {
        if (_buttonRect == null || !CanClick || _touchControls) return;

        if (!DebugMode)
        {
            AudioManager.Instance.Stop("touch");
            AudioManager.Instance.Play("touch");
        }
        _buttonRect.DOAnchorPosY(_buttonHoverUp, _animateTime).SetUpdate(true);
        _buttonUp = true;
    }

    private void PointerExit() //move button down when mouse leaves the object area
    {
        if(_buttonRect == null || !CanClick || _touchControls) return;

        if (!DebugMode) AudioManager.Instance.Stop("touch");
        _buttonRect.DOAnchorPosY(_buttonStart, _animateTime).SetUpdate(true);
        _buttonUp = false;
    }

    public void OnPointerClick(PointerEventData eventData) //button clicked
    {
        if (!CanClick) return;
        StartCoroutine(AnimateButton());
        _buttonUp = false;
        CanClick = false;
    }

    private void Update()
    {
        IsPointerOverUIElement();
    }

    public bool IsPointerOverUIElement() //Returns 'true' if we touched or hovering on a button
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) //Animates button depending if mouse is or isn't hovering over a button layered object
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == _buttonLayer && curRaysastResult.gameObject == this.gameObject)
            {
                if (!_buttonUp && CanClick) PointerEnter();
                return true;
            } 
        }
        if (_buttonUp && CanClick) PointerExit();
        return false;
    }
    
    static List<RaycastResult> GetEventSystemRaycastResults() //gets all event system raycast results of current mouse or touch position
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    
    private IEnumerator DelayedClickRoutine() //wait the delay time, then invoke the event
    {
        _thisButton.onClick.RemoveAllListeners(); //disable listeners to avoid multiple Invokes
        yield return _delayTimer;
        _onClick.Invoke();
    }

    private IEnumerator AnimateButton() //animate the button to simulate being pressed
    {
        if (_buttonRect == null) yield return null;
        _buttonTween = _buttonRect.DOAnchorPosY(0, _animateTime).SetUpdate(true);
        yield return _buttonTween.WaitForCompletion();
        _buttonTween = _buttonRect.DOAnchorPosY(_buttonStart, _animateTime).SetUpdate(true);
        yield return _buttonTween.WaitForCompletion();
        _thisButton.onClick.AddListener(() => StartCoroutine(DelayedClickRoutine())); //re-enable listener
        CanClick = true;
    }

    private void OnDisable() //reset position of button when de-activating object
    {
        if (_buttonRect == null) return;
        _buttonRect.DOAnchorPosY(5, 0).SetUpdate(true);
        CanClick = true;
    }

    private void OnDestroy()
    {
        DOTween.Kill(_buttonRect); //kill tween to stop errors being thrown when object is destroyed
    }
}
