using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


/// <summary>
/// Takes the inputs from all the buttons in the Main Menu UI and connects then to the SceneController and UI Interactions. 
/// </summary>
public class MainMenuUiMannager : GameBehaviour
{
    #region MainMenu
    [Header("Main Menu Buttons")]
    [SerializeField]
    private Button playFromLastSave_Button;
    [SerializeField]
    private Button newSavePressed_Button;
    [SerializeField]
    private Button mannageSavesPressed_Button;
    [SerializeField]
    private Button settingPressed_Button;
    [SerializeField]
    private Button quitGamePressed;
    #endregion 

    #region SettingsMenu
    [Header("Settings Menu Buttons")]
    [SerializeField]
    private Button turnType_Button;
    [SerializeField]
    private Button locomotionType_Button;
    [SerializeField]
    private Button audioButton_Button;
    [SerializeField]
    private Button graphics_Button;
    #endregion

    #region SaveMenu
    [Header("Save Menu Buttons")]
    [SerializeField]
    private Button continueSave_button;
    [SerializeField]
    private Button deleteSave_button;
    #endregion

    private void Awake()
    {
        SubMainMenuButtons();
    }
    private void OnDestroy()
    {
        UnSubMainMenuButtons();
    }
    /// <summary>
    /// Adds Listeners to all the Buttons In the Main Menu UI
    /// </summary>
    private void SubMainMenuButtons()
    {
        playFromLastSave_Button.onClick.AddListener(OnPlayFromLastSavePressed);
        newSavePressed_Button.onClick.AddListener(OnNewSavePressed);
        mannageSavesPressed_Button.onClick.AddListener(OnMannageSavesPressed);
        settingPressed_Button.onClick.AddListener(OnSettingPressed);
        quitGamePressed.onClick.AddListener(OnQuitGamePressed);
    }

    /// <summary>
    /// Removes Listeners to all the Buttons In the Main Menu UI
    /// </summary>
    private void UnSubMainMenuButtons()
    {
        playFromLastSave_Button.onClick.RemoveListener(OnPlayFromLastSavePressed);
        newSavePressed_Button.onClick.RemoveListener(OnNewSavePressed);
        mannageSavesPressed_Button.onClick.RemoveListener(OnMannageSavesPressed);
        settingPressed_Button.onClick.RemoveListener(OnSettingPressed);
        quitGamePressed.onClick.RemoveListener(OnQuitGamePressed);
    }

    [ContextMenu("OnPlayFromLastSavePressed")]
    public void OnPlayFromLastSavePressed()
    {
        print("OnPlayFromLastSavePressed");

    }

    [ContextMenu("OnNewSavePressed")]
    public void OnNewSavePressed()
    {
        print("OnNewSavePressed");
    }

    [ContextMenu("OnMannageSavesPressed")]
    public void OnMannageSavesPressed()
    {
        print("OnMannageSavesPressed");
    }

    [ContextMenu("OnSettingPressed")]
    public void OnSettingPressed()
    {
        print("OnSettingPressed");
    }

    [ContextMenu("OnQuitGamePressed")]
    public void OnQuitGamePressed()
    {
        print("OnQuitGamePressed");
    }
}
