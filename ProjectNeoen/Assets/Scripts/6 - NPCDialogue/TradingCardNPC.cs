using UnityEngine;
using UnityEngine.UI;

public class TradingCardNPC : MonoBehaviour
{
    [field: SerializeField] public Sprite FrontSprite { get; private set; }
    [field: SerializeField] public Sprite BackSprite { get; private set; } 
    [field: SerializeField] public NPCDialogue FindNPCDialogue { get; private set; }
    [field: SerializeField] public NPCDialogue NonObjectiveDialogue { get; private set; }

    public bool ShowTradingCard = true;
}
