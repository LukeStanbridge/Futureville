using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Avatar : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterSelectionManager _charSelectManager;
    [SerializeField] private Outline _avatarOutline;
    [SerializeField] private RuntimeAnimatorController _avatarController;
    [SerializeField] private RuntimeAnimatorController _avatarControllerR;
    [SerializeField] private Color _notSelected;
    [SerializeField] private Color _selected;
    [SerializeField] private UISpriteAnimation _spriteAnimation;
    [field: SerializeField] public bool Symetrical { get; private set; }

    private void Awake()
    {
        _avatarOutline.effectColor = _notSelected;
    }

    public void OnPointerClick(PointerEventData eventData) //set avatar on click
    {
        if (_charSelectManager.PlayerAvatar == this) return;
        transform.DOScale(1.1f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.DOScale(1.05f, 0.1f).SetEase(Ease.InOutSine);
        });
        _spriteAnimation.Func_PlayUIAnim(_charSelectManager.PlaySpecialAnimation);
        _avatarOutline.effectColor = _selected;
        _avatarOutline.effectDistance = new Vector2(3, 3);
        _charSelectManager.SetPlayerAvatar(this, _avatarController, _avatarControllerR);
    }

    public void DeselectAvatar() //deselect avatar
    {
        transform.DOScale(1f, 0.1f);
        _spriteAnimation.Func_StopUIAnim(_charSelectManager.PlaySpecialAnimation);
        _avatarOutline.effectColor = _notSelected;
        _avatarOutline.effectDistance = new Vector2(2, 2);
    }
}
