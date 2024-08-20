using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)] public float Volume;
    [Range(0f, 4f)] public float Pitch;

    public bool Loop;
    public bool Pausable;

    [HideInInspector]
    public AudioSource Source;
}
