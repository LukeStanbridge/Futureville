using TMPro;
using UnityEngine;

public class ResponseOption : MonoBehaviour
{
    [SerializeField] private int _weightValue;
    public int GetResponseWeight() { return _weightValue; }
    private TextMeshProUGUI _responseText;

    private void Awake()
    {
        _responseText = GetComponent<TextMeshProUGUI>();
    }

    public void SetResponse(string dialogue, int responseWeight)
    {
        _responseText.text = dialogue;
        _weightValue = responseWeight;
    }

    public void ClearText()
    {
        _responseText.text = "";
    }
}
