using DG.Tweening;
using UnityEngine;

public class BlowingWind : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Tween _wind;
    [SerializeField] private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.localPosition;
    }

    public void PlayWindEffect()
    {
        _wind = transform.DOLocalMove(_position, _timer)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void KillWindEffects()
    {
        _wind.Kill();
        transform.localPosition = _startPos;
    }
}
