using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Europa
{
    public enum CurrentScenes
    {
        _ERROR,
        _TUTORIAL,
        _ESSENTIALES,
        _MAINMENU_SCENE,
        _FARM_SCENE,
        _DOME_SCENE,
        _FISHINGTEST_SCENE
    }

    public class SceneController : Singleton<SceneController>
    {
        [Header("Scenes")]
        public CurrentScenes[] IgnoreScenes;
        public CurrentScenes currentEnviromentScene;

        [Header("DefaultTransform")]
        [SerializeField]
        private Transform defaultTransform;

        [Header("Debug")]
        [SerializeField] bool debug;
        [SerializeField] CurrentScenes starterScene;

        private void Start()
        {
            currentEnviromentScene = DetectCurrentActiveEnviromentScene();

            if (currentEnviromentScene == CurrentScenes._ERROR)
                Debug.LogError("Was unable to find Scene, check the name and ensure the scene exists");

            if (debug)
            {
                Debug.LogWarning($"Switching to {starterScene} from {currentEnviromentScene} If this is not what you wanted toggle on Debug");
                StartCoroutine(SwitchScene(starterScene));
            }

            else if (currentEnviromentScene != CurrentScenes._MAINMENU_SCENE && !debug)
            {
                Debug.LogWarning($"Switching to Main Menu from {currentEnviromentScene} If this is not what you wanted toggle on Debug");
                StartCoroutine(SwitchScene(CurrentScenes._MAINMENU_SCENE));
            }
        }

        /// <summary>
        /// Will find the active environment scene that is currently active
        /// </summary>
        private CurrentScenes DetectCurrentActiveEnviromentScene()
        {
            foreach (var scene in Enum.GetValues(typeof(CurrentScenes)))
            {
                CurrentScenes _Scene = (CurrentScenes)scene;
                if (_Scene == CurrentScenes._ERROR) continue;

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var activeScene = SceneManager.GetSceneAt(i);
                    if (activeScene.name.Equals(_Scene.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        if (Array.Exists(IgnoreScenes, s => s == _Scene))
                            continue;
                        return _Scene;
                    }
                }
            }

            return CurrentScenes._ERROR;
        }

        /// <summary>
        /// Converts a string of the Scene to an Enum of the scene
        /// </summary>
        /// <param name="_SceneName"></param>
        /// <returns></returns>
        private CurrentScenes GetSceneFromString(string _SceneName)
        {
            CurrentScenes _Scene;
            print($"SceneName = {_SceneName}");

            switch (_SceneName)
            {
                case "_ESSENTIALES":
                    _Scene = CurrentScenes._ESSENTIALES;
                    break;
                case "_MAINMENU_SCENE":
                    _Scene = CurrentScenes._MAINMENU_SCENE;
                    break;
                case "_FARM_SCENE":
                    _Scene = CurrentScenes._FARM_SCENE;
                    break;
                case "_DOME_SCENE":
                    _Scene = CurrentScenes._DOME_SCENE;
                    break;
                case "_TUTORIAL":
                    _Scene = CurrentScenes._TUTORIAL;
                    break;
                case "_FISHINGTEST_SCENE":
                    _Scene = CurrentScenes._FISHINGTEST_SCENE;
                    break;

                default:
                    _Scene = CurrentScenes._ERROR;
                    break;
            }
            return _Scene;
        }

        /// <summary>
        /// Takes in a string and uses that to change the scene
        /// </summary>
        /// <param name="_sceneName"></param>
        public void ChangeScene(string _sceneName)
        {
            CurrentScenes scenes = GetSceneFromString(_sceneName);
            if (scenes == CurrentScenes._ERROR)
            {
                Debug.LogError($"Count not find {_sceneName} Check you're using the right name, or add the scene to the scene controller Enum and switch statment");
                return;
            }
            else
            {
                StartCoroutine(SwitchScene(scenes));
            }
        }

        /// <summary>
        /// Takes in an Enum of Scene and uses that to change the scene
        /// </summary>
        /// <param name="_sceneName"></param>
        public void ChangeScene(CurrentScenes _sceneName)
        {
            if (_sceneName == CurrentScenes._ERROR)
            {
                Debug.LogError($"Count not find {_sceneName} Check you're using the right name, or add the scene to the scene controller Enum and switch statment");
                return;
            }
            else
            {
                StartCoroutine(SwitchScene(_sceneName));
            }
        }

        /// <summary>
        /// Takes in an Enum of the Scene and uses that to change the Scene Asynchronously, unloading the current environment scene, and waits while the two operations are completing
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <returns></returns>
        IEnumerator SwitchScene(CurrentScenes _sceneName)
        {
            AsyncOperation unload;
            if (currentEnviromentScene != CurrentScenes._ERROR)
            {
                unload = SceneManager.UnloadSceneAsync(currentEnviromentScene.ToString());
                while (!unload.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }

            }

            AsyncOperation load = SceneManager.LoadSceneAsync(_sceneName.ToString(), LoadSceneMode.Additive);
            currentEnviromentScene = _sceneName;


            while (!load.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();

            Transform waypoint = GetWaypointTransform();

            _PLAYER.UpdatePlayerTransform(waypoint);


            yield return null;
        }

        private Transform GetWaypointTransform()
        {
            // Try to find the SceneInfoContainer
            SceneInfoContainer sceneInfoContainer = FindObjectOfType<SceneInfoContainer>();

            // Check if SceneInfoContainer exists and has waypoints
            if (sceneInfoContainer != null && sceneInfoContainer.entranceWaypoints.Count > 0)
            {
                // Get the first waypoint
                Transform waypoint = sceneInfoContainer.entranceWaypoints[0];

                // Check if the first waypoint is not null
                if (waypoint != null)
                {
                    return waypoint;
                }
                else
                {
                    Debug.LogError("The first waypoint in the list is null. Using default transform.");
                    return defaultTransform; // Return defaultTransform if the waypoint is null
                }
            }
            else
            {
                Debug.LogError("SceneInfoContainer is either missing or has no waypoints. Using default transform.");
                return defaultTransform; // Return defaultTransform if no valid SceneInfoContainer or waypoint
            }
        }

        /// <summary>
        /// This function will quit the game, if called this will quit without saving and should only be called after save has been called. 
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }

        #region TestLoads

        /// <summary>
        /// Test Load to Load the Main Menu from within Unity
        /// </summary>
        [ContextMenu("Load Main Menu")]
        public void LoadMainMenu() => StartCoroutine(SwitchScene(CurrentScenes._MAINMENU_SCENE));
        /// <summary>
        /// Test Load to Load the Farm Scene from within Unity
        /// </summary>
        [ContextMenu("Load Farm Scene")]
        public void LoadFarmScene() => StartCoroutine(SwitchScene(CurrentScenes._FARM_SCENE));
        /// <summary>
        /// Test Load to Load the Dome Scene from within Unity
        /// </summary>
        [ContextMenu("Load Dome Scene")]
        public void LoadDomeScene() => StartCoroutine(SwitchScene(CurrentScenes._DOME_SCENE));

        #endregion
    }
}
