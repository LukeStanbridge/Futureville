using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingObjects : MonoBehaviour
{
    [SerializeField] private float _angle;
    [SerializeField] private float _timer;

    void Start()
    {
        transform.DORotate(new Vector3(0, 0, _angle), _timer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
