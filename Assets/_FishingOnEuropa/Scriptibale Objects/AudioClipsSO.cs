using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Europa/Audio", order = 1)]

    public class AudioClipsSO : ScriptableObject
    {
        //For every clip in the list, there will be an enum version to make it easier when calling the clip
        public enum ClipEnum { ButtonClick}
        public enum BackgroundMusicClips { Valley}
        public List<AudioClip> audioClips;

        public AudioClip GetBGMusicClipFromEnum(BackgroundMusicClips clipEnum)
        {
            AudioClip clip = null;

            foreach (AudioClip _clip in audioClips)
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

            foreach (AudioClip _clip in audioClips)
            {
                if (_clip.name.Contains(clipEnum.ToString()))
                {
                    clip = _clip;
                    break;
                }
            }

            return clip;
        }
    }
}
