using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnvironmentGridLayout : MonoBehaviour
{
    public RectTransform CardSection;
    public RectTransform SectionHeading;
    private RectTransform SectionContent;
    public GameObject SectionContentPrefab;
    public List<RectTransform> DescriptionLabels;
    public float CardScale;
    public float BorderImagePadding;
    public float XPosPadding;
    public float YPosPadding;
    public float _lineWidth;
    public float _posY = 0;
    private float _contentWidth;
    private float _contentHeight;
    private float _lineHeight;

    public void AssignDescriptionLabels(List<string> list)
    {
        SectionContent = GetComponent<RectTransform>();
        _contentWidth = GetComponent<RectTransform>().sizeDelta.x;

        foreach (string label in list)
        {
            GameObject enviroDescription = Instantiate(SectionContentPrefab, SectionContent);
            enviroDescription.GetComponent<TextMeshProUGUI>().text = label;
            DescriptionLabels.Add(enviroDescription.GetComponent<RectTransform>());
        }

        _lineHeight = DescriptionLabels[0].sizeDelta.y;
        _contentHeight = _lineHeight + _posY;

        PositionLabels();
    }

    private void PositionLabels()
    {
        foreach (RectTransform rect in DescriptionLabels)
        {
            float textWidth = rect.GetComponent<TextMeshProUGUI>().preferredWidth;
            float previousLength = _lineWidth;
            _lineWidth += (textWidth + (BorderImagePadding * 2) + XPosPadding);

            if (_lineWidth > _contentWidth) //place description label on next line if it doesn't fit inside content box
            {
                previousLength = 0;
                _lineWidth = (textWidth + (BorderImagePadding * 2) + XPosPadding);
                _posY += _lineHeight + YPosPadding;
                _contentHeight = _lineHeight + _posY;
            }

            rect.localPosition = new Vector2(rect.localPosition.x + previousLength, rect.localPosition.y - _posY);
        }

        SectionContent.sizeDelta = new Vector2(_contentWidth, _contentHeight); //adjust size of content box
        CardSection.sizeDelta = new Vector2(_contentWidth, _contentHeight + SectionHeading.sizeDelta.y); // adjust size of card information section
    }
}
