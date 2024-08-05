using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService;

public enum Scenes
{
    _ERROR,
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

    [Header("DefultTransform")]
    [SerializeField]
    private Transform defultTransform;

    private void Start()
    {
        currentEnviromentScene = DetectCurrentActiveEnviromentScene();

        if( currentEnviromentScene == Scenes._ERROR )
            Debug.LogError("Was Unable to find Scene, Check The name and Ensure the Scene Exsists");
        
    }

    /// <summary>
    /// Will Find the active Enviroment Scene that is curretly Active
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private Scenes DetectCurrentActiveEnviromentScene()
    {
        Scenes _Sceme;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            _Sceme = GetSceneFromString(SceneManager.GetSceneAt(i).name.ToString());
            if (_Sceme == Scenes._ERROR)
                return _Sceme;

            foreach (Scenes _scenes in IgnoreScenes)
            {
                if (_Sceme != _scenes)
                    return _Sceme;
            }
        }
        _Sceme = Scenes._ERROR;
        return _Sceme;
    }
    
    /// <summary>
    /// Converts a string of the Scene to a Enum of the scene
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
            default:
                _Scene = Scenes._ERROR;
                break;

        }
        return _Scene;
    }

    /// <summary>
    /// takes in a string and uses that to change the scene
    /// </summary>
    /// <param name="_sceneName"></param>
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);

    }
    /// <summary>
    /// Takes in a Enum of Scene and uses that to change the scene
    /// </summary>
    /// <param name="_sceneName"></param>
    public void ChangeScene(Scenes _sceneName)
    {
        Debug.LogWarning("Change Scenes Is depricated, Use Start StartCoroutine(SwitchScene(Scenes.SceneNameEnum)");
        SceneManager.UnloadSceneAsync(currentEnviromentScene.ToString());
        SceneManager.LoadSceneAsync(_sceneName.ToString(), LoadSceneMode.Additive);
        currentEnviromentScene = _sceneName;
    }
    /// <summary>
    /// Takes In a Enum Of the Scene and uses that to change the Scene Asyncly, Unloading the current enviroment scene, and waits while the two opperations are completing
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    IEnumerator SwitchScene(Scenes _sceneName)
    {
        AsyncOperation unlaod = SceneManager.UnloadSceneAsync(currentEnviromentScene.ToString());
        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneName.ToString(), LoadSceneMode.Additive);
        currentEnviromentScene = _sceneName;

        while(unlaod.isDone == false) { 
            yield return new WaitForEndOfFrame();
        }
        while (load.isDone == false)
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
            _PLAYER.UpdatePlayerTransform(defultTransform);
            Debug.LogError("Was Not Able To Find WayPoint Please Make sure a SceneInfoContainer Is active in each Enviroment Scene and has a waypoint within its list");
        }


        yield return null;
    }

    /// <summary>
    /// This Function Will quit the game, if called this will quit without saving and should only be called after save has been called. 
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
