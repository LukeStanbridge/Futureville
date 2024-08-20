using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrontCardData : MonoBehaviour
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _windowImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _npc;
    [SerializeField] private TextMeshProUGUI _jobTitle;
    [SerializeField] private TextMeshProUGUI _jobDescription;

    public void SetFrontCard(CardType cardType, Sprite npcSprite, string jobTitle, string jobDescription)
    {
        _cardImage.color = cardType.CardColour;
        _windowImage.color = cardType.NPCWindowColour;
        _iconImage.sprite = cardType.IconSprite;
        _npc.sprite = npcSprite;
        _jobTitle.text = jobTitle;
        _jobDescription.text = jobDescription;
    }
}
