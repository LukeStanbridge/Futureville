using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioManager _soundManager;
    [SerializeField] private float _triggerTime;
    [SerializeField] private float _timer;
    [SerializeField] private string _clipName;
    [SerializeField] private bool _audioTriggered = false;

    private void Update()
    {
        if (_audioTriggered) return;
        _timer += Time.unscaledDeltaTime;
        if (_timer >= _triggerTime)
        {
            _soundManager.PlayAudioClip(_clipName);
            _audioTriggered = true;
            _timer = 0;
        }
    }
}
