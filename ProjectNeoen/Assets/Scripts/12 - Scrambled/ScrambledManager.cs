using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrambledManager : MonoBehaviour
{
    [SerializeField] private Image _sourceImage; // Reference to the source image
    [SerializeField] private RectTransform _targetParent; // Parent container for sliced images
    [SerializeField] GameObject _slicePrefab; // Prefab for the sliced images
    [SerializeField] private int _size; // size of puzzle
    [SerializeField] private int _borderSize;

    [SerializeField] private MiniGameManager _miniGameManager;
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private SceneTransitionManager _transition;
    [SerializeField] private Canvas _canvas;
    
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Sprite[] _gamePicture;
    [SerializeField] private Sprite _currentSprite;
    [SerializeField] private Image _unscrambledImage;
    [SerializeField] private GameObject _unscrambledPanel;

    [SerializeField] private GameObject _winImage;
    [SerializeField] private ParticleSystem _confettiPS;
    private PlayerControls _playerControls;
    [SerializeField] private List<RectTransform> _pieces;
    [SerializeField] private int _emptyLocation;
    private bool _shuffling = false;
    private bool _playerWins;
    private bool _checkOriginal;
    [SerializeField] private bool _isMoving;
    [SerializeField] private float _timeCount;
    private string _winText;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _pieces = new List<RectTransform>();
        _unscrambledPanel.gameObject?.SetActive(false);
        _confettiPS.Stop();
        _winImage.SetActive(false);
        _winText = _miniGameManager._win.Description;

        if (!_miniGameManager._debugTesting)
        {
            _transition = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransitionManager>();
            _canvas.worldCamera = Camera.main;
            this.gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        }
    }

    private void ResetGame()
    {
        StopAllCoroutines();
        _shuffling = true;
        _playerWins = false;
        _checkOriginal = false;
        _isMoving = false;
        _timeCount = 0;
        _timeText.text = $"{(int)_timeCount / 60:D2}:{(int)_timeCount % 60:D2}";
        _unscrambledPanel?.SetActive(false);
        _winImage?.SetActive(false);

        RandomSetImage();

        if (_pieces != null)
        {
            foreach (var piece in _pieces)
            {
                Destroy(piece.gameObject);
            }
            _pieces.Clear();
        }

        if (_pieces.Count == 0) SliceUIImage();

        StartCoroutine(WaitShuffle(0f));
        _miniGameManager.SetReset(false);
    }

    private void RandomSetImage()
    {
        int randomIndex = Random.Range(0, _gamePicture.Length);
        _currentSprite = _gamePicture[randomIndex];
        _slicePrefab.GetComponent<Image>().sprite = _currentSprite; 
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        if (_miniGameManager.ResetGame) ResetGame(); //reset game after on starting game
        //send out a ray to check if we click a piece
        if (_miniGameManager.Playing && !_shuffling && !_checkOriginal)
        {
            //counting timer
            if (!_playerWins) _timeCount += Time.deltaTime;
            _timeText.text = $"{(int)_timeCount / 60:D2}:{(int)_timeCount % 60:D2}";
            if (_timeCount >= 5999.9) EndGame();
        }
    }

    public void TrySwap(RectTransform piece)
    {
        if (_isMoving) return;

        //go through list, the index tells us the position.
        for (int i = 0; i < _pieces.Count; i++)
        {
            if (_pieces[i] == piece)
            {
                //check each direction to see if valid move.
                //we break out on success 
                if (AnimateSwap(i, -_size, _size)) { break; }
                if (AnimateSwap(i, +_size, _size)) { break; }
                if (AnimateSwap(i, -1, 0)) { break; }
                if (AnimateSwap(i, +1, _size - 1)) { break; } 
            }
        }
    }

    private void SliceUIImage()
    {
        if (_sourceImage == null || _targetParent == null || _slicePrefab == null)
        {
            Debug.LogWarning("Please assign sourceImage, targetParent, and slicePrefab in the Inspector.");
            return;
        }

        int gamepiece = 0;

        // Calculate the size of each slice
        float sliceWidth = _sourceImage.rectTransform.rect.width / _size;
        float sliceHeight = _sourceImage.rectTransform.rect.height / _size;
        float imageWidth = _targetParent.rect.width / _size;
        float imageHeight = _targetParent.rect.height / _size;

        // Loop through each row and column to slice the image
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                // Calculate position for the slice
                float xPos = col * sliceWidth;
                float yPos = row * sliceHeight;
                float xImagePos = col * imageWidth + (imageHeight * 0.5f);
                float yImagePos = row * imageHeight + (imageHeight * 0.5f);

                // Create a new slice GameObject
                GameObject slice = Instantiate(_slicePrefab, _targetParent);
                slice.gameObject.name = gamepiece.ToString();
                slice.GetComponent<PuzzlePiece>().SetManager(this);

                // Set the slice's image to the corresponding part of the source image
                Image sliceImage = slice.GetComponent<Image>();
                sliceImage.sprite = Sprite.Create(_sourceImage.sprite.texture,
                                                   new Rect(xPos, yPos, sliceWidth, sliceHeight),
                                                   new Vector2(0, 1f));

                // Set the slice's position and size
                RectTransform sliceRectTransform = slice.GetComponent<RectTransform>();
                sliceRectTransform.anchorMin = new Vector2(0f, 0f); // Anchor to top-left
                sliceRectTransform.anchorMax = new Vector2(0f, 0f); // Anchor to top-left
                sliceRectTransform.pivot = new Vector2(0.5f, 0.5f); // Pivot at top-left
                sliceRectTransform.sizeDelta = new Vector2(imageWidth - _borderSize, imageHeight - _borderSize);
                sliceRectTransform.anchoredPosition = new Vector2(xImagePos, yImagePos);
                sliceRectTransform.GetComponent<BoxCollider2D>().size = sliceRectTransform.sizeDelta;

                _pieces.Add(sliceRectTransform);
                gamepiece++;

                if (row == 0 && col == _size - 1)
                {
                    sliceRectTransform.gameObject.SetActive(false);
                    _emptyLocation = _pieces.Count - 1;
                }
                else
                {
                    sliceRectTransform.gameObject.SetActive(true);
                }
            }
        }
    }

    //colCheck is used to stop horizontal moves wrapping.
    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % _size) != colCheck) && ((i + offset) == _emptyLocation))
        {
            //swap them in game state
            (_pieces[i], _pieces[i + offset]) = (_pieces[i + offset], _pieces[i]);

            if (_playerControls.Movement.ClickMove.WasPressedThisFrame() || _playerControls.Movement.TouchMove.WasPerformedThisFrame()) StartCoroutine(Move(_pieces[i], _pieces[i + offset]));

            //swap their transforms
            (_pieces[i].localPosition, _pieces[i + offset].localPosition) = ((_pieces[i + offset].localPosition, _pieces[i].localPosition));
            //update empty location
            _emptyLocation = i;
            return true;
        }
        return false;
    }

    private bool AnimateSwap(int i, int offset, int colCheck)
    {
        if (((i % _size) != colCheck) && ((i + offset) == _emptyLocation))
        {
            //swap them in game state
            (_pieces[i], _pieces[i + offset]) = (_pieces[i + offset], _pieces[i]);

            StartCoroutine(Move(_pieces[i], _pieces[i + offset]));
            StartCoroutine(Move(_pieces[i + offset], _pieces[i]));

            //update empty location
            _emptyLocation = i;

            //check for completion once all tiles line up
            if (!_shuffling && CheckCompletion())
            {
                StartCoroutine(WinGame(1));
                _playerWins = true;
            }

            return true;
        }
        return false;
    }

    //named the pieces in order so we can use thisa check completion
    private bool CheckCompletion()
    {
        for (int i = 0; i < _pieces.Count; i++)
        {
            if (_pieces[i].name != $"{i}")
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator Move(Transform startPos, Transform endPos)
    {
        _isMoving = true;

        _soundManager.PlayAudioClip("Slide");

        float elapsedTime = 0;
        float duration = 0.3f;

        Vector2 start = startPos.localPosition;
        Vector2 end = endPos.localPosition;

        while (elapsedTime < duration)
        {

            startPos.localPosition = Vector2.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        startPos.localPosition = end;

        _isMoving = false;
    }

    private IEnumerator WinGame(float duration)
    {
        yield return new WaitForSeconds(duration);

        _miniGameManager.SetPlaying(false);
        _miniGameManager._win.Description = AppendString(_winText, _timeText.text);
        _confettiPS.Play();
        _winImage?.SetActive(true);

        yield return new WaitForSeconds(5);
        //_confettiPS.Stop();
        _winImage?.SetActive(false);
        EndGame();
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        _shuffling = false;
    }

    private void Shuffle()
    {
        int count = 0;
        int last = 3;
        while (count < (_size * _size * _size))
        {
            //pick a random location
            int random = Random.Range(0, _size * _size);
            //forbid undoing last move
            if (random == last) { continue; }
            last = _emptyLocation;
            //try surrounding spaces looking for valid move
            if (SwapIfValid(random, -_size, _size)) count++;
            else if (SwapIfValid(random, +_size, _size)) count++;
            else if (SwapIfValid(random, -1, 0)) count++;
            else if (SwapIfValid(random, +1, _size - 1)) count++;
        }
    }

    private string AppendString(string text, string append1)
    {
        text = text.Replace("{1}", append1);
        return text;
    }

    public void ShowOriginal()
    {
        _unscrambledImage.GetComponent<Image>().sprite = _currentSprite;
        _unscrambledPanel.SetActive(!_unscrambledPanel.activeSelf);
        _checkOriginal = !_checkOriginal;
    }

    public void EndGame()
    {
        StopAllCoroutines();
        _miniGameManager.EndGame(_playerWins);
    }
}
