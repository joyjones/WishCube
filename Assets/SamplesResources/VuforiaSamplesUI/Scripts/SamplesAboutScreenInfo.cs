/*===============================================================================
Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections.Generic;

public class SamplesAboutScreenInfo
{

    #region PRIVATE_MEMBERS

    readonly Dictionary<string, string> titles;
    readonly Dictionary<string, string> descriptions;

    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_METHODS

    public string GetTitle(string titleKey)
    {
        return GetValuefromDictionary(titles, titleKey);
    }

    public string GetDescription(string descriptionKey)
    {
        return GetValuefromDictionary(descriptions, descriptionKey);
    }

    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS

    string GetValuefromDictionary(Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            string value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        return "Key not found.";
    }

    #endregion // PRIVATE_METHODS


    #region CONSTRUCTOR

    public SamplesAboutScreenInfo()
    {

        // Init our Title Strings

        titles = new Dictionary<string, string>()
        {
            { "ModelMode", "模型模式" },
            { "PlanetMode", "星球模式" },
        };

        // Init our Common Cache Strings
        string vuforiaVersion = Vuforia.VuforiaUnity.GetVuforiaLibraryVersion();
        string unityVersion = UnityEngine.Application.unityVersion;
        UnityEngine.Debug.Log("Vuforia " + vuforiaVersion + "\nUnity " + unityVersion);

        string description = "\n<size=26>描述:</size>";
        // Init our Description Strings

        descriptions = new Dictionary<string, string>();
        descriptions.Add("ModelMode", description + "\n" + "将魔方替换为某一套模型。");
        descriptions.Add("PlanetMode", description + "\n" + "将魔方的表面当做一个星球。");
    }

    #endregion // CONSTRUCTOR
}
