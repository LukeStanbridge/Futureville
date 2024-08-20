using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    [SerializeField] private AudioManager _soundManager;
    [field: SerializeField] public int _pieceNumber { get; private set; }
    [SerializeField] private GameObject _dragCopy;
    [SerializeField] private InputGroup _inputGroup;
    [SerializeField] private float _offset;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _parentTransform;
    private CanvasGroup _canvasGroup;
    [SerializeField] private Vector2 _startSize;
    [SerializeField] private bool _firstDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.5f;

        if(_firstDrag) 
        {
            _startSize = _rectTransform.sizeDelta;
            _rectTransform.sizeDelta = new Vector2(_startSize.x * 0.75f, _startSize.y * 0.75f);
            _firstDrag = false;
        }

        if (_parentTransform.tag == "PickinTeams") //swaping game piece
        {
            _parentTransform.gameObject.GetComponent<UserInput>().SetNumber(0); //reset input number 
        }
        else //dragging from main board
        {
            GameObject number = Instantiate(_dragCopy, this.gameObject.transform.localPosition, this.gameObject.transform.rotation, _parentTransform);
            number.transform.SetSiblingIndex(_pieceNumber - 1);
            number.gameObject.name = "Number" + _pieceNumber.ToString();
            CanvasGroup canvasGroup = number.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        _soundManager.Play("Pickup");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1;
        
        if (_inputGroup.selectedInput == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            if (_inputGroup.selectedInput.transform.childCount > 0)
            {
                if (_parentTransform.transform.tag == "PickinTeams") //switch pieces if number already occupies space
                {
                    HandleSwapItems(eventData);
                }
                else
                {
                    Destroy(_inputGroup.selectedInput.transform.GetChild(0).gameObject); //switch pieces on newly dropped number
                }
                _soundManager.Play("Swap");
            }
            else _soundManager.Play("Putdown");

            this.transform.SetParent(_inputGroup.selectedInput.transform);
            _parentTransform = this.transform.parent;
            _rectTransform.anchoredPosition = new Vector2(_inputGroup.GetComponent<GridLayoutGroup>().cellSize.x / 2, -_inputGroup.GetComponent<GridLayoutGroup>().cellSize.y / 2);
        }
    }

    public void SetInputGroup(InputGroup inputGroup)
    {
        _inputGroup = inputGroup;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _parentTransform = this.transform.parent;
        _firstDrag = true;
    }

    private void HandleSwapItems(PointerEventData eventData)
    {
        GameObject existingPiece = _inputGroup.selectedInput.transform.GetChild(0).gameObject;
        existingPiece.transform.SetParent(_parentTransform);
        existingPiece.GetComponent<DragAndDrop>()._parentTransform = _parentTransform;
        existingPiece.GetComponent<DragAndDrop>()._parentTransform.GetComponent<UserInput>().SetNumber(_pieceNumber);
        existingPiece.GetComponent<DragAndDrop>()._parentTransform.GetComponent<UserInput>().ChnageChildOnDrop();
        _parentTransform.GetComponent<UserInput>().SetNumber(existingPiece.GetComponent<DragAndDrop>()._pieceNumber);
        existingPiece.GetComponent<RectTransform>().anchoredPosition = new Vector2(_inputGroup.GetComponent<GridLayoutGroup>().cellSize.x / 2, -_inputGroup.GetComponent<GridLayoutGroup>().cellSize.y / 2);
    }
}
