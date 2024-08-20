using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Electron : MonoBehaviour
{
    
    [SerializeField] private ElectronGrabManager _electronGrabManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private float _gravityMultiplier;
    [SerializeField] private float _scoreValue;
    [SerializeField] private float _missValue;
    [SerializeField] private ParticleSystem _playerHit;
    [SerializeField] private ParticleSystem _electronTrail;

    private Rigidbody2D _electronRB;
    private Vector2 _screenBounds;

    private void Awake()
    {
        _electronGrabManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ElectronGrabManager>();
        _screenBounds = _electronGrabManager.ScreenBounds();
        Instantiate(_electronTrail, this.transform.position, this.transform.rotation, this.transform); //create trail
        _electronRB = GetComponent<Rigidbody2D>();
        _electronRB.gravityScale = 0.5f + (_electronGrabManager.GameQuarter * _gravityMultiplier); //set speed
    }

    public void SetAudioManager(AudioManager audioManager) 
    { 
        _audioManager = audioManager;
        _audioManager.PlayAudioClip("ElectronSpawn");
    }

    private void FixedUpdate() 
    {
        if (transform.localPosition.y <= -_electronGrabManager.GameBounds.GetComponent<RectTransform>().sizeDelta.y - 1) //subtract score and destroy object when player misses electron
        {
            _electronGrabManager.AddScore(_missValue);
            _audioManager.PlayAudioClip("ElectronMiss");
            Destroy(this.gameObject); 
        }
        if (_electronGrabManager.GameOver) Destroy(this.gameObject); //destroy object when game finishes
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") //add score and instatatiate effects when electron collides with player
        {
            if (_electronGrabManager.Malfunction) return;
            if (collision.GetComponent<GrabberMovement>() == null) return;
            _electronGrabManager.AddScore(_scoreValue);
            Instantiate(_playerHit, this.transform.position, this.transform.rotation);
            GrabberMovement player = collision.GetComponent<GrabberMovement>();
            player.PulsePlayer();
            player.ColorChangePlayer();
            _audioManager.PlayAudioClip("ElectronHit");
            Destroy(this.gameObject);
        }
    }
}
