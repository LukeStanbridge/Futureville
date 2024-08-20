using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BossBattleManager : MonoBehaviour
{
    [Header("Important References")]
    [SerializeField] private MiniGameManager _miniGameManager;
    [SerializeField] private AudioManager _soundManager;
    private DialogueTrigger _dialogueTrigger;
    [SerializeField] private DiceRoll _diceRoll;
    [SerializeField] private MayorAnimations _mayorAnimations;

    [Header("Scores")]
    [SerializeField] private int _mayorScore = 0;
    [SerializeField] private int _playerScore = 0;
    [SerializeField] private int _persuasionFactor = 0;
    [SerializeField] private int _persuasionBonusFactor = 0;
    [SerializeField] private int _hinderanceFactor = 0;
    [SerializeField] private List<int> _mayorRoundScore;
    [SerializeField] private List<int> _playerRoundScore;

    [Header("Data Lists")]
    [SerializeField] private List<string> _bossDialogue; 
    [SerializeField] private List<string> _playerDialogue; 
    [SerializeField] private List<int> _bossWeighting; 
    [SerializeField] private List<int> _playerWeighting;

    [SerializeField] private bool _playerWins;
    [SerializeField] private int _roundIndex = 0;

    [Header("UI Objects and Variables")]
    [SerializeField] private GameObject _roundOverlay;
    [SerializeField] private GameObject _roundTotals;
    [SerializeField] private GameObject _roundOutcome;
    [SerializeField] private TextMeshProUGUI _bossText;
    [SerializeField] private ResponseOption[] _playerResponseOptions;
    [SerializeField] private Button[] _playerResponseButtons;
    [SerializeField] private TextMeshProUGUI _roundNumber;
    [SerializeField] private TextMeshProUGUI _persuasionBonus;

    [Header("Round Overlay UI")]
    [SerializeField] private float _diceRollTimer;
    [SerializeField] private float _displayWaitTimer;
    [SerializeField] private TextMeshProUGUI _overlayRoundNumber;
    [SerializeField] private TextMeshProUGUI _winLoseRoundNumber;
    [SerializeField] private TextMeshProUGUI _mayorResponseValue;
    [SerializeField] private TextMeshProUGUI _hindranceValue;
    [SerializeField] private TextMeshProUGUI _mayorTotalScore;
    [SerializeField] private TextMeshProUGUI _playerResponseValue;
    [SerializeField] private TextMeshProUGUI _persuasionBonusValue;
    [SerializeField] private TextMeshProUGUI _persuasionFactorValue;
    [SerializeField] private TextMeshProUGUI _playerTotalValue;
    [SerializeField] private TextMeshProUGUI _roundWinnerText;
    [SerializeField] private TextMeshProUGUI _newPersuasionValue;
    [SerializeField] private GameObject _newPersuasionObj;
    [SerializeField] private GameObject _newPersuasionNumberObj;

    private void Awake()
    {
        _dialogueTrigger = GetComponent<DialogueTrigger>();
        _roundOverlay?.SetActive(false);
        _roundNumber.text = "";
        _persuasionBonus.text = "";
    }

    private void Start()
    {
        ButtonInteraction(false);
    }

    private void OnButtonClick(bool pressAllowed)
    {
        foreach (Button playerResponse in _playerResponseButtons)
        {
            if (pressAllowed)
            {
                playerResponse.gameObject.GetComponent<ButtonAnimation>()._onClick.AddListener(() => NextRound(playerResponse.transform.parent.GetChild(playerResponse.transform.GetSiblingIndex()-1).GetComponentInChildren<ResponseOption>().GetResponseWeight(), playerResponse));
            }
            else playerResponse.gameObject.GetComponent<ButtonAnimation>()._onClick.RemoveAllListeners();
        }
    }

    private void ButtonInteraction(bool interact)
    {
        foreach (Button playerResponse in _playerResponseButtons)
        {
            if (playerResponse.gameObject.GetComponent<ButtonAnimation>().CanClick == interact) return;
            playerResponse.gameObject.GetComponent<ButtonAnimation>().ResetButton();
            playerResponse.gameObject.GetComponent<ButtonAnimation>().SetCanClick(interact);
        }
    }

    private void ResetGame()
    {
        _roundOverlay?.SetActive(false);
        _playerWins = false;
        _roundIndex = 0;
        _mayorScore = 0;
        _playerScore = 0;
        _persuasionFactor = 0;
        _persuasionBonusFactor = 0;
        _hinderanceFactor = 0;

        _newPersuasionObj?.SetActive(true);
        _newPersuasionNumberObj?.SetActive(true);

        ClearResponses();
        _playerRoundScore.Clear();
        _mayorRoundScore.Clear();
        ButtonInteraction(false);
        _dialogueTrigger.TriggerDialogue();
        _diceRoll.ResetDicePos();
        _mayorAnimations.MayorNeutral();

        _miniGameManager.SetReset(false);
    }

    private void Update()
    {
        if (_miniGameManager.ResetGame) ResetGame(); //reset game after pressing "start game" in the MINI GAME UI
        if (_miniGameManager.Playing)
        {
            //maybe add timer per round
            _miniGameManager.SetPlaying(false);
        }
    }

    public void StartDialogue(Dialogue bossDialogue, Dialogue playerDialogue)
    {
        EventSystem.current.SetSelectedGameObject(null);
        _roundNumber.text = (_roundIndex + 1).ToString();
        _persuasionBonus.text = _persuasionBonusFactor.ToString();

        if (_roundIndex >= bossDialogue.Round.Count()) return;

        _bossDialogue.Clear();
        _bossWeighting.Clear();

        for (int i = 0; i < bossDialogue.Round[_roundIndex].Responses.Count(); i++)
        {
            _bossDialogue.Add(bossDialogue.Round[_roundIndex].Responses[i]);
            _bossWeighting.Add(bossDialogue.Round[_roundIndex].Weighting[i]);
        }

        CounterResponse(playerDialogue);
    }

    public void CounterResponse(Dialogue playerDialogue)
    {
        if (_bossDialogue.Count == 0)
        {
            EndGame();
            return;
        }

        // check if three responses
        int randomIndex;
        if (_bossDialogue.Count == 1) randomIndex = 0;
        else randomIndex = Random.Range(0, 3);

        string response = _bossDialogue[randomIndex];
        _mayorScore = _bossWeighting[randomIndex];

        StopAllCoroutines();
        StartCoroutine(TypeSentence(response, playerDialogue));
    }

    IEnumerator TypeSentence(string sentence, Dialogue playerDialogue)
    {
        if (sentence == _dialogueTrigger.BossDialogue.Round[0].Responses[0]) yield return new WaitForSeconds(0.1f);
        _bossText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _bossText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        PlayerResponses(playerDialogue);
    }

    private void ClearResponses()
    {
        _playerDialogue.Clear();
        for (int i = 0; i < _playerResponseOptions.Count(); i++)
        {
            _playerResponseOptions[i].ClearText();
        }
    }

    public void PlayerResponses(Dialogue dialogue)
    {
        _playerDialogue.Clear();
        _playerWeighting.Clear();

        for (int i = 0; i < dialogue.Round[_roundIndex].Responses.Count(); i++)
        {
            _playerDialogue.Add(dialogue.Round[_roundIndex].Responses[i]);
            _playerWeighting.Add(dialogue.Round[_roundIndex].Weighting[i]);
        }

        PlayerResponses();
    }

    public void PlayerResponses()
    {
        if (_playerDialogue.Count == 0)
        {
            EndGame();
            return;
        }

        //Randomize player response locations
        List<int> generatedNumbers = new List<int>();
        for (int i = 0; i < _playerDialogue.Count; i++)
        {
            int randomNumber;
            do
            {
                // Generate a random integer between 1 and 3
                randomNumber = Random.Range(0, 3);
            } while (generatedNumbers.Contains(randomNumber));

            // Add the generated number to the list
            generatedNumbers.Add(randomNumber);

            _playerResponseOptions[randomNumber].SetResponse(_playerDialogue[i], _playerWeighting[i]);
        }
        EventSystem.current.SetSelectedGameObject(null);
        OnButtonClick(true);
        ButtonInteraction(true);
    }


    private IEnumerator DisplayMayorValues(int weight)
    {
        ButtonInteraction(false);
        yield return new WaitForSeconds(_displayWaitTimer);
        _mayorResponseValue.text = _mayorScore.ToString();
        _soundManager.Play("Swoosh1");
        PulseText(_mayorResponseValue.transform);

        yield return new WaitForSeconds(_displayWaitTimer);
        int randomDiceSide = Random.Range(0, 6);
        _diceRoll.StartDiceRoll(randomDiceSide);

        yield return new WaitForSeconds(_diceRollTimer);
        _hinderanceFactor = randomDiceSide + 1;
        _hindranceValue.text = _hinderanceFactor.ToString();
        _soundManager.Play("Swoosh1");
        PulseText(_hindranceValue.transform);

        yield return new WaitForSeconds(_displayWaitTimer);
        _mayorScore += _hinderanceFactor;
        _mayorRoundScore.Add(_mayorScore);
        _mayorTotalScore.text = _mayorScore.ToString();
        _soundManager.Play("Swoosh2");
        PulseText(_mayorTotalScore.transform);

        yield return new WaitForSeconds(_displayWaitTimer);
        StartCoroutine(DisplayPlayerValues(weight));
    }

    private IEnumerator DisplayPlayerValues(int weight)
    {
        yield return new WaitForSeconds(_displayWaitTimer);
        _playerResponseValue.text = weight.ToString();
        _soundManager.Play("Swoosh1");
        PulseText(_playerResponseValue.transform);

        yield return new WaitForSeconds(_displayWaitTimer);
        _persuasionBonusValue.text = _persuasionBonusFactor.ToString();
        _soundManager.Play("Swoosh1");
        PulseText(_persuasionBonusValue.transform);
        _newPersuasionValue.text = "";

        yield return new WaitForSeconds(_displayWaitTimer);
        int randomDiceSide = Random.Range(0, 6);
        _diceRoll.StartDiceRoll(randomDiceSide);

        yield return new WaitForSeconds(_diceRollTimer);
        _persuasionFactor = randomDiceSide + 1;
        _persuasionFactorValue.text = _persuasionFactor.ToString();
        _soundManager.Play("Swoosh1");
        PulseText(_persuasionFactorValue.transform);

        yield return new WaitForSeconds(_displayWaitTimer);
        _playerScore = weight + _persuasionFactor + _persuasionBonusFactor;
        _playerRoundScore.Add(_playerScore);
        _playerTotalValue.text = _playerScore.ToString();
        _soundManager.Play("Swoosh2");
        PulseText(_playerTotalValue.transform);

        yield return new WaitForSeconds(2);
        StartCoroutine(DisplayWinnerPanel());
    }

    private IEnumerator DisplayWinnerPanel()
    {
        _roundIndex++;
        if (_roundIndex == 6)
        {
            _newPersuasionObj?.SetActive(false);
            _newPersuasionNumberObj?.SetActive(false);
        }
        _roundTotals.SetActive(false);
        _roundOutcome.SetActive(true);

        if (_playerScore >= _mayorScore)
        {
            _persuasionBonusFactor += 2;
            _roundWinnerText.text = "You win this round!";
            _soundManager.Play("WinRound");

            yield return new WaitForSeconds(_displayWaitTimer);

            if (_roundIndex == 6) _playerWins = true;
            else
            {
                _newPersuasionValue.text = _persuasionBonusFactor.ToString();
                PulseText(_newPersuasionValue.transform);
                _mayorAnimations.MayorUpset();
            }
        }
        else
        {
            _persuasionBonusFactor--;
            _roundWinnerText.text = "You lose this round!";
            _soundManager.Play("LoseRound");

            yield return new WaitForSeconds(_displayWaitTimer);
            if (_roundIndex != 6)
            {
                _newPersuasionValue.text = _persuasionBonusFactor.ToString();
                PulseText(_newPersuasionValue.transform);
                _mayorAnimations.MayorHappy();
            }
        }

        yield return new WaitForSeconds(2f);

        _playerScore = 0;
        _mayorScore = 0;

        for (int i = 0; i < _playerResponseOptions.Count(); i++)
        {
            _playerResponseOptions[i].ClearText();
        }

        _bossText.text = "";

        yield return new WaitForSeconds(_displayWaitTimer);
        _roundOverlay.SetActive(false);

        if (_roundIndex <= 5) _dialogueTrigger.TriggerDialogue();
        else EndGame();
    }

    public void NextRound(int weight, Button pressedButton)
    {
        _roundOverlay.SetActive(true);
        _roundTotals.SetActive(true);
        _roundOutcome.SetActive(false);
        _overlayRoundNumber.text = "Round " + (_roundIndex + 1);
        _winLoseRoundNumber.text = "Round " + (_roundIndex + 1);
        _mayorResponseValue.text = "";
        _hindranceValue.text = "";
        _mayorTotalScore.text = "";
        _playerResponseValue.text = "";
        _persuasionBonusValue.text = "";
        _persuasionFactorValue.text = "";
        _playerTotalValue.text = "";
        StartCoroutine(DisplayMayorValues(weight));
        OnButtonClick(false);
        _roundWinnerText.text = "";
    }

    private void PulseText(Transform text) //pulse the 3,2,1 leading into the game
    {
        text.localScale = new Vector3(1, 1, 1);
        text.transform.DOScale(text.localScale * 2.5f, 0.2f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
        {
            text.transform.DOScale(Vector3.one * 1, 0.2f).SetEase(Ease.InOutSine).SetUpdate(true);
        });
    }

    public void EndGame()
    {
        ClearResponses();
        _miniGameManager.EndGame(_playerWins);
    }
}
