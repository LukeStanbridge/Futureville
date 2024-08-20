using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradingCard : MonoBehaviour, IPointerClickHandler
{
    public int HierachyPos;
    public int TradingCardID;
    public RectTransform Rect;
    public GameObject Front;
    public GameObject Back;

    public Transform OriginalParent;
    public bool RecommendedCard;

    public TradingCardLayout TradingCardData;
    public FrontCardData CardData;
    public EnvironmentGridLayout Enviro;
    public StatSetter StatsSetter;
    public SkillSetter SkillSetter;
    public SkillSetter QualificationSetter;
    public BenefitSetter BenefitSetter;

    public TradingCardManager TradingCardManager;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData) //display card on click in trading card collection
    {
        if (TradingCardManager.CardOnDisplay) return; //ignore if card is already being displayed
        TradingCardManager.CardOnDisplay = true;
        TradingCardManager.DisplayCollectedCard(this);
    }

    public void BuildCard()
    {
        CardData.SetFrontCard(TradingCardData.CardType, TradingCardData.NpcSprite, TradingCardData.NpcJob, TradingCardData.JobDescription);
        Enviro.AssignDescriptionLabels(TradingCardData.EnvironmentDescriptions);
        StatsSetter.SetCardStats(TradingCardData.CardStats);
        SkillSetter.AssignCardSkills(TradingCardData.Skills);
        QualificationSetter.AssignCardSkills(TradingCardData.Qualifications);
        BenefitSetter.AssignCardBenefits(TradingCardData.SalaryPercent, TradingCardData.SalaryAdjustment, TradingCardData.FutureDemand, TradingCardData.DemandExpectation, TradingCardData.Commish);
        Back.SetActive(false);
    }
}
