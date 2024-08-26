using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    _ERROR,
    _TUTORIAL,
    _ESSENTIALES,
    _MAINMENU_SCENE,
    _FARM_SCENE,
    _DOME_SCENE,
}

public class SceneController : Singleton<SceneController>
{
    [Header("Scenes")]
    public Scenes[] IgnoreScenes;
    public Scenes currentEnviromentScene;

    [Header("DefaultTransform")]
    [SerializeField]
    private Transform defaultTransform;

    [Header("Debug")]
    [SerializeField] bool debug;

    private void Start()
    {
        currentEnviromentScene = DetectCurrentActiveEnviromentScene();

        if (currentEnviromentScene == Scenes._ERROR)
            Debug.LogError("Was unable to find Scene, check the name and ensure the scene exists");

        if (currentEnviromentScene != Scenes._MAINMENU_SCENE && !debug)
        {
            Debug.LogWarning($"Switching to Main Menu from {currentEnviromentScene} If this is not what you wanted toggle on Debug");
            StartCoroutine(SwitchScene(Scenes._MAINMENU_SCENE));
        }
    }

    /// <summary>
    /// Will find the active environment scene that is currently active
    /// </summary>
    private Scenes DetectCurrentActiveEnviromentScene()
    {
        foreach (var scene in Enum.GetValues(typeof(Scenes)))
        {
            Scenes _Scene = (Scenes)scene;
            if (_Scene == Scenes._ERROR) continue;

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

        return Scenes._ERROR;
    }

    /// <summary>
    /// Converts a string of the Scene to an Enum of the scene
    /// </summary>
    /// <param name="_SceneName"></param>
    /// <returns></returns>
    private Scenes GetSceneFromString(string _SceneName)
    {
        Scenes _Scene;
        print($"SceneName = {_SceneName}");

        switch (_SceneName)
        {
            case "_ESSENTIALES":
                _Scene = Scenes._ESSENTIALES;
                break;
            case "_MAINMENU_SCENE":
                _Scene = Scenes._MAINMENU_SCENE;
                break;
            case "_FARM_SCENE":
                _Scene = Scenes._FARM_SCENE;
                break;
            case "_DOME_SCENE":
                _Scene = Scenes._DOME_SCENE;
                break;
            case "_TUTORIAL":
                _Scene = Scenes._TUTORIAL;
                break;
            default:
                _Scene = Scenes._ERROR;
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
        Scenes scenes = GetSceneFromString(_sceneName);
        if (scenes == Scenes._ERROR)
        {
            Debug.LogError($"Count not find {_sceneName} Check you're using the right name, or add the scene to the scene controller Enum and switch statment");
            return;
        }
        else
        {
            SwitchScene(scenes);
        }
    }

    /// <summary>
    /// Takes in an Enum of Scene and uses that to change the scene
    /// </summary>
    /// <param name="_sceneName"></param>
    public void ChangeScene(Scenes _sceneName)
    {
        Debug.LogWarning("Change Scenes Is deprecated, Use Start StartCoroutine(SwitchScene(Scenes.SceneNameEnum)");
        SceneManager.UnloadSceneAsync(currentEnviromentScene.ToString());
        SceneManager.LoadSceneAsync(_sceneName.ToString(), LoadSceneMode.Additive);
        currentEnviromentScene = _sceneName;
    }

    /// <summary>
    /// Takes in an Enum of the Scene and uses that to change the Scene Asynchronously, unloading the current environment scene, and waits while the two operations are completing
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    IEnumerator SwitchScene(Scenes _sceneName)
    {
        AsyncOperation unload = SceneManager.UnloadSceneAsync(currentEnviromentScene.ToString());
        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneName.ToString(), LoadSceneMode.Additive);
        currentEnviromentScene = _sceneName;

        while (!unload.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        while (!load.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        Transform waypoint = FindObjectOfType<SceneInfoContainer>().entranceWaypoints[0];
        if (waypoint != null)
        {
            _PLAYER.UpdatePlayerTransform(waypoint);
        }
        else
        {
            _PLAYER.UpdatePlayerTransform(defaultTransform);
            Debug.LogError("Was Not Able To Find WayPoint Please Make sure a SceneInfoContainer Is active in each Environment Scene and has a waypoint within its list");
        }

        yield return null;
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
    public void LoadMainMenu() => StartCoroutine(SwitchScene(Scenes._MAINMENU_SCENE));
    /// <summary>
    /// Test Load to Load the Farm Scene from within Unity
    /// </summary>
    [ContextMenu("Load Farm Scene")]
    public void LoadFarmScene() => StartCoroutine(SwitchScene(Scenes._FARM_SCENE));
    /// <summary>
    /// Test Load to Load the Dome Scene from within Unity
    /// </summary>
    [ContextMenu("Load Dome Scene")]
    public void LoadDomeScene() => StartCoroutine(SwitchScene(Scenes._DOME_SCENE));

    #endregion
}