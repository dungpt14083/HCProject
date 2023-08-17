using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using NBCore;

//using NBCoreEditor.Command;
//using NBCoreEditor.Setting;
//using NBCoreEditor.MetaFile;
using UnityEditor.Callbacks;

namespace NBCoreEditor {
    public class NBCoreMenu {
        /*
        [MenuItem("GameObject/Capture Hierarchy...", false, 11)]
        private static void GameObject_CaptureHierarchy()
        {
            var window = EditorWindow.GetWindow<HierarchyCapturerWindow>(true, "Hierarchy Capturer", true);
            window.ShowPopup();
        }
        */

        #region Compress/Extract Assets

//        [MenuItem("Assets/Compress", false, 13)]
//        private static void Assets_CompressAssets () {
//            List<string> selectedPaths = new List<string>();
//            foreach (var assetGUID in Selection.assetGUIDs) {
//                var path = AssetDatabase.GUIDToAssetPath(assetGUID);
//                selectedPaths.Add(path);
//            }
//            if (selectedPaths.Count > 0) {
//                var command = new CompressCommand();
//                command.Execute(selectedPaths);
//            }
//        }
//
//        [MenuItem("Assets/Compress", true)]
//        private static bool Assets_ValidateCompressAssets () {
//            if (Selection.assetGUIDs.Length == 0) {
//                return false;
//            }
//
//            foreach (var assetGUID in Selection.assetGUIDs) {
//                var path = AssetDatabase.GUIDToAssetPath(assetGUID);
//                if (Path.GetExtension(path) == CompressCommand.Extension)
//                    return false;
//            }
//            return true;
//        }
//
//        [MenuItem("Assets/Extract", false, 14)]
//        private static void Assets_ExtractAssets () {
//            var zipFiles = new List<string>();
//            foreach (var assetGUID in Selection.assetGUIDs) {
//                var path = AssetDatabase.GUIDToAssetPath(assetGUID);
//                if (Path.GetExtension(path) == CompressCommand.Extension)
//                    zipFiles.Add(path);
//            }
//
//            if (zipFiles.Count > 0) {
//                var command = new ExtractCommand();
//                command.Execute(zipFiles);
//            }
//        }
//
//        [MenuItem("Assets/Extract", true)]
//        private static bool Assets_ValidateExtractAssets () {
//            foreach (var assetGUID in Selection.assetGUIDs) {
//                var path = AssetDatabase.GUIDToAssetPath(assetGUID);
//                if (Path.GetExtension(path) == CompressCommand.Extension)
//                    return true;
//            }
//            return false;
//        }

        #endregion

        [MenuItem("NBCore/Edit Settings", false, 1)]
        public static void NBCore_EditSettings () {
            Selection.activeObject = NBCoreSettings.instance;
        }

        [MenuItem("Assets/Create/Data Asset")]
        public static void Assets_CreateDataAsset () {
            AssetGenerator.CreateDataAsset(Selection.activeObject);
        }

        // TODO : review later


//
//        [MenuItem("Assets/Create/Generate Bitmap Font")]
//        private static void Assets_GenerateBitmapFont () {
//            List<BitmapCharacterInfo> bitmapCharacterInfos = AssetGenerator.GetBitmapCharacterInfos(Selection.activeObject);
//            if (bitmapCharacterInfos == null)
//                return;
//
//            if (AssetGenerator.IsFullBitmapFont(bitmapCharacterInfos)) {
//                AssetGenerator.CreateBitmapFont(Selection.activeObject as Texture2D, bitmapCharacterInfos);
//            } else {
//                var window = EditorWindow.GetWindow<BitmapFontBuilderWindow>(true, "BitmapFont Builder", true);
//                window.ShowPopup();
//            }
//        }



//        [MenuItem("NBCore/Init Project", false, 2)]
//        public static void UBlock_InitProjectFolder () {
//            new InitProjectCommand().Execute(null);
//        }
//
//        [MenuItem("UBlock/Window/Toolbar", false, 51)]
//        public static void Window_Toolbar () {
//            var window = EditorWindow.GetWindow<ToolbarWindow>(false, "Toolbar", true);
//            window.minSize = new Vector2(680.0f, 30f);
//            window.maxSize = new Vector2(680.0f, 30f);
//            window.Show();
//        }
//
//        [MenuItem("UBlock/Window/Terminal")]
//        public static void Window_Terminal () {
//            var window = EditorWindow.GetWindow<TerminalWindow>(false, "Terminal", true);
//            window.minSize = new Vector2(320, 240);
//            window.Show();
//        }
//
//        // Add menu item named "GameObject Info" to the Window menu
//        [MenuItem("UBlock/Window/GameObject Info", false, 52)]
//        public static void UBlock_GameObjectInfo () {
//            //Show existing window instance. If one doesn't exist, make one.
//            var window = EditorWindow.GetWindow<GameObjectInfoWindow>(true, "GO Info", true);
//            window.ShowPopup();
//        }
//
//        [MenuItem("UBlock/Build AssetBundles...", false, 101)]
//        public static void UBlock_BuildAssetBundles () {
//            var window = EditorWindow.GetWindow<AssetBundlesGeneratorWindow>(true, "AssetBundle Generator", true);
//            window.versionInfo = BuildScript.InitVersionInfo();
//            window.ShowPopup();
//        }
    }
}