using DG.Tweening;
using UnityEngine;

public class FlyingBirds : MonoBehaviour
{
    [SerializeField] private float _flyOverTimer;
    [SerializeField] private float _flyingTime;
    [SerializeField] private float _offset;
    [SerializeField] private Transform _playerPos;
    [SerializeField] private Vector3 _startPos;
    
    private void Start()
    {
        _startPos = transform.position;
        InvokeRepeating("BirdFlyOver", 0, _flyOverTimer);
    }

    private void BirdFlyOver()
    {
        transform.position = new Vector3(transform.position.x, _playerPos.position.y + _offset, transform.position.z);
        transform.DOMoveX(_startPos.x * -1, _flyingTime).OnComplete(() =>
        {
            transform.position = _startPos;
        });
    }
}
