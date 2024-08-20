using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _idleArray;
    [SerializeField] private Sprite[] _specialArray;
    [SerializeField] private float _speed = .04f;
    [SerializeField] private int _counter;

    private int m_IndexSprite;
    private Coroutine m_CoroutineAnim;
    private bool IsDone;

    public void Func_PlayUIAnim(bool playSpecialAnim)
    {
        IsDone = false;
        if (playSpecialAnim) StartCoroutine(Func_PlayAnimUI());
        else StartCoroutine(PlayAnimUI());
    }

    public void Func_StopUIAnim(bool playSpecialAnim)
    {
        IsDone = true;
        if (playSpecialAnim) StopCoroutine(Func_PlayAnimUI());
        else StopCoroutine(PlayAnimUI());
    }

    IEnumerator Func_PlayAnimUI()
    {
        yield return new WaitForSeconds(_speed);

        if (_counter >= 10)
        {
            if (m_IndexSprite >= _specialArray.Length)
            {
                m_IndexSprite = 0;
                _counter = 0;
            }
            _image.sprite = _specialArray[m_IndexSprite];
            m_IndexSprite += 1;
            if (IsDone == false)
                m_CoroutineAnim = StartCoroutine(Func_PlayAnimUI());
        }
        else
        {
            if (m_IndexSprite >= _idleArray.Length)
            {
                m_IndexSprite = 0;
                _counter++;
            }
            _image.sprite = _idleArray[m_IndexSprite];
            m_IndexSprite += 1;
            if (IsDone == false)
                m_CoroutineAnim = StartCoroutine(Func_PlayAnimUI());
        }

        if (IsDone) //reset animation on deselect
        {
            _image.sprite = _idleArray[0];
            m_IndexSprite = 0;
        }
    }

    IEnumerator PlayAnimUI()
    {
        yield return new WaitForSeconds(_speed);

        if (m_IndexSprite >= _idleArray.Length) m_IndexSprite = 0;
        _image.sprite = _idleArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            m_CoroutineAnim = StartCoroutine(PlayAnimUI());

        if (IsDone) //reset animation on deselect
        {
            _image.sprite = _idleArray[0];
            m_IndexSprite = 0;
        }
    }
}
