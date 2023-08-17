using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBCore;
using System.Runtime.Serialization.Formatters.Binary;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;


public enum ConfigType
{
    WearingItem = 0,

} // ConfigType


#if UNITY_EDITOR

public class CFTool : MonoBehaviour
{
    ConfigSample sampleConfig;

    //string sourcePath = "TextConfig/config/";
    string sourcePath = "DataConfig/Text/";

    string desFolderName = "DataConfig";

    private string desPath = "";

    void Start()
    {

        desPath = Application.dataPath + "/../../" + desFolderName + "/";
        sourcePath = Application.dataPath + "/../../" + sourcePath;

    } // Start ()

    #region PRIVATE
   
    private void loadDataConfig<TConfigTable> (ref TConfigTable configTable, params string[] paths) where TConfigTable : IConfigDataTable, new ()
    {
        try
        {
            if (configTable == null)
            {
                configTable = new TConfigTable();

                configTable.BeginLoadAppend();
                foreach (var path in paths)
                {
                    string text = File.ReadAllText(path);
                    Debug.Log("load text: " + text);
                    if (!string.IsNullOrEmpty(text))
                    {
                        configTable.LoadFromString(text);

                    } // if
                }
                configTable.EndLoadAppend();
                Debug.Log(string.Format(" Config [{0}] loaded", configTable.GetName()));
            }
        } catch (System.Exception ex)
        {
            Debug.LogError(string.Format("load config {0} error = {1} ", configTable.GetName(), ex.ToString()));
        }
    }


    private void loadConfigToBinary<T, V>(string configDesPath, params string[] sourcePath) where T : IConfigDataTable, new () where V : BaseItemData, new()
    {
        T itemConfig = default(T);
        loadDataConfig(ref itemConfig, sourcePath);
        if(itemConfig != null)
        {
            itemConfig.ToBinary<V>(configDesPath);
        } // if
    } // loadConfigToBinary ()
    #endregion


    #region PUBLIC CALL
    

    public void GenerateTextConfig()
    {

    } // GenerateTextConfig ()



    public void OpenFolderPanelExample()
    {
        return;
        string path = EditorUtility.OpenFolderPanel("Load Config Folder", "", "");
        Debug.LogWarning("choose folder : " + path);
    } // openFolderPanelExample

    #endregion


} // CFTool

#endif
