using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnergyDrinkPowerUp : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CommunityChaosManager _communityChaosManager;
    [SerializeField] private Image _energyDrinkFill;
    [SerializeField] private float _drinkTimer;
    public bool _drinkEnergy;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) //apply energy drink effects
    {
        if (_drinkEnergy) return;

        _communityChaosManager.EnergyDrinkPowerUp();
        StartCoroutine(EnergyDrink());
        _drinkEnergy = true;
    }

    private IEnumerator EnergyDrink() //deplete energy drink image over time
    {
        float timeLeft = 10;
        _energyDrinkFill.fillAmount = timeLeft / _drinkTimer;

        while (timeLeft >= 0 && !_communityChaosManager.PlayerWins)
        {
            if (_communityChaosManager._miniGameManager.ResetGame) break;
            float fillAmount = timeLeft / _drinkTimer;
            _energyDrinkFill.fillAmount = fillAmount;
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
    }

    public void ResetEnergyDrink() //reset energy drink to full
    {
        _drinkEnergy = false;
        _energyDrinkFill.fillAmount = 1;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
    }
}
