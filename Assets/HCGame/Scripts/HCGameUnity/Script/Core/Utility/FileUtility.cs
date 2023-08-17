using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


#if UNITY_EDITOR
public static class FileUtility {

    /// <summary>
    /// Get All Directory from Assets path
    /// </summary>
    /// <returns></returns>
    public static string[] GetAllDirectories (string path) {

        string basePath = Application.dataPath.Replace("Assets", string.Empty);
        string fullpath = basePath + path;
        string[] results = Directory.GetDirectories(fullpath);

        results = removeFolderPath(results);

        return results;
    }

    /// <summary>
    /// Get All Files from Assets path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetAllFiles(string path) {
        string basePath = Application.dataPath.Replace("Assets", string.Empty);
        string fullpath = basePath + path;
        string[] results = Directory.GetFiles(fullpath);
        results = removeFolderPath(results);
        return results;

    } // GetAllFiles ()


    public static string[] GetAllFilesIncludeChild(string path)
    {
        string[] directories = GetAllDirectories(path);

        List<string> files = new List<string>();
        foreach(string directory in directories)
        {
            string[] fileInChilds = GetAllFilesIncludeChild(directory);
            files.AddRange(fileInChilds);
        } // foreach
        files.AddRange(GetAllFiles(path));
        return files.ToArray();
            
    } // GetallFilesIncludeChild ()

    public static bool CreatePrefabFile (string filePath, GameObject go, bool isCreateFolderPath = false) {
        bool isSuccess = false;

        if (isCreateFolderPath) {
            // check folder path first
            string directoryPath = filePath.Remove(filePath.LastIndexOf("/"));
            bool createDirectoryResult = CreateDirectory(directoryPath);

            if (!createDirectoryResult) {
                return false;

            } // if
            
        } // if
        GameObject cloneObject = GameObject.Instantiate(go);

        GameObject ob = PrefabUtility.SaveAsPrefabAssetAndConnect(cloneObject, filePath, InteractionMode.AutomatedAction,out isSuccess);
        GameObject.DestroyImmediate(cloneObject);
        return isSuccess;
    } //

    public static bool CreatePrefabFileFromInstantiate(string filePath, GameObject go, bool isCreateFolderPath = false)
    {
        bool isSuccess = false;

        if (isCreateFolderPath)
        {
            // check folder path first
            string directoryPath = filePath.Remove(filePath.LastIndexOf("/"));
            bool createDirectoryResult = CreateDirectory(directoryPath);

            if (!createDirectoryResult)
            {
                return false;

            } // if

        } // if
        GameObject ob = PrefabUtility.SaveAsPrefabAssetAndConnect(go, filePath, InteractionMode.AutomatedAction, out isSuccess);
        GameObject.DestroyImmediate(go);
        return isSuccess;
    } // 

    public static bool CreateDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            return true;
        } // if
        bool result = false;

        if (directoryPath.Contains("/"))
        {
            string subdirectory = directoryPath.Remove(directoryPath.LastIndexOf("/"));
            if (CreateDirectory(subdirectory))
            {
                DirectoryInfo info = Directory.CreateDirectory(directoryPath);
                result = (info != null);
            }// if
        } // if
        return result;
    }
    


    #region INTERNAL




    private static string[] removeFolderPath (string[] originalPaths) {
        string basePath = Application.dataPath.Replace("Assets", string.Empty);
        //change '\' to '/'
        for (int i = 0; i < originalPaths.Length; i++) {
            string directory = originalPaths[i];
            string newString = directory.Replace(@"\", "/");
            //remove base path
            newString = newString.Replace(basePath, "");
            // add to list
            originalPaths[i] = newString;
        } // for

        return originalPaths;
    } // removeFolderPath ()

    #endregion

}
#endif
