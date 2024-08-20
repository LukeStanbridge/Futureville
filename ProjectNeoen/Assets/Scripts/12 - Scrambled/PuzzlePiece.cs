using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ScrambledManager _manager;
    private RectTransform _rectTransform;

    public void SetManager(ScrambledManager manager) { _manager = manager; }

    private void Awake()
    {
        _rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _manager.TrySwap(_rectTransform);
    }
}
