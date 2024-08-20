using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmClockRinging : MonoBehaviour
{
    [SerializeField] private Transform _clock;
    [SerializeField] private Tween _clockTween;
    [SerializeField] private float _angle;
    [SerializeField] private float _timer;
    [SerializeField] private int _loops;
    [field: SerializeField] public bool FinishedRinging { get; private set; }
    public void ResetAlarm() => FinishedRinging = false;

    public void TriggerClockRinging() //play clock ringing and then end game
    {
        //trigger alarm audio
        _clock.transform.localRotation = Quaternion.Euler(0, 0, -_angle);
        _clockTween =_clock.DORotate(new Vector3(0, 0, _angle), _timer).SetEase(Ease.InOutSine).SetLoops(_loops, LoopType.Yoyo).OnComplete(() =>
        {
            ResetClockRotation();
            //this.gameObject.GetComponent<CommunityChaosManager>().EndGame();
            FinishedRinging = true;
        });
    }

    public void ResetClockRotation()
    {
        _clockTween.Kill();
        _clock.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
