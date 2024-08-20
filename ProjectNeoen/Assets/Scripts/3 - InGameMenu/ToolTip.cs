using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Transform _label;
    [SerializeField] private bool _keepOpen;

    private void Awake()
    {
        _label.gameObject?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _label.gameObject?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _label.gameObject?.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_keepOpen) _label.gameObject?.SetActive(false);
    }

    private void OnDisable()
    {
        _label.gameObject?.SetActive(false);
    }
}
