using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _diceRect;
    [SerializeField] private Rigidbody2D _diceRigidbody;
    [SerializeField] private Vector2 _rollForce;
    [SerializeField] private float _forceMultiplier;
    [SerializeField] private Sprite[] _diceRollArray;
    [SerializeField] private List<DiceNumber> _diceRoll;
    [SerializeField] private float _speed = .04f;
    [SerializeField] private int _rollCount = 0;
    [SerializeField] private int _totalRolls;

    [SerializeField] private int m_IndexSprite;
    private Coroutine m_CoroutineAnim;
    public void ResetDicePos()
    {
        _diceRect.localPosition = Vector3.zero;
        _image.sprite = _diceRollArray[0];
        _diceRect.sizeDelta = new Vector2(_diceRollArray[0].rect.width, _diceRollArray[0].rect.height);
    }

    [System.Serializable]
    public struct DiceNumber
    {
        public string _number;
        public Sprite[] _dice;
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _diceRect = GetComponent<RectTransform>();
        _diceRigidbody = GetComponent<Rigidbody2D>();
    }

    public void StartDiceRoll(int diceResult)
    {
        TossUp();
        StartCoroutine(RollDice(diceResult));
        StartCoroutine(DiceAudio());
    }

    private IEnumerator DiceAudio()
    {
        yield return new WaitForSeconds(1.15f);
        _soundManager.Play("DiceRoll");
    }

    private IEnumerator RollDice(int diceResult)
    {
        yield return new WaitForSeconds(_speed);

        if (m_IndexSprite >= _diceRollArray.Length)
        {
            m_IndexSprite = 0;
            _rollCount++;

            if (_rollCount >= _totalRolls)
            {
                StartCoroutine(FinalLandingRoll(_diceRoll[diceResult]._dice));
                _rollCount = 0;
                yield break;
            }
        }
        //_diceRect.sizeDelta = new Vector2(_diceRollArray[m_IndexSprite].rect.width, _diceRollArray[m_IndexSprite].rect.height);
        _image.sprite = _diceRollArray[m_IndexSprite];
        m_IndexSprite += 1;
        m_CoroutineAnim = StartCoroutine(RollDice(diceResult));
    }

    private IEnumerator FinalLandingRoll(Sprite[] finalDiceRoll)
    {
        yield return new WaitForSeconds(_speed);

        if (m_IndexSprite >= finalDiceRoll.Length)
        {
            m_IndexSprite = 0;
            FinishRolling();
            yield break;    
        }
        //_diceRect.sizeDelta = new Vector2(_diceRollArray[m_IndexSprite].rect.width, _diceRollArray[m_IndexSprite].rect.height);
        _image.sprite = finalDiceRoll[m_IndexSprite];
        m_IndexSprite += 1;
        m_CoroutineAnim = StartCoroutine(FinalLandingRoll(finalDiceRoll));
    }

    private void TossUp()
    {
        _diceRigidbody.isKinematic = false;
        _diceRigidbody.AddForce(_rollForce * _forceMultiplier);
    }

    private void FinishRolling()
    {
        _diceRigidbody.isKinematic = true;
        _diceRigidbody.velocity = Vector2.zero;
    }
}
