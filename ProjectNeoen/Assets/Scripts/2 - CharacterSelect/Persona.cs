using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Persona : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterSelectionManager _charSelectManager;
    [SerializeField] private Outline _personaOutline;
    [SerializeField] private Color _notSelected;
    [SerializeField] private Color _selected;

    public PersonaType PlayerPersona;
    public enum PersonaType
    {
        Realistic,
        Investigative,
        Artistic,
        Social,
        Enterprising,
        Conventional
    }

    private void Awake()
    {
        _personaOutline.effectColor = _notSelected;
    }

    public void OnPointerClick(PointerEventData eventData) //set persona on click
    {
        if (_charSelectManager.PlayerPersona == this) return;
        transform.DOScale(1.1f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.DOScale(1.05f, 0.1f).SetEase(Ease.InOutSine);
        });
        _personaOutline.effectColor = _selected;
        _personaOutline.effectDistance = new Vector2(3, 3);
        _charSelectManager.SetPlayerPersona(this);
    }

    public void DeselectPersona() //deselect persona
    {
        transform.DOScale(1f, 0.1f);
        _personaOutline.effectColor = _notSelected;
        _personaOutline.effectDistance = new Vector2(0, 0);
    }
}
