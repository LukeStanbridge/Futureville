using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _imageArray;
    [SerializeField] private float _speed = .083f;
    private int m_IndexSprite;

    private void Start()
    {
        _image.sprite = _imageArray[0];
        StartCoroutine(PlayAnimUI());
    }

    IEnumerator PlayAnimUI() //Loop images to display animation on UI Canvas 
    {
        yield return new WaitForSeconds(_speed);

        if (m_IndexSprite >= _imageArray.Length) m_IndexSprite = 0;
        _image.sprite = _imageArray[m_IndexSprite];
        m_IndexSprite += 1;
        StartCoroutine(PlayAnimUI());
    }
}
