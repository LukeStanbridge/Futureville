using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue BossDialogue;
    public Dialogue PlayerDialogue;
    private BossBattleManager _manager;

    private void Awake()
    {
        _manager = GetComponent<BossBattleManager>();
    }

    public void TriggerDialogue()
    {
        _manager.StartDialogue(BossDialogue, PlayerDialogue);
    }
}
