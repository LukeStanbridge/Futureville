using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WindowAnimation : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _shimmer;
    [SerializeField] private Sprite[] _openWindow;
    [SerializeField] private float _speed = .04f;
    [SerializeField] private float _startTime;
    [SerializeField] private float _repeatTime;
    private int _indexSprite;

    public void ResetWindow()
    {
        _image.sprite = _shimmer[0];
        InvokeRepeating("WindowShimmer", _startTime, _repeatTime);
    }

    public void WindowShimmer()
    {
        StartCoroutine(PlayAnim(_shimmer));
    }

    public void OpenWindow()
    {
        CancelInvoke();
        StartCoroutine(PlayAnim(_openWindow));
    }

    private IEnumerator PlayAnim(Sprite[] animation)
    {
        _indexSprite = 0;
        while (_indexSprite < animation.Length)
        {
            _image.sprite = animation[_indexSprite];
            _indexSprite += 1;
            yield return new WaitForSeconds(_speed);
        }
    }
}
