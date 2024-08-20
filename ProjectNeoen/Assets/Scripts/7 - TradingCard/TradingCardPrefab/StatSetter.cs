using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatSetter : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _statRect;
    [SerializeField] private GridLayoutGroup _layoutGroup;
    [SerializeField] private float _graphWidth;
    [SerializeField] private float _graphHeight;

    public void SetCardStats(List<float> stats)
    {
        _graphWidth = _layoutGroup.cellSize.x;
        _graphHeight = _statRect[0].sizeDelta.y;

        for (int i = 0; i < _statRect.Count; i++) 
        {
            _statRect[i].sizeDelta = new Vector2((stats[i] / 100) * _graphWidth, _graphHeight);
        }
    }
}
