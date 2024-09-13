using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum GameQuality
    {
        Low, Medium, High,
    }
    public enum RotationType
    {
        snap,
        smooth
    }

    public enum Language
    {
        English,
        Chinese,
        Russian,
        Spanish,
        Portuguese,
        German,
        Japanses,
        French,
        Polish,
        Turkish

    }

    [CreateAssetMenu(fileName = "SettingsAsset", menuName = "Europa/Settings", order = 1)]
    public class SettingsSO : ScriptableObject
    {
        [Header("Movement Settings")]
        public RotationType turnType;
        public int turnAngle;
        public float turnSpeed;

        [Header("Language")]
        public Language language;

        [Header("Audio Settings")]
        public float masterVolume;
        public float musicVolume;
        public float sFXVolume;
        public float hybridVolume;
        public float voicesVolume;

        [Header("Graphics Settings")]
        public GameQuality gameQuality;

        [Header("PostProsessing")]
        public bool postProssessing;
        public float bloomAmout;
    }

}
