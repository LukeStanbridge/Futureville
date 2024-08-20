using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string Name;
    public DialogueRound[] Round;

    [Serializable]
    public class DialogueRound
    {
        public int [] Weighting;
        [TextArea(3,10)]
        public string[] Responses;
    }
}
