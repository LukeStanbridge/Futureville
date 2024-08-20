using UnityEngine;
using UnityEngine.EventSystems;

public class WindowPowerUp : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CommunityChaosManager _communityChaosManager;
    [SerializeField] private Sprite _window;
    public bool _openWindow;
    private WindowAnimation _windowAnimation;

    private void Awake()
    {
        _windowAnimation = GetComponent<WindowAnimation>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_openWindow) return;

        _communityChaosManager.WindowPowerUp();
        _windowAnimation.OpenWindow();
        _openWindow = true;
    }

    public void ResetWindow()
    {
        _openWindow = false;
        _windowAnimation.ResetWindow();
    }
}
