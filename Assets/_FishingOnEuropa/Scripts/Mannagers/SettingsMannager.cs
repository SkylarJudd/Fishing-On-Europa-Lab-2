using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Europa;


public class SettingsMannager : Singleton<SettingsMannager>
{
    [SerializeField]
    private int snapTurnAmout;
    [SerializeField]
    private RotationType Type;
    public SettingsSO playerSettings;

        /// <summary>
        /// Updates the turn Type Setting
        /// </summary>
        /// <param name="_turnType"></param>
        public void UpdateTurnType(RotationType _turnType)
        {
            playerSettings.turnType = _turnType;
        }
        /// <summary>
        /// Updates the turn Angle Setting
        /// </summary>
        /// <param name="_turnAngle"></param>
        public void UpdateTurnAngle(int _turnAngle)
        {
            playerSettings.turnAngle = _turnAngle;
        }
        /// <summary>
        /// Updates the turn Angle Setting
        /// </summary>
        /// <param name="_turnSpeed"></param>
        public void UpdateSmoothTurnSpeed(int _turnSpeed)
        {
            playerSettings.turnSpeed = _turnSpeed;
        }
        /// <summary>
        /// Updates the Master Volume Setting
        /// </summary>
        /// <param name="_masterVolume"></param>
        public void UpdateMasterVolume(float _masterVolume)
        {
            playerSettings.masterVolume = _masterVolume;
        }
        /// <summary>
        /// Updates the Music Volume Setting
        /// </summary>
        /// <param name="_musicVolume"></param>
        public void UpdateMusicVolume(float _musicVolume)
        {
            playerSettings.musicVolume = _musicVolume;
        }
        /// <summary>
        /// Updates the SFX Seting Setting
        /// </summary>
        /// <param name="_sFXVolume"></param>
        public void UpdateSFXVolume(float _sFXVolume)
        {
            playerSettings.sFXVolume = _sFXVolume;
        }
        /// <summary>
        /// Updates the Hybrid Volume Setting
        /// </summary>
        /// <param name="_hybridVolume"></param>
        public void UpdateHybridVolume(float _hybridVolume)
        {
            playerSettings.hybridVolume = _hybridVolume;
        }
        /// <summary>
        /// Updates the Voices Volume Setting
        /// </summary>
        /// <param name="_voicesVolume"></param>
        public void UpdateVoicesVolume(float _voicesVolume)
        {
            playerSettings.voicesVolume = _voicesVolume;
        }
        /// <summary>
        /// Updates the GameQuality Setting
        /// </summary>
        /// <param name="_GameQuality"></param>
        public void UpdategameQuality(GameQuality _GameQuality)
        {
            playerSettings.gameQuality = _GameQuality;
        }
        /// <summary>
        /// Updates the Use Post Possessing Setting
        /// </summary>
        /// <param name="_postProssessing"></param>
        public void UpdatepostProssessing(bool _postProssessing)
        {
            playerSettings.postProssessing = _postProssessing;
        }
        /// <summary>
        /// Updates the bloom Amount Setting
        /// </summary>
        /// <param name="_bloomAmout"></param>
        public void UpdatebloomAmout(float _bloomAmout)
        {
            playerSettings.bloomAmout = _bloomAmout;
        }
        /// <summary>
        /// Updates the Language
        /// </summary>
        /// <param name="_language"></param>
        public void UpdateLanguage(Language _language)
        {
            playerSettings.language = _language;
        }


        /// <summary>
        /// Loads the current settings from the active save
        /// </summary>
        public void LoadSettings()
        {
            playerSettings.turnType = (RotationType)_TSM.currentSave.settings.moveSettings.turnType.Value;
            playerSettings.turnAngle = _TSM.currentSave.settings.moveSettings.turnAngle.Value;
            playerSettings.turnSpeed = _TSM.currentSave.settings.moveSettings.turnSpeed.Value;

            playerSettings.language = (Language)_TSM.currentSave.settings.languageSettings.language.Value;

            playerSettings.masterVolume = _TSM.currentSave.settings.audioSettings.masterVolume.Value;
            playerSettings.musicVolume = _TSM.currentSave.settings.audioSettings.musicVolume.Value;
            playerSettings.sFXVolume = _TSM.currentSave.settings.audioSettings.sFXVolume.Value;
            playerSettings.hybridVolume = _TSM.currentSave.settings.audioSettings.hybridVolume.Value;
            playerSettings.voicesVolume = _TSM.currentSave.settings.audioSettings.voicesVolume.Value;

            playerSettings.gameQuality = (GameQuality)_TSM.currentSave.settings.graphicSettings.gameQuality.Value;

            playerSettings.postProssessing = _TSM.currentSave.settings.graphicSettings.postProcessing.Value; 
            playerSettings.bloomAmout = _TSM.currentSave.settings.graphicSettings.bloomAmount.Value; 
        }

        /// <summary>
        /// Saves the Current settings to the active save
        /// </summary>
        public void SaveSettings()
        {
            _TSM.currentSave.settings.moveSettings.turnType.Value =  (int)playerSettings.turnType;
            _TSM.currentSave.settings.moveSettings.turnAngle.Value = playerSettings.turnAngle;
            _TSM.currentSave.settings.moveSettings.turnSpeed.Value = playerSettings.turnSpeed;

            _TSM.currentSave.settings.languageSettings.language.Value = (int)playerSettings.language;

            _TSM.currentSave.settings.audioSettings.masterVolume.Value = playerSettings.masterVolume;
            _TSM.currentSave.settings.audioSettings.musicVolume.Value = playerSettings.musicVolume;
            _TSM.currentSave.settings.audioSettings.sFXVolume.Value = playerSettings.sFXVolume;
            _TSM.currentSave.settings.audioSettings.hybridVolume.Value = playerSettings.hybridVolume;
            _TSM.currentSave.settings.audioSettings.voicesVolume.Value = playerSettings.voicesVolume;

            _TSM.currentSave.settings.graphicSettings.gameQuality.Value = (int)playerSettings.gameQuality;

            _TSM.currentSave.settings.graphicSettings.postProcessing.Value = playerSettings.postProssessing;
            _TSM.currentSave.settings.graphicSettings.bloomAmount.Value = playerSettings.bloomAmout;
        }
    }



