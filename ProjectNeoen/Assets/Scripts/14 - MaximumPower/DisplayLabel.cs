using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Transform _label;
    [SerializeField] private ToolManager _toolManager;

    public void SetManager(ToolManager manager) => _toolManager = manager;
    public void HideLabel() => _label.gameObject.SetActive(false);
    public void ShowLabel() => _label.gameObject.SetActive(true); 

    private void Awake()
    {
        _label.gameObject?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _label.gameObject?.SetActive(true);
        if(_toolManager.CurrentTool != this) _toolManager.HideActiveTool();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_toolManager.CurrentTool != this) _label.gameObject?.SetActive(false);
        _toolManager.ShowHiddenTool();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _label.gameObject?.SetActive(true);
        _toolManager.SetCurrentTool(this);
    }
}
