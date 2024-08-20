using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class GameUIManager : MonoBehaviour
{
    [Header("Game UI References")]
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject _tradingCards;
    [SerializeField] private GameObject _tradingCardsScroll;
    [SerializeField] private GameObject _recommendedCards;
    [SerializeField] private GameObject _endGameOptions;
    [SerializeField] private GameObject _gameObjective;
    [SerializeField] private GameObject _settingScreen;
    [SerializeField] private TradingCardManager _tradingCardManager;
    [SerializeField] private NPCDialogueManager _npcDialogueManager;
    [SerializeField] private AudioManager _audioManager;

    [Header("Settings Screen")]
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private TextMeshProUGUI _soundText;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    [SerializeField] private Sprite _backgroundSoundOn;
    [SerializeField] private Sprite _backgroundSoundOff;
    [SerializeField] private Image _soundImage;
    [SerializeField] private Image _backgroundSoundImage;
    [SerializeField] private GameObject _restartMenu;
    [SerializeField] private bool _soundOn = true;
    [SerializeField] private bool _showRecommend;
    [SerializeField] private bool _endGameOption;

    [Header("End Game Options")]
    [SerializeField] private Transform _soundButton;
    [SerializeField] private Transform _continueButton;
    [SerializeField] private Transform _continueText;

    [Header("Trading Cards")]
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private GameObject _nextButton;
    [TextArea(3, 10)]
    [SerializeField] private string _tradingCardURL;
    public void SetShowRecommend(bool recommend) => _showRecommend = recommend;

    private void Start()
    {
        _recommendedCards?.SetActive(false);
        _endGameOptions?.SetActive(false);
    }

    public void ShowGameObjective() //display objective screen
    {
        GameManager.Instance.UpdateGameState(GameState.InGameMenus);
        _overlay.gameObject.SetActive(true);
        _gameObjective.SetActive(true);
        _tradingCards.SetActive(false);
        _settingScreen.SetActive(false);
    }

    public void ShowTradingCards() //display trading card menu
    {
        GameManager.Instance.UpdateGameState(GameState.InGameMenus);
        _overlay.gameObject?.SetActive(true);
        _tradingCards?.SetActive(true);
        _tradingCardsScroll?.SetActive(true);
        _recommendedCards?.SetActive(false);
        _gameObjective?.SetActive(false);
        _settingScreen?.SetActive(false);
        _tradingCardManager.ResetScrollRect();

        if (_showRecommend)
        {
            _returnButton?.SetActive(false);
            _nextButton?.SetActive(true);
        }
        else
        {
            _returnButton?.SetActive(true);
            _nextButton?.SetActive(false);
        }
    }

    public void ShowRecommendedCards() //next button to show recommended cards
    {
        if (!_tradingCardManager.CancelIfFlipping()) return; //cancel if card is flipping
        _tradingCardManager.ResetDisplayedCard();
        _tradingCardsScroll?.SetActive(false);
        _recommendedCards?.SetActive(true);
        _tradingCardManager.ResetScrollRect();
        _endGameOption = true;
        _showRecommend = false;
    }

    public void ShowSettingsScreen()
    {
        GameManager.Instance.UpdateGameState(GameState.InGameMenus);
        DisplaySettingsPanelOptions();
        _overlay.gameObject?.SetActive(true);
        _settingScreen?.SetActive(true);
        _restartMenu?.SetActive(false);
        _optionsMenu?.SetActive(true);
        _tradingCards?.SetActive(false);
        _recommendedCards?.SetActive(false);
        _gameObjective?.SetActive(false);
        if(_endGameOption) _endGameOption = false;
    }

    private void DisplaySettingsPanelOptions()
    {
        if(_endGameOption)
        {
            if (!_tradingCardManager.CancelIfFlipping()) return; //cancel if card is flipping
            _tradingCardManager.ResetDisplayedCard(); //close card if it's still being displayed
            _soundText.gameObject?.SetActive(false);
            _soundButton.gameObject?.SetActive(false);
            _continueButton.gameObject?.SetActive(true);
            _continueText.gameObject?.SetActive(true);
        }
        else
        {
            _soundText.gameObject?.SetActive(true);
            _soundButton.gameObject?.SetActive(true);
            _continueButton.gameObject?.SetActive(false);
            _continueText.gameObject?.SetActive(false);
        }
    }

    public void Return() //button to return to the game from the game menu
    {
        if (!_tradingCardManager.CancelIfFlipping()) return; //cancel if card is flipping

        _tradingCardManager.ResetDisplayedCard(); //close card if it's still being displayed
        _overlay.gameObject.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.Futureville);
    }

    public void CloseSettings()
    {
        _overlay.gameObject.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.Futureville);
    }

    public void CloseEndGameOptions()
    {
        _endGameOptions?.SetActive(false);
        _overlay.gameObject.SetActive(false);
        _npcDialogueManager.EndDialogue();
        GameManager.Instance.UpdateGameState(GameState.Futureville);
    }

    public void SoundButton()
    {
        // able/disable background sound
        _soundOn = !_soundOn;
        if (_soundOn == false) 
        {
            _soundText.text = "Sound Off";
            _soundText.color = new Color(0, 0, 0, 0.5f);
            _soundImage.sprite = _soundOffSprite;
            _backgroundSoundImage.sprite = _backgroundSoundOff;
            _audioManager.MuteAudio(true);
        }
        else
        {
            _soundText.text = "Sound On";
            _soundText.color = new Color(0, 0, 0, 1f);
            _soundImage.sprite = _soundOnSprite;
            _backgroundSoundImage.sprite = _backgroundSoundOn;
            _audioManager.MuteAudio(false);
        }
    }

    public void RestartButton()
    {
        _restartMenu.SetActive(true);
        _optionsMenu.SetActive(false);
    }

    public void Restart() //button to restart the game
    {
        DOTween.KillAll();
        GameManager.Instance = null;
        Destroy(GameManager.Instance);
        SceneManager.LoadScene("MainGame");
    }

    public void ExitGameButton() //button to quit then game
    {
        Application.Quit();
    }
    
    public void DoNotRestartButtton()
    {
        _restartMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void Exit() //button to exit out of the game and take player to the trading cards in learning hub
    {
        if (!_tradingCardManager.CancelIfFlipping()) return;
        Application.OpenURL(_tradingCardURL);
    }

    public void AppendString(string cardID)
    {
        _tradingCardURL = _tradingCardURL + cardID + ", ";
    }
}

