using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : GameBehaviour
{
    //Changes Scene to the one named in string
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    //Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }

}
