using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowAnimation : MonoBehaviour
{
    [Header("Tail Variables")]
    [SerializeField] private Transform _cowTail;
    [SerializeField] private float _tailAngle;
    [SerializeField] private float _tailTimer;

    [Header("Ear Variables")]
    [SerializeField] private Transform _cowEarL;
    [SerializeField] private Transform _cowEarR;
    [SerializeField] private float _earAngle;
    [SerializeField] private float _earTimerHigh;
    [SerializeField] private float _earTimerLow;
    [SerializeField] private float _beginTime;

    [Header("Head Variables")]
    [SerializeField] private Transform _cowHead;
    [SerializeField] private float _headDistance;
    [SerializeField] private float _headTimer;

    void Start()
    {
        CowTail();
        InvokeRepeating("Ears", 2, _beginTime);
        HeadMovement();
    }

    private void CowTail() //contiuous swing
    {
        float angle = (this.transform.rotation.y == 0) ? 0 : 180;
        _cowTail.transform.DORotate(new Vector3(0, angle, _tailAngle), _tailTimer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    
    private void Ears() //random wiggle
    {
        float randomL = Random.Range(_earTimerLow, _earTimerHigh);
        float randomR = Random.Range(_earTimerLow, _earTimerHigh);

        float angle = (this.transform.rotation.y == 0) ? 0 : 180;

        _cowEarL.transform.DORotate(new Vector3(0, angle, -_earAngle), randomL).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        _cowEarR.transform.DORotate(new Vector3(0, angle, _earAngle), randomR).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
    }

    private void HeadMovement() //slow bobbing
    {
        _cowHead.transform.DOMoveY(_cowHead.transform.position.y + _headDistance, _headTimer).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
