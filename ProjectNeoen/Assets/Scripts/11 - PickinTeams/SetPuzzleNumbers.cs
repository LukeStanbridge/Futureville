using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetPuzzleNumbers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _topNumberText;
    [SerializeField] private int _topNumber;
    [SerializeField] private TextMeshProUGUI _bottomNumberText;
    [SerializeField] private int _bottomNumber;

    private void Awake()
    {
        SetNumbers();
    }

    private void SetNumbers()
    {
        _topNumberText.text = (_topNumber == 0) ? _topNumberText.text = "" : _topNumberText.text = _topNumber.ToString();
        _bottomNumberText.text = (_bottomNumber == 0) ? _bottomNumberText.text = "" : _bottomNumberText.text = _bottomNumber.ToString();
    }
}
