using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    /// <summary>
    /// Plays an Audio Clip with adjusted pitch value 
    /// </summary>
    /// <param name="_clip">The clip to play</param>
    /// <param name="_source">the audio source to play on</param>
    public void PlaySound(AudioClip _clip, AudioSource _source, float _volume = 1)
    {
        if (_source == null || _clip == null)
            return;

        _source.clip = _clip;
        _source.pitch = Random.Range(0.8f, 1.2f);
        _source.volume = _volume;
        _source.Play();
    }
}
