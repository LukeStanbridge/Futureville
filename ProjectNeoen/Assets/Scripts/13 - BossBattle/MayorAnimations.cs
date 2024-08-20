using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MayorAnimations : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _neutral;
    [SerializeField] private Sprite[] _happy;
    [SerializeField] private Sprite[] _upset;
    [SerializeField] private float _speed = .04f;
    private int _indexSprite;
    private Coroutine _mayorAnim;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = _neutral[0];
    }

    private void ResetCoroutine()
    {
        if (_mayorAnim == null) return;
        StopCoroutine(_mayorAnim);
        _indexSprite = 0;
    }

    public void MayorNeutral()
    {
        ResetCoroutine();
        _mayorAnim = StartCoroutine(PlayAnim(_neutral));
    }

    public void MayorHappy()
    {
        ResetCoroutine();
        _mayorAnim = StartCoroutine(PlayAnim(_happy));
    }

    public void MayorUpset()
    {
        ResetCoroutine();
        _mayorAnim = StartCoroutine(PlayAnim(_upset));
    }

    private IEnumerator PlayAnim(Sprite[] animation)
    {
        yield return new WaitForSeconds(_speed);

        if (_indexSprite >= animation.Length) _indexSprite = 0;
        _image.sprite = animation[_indexSprite];
        _indexSprite += 1;
        _mayorAnim = StartCoroutine(PlayAnim(animation));
    }
}
