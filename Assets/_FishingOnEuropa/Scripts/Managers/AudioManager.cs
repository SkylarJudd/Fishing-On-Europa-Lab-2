using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Europa.AudioClipsSO;

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

        public List<GameObject> audioSourcePool;

        [SerializeField]
        AudioSource bgMusicAudioSource;

        public AudioClipsSO AudioClipsSO;
        

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
        public AudioClip GetBGMusicClipFromEnum(BackgroundMusicClips clipEnum)
        {
            AudioClip clip = null;

            foreach (AudioClip _clip in AudioClipsSO.audioClips)
            {
                if (_clip.name.Contains(clipEnum.ToString()))
                {
                    clip = _clip;
                    break;
                }
            }

            return clip;
        }

        public AudioClip GetClipFromEnum(ClipEnum clipEnum)
        {
            AudioClip clip = null;

            foreach (AudioClip _clip in AudioClipsSO.audioClips)
            {
                if (_clip.name.Contains(clipEnum.ToString()))
                {
                    clip = _clip;
                    break;
                }
            }

            return clip;
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
        /// Change background music
        /// </summary>
        /// <param name="_clip"></param>
        public void ChangeBackgroundMusic(AudioClipsSO.BackgroundMusicClips _clip )
        {
            bgMusicAudioSource.clip = GetBGMusicClipFromEnum(_clip);

        }

        /// <summary>
        /// Move audio source game object from pool to location and play audio
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="position"></param>
        /// <param name="isChild"> If you want audio to be child of param parent</param>
        /// <param name="parent"></param>
        public void PlaySoundAtLocation(AudioClipsSO.ClipEnum _clip, Vector3 position, bool isChild, Transform parent)
        {
            //get free audio
            var audioSource = audioSourcePool.FirstOrDefault();

            AudioClip clip = AudioClipsSO.GetClipFromEnum(_clip);

            //remove from list
            audioSourcePool.Remove(audioSource);

            //move to position
            if(isChild)
            {
                audioSource.transform.parent = parent;
                audioSource.transform.position = Vector3.zero;
            }
            else
                audioSource.transform.position = position;

            PlaySound(clip, audioSource.GetComponent<AudioSource>(), 1);
            ExecuteAfterSeconds(clip.length, () => AddAudioSourceToPool(audioSource));

        }

        

        /// <summary>
        /// Return audio source gameobject to object pool
        /// </summary>
        /// <param name="_audioSource">Audio source game object</param>
        void AddAudioSourceToPool(GameObject _audioSource)
        {
            if(!audioSourcePool.Contains(_audioSource))
                audioSourcePool.Add(_audioSource);

            //reset location
            if(_audioSource.transform.parent != gameObject.transform)
                _audioSource.transform.parent = gameObject.transform;

            _audioSource.transform.position = Vector3.zero;
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

