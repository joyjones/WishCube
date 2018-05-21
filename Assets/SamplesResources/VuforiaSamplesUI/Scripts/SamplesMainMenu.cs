/*===============================================================================
Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class SamplesMainMenu : MonoBehaviour
{

    #region PUBLIC_MEMBERS

    public enum MenuItem
    {
        ModelMode,
        PlanetMode
    }

    public Canvas AboutCanvas;
    public Text AboutTitle;
    public Text AboutDescription;

    public static bool isAboutScreenVisible;

    // initialize static enum with one of the items
    public static MenuItem menuItem = MenuItem.ModelMode;

    public const string MenuScene = "1-Menu";
    public const string LoadingScene = "2-Loading";

    SamplesAboutScreenInfo aboutScreenInfo;

    #endregion // PUBLIC_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        // reset about screen state variable to false when returning from AR scene
        isAboutScreenVisible = false;

        if (aboutScreenInfo == null)
        {
            // initialize if null
            aboutScreenInfo = new SamplesAboutScreenInfo();
        }
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    public static string GetSceneToLoad()
    {
        // called by SamplesLoadingScreen to load selected AR scene
        return "3-" + menuItem.ToString();
    }

    public static void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void BackToMenu()
    {
        // called to return to Menu from About screen
        AboutCanvas.sortingOrder = 0;
        isAboutScreenVisible = false;
    }

    public void LoadAboutScene(string key)
    {
        var itemSelected = (MenuItem)Enum.Parse(typeof(MenuItem), key);
        UpdateConfiguration(itemSelected);

        // This method called from list of Sample App menu buttons
        switch (itemSelected)
        {
            case MenuItem.ModelMode:
                menuItem = MenuItem.ModelMode;
                break;
            case MenuItem.PlanetMode:
                menuItem = MenuItem.PlanetMode;
                break;
        }

        AboutTitle.text = aboutScreenInfo.GetTitle(menuItem.ToString());
        AboutDescription.text = aboutScreenInfo.GetDescription(menuItem.ToString());

        AboutCanvas.transform.parent.transform.position = Vector3.zero; // move canvas into position
        AboutCanvas.sortingOrder = 2; // bring canvas in front of main menu
        isAboutScreenVisible = true;

    }

    public void UpdateConfiguration(MenuItem scene)
    {

        var config = VuforiaConfiguration.Instance;
        var dbConfig = config.DatabaseLoad;

        // all settings which are changed for a scene, have to be reset here
        // because any change is persistent throughout the whole application
        dbConfig.DataSetsToLoad = dbConfig.DataSetsToActivate = new string[0];
        config.Vuforia.MaxSimultaneousImageTargets = 4;

        switch (scene)
        {
            case MenuItem.ModelMode:
            case MenuItem.PlanetMode:
                dbConfig.DataSetsToLoad = dbConfig.DataSetsToActivate = new[] { "VuforiaMars_Images", "dev1" };
                break;
        }
    }

    #endregion // PUBLIC_METHODS

}