using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayorIntro : MonoBehaviour
{
    [field: SerializeField] public NPCDialogue GameIntro { get; private set; }
    public bool OpenConvo = true;
}
