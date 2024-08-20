using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PickinTeamsManager : MonoBehaviour
{
    [SerializeField] private InputGroup _inputGroup;
    [SerializeField] private MiniGameManager _miniGameManager;
    [SerializeField] private AudioManager _soundManager;

    [Header("Puzzles")]
    [SerializeField] private Sprite[] _backgroundSprites;
    [SerializeField] private Image _backgroundImage;

    [Header("UI Objects")]
    [SerializeField] private GameObject[] _kakuroPuzzles;
    [SerializeField] private List<DragAndDrop> _dragAndDropNumbers;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private GameObject _timer;
    [SerializeField] private GameObject _disableInteractions;
    [SerializeField] private Transform _numberContainer;

    [Header("Puzzle Check")]
    [SerializeField] private bool _completeAnswersCheck; // for debugging
    [SerializeField] private Color _wrongColor;
    [SerializeField] private Color _startColor;
    [SerializeField] private bool _solveComplete = true;
    [SerializeField] private bool _instructionsVisible = false;
    [SerializeField] private bool _playerWins = false;
    [SerializeField] private float _solveTimer;
    [SerializeField] private bool _autoWin;
    private int _correctNumbers;
    private int _wrongNumbers;

    //global variables
    private TextMeshProUGUI _timeText;
    private float _timeCount;
    private string _winText;

    private void Awake()
    {
        _gameOver.SetActive(false);
        _timeText = _timer.GetComponentInChildren<TextMeshProUGUI>();
        _disableInteractions.SetActive(false);
        _winText = _miniGameManager._win.Description;
    }

    private void Update()
    {
        if(_miniGameManager.ResetGame) ResetGame(); //reset game after on starting game
        if (_miniGameManager.Playing)
        {
            _timeCount += Time.deltaTime;
            _timeText.text = $"{(int)_timeCount / 60:D2}:{(int)_timeCount % 60:D2}";
            if (_timeCount >= 5999.9) GameOver();
        }
    }

    private void ResetDragAndDropNumbers()
    {
        _dragAndDropNumbers.Clear();
        foreach (Transform number in _numberContainer)
        {
            _dragAndDropNumbers.Add(number.GetComponent<DragAndDrop>());
        }
    }

    private void RandomKakuroPuzzle()
    {
        int randomIndex = Random.Range(0, _kakuroPuzzles.Length);
        _kakuroPuzzles[randomIndex].SetActive(true);
        _backgroundImage.sprite = _backgroundSprites[randomIndex];
        _inputGroup = _kakuroPuzzles[randomIndex].GetComponent<InputGroup>();
    }

    private void ReferenceInputGroup()
    {
        foreach (DragAndDrop number in _dragAndDropNumbers)
        {
            number.SetInputGroup(_inputGroup);
        }
    }

    private void CheckSolution(InputGroup inputGroup)
    {
        if (!_solveComplete) return; //stop user from spamming solve button

        _solveComplete = false;
        _wrongNumbers = 0;

        //check if each inputslot  has been populated with an answer
        if (_completeAnswersCheck)
        {
            foreach (UserInput userInput in inputGroup.userInputs)
            {
                if (userInput.setNumber == 0) return;
            }
        }

        _correctNumbers = 0;
        for (int i = 0; i < inputGroup.userInputs.Count; i++)
        {
            if (inputGroup.userInputs[i].setNumber != 0)
            {
                int num = inputGroup.userInputs[i].setNumber;
                if (num == inputGroup.userInputs[i].CorrectNumber())
                {
                    _correctNumbers++;
                    inputGroup.userInputs[i].CorrectAnimation();
                }
                else
                {
                    inputGroup.userInputs[i].IncorrectAnimation();
                    StartCoroutine(Incorrect(inputGroup.userInputs[i]));
                    _soundManager.Play("SolveWrong");
                    _wrongNumbers++;
                }
            }
        }

        if(_wrongNumbers == 0) _soundManager.Play("SolveCorrect");

        StartCoroutine(SolveComplete());
        if (_correctNumbers == inputGroup.userInputs.Count || _autoWin)
        {
            _playerWins = true;
            GameOver();
        }
    }

    IEnumerator SolveComplete()
    {
        yield return new WaitForSeconds(_solveTimer);
        _solveComplete = true;
    }

    IEnumerator Incorrect(UserInput userInput) //display red for a few seconds if incorrect
    {
        yield return new WaitForSeconds(0.05f);// small wait time for touch controls to catch up

        _solveComplete = false;
        _disableInteractions.SetActive(true);
        userInput.background.effectColor = _wrongColor;
        userInput.background.effectDistance = new Vector2(5, 5);
        TryAgain(true);

        yield return new WaitForSeconds(2);

        userInput.background.effectColor = _startColor;
        userInput.background.effectDistance = new Vector2(2, 2); 
        TryAgain(false);
        _disableInteractions.SetActive(false);
        _solveComplete = true;
    }

    private void TryAgain(bool solveUI)
    {
        _gameOver.SetActive(solveUI);
    }

    private void GameOver()
    {
        //stop game and show the start UI
        _disableInteractions.SetActive(true);
        _miniGameManager._win.Description = AppendString(_winText, _timeText.text);
        EndGame();
    }

    private string AppendString(string text, string append1)
    {
        text = text.Replace("{1}", append1);
        return text;
    }

    private void ResetGame() //reset the board
    {
        if (_inputGroup != null) _inputGroup.gameObject.SetActive(false);
        ResetDragAndDropNumbers();
        RandomKakuroPuzzle();
        ReferenceInputGroup();

        foreach (UserInput userInput in _inputGroup.userInputs) //reset user input tiles
        {
            if (userInput.transform.childCount > 0)
            {
                userInput.SetNumber(0);
                Destroy(userInput.transform.GetChild(0).gameObject);
            }
        }

        //reset important components
        _timeCount = 0;
        _gameOver.SetActive(false);
        _disableInteractions.SetActive(false);
        _miniGameManager.SetReset(false);
        _playerWins = false;
        _instructionsVisible = false;
    }

    public void Solve() //accessible function for solve button
    { 
        if (_instructionsVisible || _playerWins) return; //disbale usage when instructions menu is visible
        CheckSolution(_inputGroup);
    }

    public void EndGame()
    {
        _miniGameManager.EndGame(_playerWins);
    }
}
