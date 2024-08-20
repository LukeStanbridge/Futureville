using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteRings : MonoBehaviour
{
    [SerializeField] private GameObject[] _rings;
    [SerializeField] private float _timer;
    [SerializeField] private float _scaleSpeed;
    [SerializeField] private int _ringCounter;
    [SerializeField] private bool _ringDisplay;
    [SerializeField] private float _ringAlpha;
    private Coroutine _ringCoroutine;

    private void Awake()
    {
        _ringCounter = 0;
        _ringDisplay = true;
        _ringAlpha = 0;
        //StartCoroutine(DisplayRings(_rings[0], _ringDisplay, _ringAlpha));
    }

    IEnumerator DisplayRings(GameObject ring, bool active, float alpha)
    {
        ring.SetActive(active);
        ring.GetComponent<SpriteRenderer>().DOFade(alpha, _scaleSpeed);
        yield return new WaitForSeconds(_timer);
        _ringCounter++;

        if (_ringCounter >= _rings.Length)
        {
            _ringCounter = 0;
            if (_ringDisplay)
            {
                _ringDisplay = false;
                _ringAlpha = 0;
            }
            else
            {
                _ringDisplay = true;
                _ringAlpha = 1;
            }
        }

        _ringCoroutine = StartCoroutine(DisplayRings(_rings[_ringCounter], _ringDisplay, _ringAlpha));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.State != GameState.Futureville) return;
        if (collision.gameObject.tag == "Player")
        {
            _ringCoroutine = StartCoroutine(DisplayRings(_rings[0], _ringDisplay, _ringAlpha));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && _ringCoroutine != null)
        {
            StopCoroutine(_ringCoroutine);
        }
    }
}
