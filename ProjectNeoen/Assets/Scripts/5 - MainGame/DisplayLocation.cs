using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLocation : MonoBehaviour
{
    [SerializeField] private SceneTransitionManager _transitionManager;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private RectTransform _locationTransform;
    [SerializeField] private float _duration;
    private Color _startColour = new Color(0, 0, 0, 0);
    private Color _textStartColour = new Color(0, 0, 0, 0);
    private Color _endColour = new Color(0, 0, 0, 1);
    private Color _textEndColour = new Color(0, 0, 0, 1);

    private bool _fadeIn;
    private bool _displayingText;
    private Image _locationBackground;

    private void Awake()
    {
        _locationTransform.gameObject?.SetActive(false);
    }

    public void DisplayLocationText(string location, Color backgroundColour, Vector2 size)
    {
        if (_transitionManager.Transitioning || _displayingText) return;
        _displayingText = true;
        _locationTransform.gameObject?.SetActive(true);
        _locationTransform.sizeDelta = size;
        _locationBackground = _locationTransform.GetComponent<Image>();
        _locationBackground.color = backgroundColour;
        _locationText = _locationTransform.GetComponentInChildren<TextMeshProUGUI>();
        _locationText.text = location;
        _startColour = new Color(backgroundColour.r, backgroundColour.g, backgroundColour.b, 0);
        _endColour = backgroundColour;
        _fadeIn = true;
        StartCoroutine(ShowText(_startColour, _endColour, _textStartColour, _textEndColour));
    }

    private IEnumerator ShowText(Color start, Color end, Color textStart, Color textEnd)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _locationBackground.color = Color.Lerp(start, end, elapsedTime / _duration);
            _locationText.color = Color.Lerp(textStart, textEnd, elapsedTime / _duration);
            yield return null;
        }

        _locationBackground.color = end;

        yield return new WaitForSecondsRealtime(1);

        if (_fadeIn)
        {
            StartCoroutine(ShowText(end, start, textEnd, textStart));
            _fadeIn = false;
        }
        else
        {
            _locationTransform?.gameObject?.SetActive(false);
            _displayingText = false;
        }

        yield return null;
    }
}
