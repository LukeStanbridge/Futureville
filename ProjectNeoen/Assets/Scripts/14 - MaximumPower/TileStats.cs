using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStats : MonoBehaviour
{
    [SerializeField] private List<Sprite> treeSprites;

    public int TreeHeight;
    public int OriginalTreeHeight;
    public int WindSpeed;
    public int Elevation;
    public int ProtectedSpecies;
    public bool DefaultTurbine;
    public bool HasTurbine;
    public bool Water;
    public bool StarterDirt;
    public int ListIndex;

    public int HillsWakeEffectPercent;
    public int TreesWakeEffectPercent;
    public int TurbineWakeEffectPercent;

    public int TurbulencePercent;
    public int EnviroImpactPercent;

    public float WindGeneration;
}
