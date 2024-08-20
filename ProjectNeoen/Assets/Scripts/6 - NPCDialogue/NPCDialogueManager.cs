using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NPCDialogueManager : MonoBehaviour
{
    [Header("Scene Transition References")]
    [SerializeField] private SceneTransitionManager _transition;
    [SerializeField] private string _sceneName;
    [SerializeField] private PlayerAnimations _playerAnimations;
    [SerializeField] private MayorIntro _mayor;
    [SerializeField] private DisplayLocation _displayLocation;
    [SerializeField] private CollectedCards _collectedCards;

    [Header("NPC Refernces")]
    [SerializeField] private ActivateDialogue _currentNPC;
    [SerializeField] private Transform _miniGameNPC;
    [SerializeField] private Transform _tradingCardNPC;
    [SerializeField] private bool _miniGamePlayed;
    [SerializeField] private Color _startLocationColour;
    private NPCDialogue _currentDialogue;
    private string _sceneToLoad;
    private bool _mayorIntro = true;

    [Header("Dialogue UI Components")]
    [SerializeField] private string _currentResponse;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private RectTransform _dialogueBoxRect;
    [SerializeField] private Button _activeTalkButton;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _clickableBox;
    [SerializeField] private GameObject _yesButton;
    [SerializeField] private GameObject _noButton;
    [SerializeField] private List<int> _boldIndexes;
    [field: SerializeField] public bool DialogueOpen { get; private set; }
    public Queue<string> _sentences = new Queue<string>();
    public Button GetNPCButton() { return _activeTalkButton; }

    [Header("Trading Card Object References")]
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private TradingCardManager _tradingCardManager;
    [SerializeField] private GameObject _tradingCardBackground;

    private void Awake()
    {
        SetManagerReferences();
    }

    private void SetManagerReferences() //link the dialogue manager to all the NPC's
    {
        foreach (Transform t in _miniGameNPC)
        {
            ActivateDialogue trigger = t.GetComponent<ActivateDialogue>();
            trigger.DialogueManager = this;
        }

        foreach (Transform t in _tradingCardNPC)
        {
            ActivateDialogue trigger = t.GetComponent<ActivateDialogue>();
            trigger.DialogueManager = this;
        }
    }

    public void ActivateButton(ActivateDialogue npc, Button talkButton) //set current npc and activate talk button
    {
        _currentNPC = npc;
        _activeTalkButton = talkButton;
        _activeTalkButton.gameObject.SetActive(true);
        IconAnimation icon = _activeTalkButton.GetComponentInChildren<IconAnimation>();
        icon.StartTween();

        if (_currentNPC.TryGetComponent<MayorIntro>(out MayorIntro mayorIntro))
        {
            if (mayorIntro.OpenConvo)
            {
                NPCDialogue open = mayorIntro.GameIntro;
                StartDialogue(open, null);
                _activeTalkButton.onClick.AddListener(TriggerNPCDialogue);
                mayorIntro.OpenConvo = false;
                return;
            }
        }

        //set correct listener function to the talk button
        if (_currentNPC.MiniGameNPC)
        {
            _activeTalkButton.onClick.RemoveAllListeners();
            _activeTalkButton.onClick.AddListener(TriggerNPCDialogue);
        }
    }

    public void DeactivateButton(Button talkButton) //deactivate talk button and remove npc reference
    {
        if(!_transition.Transitioning) _currentNPC = null;
        talkButton.onClick.RemoveAllListeners();
        talkButton.gameObject.SetActive(false);
        IconAnimation icon = talkButton.GetComponentInChildren<IconAnimation>();
        icon.KillTween();
    }

    public void TriggerDialogue()
    {
        ProgressDialogue();
        StartDialogue(_currentDialogue, _sceneToLoad);
    }

    public void StartDialogue(NPCDialogue dialogue, string sceneName) //start dialogue with an npc
    {
        if (sceneName != null) _sceneName = sceneName;
        DialogueOpen = true;
        _activeTalkButton.gameObject.SetActive(false);
        _dialogueBoxRect.DOAnchorPosY(180, 0.5f);
        _nameText.text = _currentNPC.GetComponent<MiniGameNPC>().Name;
        _sentences.Clear();

        if (_currentNPC.MiniGameNPC) _continueButton.gameObject.SetActive(true);
        if (_currentNPC.TradingCardNPC) _continueButton.gameObject.SetActive(false);

        foreach (string sentence in dialogue.Sentences)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() //continue button to progess dialogue to next line or transition to mini game
    {
        if (_sentences.Count > 1) //set correct objects for dialogue when more than 1 line left
        {
            _continueButton.gameObject?.SetActive(true);
            _exitButton.gameObject?.SetActive(true);
            _clickableBox.gameObject?.SetActive(true);
            _yesButton.gameObject?.SetActive(false);
            _noButton.gameObject?.SetActive(false);
        }
        else //set objects for last line of dialogue
        {
            if (_miniGamePlayed && _currentNPC.GetComponent<MiniGameNPC>().ObjectiveNPC) _continueButton.gameObject?.SetActive(true);
            else _continueButton.gameObject?.SetActive(false);

            _clickableBox.gameObject?.SetActive(false);

            if (_currentNPC.GetComponent<MiniGameNPC>().ObjectiveNPC && !_miniGamePlayed) //set yes, no, exit buttons 
            {
                _yesButton.gameObject?.SetActive(true);
                _noButton.gameObject?.SetActive(true);
                _exitButton.gameObject?.SetActive(false);
            }
            else
            {
                _yesButton.gameObject?.SetActive(false);
                _noButton.gameObject?.SetActive(false);
                _exitButton.gameObject?.SetActive(true);
            }
        }

        if (_miniGamePlayed && _sentences.Count == 0 && _currentNPC.GetComponent<MiniGameNPC>().ObjectiveNPC) //take to trading card page after mini game played **FUNCTION THIS**
        {
            EndDialogue();
            _gameUIManager.SetShowRecommend(true);
            _gameUIManager.ShowTradingCards();
            GameManager.Instance.UpdateGameState(GameState.InGameMenus);
            return;
        }

        _currentResponse = _sentences.Dequeue();

        //checks for appending text or skipping last dialogue
        _boldIndexes.Clear();
        AppendMayorIntroText(_currentResponse);
        AppendLocationText(_currentResponse);

        StopAllCoroutines();
        StartCoroutine(TypeSentence(_currentResponse));
    }

    IEnumerator TypeSentence(string sentence) //prints dialogue letter by letter
    {
        _dialogueText.text = "";
        int index = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            if (_boldIndexes.Contains(index)) //check if letter needs to be bolded and update text
            {
                string boldLetter = "<font=\"Poppins-ExtraBold\">" + letter + "</font>";
                _dialogueText.text += boldLetter;
            }
            else _dialogueText.text += letter;

            yield return new WaitForSeconds(0.01f);
            index++;
        }
    }

    private void AppendMayorIntroText(string response) //append mayor opening dialogue when giving objective NPC details
    {
        if (_currentNPC.GetComponent<MayorIntro>() == null) return;
        if (_miniGamePlayed) return;
        if (_currentNPC.GetComponent<MayorIntro>().GameIntro.Sentences[2] != response) return;
        _currentResponse = AppendString(response, GameManager.Instance.ObjectiveNPC.Name, GameManager.Instance.ObjectiveNPC.Location);
        
    }

    private void AppendLocationText(string response) //append text when non-objective NPC giving objective NPC details
    {
        if (_miniGamePlayed) return;
        if (_sentences.Count > 0) return;
        if (!response.Contains("{1}")) return;
        _currentResponse = AppendString(response, GameManager.Instance.ObjectiveNPC.Name, GameManager.Instance.ObjectiveNPC.Location);
    }

    public void PlayMiniGame()
    {
        GameManager.Instance.UpdateGameState(GameState.Futureville);
        _miniGamePlayed = true;
        EndDialogue();
        _transition.SetSceneName(_sceneName);
        _transition.LoadMiniGame();
    }

    public void ProgressDialogue()
    {
        if (!_currentNPC.GetComponent<MiniGameNPC>().ObjectiveNPC) return;
        _continueButton.gameObject.SetActive(true);
    }

    public void EndDialogue() //close dialogue
    {
        _dialogueBoxRect.DOAnchorPosY(-280, 0.5f).SetUpdate(true);
        DialogueOpen = false;
        if (_mayorIntro)
        {
            _displayLocation.DisplayLocationText("Old Town", _startLocationColour, new Vector2(616, 133));
            _mayorIntro = false;
        }
        if (_activeTalkButton != null) ActivateButton(_currentNPC, _activeTalkButton);
        if (_currentNPC == null) return;
    }

    public void TriggerNPCDialogue() //function to add to the talk button when talking to a NPC
    {
        _playerAnimations.GetComponent<ClickController>().StopPlayer();

        MiniGameNPC npc = _currentNPC.GetComponent<MiniGameNPC>();
        DialogueOpen = true;

        if (npc.ObjectiveNPC)
        {
            if (!_miniGamePlayed) //talking to the objective npc before playing the mini game
            {
                _currentDialogue = npc.PreObjectiveDialogue;
                _sceneToLoad = npc.SceneToLoad.ToString();
                TriggerDialogue();
            }
            else //talking to objective NPC after mini game played
            {
                _currentDialogue = npc.PostObjectiveDialogue;
                _sceneToLoad = null;
                DisplayTradingCard();
            }
        }
        else //talking to non-objective NPC
        {
            if (!_miniGamePlayed) //talking to the objective npc before playing the mini game
            {
                _currentDialogue = npc.PreMiniGameDialogue;
                _sceneToLoad = npc.SceneToLoad.ToString();
            }
            else //talking to non-fobjective NPC after mini game played
            {
                _currentDialogue = npc.PostMiniGameDialogue;
                _sceneToLoad = null;
            }
            DisplayTradingCard();
        }

        _playerAnimations.FaceNPC(_currentNPC.transform.position);
        FacePlayer();
    }

    public void DisplayTradingCard() //function to add to the talk button when talking to a trading card npc
    {
        MiniGameNPC npc;
        npc = _currentNPC.GetComponent<MiniGameNPC>();

        if (npc.ShowTradingCard) //check if card can be shown
        {
            //disable card being shown if it is already collected
            if (_collectedCards.CheckCardCollected(Convert.ToInt32(npc.TradingCardID))) npc.ShowTradingCard = false;
            else
            {
                _tradingCardBackground.SetActive(true);
                _tradingCardManager.SetNPCTradingCard(_currentNPC);
            }
        }
        else TriggerDialogue();
    }

    public void UpdateObjectiveLocation()
    {
        string objectiveNPC = _objectiveText.text;
        objectiveNPC = AppendFont(objectiveNPC);
        objectiveNPC = AppendString(objectiveNPC, GameManager.Instance.ObjectiveNPC.Name, GameManager.Instance.ObjectiveNPC.Location);
        _objectiveText.text = objectiveNPC;
    }

    private void FacePlayer()
    {
        Vector3 playerPos = _playerAnimations.transform.position;
        Vector3 npcPos = _currentNPC.transform.position;
        SpriteRenderer npcSprite = _currentNPC.GetComponent<SpriteRenderer>();
        if (npcPos.x < playerPos.x) npcSprite.flipX = true;
        if (npcPos.x > playerPos.x) npcSprite.flipX = false;
    }

    private string AppendString(string text, string append1, string append2)
    {
        text = text.Replace("{1}", append1);
        text = text.Replace("{2}", append2);
        GetIndicesOfWord(text, append1);
        GetIndicesOfWord(text, append2);
        return text;
    }

    private void GetIndicesOfWord(string text, string word) //log the index of each letter that needs to be BOLDED in NPC dialogue to a list
    {
        int index = text.IndexOf(word);
        while (index != -1)
        {
            for (int i = 0; i < word.Length; i++)
            {
                _boldIndexes.Add(index + i);
            }
            index = text.IndexOf(word, index + word.Length);
        }
    }

    private string AppendFont(string text)
    {
        string before = "<font=\"Poppins-ExtraBold\">";
        string after = "</font>";
        string first = "{1}";
        
        int indexOne = text.IndexOf(first);
        string newText = text.Insert(indexOne, before);
        newText = newText.Insert(indexOne + first.Length + before.Length, after);

        string second = "{2}";
        int indexTwo = newText.IndexOf(second);
        newText = newText.Insert(indexTwo, before);
        newText = newText.Insert(indexTwo + second.Length + before.Length, after);

        
        return newText;
    }
}
