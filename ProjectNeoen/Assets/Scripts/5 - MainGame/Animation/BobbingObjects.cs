using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingObjects : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private float _distance;
    private void Start()
    {
        transform.DOMoveY(transform.position.y + _distance, _timer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
