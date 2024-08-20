using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BenefitSetter : MonoBehaviour
{
    public TextMeshProUGUI SalaryIncrease;
    public TextMeshProUGUI SalaryAdjustment;
    public TextMeshProUGUI FutureDemand;
    public TextMeshProUGUI DemandExpectation;
    public GameObject DollarSymbol;
    public GameObject CommishText;

    public void AssignCardBenefits(int salaryIncrease, string salaryAdjustment, string futureDemand, string demandExpectation, bool commish)
    {
        SalaryIncrease.text = salaryIncrease.ToString() + "%";
        SalaryAdjustment.text = salaryAdjustment;
        FutureDemand.text = futureDemand;
        DemandExpectation.text = demandExpectation;
        DemandExpectation.GetComponent<RectTransform>().anchoredPosition = new Vector2(FutureDemand.preferredWidth + 50, 0);

        if (commish)
        {
            DollarSymbol.SetActive(true);
            CommishText.SetActive(true);
        }
        else
        {
            DollarSymbol.SetActive(false);
            CommishText.SetActive(false);
        }
    }
}
