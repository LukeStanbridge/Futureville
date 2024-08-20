using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UserInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public InputGroup inputGroup;
    public Outline background;
    [SerializeField] private int _correctNumber;
    [field: SerializeField] public int setNumber { get; private set; }
    [SerializeField] private GameObject _childGameObject;
    [SerializeField] private float _animateTime;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inputGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inputGroup.OnTabExit(this);
    }

    private void Awake()
    {
        background = GetComponent<Outline>();
        inputGroup = GameObject.FindGameObjectWithTag("KakuroPuzzle").GetComponent<InputGroup>();
        inputGroup.Subscribe(this);
    }

    public void Update()
    {
        //ForceCorrectNumberPosition();
    }

    //private void ForceCorrectNumberPosition()//removes bug for when quick dragging/releasing the number doesn't snap to center of gameobject
    //{
    //    if (this.gameObject.transform.childCount > 0) 
    //    {
    //        if (_childGameObject == null)
    //        {
    //            _childGameObject = this.gameObject.transform.GetChild(0).gameObject;
    //            if (setNumber == 0) setNumber = _childGameObject.GetComponent<DragAndDrop>()._pieceNumber;

    //            foreach (UserInput input in inputGroup.userInputs) //remove stray set numbers from quick drag/release
    //            {
    //                if (input.gameObject.transform.childCount == 0) 
    //                {
    //                    input.setNumber = 0;
    //                }
    //            }
    //        }

    //        float xPos = GetComponent<RectTransform>().sizeDelta.x / 2;
    //        float yPos = GetComponent<RectTransform>().sizeDelta.y / 2;
    //        if (_childGameObject.GetComponent<RectTransform>().anchoredPosition.x != xPos || _childGameObject.GetComponent<RectTransform>().anchoredPosition.y != -yPos)
    //        {
    //            _childGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, -yPos);
    //        }
    //    }
    //}

    public int CorrectNumber()
    {
        return _correctNumber;
    }

    public void SetNumber(int userNumber)
    {
        setNumber = userNumber;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            setNumber = eventData.pointerDrag.GetComponent<DragAndDrop>()._pieceNumber;
            _childGameObject = eventData.pointerDrag.gameObject;
        }
    }

    public void ChnageChildOnDrop()
    {
        _childGameObject = transform.GetChild(0).gameObject;
    }

    public void CorrectAnimation()
    {
        _childGameObject.transform.DORotate(new Vector3(0, 0, 360), _animateTime, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear);
        _childGameObject.transform.DOScale(Vector3.one * 1.3f, 0.2f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _childGameObject.transform.DOScale(Vector3.one * 1, 0.2f).SetEase(Ease.InOutSine);
        });
        _childGameObject.GetComponent<Image>().DOColor(Color.green, 0.5f).OnComplete(() =>
        {
            _childGameObject.GetComponent<Image>().DOColor(Color.white, 0.2f);
        });
    }

    public void IncorrectAnimation()
    {
        _childGameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).OnComplete(() =>
        {
            _childGameObject.GetComponent<Image>().DOColor(Color.white, 0.2f);
        });
        _childGameObject.transform.DOShakeScale(0.7f, 0.3f, 20, 360, true, ShakeRandomnessMode.Full).OnComplete(() =>
        {
            _childGameObject.transform.localScale = Vector3.one;
        });
    }
}
