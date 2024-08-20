using UnityEngine;


public class MiniGameNPC : MonoBehaviour
{
    [field: SerializeField] public SceneName SceneToLoad { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Location { get; private set; }
    [field: SerializeField] public string TradingCardID { get; private set; }
    //[field: SerializeField] public Sprite FrontSprite { get; private set; }
    //[field: SerializeField] public Sprite BackSprite { get; private set; }
    [field: SerializeField] public TradingCardLayout TradingCardData { get; private set; }
    [field: SerializeField] public NPCDialogue PreObjectiveDialogue { get; private set; }
    [field: SerializeField] public NPCDialogue PostObjectiveDialogue { get; private set; }
    [field: SerializeField] public NPCDialogue PreMiniGameDialogue { get; private set; }
    [field: SerializeField] public NPCDialogue PostMiniGameDialogue { get; private set; }
    [field: SerializeField] public bool ObjectiveNPC { get; private set; }
    [field: SerializeField] public bool FuturevilleNPC { get; private set; }
    public void SetObjectiveNPC() => ObjectiveNPC = true;
    public bool ShowTradingCard = true;
    public enum SceneName
    {
        ElectronGrab,
        MaximumPower,
        Scrambled,
        CommunityChaos,
        BossBattle,
        PickinTeams,
        None
    }
}
