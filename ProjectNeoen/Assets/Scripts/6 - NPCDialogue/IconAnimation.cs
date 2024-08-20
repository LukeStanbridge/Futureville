using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnimation : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private float _distance;
    [SerializeField] private float _startPosY;
    [SerializeField] private Tween _tween;

    private void Awake()
    {
        _startPosY = this.transform.localPosition.y;
    }

    public void StartTween()
    {
        _tween = transform.DOMoveY(transform.position.y + _distance, _timer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void KillTween()
    {
       _tween.Kill();
        this.transform.localPosition = new Vector3(0, _startPosY, 0);
    }
}
