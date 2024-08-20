using UnityEngine;
using UnityEngine.UI;

public class TileLogic : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _selected;
    private Color _normal;
    private MaximumPowerManager _maximumPowerManager;
    [SerializeField] private AudioManager _audioManager;
    public bool _selectedTile { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _maximumPowerManager = FindObjectOfType<MaximumPowerManager>();
    }

    public void SetAudioManager(AudioManager audioManager) { _audioManager = audioManager; }

    private void OnMouseDown()
    {
        if (!_maximumPowerManager.MiniGameUIManager.Playing) return;
        if (_maximumPowerManager._ignoredTiles.Contains(this.gameObject)) return;
        if (_maximumPowerManager._statsDisplayed) return;

        _maximumPowerManager.SetTile(this.gameObject);

        if (_maximumPowerManager.PlacingTurbine) //placing turbine
        {
            //if (_maximumPowerManager._turbineCounter <= 0) return;
            if (_maximumPowerManager._tileSelected.transform.childCount > 0) return; // check if turbine already placed
            if (_maximumPowerManager.TurbineCount >= 10) return; 
            GameObject turbine = Instantiate(_maximumPowerManager.TurbinePrefab);
            _maximumPowerManager.CurrentTurbine = turbine.GetComponent<Windmill>();
            _audioManager.Play("Thud");
            PlaceTurbine(_maximumPowerManager.CurrentTurbine);
        }
        else if (_maximumPowerManager.ClearTurbine) //clearing placed turbine
        {
            RemoveTurbine();
        }
        else if (_maximumPowerManager.PlaceTree) //planting tree to empty tile
        {
            PlantTree();
        }
        else if (_maximumPowerManager.ClearingSpace) //clearing planted tree
        {
            ClearTree();
        }
        else if (_maximumPowerManager.UndoingSpace) //reset tile to original state
        {
            UndoSpace();
        }

        if (_maximumPowerManager._tileSelected == null) return;
    }

    public void SetStarterTurbine()
    {
        GameObject turbine = Instantiate(_maximumPowerManager.TurbinePrefab);
        _maximumPowerManager.CurrentTurbine = turbine.GetComponent<Windmill>();
        ClearTrees(true);
        PlaceTurbine(_maximumPowerManager.CurrentTurbine);
    }

    private void PlaceTurbine(Windmill currentTurbine)
    {
        if (currentTurbine == null) return; // check for already spawned turbine

        currentTurbine.gameObject.transform.SetParent(this.transform, false);

        TileStats stats = currentTurbine.GetComponentInParent<TileStats>();
        if (stats.Water) return;
        currentTurbine.gameObject.layer = 2; //ignore this object with mouse
        currentTurbine.SnapToTile();
        stats.HasTurbine = true;


        _maximumPowerManager.AddTurbine(currentTurbine.gameObject, stats);
        if(!_maximumPowerManager.UndoingSpace) _maximumPowerManager.SetTile(null);
        _maximumPowerManager.DetachTurbine();

        currentTurbine.GetComponentInParent<PolygonFiller>().DisplayDirt();
    }

    private void RemoveTurbine()
    {
        if (_maximumPowerManager._tileSelected == null) return;

        if (_maximumPowerManager._tileSelected.transform.GetComponentInChildren<Windmill>()) //remove turbine if ther is one on the selected tile
        {
            Windmill turbine = _maximumPowerManager._tileSelected.transform.GetComponentInChildren<Windmill>();
            TileStats stats = turbine.GetComponentInParent<TileStats>();
            stats.HasTurbine = false;
            _maximumPowerManager.RemoveTurbine(turbine.gameObject, stats);
            _audioManager.Play("Remove"); //get new audio
            Destroy(turbine.gameObject);
        }
    }

    private void PlantTree()
    {
        if (_maximumPowerManager._tileSelected == null) return;
        if (this.transform.childCount > 0) return;

        TileStats stats = _maximumPowerManager._tileSelected.GetComponent<TileStats>();
        if (stats.Water) return;

        stats.TreeHeight = 25;
        _maximumPowerManager._generateTileSet.AddTree(stats, 0);
        _maximumPowerManager._generateTileSet.AdjustTreeWakeEffect(stats);
        
        if (stats.StarterDirt/* || stats.DefaultTurbine*/) stats.GetComponent<PolygonFiller>().DisplayGrass();
        else stats.GetComponent<PolygonFiller>().HideColour();
        _audioManager.Play("Plant"); //get new audio
    }

    private void ClearTree()
    {
        if (_maximumPowerManager._tileSelected == null) return; //do nothing if not hovering over tile

        if (_maximumPowerManager._tileSelected.GetComponent<TileStats>().TreeHeight > 0) //decrease tree height if there are trees on tile
        {
            ClearTrees(false);
            _audioManager.Play("Saw");
        }
    }

    private void ClearTrees(bool isStarterTurbine)
    {
        TileStats stats = _maximumPowerManager._tileSelected.GetComponent<TileStats>();

        if (isStarterTurbine || _maximumPowerManager.PlacingTurbine) stats.TreeHeight = 0;
        else
        {
            stats.TreeHeight -= 25;
            _maximumPowerManager._generateTileSet.CutTree(this, stats.TreeHeight);
        }
        if (!isStarterTurbine) _maximumPowerManager._generateTileSet.AdjustTreeWakeEffect(stats);
    }

    private void UndoSpace()
    {
        if (_maximumPowerManager._tileSelected == null) return;
        
        if (this.transform.GetComponentInChildren<Windmill>())
        {
            if (_maximumPowerManager._tileSelected.GetComponent<TileStats>().DefaultTurbine) return;
            Windmill turbine = _maximumPowerManager._tileSelected.transform.GetComponentInChildren<Windmill>();
            
            turbine.GetComponentInParent<PolygonFiller>().HideColour();
            TileStats tileStats = turbine.GetComponentInParent<TileStats>();
            tileStats.HasTurbine = false;
            _maximumPowerManager.RemoveTurbine(turbine.gameObject, tileStats);
            _audioManager.Play("Undo");
            Destroy(turbine.gameObject);
        }

        if (_maximumPowerManager._tileSelected.GetComponent<TileStats>().DefaultTurbine)
        {
            SetStarterTurbine();
            _audioManager.Play("Undo");
        }

        TileStats stats = _maximumPowerManager._tileSelected.GetComponent<TileStats>();
        stats.TreeHeight = stats.OriginalTreeHeight;

        if (this.transform.childCount > 0 && this.transform.GetChild(0).name == "Tree")
        {
            _maximumPowerManager._generateTileSet.CutTree(this, stats.TreeHeight);
        }
        else
        {
            if (stats.OriginalTreeHeight < 25) return;

            int index = (stats.TreeHeight / 25) - 1;
            _maximumPowerManager._generateTileSet.AddTree(stats, index);
            _maximumPowerManager._generateTileSet.AdjustTreeWakeEffect(stats);
            _audioManager.Play("Undo");
            stats.GetComponent<PolygonFiller>().HideColour();
        }
    }
}
