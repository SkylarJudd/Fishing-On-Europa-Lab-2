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
        /// Updates the Use Post Prossessing Setting
        /// </summary>
        /// <param name="_postProssessing"></param>
        public void UpdatepostProssessing(bool _postProssessing)
        {
            playerSettings.postProssessing = _postProssessing;
        }
        /// <summary>
        /// Updates tthe bloom Amout Setting
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
        /// Loads the curret settings from the active save
        /// </summary>
        public void LoadSettings()
        {
            playerSettings.turnType = (RotationType)_TSM.currentSave.turnType;
            playerSettings.turnAngle = _TSM.currentSave.turnAngle;
            playerSettings.turnSpeed = _TSM.currentSave.turnSpeed;

            playerSettings.language = (Language)_TSM.currentSave.language;

            playerSettings.masterVolume = _TSM.currentSave.masterVolume;
            playerSettings.musicVolume = _TSM.currentSave.musicVolume;
            playerSettings.sFXVolume = _TSM.currentSave.sFXVolume;
            playerSettings.hybridVolume = _TSM.currentSave.hybridVolume;
            playerSettings.voicesVolume = _TSM.currentSave.voicesVolume;

            playerSettings.gameQuality = (GameQuality)_TSM.currentSave.gameQuality;

            playerSettings.postProssessing = _TSM.currentSave.postProssessing;
            playerSettings.bloomAmout = _TSM.currentSave.bloomAmout;
        }

        /// <summary>
        /// Saves the Curret settings to the active save
        /// </summary>
        public void SaveSettings()
        {
            _TSM.currentSave.turnType =  (int)playerSettings.turnType;
            _TSM.currentSave.turnAngle = playerSettings.turnAngle;
            _TSM.currentSave.turnSpeed = playerSettings.turnSpeed;

            _TSM.currentSave.language = (int)playerSettings.language;

            _TSM.currentSave.masterVolume = playerSettings.masterVolume;
            _TSM.currentSave.musicVolume = playerSettings.musicVolume;
            _TSM.currentSave.sFXVolume = playerSettings.sFXVolume;
            _TSM.currentSave.hybridVolume = playerSettings.hybridVolume;
            _TSM.currentSave.voicesVolume = playerSettings.voicesVolume;

            _TSM.currentSave.gameQuality = (int)playerSettings.gameQuality;

            _TSM.currentSave.postProssessing = playerSettings.postProssessing;
            _TSM.currentSave.bloomAmout = playerSettings.bloomAmout;
        }
    }



