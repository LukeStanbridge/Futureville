using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public bool _debugTesting;
    [SerializeField] private SceneTransitionManager _transition;
    
    [Header("MINI GAME")]
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _miniGameUI;
    [SerializeField] private GameObject _titlePage;
    [SerializeField] private float _titlePageTimer;
    [SerializeField] private GameObject _leadInTimerPage;

    [Header("LOGO ANIIMATION")]
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _imageArray;
    [SerializeField] private float _speed = .04f;
    private int m_IndexSprite;
    private Coroutine m_CoroutineAnim;
    [SerializeField] private Coroutine _leadInCoroutine;

    [Header("INTRO PANELS")]
    [SerializeField] private GameObject _introPanel;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private int _introPanelIndex;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private TextMeshProUGUI _pictureInstructionText;
    [SerializeField] private Image _instructionImage;
    [SerializeField] private RectTransform _imageSize;
    [SerializeField] private List<Instruction> _instructions;

    [Header("IN-GAME OPTIONS")]
    [SerializeField] private GameObject _optionsButton;
    [SerializeField] private GameObject _miniGameOptions;

    [Header("INSTRUCTIONS PANELS")]
    [SerializeField] private GameObject _instructionsPanel;
    [SerializeField] private List<GameObject> _instructionPanels;
    [SerializeField] private GameObject _nextPanelButton;
    [SerializeField] private GameObject _backToGameButton;
    [SerializeField] private int _panelIndex;

    [Header("END GAME PANEL")]
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] public Instruction _win;
    [SerializeField] public Instruction _lose;
    [SerializeField] private TextMeshProUGUI _endGameText;
    [SerializeField] private Image _endGameImage;
    
    //getters and setters for state of game
    public bool Playing { get; private set; }
    public void SetPlaying(bool play) { Playing = play; }
    public bool ResetGame { get; private set; }
    public void SetReset(bool reset) { ResetGame = reset; }
    public void CloseEndGamePanel() { _endGamePanel?.SetActive(false); }
    public void DisableOptionsButton() { _optionsButton?.SetActive(false); }

    private void KillTweensInScene()
    {
        if (DOTween.TotalActiveTweens() > 0)
        {
            var tweens = new List<Tween>();
            tweens.AddRange(DOTween.PlayingTweens());
            for (int i = 0; i < tweens.Count; i++)
            {
                var tween = tweens[i];
                if (tween != null && tween.target is GameObject target && target.scene == gameObject.scene) // <= this
                    tween.Kill();
            }
        }
    }

    private void DebugMode() //DEBUG ONLY: delete later, checks for sound manager for buttons 
    {
        if (TryGetComponent<AudioManager>(out AudioManager soundManager)) { }
        if (soundManager == null)
        {
            List<ButtonAnimation> foundButtons = new List<ButtonAnimation>();
            var buttons = FindObjectsOfType<ButtonAnimation>() as ButtonAnimation[];
            foundButtons.AddRange(buttons);
            foreach (var button in foundButtons)
            {
                button.DebugMode = true;
            }
        }
    }

    private void Awake()
    {
        if (_debugTesting) DebugMode();
        else _transition = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransitionManager>();

        //set starter game objects
        _miniGameUI?.SetActive(true);
        _titlePage?.SetActive(true);
        _introPanel?.SetActive(false);
        _leadInTimerPage?.SetActive(false);
        _optionsButton?.SetActive(false);
        _miniGameOptions?.SetActive(false);
        _instructionsPanel?.SetActive(false);
        _endGamePanel?.SetActive(false);

        //remove second instruction panel from list if not being used
        if (_instructionPanels.Count > 1) _instructionPanels[1]?.SetActive(false);
        if (_instructionPanels.Count > 2) _instructionPanels[2]?.SetActive(false);
        _instructionPanels[0]?.SetActive(false);

        Playing = false;
        ResetGame = false;
        _panelIndex = 0;
        _imageSize = _instructionImage.GetComponent<RectTransform>();

        StartCoroutine(TitlePage());
    }

    IEnumerator TitlePage()
    {
        _image.color = new Color(1,1,1,0);
        yield return new WaitForSeconds(0.5f);
        _image.color = new Color(1, 1, 1, 1);
        StartCoroutine(PlayAnimUI());
        yield return new WaitForSeconds(_titlePageTimer);
        OpenIntroPanels();
    }

    IEnumerator PlayAnimUI()
    {
        yield return new WaitForSeconds(_speed);

        if (m_IndexSprite >= _imageArray.Length)
        {
            m_IndexSprite = 0;
            yield break;
        }
        _image.sprite = _imageArray[m_IndexSprite];
        m_IndexSprite += 1;
        m_CoroutineAnim = StartCoroutine(PlayAnimUI());
    }

    private void OpenIntroPanels() //display the intro panels 
    {
        _soundManager.FadeIn("MiniGameMenuTheme", 1.5f);
        _introPanelIndex = 0;  
        Time.timeScale = 0;
        _miniGame?.SetActive(true);
        _miniGameUI?.SetActive(true);
        _titlePage?.SetActive(false);
        _introPanel?.SetActive(true);
        _nextButton?.SetActive(true);
        _startButton?.SetActive(false);
        SetIntroPanel();
    }

    private void SetIntroPanel() //change to next panel based on layout
    {
        if (_introPanelIndex == _instructions.Count - 1) //swap out button on the last instruction panel
        {
            _nextButton?.SetActive(false);
            _startButton?.SetActive(true);
        }

        if (_instructions[_introPanelIndex].TextWithImage) //set instruction with an image next to the text
        {
            _instructionText.gameObject.SetActive(false);
            _pictureInstructionText.gameObject.SetActive(true);
            _instructionImage.gameObject.SetActive(true);

            _pictureInstructionText.text = _instructions[_introPanelIndex].Description;
            _instructionImage.sprite = _instructions[_introPanelIndex].InstructionObjectImage;
            _imageSize.sizeDelta = _instructions[_introPanelIndex].ImageDimensions;
            _imageSize.transform.localScale = new Vector3(_instructions[_introPanelIndex].Scale, _instructions[_introPanelIndex].Scale, _instructions[_introPanelIndex].Scale);
        }
        else //set instruction without an image next to the text
        {
            _instructionText.gameObject.SetActive(true);
            _pictureInstructionText.gameObject.SetActive(false);
            _instructionImage.gameObject.SetActive(false);

            _instructionText.text = _instructions[_introPanelIndex].Description;
        }
    }

    private IEnumerator ClosePanel()
    {
        Time.timeScale = 0;
        _soundManager.Stop("MiniGameMenuTheme");
        yield return new WaitForSecondsRealtime(0.3f);
        _introPanel?.SetActive(false);
        _endGamePanel?.SetActive(false);
        _leadInTimerPage?.SetActive(true);
        if(_leadInCoroutine == null) _leadInCoroutine = StartCoroutine(LeadInTimer());
    }

    private IEnumerator LeadInTimer() //start lead in time
    {
        _soundManager.PlayAudioClip("Countdown");
        _optionsButton?.SetActive(false);
        TextMeshProUGUI numberText = _leadInTimerPage.GetComponent<TextMeshProUGUI>();
        numberText.text = "3";
        PulseText(numberText.transform);
        yield return new WaitForSecondsRealtime(1);
        numberText.text = "2";
        PulseText(numberText.transform);
        yield return new WaitForSecondsRealtime(1);
        numberText.text = "1";
        PulseText(numberText.transform);
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;
        _soundManager.Stop("MiniGameMenuTheme");
        _soundManager.FadeIn("MiniGameMenuTheme", 1);
        _miniGame?.SetActive(true);
        _miniGameUI.SetActive(false);
        _optionsButton.SetActive(true);
        _leadInTimerPage?.SetActive(false);
        Playing = true;
        _leadInCoroutine = null;
    }

    private void PulseText(Transform text) //pulse the 3,2,1 leading into the game
    {
        text.localScale = new Vector3(1, 1, 1);
        text.transform.DOScale(text.localScale * 1.2f, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
        {
            text.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true);
        });
    }

    private void DisplayWinLose(bool win) //display win or lose message in end game panel
    {
        _miniGameUI?.SetActive(true);
        _endGamePanel?.SetActive(true);

        if (win)
        {
            _soundManager.Play("GameOverWin");
            _endGameText.text = _win.Description;
            _endGameImage.sprite = _win.InstructionObjectImage;
        }
        else
        {
            _soundManager.Play("GameOverLose");
            _endGameText.text = _lose.Description;
            _endGameImage.sprite = _lose.InstructionObjectImage;
        }
    }

    public void EndGame(bool win) //function to quit mini game
    {
        _soundManager.Stop("MiniGameMenuTheme");
        _soundManager.FadeIn("MiniGameMenuTheme", 5);
        _miniGameUI.SetActive(true);
        _optionsButton?.SetActive(false);
        Playing = false;
        DisplayWinLose(win);
    }

    #region Mini-Game Buttons
    public void StartGame() //buttton to start or restart the mini gameg
    {
        Time.timeScale = 1;
        _miniGame.SetActive(true);
        _miniGameOptions?.SetActive(false);
        StartCoroutine(ClosePanel());
        ResetGame = true;
    }

    public void NextIntroPanel() //button to progress intro panels
    {
        if (_introPanelIndex >= _instructions.Count) return;
        _introPanelIndex++;
        SetIntroPanel();
    }

    public void OpenGameOptions() //button to open the game options and pause mini game
    {
        Time.timeScale = 0;
        Playing = false;
        _introPanelIndex = 0;
        _miniGameUI?.SetActive(true);
        _miniGameOptions?.SetActive(true);
        _soundManager.PauseAllAudio();
    }

    public void CloseGameOptions() //close the game options panel and return to the game
    {
        Time.timeScale = 1;
        Playing = true;
        _miniGameOptions?.SetActive(false);
        _miniGameUI?.SetActive(false);
        _soundManager.UnPauseAllAudio();
    }

    public void OpenInstructionPanels() //open the instructions panel while the game is paused
    {
        if (_panelIndex == 0) //display first panel
        {
            _optionsButton.SetActive(false);
            _instructionsPanel?.SetActive(true);
            _miniGameOptions?.SetActive(false);
            _instructionPanels[_panelIndex]?.SetActive(true);
            if (_instructionPanels.Count > 1)
            {
                _nextPanelButton.SetActive(true);
                _backToGameButton.SetActive(false);
            }
            else
            {
                _nextPanelButton.SetActive(false);
                _backToGameButton.SetActive(true);
            }
        }
    }

    public void CloseInstructions()
    {
        _optionsButton.SetActive(true);
        _instructionPanels[_instructionPanels.Count - 1]?.SetActive(false);
        _instructionsPanel?.SetActive(false);
        _panelIndex = 0;
        OpenGameOptions();
    }

    public void NextInstructionPanel()
    {
        _panelIndex++;
        _instructionPanels[_panelIndex - 1].SetActive(false);
        _instructionPanels[_panelIndex].SetActive(true);
    }

    public void PreviousInstructionPanel() //go back a page in the instructions
    {
        _panelIndex--;
        _instructionPanels[_panelIndex + 1].SetActive(false);
        _instructionPanels[_panelIndex].SetActive(true);
    }

    public void BackToFutureville() //return to the main world
    {
        KillTweensInScene();
        if (!_debugTesting)
        {
            Time.timeScale = 1;
            ResetGame = true;
            _transition.ReturnToGame(); //button to leave the mini game back to futureville
        }
    }
    #endregion
}
