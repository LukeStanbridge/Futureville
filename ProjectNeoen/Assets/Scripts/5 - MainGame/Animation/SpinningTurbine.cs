using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTurbine : MonoBehaviour
{
    [SerializeField] private float _time;

    private void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), _time, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetRelative()
            .SetEase(Ease.Linear);
    }
}
