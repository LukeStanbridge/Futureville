using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentLayoutGrid : MonoBehaviour
{
    public List<TradingCard> CollectedCards;
    [SerializeField] private float _scaleSize;
    [SerializeField] private float _yPosOffset = 0;
    private float _yPosIncrement = 0;
    private int _cardCounter = 0;
    private int _lineCounter = 0;
    private int _maxCards = 5;
    private float _lineHeight = 0;

    public void SetCard(TradingCard card) //positon card in trading card content page
    {
        CollectedCards.Add(card);
        RectTransform cardRect = card.GetComponent<RectTransform>();

        _cardCounter++;
        if (_cardCounter > _maxCards)
        {
            _lineCounter++;
            _cardCounter = 1;
            _yPosIncrement += (cardRect.sizeDelta.y * _scaleSize) + _yPosOffset;
            if (_lineCounter >= 2)
            {
                _lineHeight += (-cardRect.sizeDelta.y * _scaleSize) - _yPosOffset;
                this.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                this.GetComponent<RectTransform>().offsetMin = new Vector3(0, _lineHeight);
            }
        }
        
        cardRect.anchorMin = new Vector2(0, 1);
        cardRect.anchorMax = new Vector2(0, 1);
        cardRect.anchoredPosition = new Vector3(0, 0, 0);

        float yPos = (cardRect.sizeDelta.y * _scaleSize) / 2;
        yPos += _yPosIncrement + _yPosOffset;

        float xPos = this.transform.parent.GetComponent<RectTransform>().sizeDelta.x / 6;
        xPos *= _cardCounter;

        cardRect.localScale = new Vector3(_scaleSize, _scaleSize, _scaleSize);
        cardRect.anchoredPosition = new Vector3(xPos, -yPos, 0);
    }
}
