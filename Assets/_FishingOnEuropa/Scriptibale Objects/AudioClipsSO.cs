using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Europa/Audio", order = 1)]

    public class AudioClipsSO : ScriptableObject
    {
        //For every clip in the list, there will be an enum version to make it easier when calling the clip
        public enum ClipEnum { ButtonClick, PlayerItemPickup, GravelStep, GrassStep, StoneStep,
            HybridCapture, LineBreak, LineThrow, LureDrop, ReelFast, ReelNormal, HybridSwimming_Swim,
            HybridSwimming_Dive, HybridSwimming_Submerge, BigSplash, LongSplash, SmallSplash, TinySplash,
            TinierSplash
        }
        public enum BackgroundMusicClips { Valley}
        public enum WalkingAudio { Grass, Stone, Gravel}
        public List<AudioClip> audioClips;
        public List<AudioClip> audioClips_grassWalk;
        public List<AudioClip> audioClips_gravelWalk;
        public List<AudioClip> audioClips_stoneWalk;

        public AudioClip WalkingAudioClip(WalkingAudio _enum)
        {
            switch(_enum)
            {
                case WalkingAudio.Grass:
                    return audioClips_grassWalk[Random.Range(0, audioClips_grassWalk.Count)];
                case WalkingAudio.Stone:
                    return audioClips_grassWalk[Random.Range(0, audioClips_stoneWalk.Count)];
                case WalkingAudio.Gravel:
                    return audioClips_grassWalk[Random.Range(0, audioClips_gravelWalk.Count)];

            }

            return null;
        }

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

            switch(clipEnum)
            {
                case ClipEnum.GrassStep:
                    clip = WalkingAudioClip(WalkingAudio.Grass);
                    break;
                case ClipEnum.StoneStep:
                    clip = WalkingAudioClip(WalkingAudio.Stone);
                    break;
                case ClipEnum.GravelStep:
                    clip = WalkingAudioClip(WalkingAudio.Gravel);
                    break;

                default:
                    foreach (AudioClip _clip in audioClips)
                    {
                        if (_clip.name.Contains(clipEnum.ToString()))
                        {
                            clip = _clip;
                            break;
                        }
                    }
                    break;
            }

            

            return clip;
        }
    }
}
