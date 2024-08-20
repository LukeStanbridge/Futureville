using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TradingCardData", menuName = "TradingCardData")]
public class TradingCardLayout : ScriptableObject
{
    [Header("Front Card")]
    public CardType CardType;
    public Sprite NpcSprite;
    public string NpcJob;
    [TextArea(2, 10)]
    public string JobDescription;

    [Header("Environment")]
    public List<string> EnvironmentDescriptions;

    [Header("CardStats")]
    public List<float> CardStats;

    [Header("CardSkills")]
    public List<string> Skills;

    [Header("CardQualifications")]
    [TextArea(2,10)]
    public List<string> Qualifications;

    [Header("Benefits")]
    public int SalaryPercent;
    [TextArea(2, 10)]
    public string SalaryAdjustment;
    public string FutureDemand;
    [TextArea(2, 10)]
    public string DemandExpectation;
    public bool Commish;    
}
