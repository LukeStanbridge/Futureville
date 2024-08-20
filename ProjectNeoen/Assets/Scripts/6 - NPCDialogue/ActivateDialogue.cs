using UnityEngine;
using UnityEngine.UI;

public class ActivateDialogue : MonoBehaviour
{
    [SerializeField] private Button _talkIndicator;
    [SerializeField] private MiniGameNPC _miniGameNPC;
    [SerializeField] private TradingCardNPC _tradingCardNPC;
    [field: SerializeField] public bool MiniGameNPC { get; private set; }
    [field: SerializeField] public bool TradingCardNPC { get; private set; }

    public NPCDialogueManager DialogueManager;
    public bool CanClickNPC;

    private void Awake()
    {
        _talkIndicator.gameObject.SetActive(false);
        CanClickNPC = false;

        if (TryGetComponent<MiniGameNPC>(out MiniGameNPC miniGameNPC))
        {
            _miniGameNPC = miniGameNPC;
            MiniGameNPC = true;
        }

        if (TryGetComponent<TradingCardNPC>(out TradingCardNPC tradingCardNPC))
        {
            _tradingCardNPC = tradingCardNPC;
            TradingCardNPC = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.State != GameState.Futureville) return;
        if (collision.gameObject.tag == "Player")
        {
            CanClickNPC = true;
            if (!DialogueManager.DialogueOpen) DialogueManager.ActivateButton(this, _talkIndicator);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CanClickNPC = false;
            DialogueManager.DeactivateButton(_talkIndicator);
        }
    }

    public void OpeningConvo()
    {
        CanClickNPC = true;
        DialogueManager.ActivateButton(this, _talkIndicator);
    }
}
