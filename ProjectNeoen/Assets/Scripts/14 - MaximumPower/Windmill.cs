using DG.Tweening;
using UnityEngine;

public class Windmill : MonoBehaviour
{
    [field: SerializeField] private MaximumPowerManager _maximumPowerManager;
    [SerializeField] private float _time;
    [SerializeField] private GameObject _blades;
    private Tween _spinningBlades;
    private Vector3 _bladeRotation;

    private void Awake()
    {
        _maximumPowerManager = FindObjectOfType<MaximumPowerManager>();
        _bladeRotation = _blades.transform.rotation.eulerAngles;
    }

    public void SnapToTile()
    {
        if (_maximumPowerManager._tileSelected == null) return;
        this.gameObject.transform.position = _maximumPowerManager._tileSelected.GetComponent<PolygonCollider2D>().bounds.center;
        //_maximumPowerManager.DetachTurbine();
    }

    public void SpinTurbine() //start spining turbine blades
    {
        _spinningBlades = _blades.transform.DORotate(new Vector3(0, 0, 360), _time, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetRelative()
            .SetEase(Ease.Linear);
    }

    public void StopTurbine() //stop spining and reset position
    {
        _spinningBlades.Kill();
        _blades.transform.rotation = Quaternion.Euler(_bladeRotation);
    }

    public void SetTurbineSpeed(float time)
    {
        _time = time;
    }
}
 
