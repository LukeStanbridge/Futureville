using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingCardManager : MonoBehaviour
{
    [Header("Display NPC card references")]
    [SerializeField] private NPCDialogueManager _npcDialogueManager;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private GameObject _NPCTradingCard;
    [SerializeField] private ActivateDialogue _currentNPC;
    [SerializeField] private Sprite _cardBackground;
    [SerializeField] private GameObject _front;
    [SerializeField] private GameObject _back;

    [Header("Display New Trading Card")]
    [SerializeField] private Transform _tradingCardBackground;
    [SerializeField] private GameObject _newTradingCardPrefab;
    [SerializeField] private Vector2 _tradingCardOffset;
    [SerializeField] private Button _closeButon;
    [SerializeField] private Button _flipButon;
    [SerializeField] private Button _addButon;
    [SerializeField] private float _cardScale;

    [Header("Trading card collection references")]
    [SerializeField] private CollectedCards _collectedCards;
    [SerializeField] private List<TradingCard> _tradingCards;
    [SerializeField] private GameObject _displayedCardPrefab;
    [SerializeField] private GameObject _cardShown;
    [SerializeField] private TradingCard _displayedCard;
    [SerializeField] private GameObject _tradingCardPrefab;
    [SerializeField] private Transform _cardContentTransform;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _placeholderCard;
    [SerializeField] private GameObject _placeholderCardPrefab;

    //new displaying collected card variables
    [SerializeField] private Transform _displayBackground;
    [SerializeField] private float _scaleSize;
    [SerializeField] private Vector2 _posOffset;
    [SerializeField] private Button _overlayCloseButon;
    [SerializeField] private Button _overlayFlipButon;
    [SerializeField] private Button _overlayAddButon;
    private Vector2 _originalPosition;
    private Vector2 _anchorMin;
    private Vector2 _anchorMax;
    private Vector3 _originalCardScale;
    private Transform _originalParent;

    [Header("Recommended cards collection references")]
    [SerializeField] private List<TradingCard> _recTradingCards;
    [SerializeField] private TradingCard _recDisplayedCard;
    [SerializeField] private Transform _recCardContentTransform;
    [SerializeField] private ScrollRect _recScrollRect;
    [SerializeField] private GameObject _recPlaceholderCard;

    [field: SerializeField] public List<MiniGameNPC> WorkingWithHands { get; private set; }
    [field: SerializeField] public List<MiniGameNPC> SolvingProblems { get; private set; }
    [field: SerializeField] public List<MiniGameNPC> BeingCreative { get; private set; }
    [field: SerializeField] public List<MiniGameNPC> WorkingWithPeople { get; private set; }
    [field: SerializeField] public List<MiniGameNPC> LeadingAndGuiding { get; private set; }
    [field: SerializeField] public List<MiniGameNPC> DataAndDetails{ get; private set; }

    public bool CardOnDisplay;
    [SerializeField] private bool _flipAllowed;
    [SerializeField] private bool _facedUp;

    public void ResetScrollRect()
    {
        _scrollRect.verticalNormalizedPosition = 1;
        _recScrollRect.verticalNormalizedPosition = 1;
    }

    private void Awake()
    {
        _tradingCardBackground.gameObject.SetActive(false);
        _displayBackground.gameObject.SetActive(false);
        _flipAllowed = true;
        CardOnDisplay = false;
        _collectedCards = GetComponent<CollectedCards>();
    }

    public void LoadCardSet(List<MiniGameNPC> recommended) //load correct set of recommended cards to be displayed
    {
        for (int i = 0; i < recommended.Count; i++)
        {
            int cardID = Convert.ToInt32(recommended[i].TradingCardID);
            GameObject npcCard = Instantiate(_newTradingCardPrefab);
            npcCard.name = recommended[i].Name + " Card";
            TradingCard card = npcCard.GetComponent<TradingCard>();
            card.TradingCardData = recommended[i].TradingCardData;
            card.BuildCard();
            AddPreBuiltCard(npcCard.GetComponent<TradingCard>(), cardID, _recCardContentTransform, _recTradingCards);
        }
        ResetScrollRect();
    }

    public void SetNPCTradingCard(ActivateDialogue npc) //set current card being displayed by a trading card NPC
    {
        _currentNPC = npc;
        _facedUp = false;
        CardOnDisplay = true;

        DisplayNewTradingCard();
    }

    private void DisplayNewTradingCard() //display card built from card data
    {
        MiniGameNPC currentNPC = _currentNPC.GetComponent<MiniGameNPC>();
        GameObject npcCard = Instantiate(_newTradingCardPrefab, _tradingCardBackground);
        npcCard.transform.SetAsFirstSibling();
        
        npcCard.name = currentNPC.Name + " Card";
        _displayedCard = npcCard.GetComponent<TradingCard>();
        _displayedCard.TradingCardData = currentNPC.TradingCardData;
        _displayedCard.TradingCardManager = this;
        _displayedCard.BuildCard();
        npcCard.transform.localScale = new Vector3(_cardScale, _cardScale, _cardScale);
        npcCard.transform.localPosition = _tradingCardOffset;
    }

    public void AddCard() //button add card to trading card collection
    {
        if (!_flipAllowed) return;

        _tradingCardBackground.gameObject.SetActive(false);
        MiniGameNPC npc = _currentNPC.GetComponent<MiniGameNPC>();

        // check if card is in recommended list and disable the "add card" button
        foreach (TradingCard card in _recTradingCards)
        {
            if (card.TradingCardID == Convert.ToInt32(npc.TradingCardID))
            {
                card.RecommendedCard = false;
            }
        }

        AddPreBuiltCard(_displayedCard, Convert.ToInt32(npc.TradingCardID), _cardContentTransform, _tradingCards); //add new card to trading card page
        npc.ShowTradingCard = false; //disbale npc from showing card to player once it has already been collected
        _gameUIManager.AppendString(npc.TradingCardID);
        _collectedCards.SetCardCollected(Convert.ToInt32(npc.TradingCardID));
        CardOnDisplay = false;
        _currentNPC.DialogueManager.TriggerDialogue();
    }

    public void AddRecommendedCard()
    {
        if (!_flipAllowed) return;
        _displayedCard.RecommendedCard = false;
        GameObject collectedCard = Instantiate(_displayedCard.gameObject);
        AddPreBuiltCard(collectedCard.GetComponent<TradingCard>(), _displayedCard.TradingCardID, _cardContentTransform, _tradingCards);
        _gameUIManager.AppendString(_displayedCard.TradingCardID.ToString());
        _displayedCard.RecommendedCard = false;
        DisableAddCardButton();
    }

    private void DisableAddCardButton()
    {
        TradingCard card = _displayedCard.GetComponentInChildren<TradingCard>();
        _collectedCards.SetCardCollected(card.TradingCardID);
        _overlayAddButon.transform.parent.gameObject.SetActive(false);
    }

    public void FlipCard() //button to flip card over
    {
        StartCoroutine(RotateCard(_displayedCard));
    }

    public void AddPreBuiltCard(TradingCard card, int TradingCardID, Transform content, List<TradingCard> cardList) //add new card to trading card collection
    {
        cardList.Add(card);

        //Set important details for collected trading card
        card.HierachyPos = (cardList.Count > 0) ? cardList.Count - 1 : 0;
        card.transform.SetParent(content, false);
        card.transform.SetSiblingIndex(card.HierachyPos);
        card.TradingCardManager = this;
        card.OriginalParent = content;
        card.TradingCardID = TradingCardID;
        card.Front.SetActive(true);
        card.Back.SetActive(false);
        //_facedUp = false;

        if (cardList == _recTradingCards)
        {
            card.RecommendedCard = true;
        }

        content.GetComponent<ContentLayoutGrid>().SetCard(card);
        card.gameObject.SetActive(true);
    }

    public void DisplayCollectedCard(TradingCard card)
    {
        _facedUp = false;
        _displayedCard = card;
        _displayBackground.gameObject.SetActive(true);
   
        if (card.RecommendedCard) _overlayAddButon.transform.parent.gameObject.SetActive(true);
        else _overlayAddButon.transform.parent.gameObject.SetActive(false);

        RectTransform cardRect = card.GetComponent<RectTransform>();
        _originalParent = cardRect.parent;
        card.transform.SetParent(_displayBackground, false);
        card.transform.SetAsFirstSibling();
        StoreDisplayedCardRect(cardRect);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = _posOffset;
        cardRect.localScale = new Vector3(_scaleSize, _scaleSize, _scaleSize);
    }

    private void StoreDisplayedCardRect(RectTransform cardRect)
    {
        _originalPosition = cardRect.anchoredPosition;
        _anchorMin = cardRect.anchorMin;
        _anchorMax = cardRect.anchorMax;
        _originalCardScale = cardRect.localScale;
    }

    public void FlipCardShown()
    {
        StartCoroutine(RotateCard(_cardShown.GetComponentInChildren<TradingCard>()));
    }

    public IEnumerator RotateCard(TradingCard card) //flip the card on y axis and display opposite side
    {
        if (!_flipAllowed) yield return null;
        _flipAllowed = false;

        //OPTIMIZE THIS
        _flipButon.transform.parent.gameObject.SetActive(false);
        _closeButon.transform.parent.gameObject.SetActive(false);
        _addButon.transform.parent.gameObject.SetActive(false);
        _overlayFlipButon.transform.parent.gameObject.SetActive(false);
        _overlayCloseButon.transform.parent.gameObject.SetActive(false);
        if (_displayedCard.RecommendedCard) _overlayAddButon.transform.parent.gameObject.SetActive(false);

        for (float i = 0; i <= 90; i += 10f)
        {
            card.Rect.rotation = Quaternion.Euler(0f, i, 0f);
            if (i == 90f)
            {
                card.Front.SetActive(_facedUp);
                card.Back.SetActive(!_facedUp);
            }
            yield return new WaitForSecondsRealtime(0.02f);
        }

        for (float i = 0; i >= 0; i -= 10f)
        {
            card.Rect.rotation = Quaternion.Euler(0f, i, 0f);
            yield return new WaitForSecondsRealtime(0.02f);
        }

        //OPTIMIZE THIS
        _flipButon.transform.parent.gameObject.SetActive(true);
        _closeButon.transform.parent.gameObject.SetActive(true);
        _addButon.transform.parent.gameObject.SetActive(true);
        _overlayFlipButon.transform.parent.gameObject.SetActive(true);
        _overlayCloseButon.transform.parent.gameObject.SetActive(true);
        if (_displayedCard.RecommendedCard) _overlayAddButon.transform.parent.gameObject.SetActive(true);

        _flipAllowed = true;
        _facedUp = !_facedUp;
    }

    public void ResetDisplayedCard()
    {
        if (!_flipAllowed || !CardOnDisplay) return;
        _displayedCard.transform.SetParent(_originalParent, false); //re-parent to collected card panel

        //reset position on collected card panel
        RectTransform cardRect = _displayedCard.gameObject.GetComponent<RectTransform>(); 
        cardRect.anchorMin = _anchorMin;
        cardRect.anchorMax = _anchorMax;
        cardRect.anchoredPosition = _originalPosition;
        cardRect.localScale = _originalCardScale;

        //reset front to be displayed
        _displayedCard.Front.SetActive(true);
        _displayedCard.Back.SetActive(false);
        _facedUp = false;

        //disable background and card clickability
        _displayBackground.gameObject.SetActive(false);
        CardOnDisplay = false;
    }

    public void CloseNPCCard() //button to close card when being displayed by NPC
    {
        if (!_flipAllowed) return;
        if (_displayedCard != null) DestroyImmediate(_displayedCard.gameObject);
        _displayedCard = null;
        _tradingCardBackground.gameObject.SetActive(false);
        CardOnDisplay = false;
        _currentNPC.DialogueManager.TriggerDialogue();
    }

    public bool CancelIfFlipping() //check if card is flipping
    {
        if (_displayedCard == null) return true;
        return _flipAllowed;
    }

    public bool CancelIfTweening() //check if card is tweening
    {
        int totalActive = DOTween.TotalActiveTweens();
        if (totalActive == 0) return true;
        else return false;
    }
}
