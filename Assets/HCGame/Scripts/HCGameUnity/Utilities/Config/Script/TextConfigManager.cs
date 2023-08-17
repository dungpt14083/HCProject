using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NBCore;

//using GFramework;

public class TextConfigManager {

    public static void LoadDataConfig<TConfigTable> (ref TConfigTable configTable, params string[] dataPaths) where TConfigTable : IConfigDataTable, new() {
        try {
            if (configTable == null) {
                configTable = new TConfigTable();

                configTable.BeginLoadAppend();
                foreach (var path in dataPaths) {
                    configTable.LoadFromAssetPath(path);
                }
                configTable.EndLoadAppend();
                Debug.Log(string.Format(" Config [{0}] loaded", configTable.GetName()));
            }
        } catch (System.Exception ex) {
            Debug.LogError(string.Format("load config {0} error = {1} ", configTable.GetName(), ex.ToString()));
        }
    }

}