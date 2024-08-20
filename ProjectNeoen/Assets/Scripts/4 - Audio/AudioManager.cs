using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    public List<Sound> PausedSounds;
    [SerializeField] private Coroutine _fade;
    public static AudioManager Instance;
    [SerializeField] private bool _mainWorld;
    [SerializeField] private bool _debugTesting;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        //else Destroy(gameObject);

        foreach (Sound sound in Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;

            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    private bool CheckMute() //ignore mute check if debugging game indivdually
    {
        if (_debugTesting) return false;
        if (GameManager.Instance.Mute) return true;
        else return false;
    }

    public bool IsAudioPlaying(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return false;
        }

        if (s.Source.isPlaying) return true;
        else return false;
    }

    public bool FindClip(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return false;
        }
        else return true;
    }

    public void Play(string name)
    {
        if (IsAudioPlaying(name) || CheckMute()) return;
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return;
        }
        if(!_mainWorld) s.Source.mute = false;
        s.Source.volume = s.Volume;
        s.Source.Play();
    }

    public void PlayAudioClip(string name)
    {
        Stop(name);
        Play(name);
    }

    public void Mute(string name)
    {
        if (CheckMute()) return;
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return;
        }
        s.Source.mute = !s.Source.mute;
    }

    public void MuteAudio(bool mute)
    {
        GameManager.Instance.Mute = mute;
        foreach (Sound sound in Sounds)
        {
            sound.Source.mute = mute;
        }
    }

    public void PauseAllAudio()
    {
        if (CheckMute()) return;
        foreach (Sound s in Sounds)
        {
            if (s.Pausable && IsAudioPlaying(s.Name))
            {
                Pause(s.Name);
                PausedSounds.Add(s);
            }
        }
    }

    public void UnPauseAllAudio()
    {
        if (CheckMute()) return;
        foreach (Sound s in PausedSounds)
        {
            UnPause(s.Name);
        }
        PausedSounds.Clear();
    }

    public void Pause(string name)
    {
        if (CheckMute()) return;
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return;
        }
        s.Source.Pause();
    }

    public void UnPause(string name)
    {
        if (CheckMute()) return;
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return;
        }
        s.Source.UnPause();
    }

    public void Stop(string name)
    {
        if (CheckMute()) return;
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.Log("Cannot find clip");
            return;
        }
        s.Source.Stop();
    }

    public void FadeOut(string soundName)
    {
        if (CheckMute()) return;
        Sound sound = Array.Find(Sounds, sound => sound.Name == soundName);
        if (sound == null)
        {
            Debug.Log("Sound: " + soundName + " not found");
            return;
        }
        _fade = StartCoroutine(StartFadeOut(sound.Source, 1f, 0));
    }

    public void FadeIn(string soundName, float fadeTime)
    {
        if (IsAudioPlaying(soundName) || CheckMute()) return;
        Sound sound = Array.Find(Sounds, sound => sound.Name == soundName);
        if (sound == null)
        {
            Debug.Log("Sound: " + soundName + " not found");
            return;
        }
        if(!_mainWorld) sound.Source.mute = false;
        _fade = StartCoroutine(StartFadeIn(sound.Source, fadeTime, sound.Volume));
    }

    public static IEnumerator StartFadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
        if (audioSource.isPlaying)
        {
            float currentTime = 0;
            float start = audioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            audioSource.Stop();
        }
    }

    public static IEnumerator StartFadeIn(AudioSource audioSource, float duration, float targetVolume)
    {
        audioSource.Play();
        if (audioSource.isPlaying)
        {
            float currentTime = 0;
            audioSource.volume = 0;
            while (currentTime < duration)
            {
                currentTime += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0, targetVolume, currentTime / duration);
                yield return null;
            }
        }
    }
}
