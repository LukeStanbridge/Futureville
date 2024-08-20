using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float _zoomTime;
    [SerializeField] private float _endZoom;
    [SerializeField] private float _startZoom;
    [SerializeField] private CinemachineVirtualCamera _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
        _camera.m_Lens.OrthographicSize = _startZoom;
    }
    
    public void ZoomIn()
    {
        StartCoroutine(Zoom());
    }

    private IEnumerator Zoom()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _zoomTime)
        {
            elapsedTime += Time.deltaTime;
            _camera.m_Lens.OrthographicSize = Mathf.Lerp(_startZoom, _endZoom, elapsedTime / _zoomTime);
            yield return null;
        }

        _camera.m_Lens.OrthographicSize = _endZoom;
        GameManager.Instance.UpdateGameState(GameState.Futureville);
        yield return null;
    }
}
