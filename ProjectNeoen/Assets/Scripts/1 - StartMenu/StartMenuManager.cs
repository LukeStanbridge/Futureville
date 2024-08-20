using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private CharacterSelectionManager _charSelectManager;

    [Header("Canvas Game Objects")]
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _characterSelection;
    [SerializeField] private GameObject _npcDialogue;
    [SerializeField] private GameObject _mainWorldButtons;
    [SerializeField] private GameObject _overlay;

    [Header("Start Menu Game Objects")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _backgroundScreen;
    [SerializeField] private GameObject _titleScreen;
    [SerializeField] private GameObject _gameObjectiveScreen;
    
    [Header("Loading Variables")]
    [SerializeField] private float _loadingTimer;
    [SerializeField] private Image _gameLogo;

    private void Awake()
    {
        LoadingPage();
    }

    private void LoadingPage()
    {
        //hide all game UI
        _backgroundScreen.SetActive(false);
        _titleScreen.SetActive(false);
        _gameObjectiveScreen.SetActive(false);
        _characterSelection.SetActive(false);
        _npcDialogue.SetActive(false);
        _mainWorldButtons.SetActive(false);
        _overlay.SetActive(false);
        
        //activate loading screen and start loading process
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen() //progress loading bar then transition to Title Screen when laoding bar is full.
    {
        yield return new WaitForSecondsRealtime(_loadingTimer);

        _gameLogo.DOFade(0, 2).SetUpdate(true);
        _loadingScreen.GetComponent<Image>().DOFade(0, 1.5f).SetUpdate(true);
        TitlePage();

        yield return new WaitForSecondsRealtime(1.5f);

        _loadingScreen.SetActive(false);
    }

    private void TitlePage() //display title page
    { 
        _backgroundScreen.SetActive(true);
        _titleScreen.SetActive(true);
    }

    public void StartGame() //button to take player to the game objective screen
    {
        _titleScreen.SetActive(false);
        _gameObjectiveScreen.SetActive(true);
    }

    public void PlayGame() //play button to go to character selection
    {
        _gameObjectiveScreen.SetActive(false);
        _charSelectManager.OpenCharacterSelection();
    }
}
