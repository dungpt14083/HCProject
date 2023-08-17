using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEditor;

//using UBlockEditor.Setting;
//using UBlockEditor.MetaFile;
//using YamlDotNet.Serialization;

namespace NBCore {
    public static class AssetGenerator {
        public const string RootPath = AssetSetting.RootPath;
        private static readonly List<string> s_CutAssets = new List<string>();

        #region Create Data Asset

        public static bool CreateDataAsset (UnityEngine.Object selectedObject) {
            Type type;
            if (ValidateDataAsset(selectedObject, out type) == false) {
                return false;
            }

            string selectedFilePath = AssetDatabase.GetAssetPath(selectedObject);

            if (selectedFilePath == "") {
                Debug.LogWarning("Asset Path is null");
                return false;
            }

            string directoryPath = RootPath;
            if (File.Exists(selectedFilePath))
                directoryPath = Path.GetDirectoryName(selectedFilePath);


            var dataAssest = ScriptableObject.CreateInstance(type);
            var fullPath = string.Format("{0}/{1}.asset", directoryPath, type.Name);
            ProjectWindowUtil.CreateAsset(dataAssest, fullPath);

            return true;
        }

        private static bool ValidateDataAsset (UnityEngine.Object selectedObject, out Type typeAsset) {
            typeAsset = null;
            if (selectedObject is MonoScript) {
                MonoScript script = (MonoScript)selectedObject;
                System.Type type = script.GetClass();
                if (type.IsSubclassOf(typeof(ScriptableObject))) {
                    typeAsset = type;
                    return true;
                }
            }
            Debug.LogWarning("Selected asset is not valid");
            return false;
        }

        public static void CreateDataAsset<T> (string path) where T : ScriptableObject {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, path + "/" + typeof(T).Name + ".asset");
        }

        #endregion

        #region Create Folder

        public static string TryCreateFolder (string folderName, bool crateParentIfNotExist = true) {
            var parentPath = string.Empty;
            if (crateParentIfNotExist) {
                parentPath = Path.GetDirectoryName(folderName);

                //Create parent folders if not exist
                var parentFolders = parentPath.Split('/');

                var currentFolder = RootPath;
                for (int i = 0; i < parentFolders.Length; i++) {
                    var newFolderPath = string.Format("{0}/{1}", currentFolder, parentFolders[i]);
                    if (!Directory.Exists(newFolderPath))
                        AssetDatabase.CreateFolder(currentFolder, parentFolders[i]);

                    currentFolder = newFolderPath;
                }
                

                if (!string.IsNullOrEmpty(parentPath)) {
                    folderName = Path.GetFileName(folderName);
                }
            }
            return TryCreateFolder(parentPath, folderName);
        }

        private static string TryCreateFolder (string path, string folderName) {
            string parentFolder = (string.IsNullOrEmpty(path)) ? RootPath : Path.Combine(RootPath, path);
            string newFolderPath = Path.Combine(parentFolder, folderName);

            if (!Directory.Exists(parentFolder)) {
                Debug.LogWarning(string.Format("The path {0} is not exist", parentFolder));
                return string.Empty;
            }

            if (Directory.Exists(newFolderPath)) {
                return newFolderPath.Substring(RootPath.Length + 1); //Remove RootPath
            }

            string guid = AssetDatabase.CreateFolder(parentFolder, folderName);
            return AssetDatabase.GUIDToAssetPath(guid).Substring(RootPath.Length + 1); //Remove RootPath
        }

        #endregion

        public static List<string> GetCutAssets () {
            return s_CutAssets;
        }

        public static void CutAssets (IEnumerable<string> assetGUIDs) {
            s_CutAssets.Clear();
            foreach (var assetGUID in assetGUIDs) {
                var cutPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                s_CutAssets.Add(cutPath);
            }
        }


//        public static void PasteAssets (string destAssetGUID) {
//            var destPath = AssetDatabase.GUIDToAssetPath(destAssetGUID);
//            Dictionary<string, string> result = new Dictionary<string, string>();
//            MoveAssets(s_CutAssets, destPath, result);
//            s_CutAssets.Clear();
//        }


//        public static void MoveAssets (IEnumerable<string> cutAssets, string destPath, IDictionary<string, string> movedAssets) {
//            if (!Directory.Exists(destPath))
//                destPath = Path.GetDirectoryName(destPath);
//
//            foreach (var current in cutAssets) {
//                string fileName = Path.GetFileName(current);
//                var newPath = Path.Combine(destPath, fileName);
//                MoveAsset(current, newPath, movedAssets);
//            }
//        }


//        public static void MoveAsset (string current, string newPath, IDictionary<string, string> movedAssets) {
//            if ((Directory.Exists(current) && Directory.Exists(newPath)) && (Path.GetFileName(current) == Path.GetFileName(newPath))) {
//                var childAssets = UAssetDatabase.GetAllAssetsAtPath(current);
//                MoveAssets(childAssets, newPath, movedAssets);
//            } else {
//                var result = AssetDatabase.MoveAsset(current, newPath);
//                if (!string.IsNullOrEmpty(result)) {
//                    Debug.LogError((object)("Paste Failed: " + result));
//                } else {
//                    movedAssets.Add(current, newPath);
//                }
//            }
//        }

        #region Create Bitmap Font

        /*
        public static bool IsFullBitmapFont(IEnumerable<BitmapCharacterInfo> bitmapCharacterInfos)
        {
            return true;
        }
        public static List<BitmapCharacterInfo> GetBitmapCharacterInfos(UnityEngine.Object selectedObject)
        {
            Texture2D selectedBitmap = selectedObject as Texture2D;
            if (selectedBitmap == null)
            {
                Debug.LogError(string.Format("{0} is not texture", selectedObject.name));
                return null;
            }

            List<BitmapCharacterInfo> bitmapCharacterInfos = GetBitmapCharacterInfosFromXml(selectedBitmap);
            if (bitmapCharacterInfos == null)
                bitmapCharacterInfos = GetBitmapCharacterInfosFromMeta(selectedBitmap);
            return bitmapCharacterInfos;
        }

        private static List<BitmapCharacterInfo> GetBitmapCharacterInfosFromXml(Texture2D selectedBitmap)
        {
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedBitmap));
            var xmlPath = Path.Combine(rootPath, Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(selectedBitmap)) + ".fnt");
            var guid = AssetDatabase.AssetPathToGUID(xmlPath);
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }
            var xmlAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(xmlPath);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNode info = xml.GetElementsByTagName("info")[0];
            XmlNode common = xml.GetElementsByTagName("common")[0];
            XmlNodeList chars = xml.GetElementsByTagName("chars")[0].ChildNodes;

            int texW = selectedBitmap.width;
            int texH = selectedBitmap.height;

            List<BitmapCharacterInfo> bitmapCharacterInfos = new List<BitmapCharacterInfo>();
            for (int i = 0; i < chars.Count; i++)
            {
                XmlNode charNode = chars[i];
                if (charNode.Attributes != null)
                {
                    var bitmapCharacterInfo = new BitmapCharacterInfo();
                    var id = int.Parse(charNode.Attributes.GetNamedItem("id").InnerText);
                    var name = ((char)id).ToString();
                    bitmapCharacterInfo.spriteName = name;

                    int xadvance = int.Parse(charNode.Attributes.GetNamedItem("xadvance").InnerText);

                    Rect uv = new Rect();
                    uv.x = float.Parse(charNode.Attributes.GetNamedItem("x").InnerText) / texW;
                    uv.y = float.Parse(charNode.Attributes.GetNamedItem("y").InnerText) / texH;
                    uv.width = float.Parse(charNode.Attributes.GetNamedItem("width").InnerText) / texW;
                    uv.height = float.Parse(charNode.Attributes.GetNamedItem("height").InnerText) / texH;
                    uv.y = 1f - uv.y - uv.height;

                    Rect vert = new Rect();
                    vert.x = float.Parse(charNode.Attributes.GetNamedItem("xoffset").InnerText);
                    vert.y = float.Parse(charNode.Attributes.GetNamedItem("yoffset").InnerText);
                    vert.width = float.Parse(charNode.Attributes.GetNamedItem("width").InnerText);
                    vert.height = float.Parse(charNode.Attributes.GetNamedItem("height").InnerText);
                    vert.y = -vert.y;
                    vert.height = -vert.height;

                    bitmapCharacterInfo.data = BitmapCharacterInfo.GetCharacterInfo(name, xadvance, uv, vert, texW, texH);
                    bitmapCharacterInfos.Add(bitmapCharacterInfo);
                }
            }

            return bitmapCharacterInfos;
        }

        private static List<BitmapCharacterInfo> GetBitmapCharacterInfosFromMeta(Texture2D selectedBitmap)
        {
            var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(AssetDatabase.GetAssetPath(selectedBitmap));
            var streamReader = File.OpenText(metaPath);
            var deserializer = new Deserializer(ignoreUnmatched: true);
            var spriteMetaData = deserializer.Deserialize<SpriteMetaFile>(streamReader);

            Dictionary<string, BitmapCharacterInfo> bitmapCharacterInfos = new Dictionary<string, BitmapCharacterInfo>();
            foreach (var fileItem in spriteMetaData.TextureImporter.fileIDToRecycleName)
            {
                var bitmapCharacterInfo = new BitmapCharacterInfo();
                bitmapCharacterInfo.spriteName = fileItem.Value;
                bitmapCharacterInfo.spriteID = fileItem.Key;

                bitmapCharacterInfos.Add(fileItem.Value, bitmapCharacterInfo);
            }

            foreach (var spriteInfo in spriteMetaData.TextureImporter.spriteSheet.sprites)
            {
                var bitmapCharacterInfo = bitmapCharacterInfos[spriteInfo.name];
                bitmapCharacterInfo.data = BitmapCharacterInfo.GetCharacterInfo(spriteInfo, selectedBitmap.width, selectedBitmap.height);
            }

            return bitmapCharacterInfos.Values.ToList();
        }

        public static void CreateBitmapFont(Texture2D selectedBitmap, IEnumerable<BitmapCharacterInfo> bitmapCharacterInfos)
        {
            if (bitmapCharacterInfos == null)
                return;

            var characterInfos = new List<CharacterInfo>();
            foreach (var bitmapCharacterInfo in bitmapCharacterInfos)
            {
                if (bitmapCharacterInfo.data.index != -1)
                {
                    characterInfos.Add(bitmapCharacterInfo.data);
                    Debug.Log(string.Format("Code: {0} Name: {1}", bitmapCharacterInfo.data.index,
                        (char)bitmapCharacterInfo.data.index));
                }
            }

            Shader shader = Shader.Find("UI/Default Font");
            Material material = new Material(shader);
            material.mainTexture = selectedBitmap;

            Font font = new Font(selectedBitmap.name);
            font.characterInfo = characterInfos.ToArray();
            font.material = material;

            string selectionPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedBitmap));
            AssetDatabase.CreateAsset(material, Path.Combine(selectionPath, selectedBitmap.name + ".mat"));
            AssetDatabase.CreateAsset(font, Path.Combine(selectionPath, selectedBitmap.name + ".fontsettings"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        */

        #endregion

        public static void CreateTextAsset (string content, string fileNamePath) {
            File.WriteAllText(fileNamePath, content);
            AssetDatabase.Refresh();
        }
    }
}

