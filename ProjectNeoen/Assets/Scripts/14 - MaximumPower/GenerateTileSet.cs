using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateTileSet : MonoBehaviour
{
    private int[,] _treeData =
    {
        { 0 , 0 , 0 , 0 , 0 , 75, 75, 75, 75, 75 },
        { 0 , 0 , 0 , 0 , 0 , 75, 75, 75, 75, 75 },
        { 0 , 0 , 0 , 0 , 0 , 75, 75, 75, 75, 75 },
        { 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0  },
        { 25, 25, 25, 25, 50, 50, 50, 50, 50, 50 },
        { 25, 25, 0 , 25, 25, 50, 50, 50, 50, 50 },
        { 25, 25, 25, 25, 25, 25, 50, 50, 50, 50 },
        { 25, 0 , 25, 0 , 25, 25, 25, 50, 50, 50 },
        { 25, 25, 25, 25, 25, 25, 25, 50, 50, 50 },
        { 25, 25, 0 , 25, 25, 0 , 25, 50, 50, 0  },
        { 25, 25, 25, 25, 25, 25, 25, 25, 25, 25 },
    };

    private int[,] _windSpeedData =
    {
        { 50, 50 , 50 , 50, 40, 30, 20, 20, 10, 10 },
        { 50, 50 , 50 , 50, 50, 40, 30, 20, 20, 10 },
        { 60, 60 , 60 , 50, 50, 50, 40, 30, 20, 20 },
        { 70, 70 , 70 , 70, 70, 60, 50, 40, 30, 20 },
        { 80, 80 , 80 , 80, 70, 60, 50, 40, 40, 30 },
        { 80, 90 , 80 , 80, 80, 70, 60, 50, 40, 40 },
        { 90, 100, 100, 90, 80, 80, 70, 60, 50, 40 },
        { 80, 80 , 90 , 80, 80, 80, 70, 70, 60, 50 },
        { 80, 80 , 80 , 80, 80, 80, 70, 70, 60, 50 },
        { 70, 80 , 80 , 80, 80, 80, 80, 70, 70, 60 },
        { 10, 10 , 10 , 10, 10, 10, 10, 10, 10, 10 },
    };

    private int[,] _elevationData =
    {
        { 100, 100, 100, 100, 100, 150, 150, 150, 150, 150 },
        { 100, 100, 100, 100, 100, 150, 150, 150, 150, 150 },
        { 100, 100, 100, 100, 100, 150, 150, 150, 150, 150 },
        { 150, 200, 150, 100, 100, 150, 150, 150, 150, 150 },
        { 350, 400, 350, 250, 100, 100, 100, 100, 100, 100 },
        { 250, 300, 300, 300, 250, 100, 100, 100, 100, 100 },
        { 400, 450, 500, 500, 450, 400, 350, 100, 100, 100 },
        { 350, 400, 450, 450, 400, 350, 300, 50 , 50 , 50  },
        { 300, 350, 400, 400, 350, 300, 300, 250, 50 , 50  },
        { 200, 200, 300, 300, 200, 200, 200, 150, 100, 50  },
        { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
    };

    private int[,] _protectedSpeciesData = 
    {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 5, 5 },
        { 0, 0, 0, 0, 0, 0, 0, 5, 5, 10 },
        { 0, 0, 0, 0, 0, 0, 5, 5, 10, 10 },
        { 0, 0, 0, 0, 0, 5, 5, 10, 10, 10 },
        { 0, 0, 0, 0, 0, 5, 10, 10, 10, 10 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    };

    private int[,] _startingTurbineData =
    {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 0, 0, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    };

    [SerializeField] private MaximumPowerManager _maximumPowerManager;
    [SerializeField] private AudioManager _audioManager;

    [Header("TileSet Generation")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private Transform _parent;
    [SerializeField] private float _elevationModifier;
    [SerializeField] public int ProductiveTurbines {  get; private set; }
    [SerializeField] public float TotalWindGeneration { get; private set; }
    [SerializeField] private List<Sprite> _trees;

    private void Awake()
    {
        foreach (Transform child in this.transform)
        {
            child.GetComponent<TileLogic>().SetAudioManager(_audioManager);
        }
    }

    public void GenerateTiles()
    {
        int listIndex = 0;
        foreach (Transform child in this.transform)
        {
            _maximumPowerManager.Tiles.Add(child.gameObject);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _maximumPowerManager.SetTile(_maximumPowerManager.Tiles[listIndex]);

                //Assign tile data to each tile
                TileStats stats = _maximumPowerManager.Tiles[listIndex].GetComponent<TileStats>();
                stats.TreeHeight = _treeData[j, i];
                stats.OriginalTreeHeight = _treeData[j, i];
                stats.WindSpeed = _windSpeedData[j, i];
                stats.Elevation = _elevationData[j, i];
                stats.ProtectedSpecies = _protectedSpeciesData[j, i];
                stats.ListIndex = listIndex;
                stats.HillsWakeEffectPercent = 0;
                stats.TreesWakeEffectPercent = 0;
                stats.TurbineWakeEffectPercent = 0;
                stats.TurbulencePercent = 0;
                stats.EnviroImpactPercent = 0;
                stats.WindGeneration = 0;

                stats.GetComponent<PolygonFiller>().HideColour();

                if (_startingTurbineData[j, i] == 1)
                {
                    stats.DefaultTurbine = true;
                    SetStartingTurbine(_maximumPowerManager.Tiles[listIndex].GetComponent<TileLogic>());
                }
                listIndex++;
            }
        }

        foreach(GameObject tile in _maximumPowerManager.Tiles) //add trees to tiles
        {
            /*if (_maximumPowerManager._ignoredTiles.Contains(tile)) continue;*/ //check if end tile
            if (tile.GetComponent<TileStats>().DefaultTurbine) continue; //check if turbine exists
            TileStats stats = tile.GetComponent<TileStats>();
            if (stats.TreeHeight == 25) AddTree(stats, 0);
            else if (stats.TreeHeight == 50) AddTree(stats, 1);
            else if (stats.TreeHeight == 75) AddTree(stats, 2);
            
        }
        CalculateTurbulence();
        CalculateEnviromental();
    }

    public void AddTree(TileStats tile, int index)
    {
        GameObject tree = new GameObject("Tree");
        
        tree.gameObject.AddComponent<SpriteRenderer>();
        tree.transform.SetParent(tile.transform, false);
        tree.transform.position = tile.GetComponent<PolygonCollider2D>().bounds.center;
        SpriteRenderer sprite = tree.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 1;
        sprite.spriteSortPoint = SpriteSortPoint.Pivot;
        sprite.sprite = _trees[index];
        //tree.transform.localScale = Vector3.one;
        if (!_maximumPowerManager._ignoredTiles.Contains(tile.gameObject)) _maximumPowerManager.Trees.Add(tree);
    }

    public void CutTree(TileLogic tile, int treeHeight)
    {
        //if (tile.transform.childCount != 1) return;
        if (treeHeight == 0)
        {
            _maximumPowerManager.RemoveTree(tile.transform.GetChild(0).gameObject);
            tile.GetComponent<PolygonFiller>().DisplayDirt();
            if (_maximumPowerManager.UndoingSpace) _audioManager.Play("Undo");
            Destroy(tile.transform.GetChild(0).gameObject);    
            return;
        }
        int index = ((int)treeHeight / 25) - 1;
        SpriteRenderer sprite = tile.gameObject.GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = _trees[index];
    }

    private void SetStartingTurbine(TileLogic tile)
    {
        tile.SetStarterTurbine();
    }

    private void CalculateTurbulence()
    {
        foreach (GameObject tile in _maximumPowerManager.Tiles)
        {
            TileStats downstream = tile.GetComponentInParent<TileStats>();

            if ((downstream.ListIndex + 1) % 11 == 0) continue; //skip if end piece

            TileStats upstream = _maximumPowerManager.Tiles[downstream.ListIndex + 1].GetComponent<TileStats>(); //get next tile in list for data comparison

            //set value if elevation on upstream tile is higher that downstream tile
            if (upstream.Elevation > downstream.Elevation) downstream.HillsWakeEffectPercent = (upstream.Elevation - downstream.Elevation) / 5; 
            else downstream.HillsWakeEffectPercent = 0;

            if (upstream.TreeHeight == 25) downstream.TreesWakeEffectPercent = 5;
            else if (upstream.TreeHeight == 50) downstream.TreesWakeEffectPercent = 10;
            else if (upstream.TreeHeight == 75) downstream.TreesWakeEffectPercent = 15;
            else downstream.TreesWakeEffectPercent = 0;
        }
    }

    private void CalculateEnviromental()
    {
        foreach ( GameObject tile in _maximumPowerManager.Tiles)
        {
            TileStats stats = tile.GetComponentInParent<TileStats>();
            if (stats.ProtectedSpecies == 10)
            {
                stats.EnviroImpactPercent = 100;
            }
            else if (stats.ProtectedSpecies == 5)
            {
                stats.EnviroImpactPercent = 50;
            }
        }
    }

    public void AdjustTreeWakeEffect(TileStats upstream)
    {
        if (upstream.ListIndex % 11 == 0) return;

        TileStats downstream = _maximumPowerManager.Tiles[upstream.ListIndex - 1].GetComponent<TileStats>(); //get next tile in list for data comparison

        if (upstream.TreeHeight == 25) downstream.TreesWakeEffectPercent = 5;
        else if (upstream.TreeHeight == 50) downstream.TreesWakeEffectPercent = 10;
        else if (upstream.TreeHeight == 75) downstream.TreesWakeEffectPercent = 15;
        else downstream.TreesWakeEffectPercent = 0;
    }

    public void AdjustTurbineWakeEffect(TileStats stats)
    {
        TileStats selected = stats;
        if (selected.ListIndex == 0) return;
        TileStats downwind = _maximumPowerManager.Tiles[selected.ListIndex - 1].GetComponent<TileStats>();
        if (selected.ListIndex > 0 && selected.HasTurbine) downwind.TurbineWakeEffectPercent = 100;
        else downwind.TurbineWakeEffectPercent = 0;
    }

    public void CalcTotalTurbulence()
    {
        foreach (GameObject tile in _maximumPowerManager.Tiles)
        {
            TileStats stats = tile.GetComponent<TileStats>();
            stats.TurbulencePercent = stats.HillsWakeEffectPercent + stats.TreesWakeEffectPercent + stats.TurbineWakeEffectPercent;
        }
    }

    public void CalcWindSpeed()
    {
        foreach (GameObject tile in _maximumPowerManager.Tiles)
        {
            TileStats stats = tile.GetComponent<TileStats>();
            
            if (stats.WindSpeed >= 90 || stats.WindSpeed <= 20) stats.WindSpeed = 0;
        }
    }

    public void SimulateWindGeneration()
    {
        TotalWindGeneration = 0;
        ProductiveTurbines = 0;

        foreach (GameObject turbine in _maximumPowerManager._placedTurbines)
        {
            TileStats stats = turbine.GetComponentInParent<TileStats>();

            if (stats.TurbulencePercent >= 100) continue; //ignore turbine in final calculation if turbulence is above 100%
            if (stats.TurbulencePercent < 0) continue; //ignore turbine in final calculation if turbulence is below 0%
            if (stats.WindSpeed == 0) continue;

            float windGeneration = stats.WindSpeed - (stats.WindSpeed * (((float)stats.EnviroImpactPercent + (float)stats.TurbulencePercent) / 100.0f));
            if (windGeneration <= 20) continue; //check for updated wind speed after calcs

            stats.WindGeneration = (windGeneration / 80.0f) * 100;

            if (stats.WindGeneration < 0) stats.WindGeneration = 0;
            else ProductiveTurbines++;

            TotalWindGeneration += (stats.WindGeneration / 100.0f);
        }
    }
}
