using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public enum GameState
    {
        MainMenu,
        Play,
        Paused,
    }

    public class GameManager : Singleton<GameManager>
    {
        [Header("Game State")]
        public GameState gameState;


        #region GameEvents
        [Header("Game Events")]
        [SerializeField]
        private ScriptableEventNoParam _onGameAwake;
        [SerializeField]
        private ScriptableEventNoParam _onGameStart;
        [SerializeField]
        private ScriptableEventNoParam _onMainMenuLoaded;
        [SerializeField]
        private ScriptableEventNoParam _onMainMenuUnloaded;
        [SerializeField]
        private ScriptableEventNoParam _onNewSave;
        [SerializeField]
        private ScriptableEventNoParam _onSave;
        [SerializeField]
        private ScriptableEventNoParam _onGameLoadStart;
        [SerializeField]
        private ScriptableEventNoParam _onGameLoadEnd;
        [SerializeField]
        private ScriptableEventNoParam _onFarmSceneLoaded;
        [SerializeField]
        private ScriptableEventNoParam _onFarmSceneUnloaded;
        [SerializeField]
        private ScriptableEventNoParam _onDomeSceneLoaded;
        [SerializeField]
        private ScriptableEventNoParam _onDomeSceneUnloaded;
        [SerializeField]
        private ScriptableEventNoParam _onTutorialSceneLoaded;
        [SerializeField]
        private ScriptableEventNoParam _onTutorialSceneUnloaded;
        [SerializeField]
        private ScriptableEventNoParam _onGamePauseStart;
        [SerializeField]
        private ScriptableEventNoParam _onGamePauseEnd;
        [SerializeField]
        private ScriptableEventNoParam _onExitGame;
        #endregion

        public override void Awake()
        {
            base.Awake();
            ActionGameAwake();
        }

        private void Start()
        {
            ActionGameStart();
        }

        private void OnApplicationQuit()
        {
            ActionExitGame();
        }

        [ContextMenu("Game Awake")]
        public void ActionGameAwake() => _onGameAwake.Raise();


        [ContextMenu("Game Start")]
        public void ActionGameStart() => _onGameStart.Raise();


        [ContextMenu("Main Menu Load")]
        public void ActionMainMenuLoad() => _onMainMenuLoaded.Raise();


        [ContextMenu("New Save")]
        public void ActionNewSave() => _onNewSave.Raise();


        [ContextMenu("Save")]
        public void ActionSave() => _onSave.Raise();


        [ContextMenu("Game Load Start")]
        public void ActionGameLoadStart() => _onGameLoadStart.Raise();


        [ContextMenu("Game Load End")]
        public void ActionGameLoadEnd() => _onGameLoadEnd.Raise();


        [ContextMenu("Farm Scene Loaded")]
        public void ActionFarmSceneLoaded() => _onFarmSceneLoaded.Raise();


        [ContextMenu("Farm Scene Unloaded")]
        public void ActionFarmSceneUnloaded() => _onFarmSceneUnloaded.Raise();


        [ContextMenu("Dome Scene Loaded")]
        public void ActionDomeSceneLoaded() => _onDomeSceneLoaded.Raise();


        [ContextMenu("Dome Scene Unloaded")]
        public void ActionDomeSceneUnloaded() => _onDomeSceneUnloaded.Raise();


        [ContextMenu("Tutorial Scene Loaded")]
        public void ActionTutorialSceneLoaded() => _onTutorialSceneLoaded.Raise();


        [ContextMenu("Tutorial Scene Unloaded")]
        public void ActionTutorialSceneUnloaded() => _onTutorialSceneUnloaded.Raise();


        [ContextMenu("Pause Game Start")]
        public void ActionPauseGameStart() => _onGamePauseStart.Raise();


        [ContextMenu("Pause Game End")]
        public void ActionPauseGameEnd() => _onGamePauseEnd.Raise();


        [ContextMenu("Exit Game")]
        public void ActionExitGame() => _onExitGame.Raise();


        
    }
}

