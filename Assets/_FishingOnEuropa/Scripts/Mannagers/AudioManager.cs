using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1;
        [Range(-3, 3)]
        public float pitch = 1;
        public bool loop = false;
        public bool playOnAwake = false;
        public AudioSource source;

        public Sound()
        {
            volume = 1;
            pitch = 1;
            loop = false;
        }
    }

    public class AudioManager : Singleton<AudioManager>
    {
        public Sound[] sounds;

        private void Start()
        {
            foreach (Sound s in sounds)
            {
                if (!s.source)
                    s.source = gameObject.AddComponent<AudioSource>();

                s.source.clip = s.clip;
                s.source.playOnAwake = s.playOnAwake;
                if (s.playOnAwake)
                    s.source.Play();

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found");
                return;
            }

            s.source.Play();
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            s.source.Stop();
        }


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
            _source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            _source.volume = _volume;
            _source.Play();
        }
    }
}

