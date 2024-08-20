using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardID
{
    public int TradingCardID;
    public bool Collected;
}

public class CollectedCards : MonoBehaviour
{
    [SerializeField] private List<MiniGameNPC> _npcs;
    [SerializeField] private List<CardID> _cards = new List<CardID>();

    private void Start()
    {
        foreach (MiniGameNPC npc in _npcs)
        {
            CardID card = new CardID();
            card.TradingCardID = Convert.ToInt32(npc.TradingCardID);
            card.Collected = false;
            _cards.Add(card);
        }
    }

    public void SetCardCollected(int cardID)
    {
        foreach(CardID card in _cards)
        {
            if (card.TradingCardID == cardID)
            {
                card.Collected = true;
                return;
            }
        }
    }

    public bool CheckCardCollected(int cardID)
    {
        foreach (CardID card in _cards)
        {
            if (card.TradingCardID == cardID)
            {
                if (card.Collected) return true;
            }
        }
        return false;
    }
}