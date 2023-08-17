using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Utility
{
    #if UNITY_EDITOR
    public sealed class FindObjectReferenceInScene : EditorWindow
    {
        // ReSharper disable once UnusedMember.Local
        [MenuItem("GameObject/FindObjectReference", true, 10)]
        private static bool CanFind()
        {
            return Selection.gameObjects.Any();
        }

        // ReSharper disable once UnusedMember.Local
        [MenuItem("GameObject/FindObjectReference", false, 10)]
        private static void Find()
        {
            var findResults = Selection.gameObjects
                .Where(x => x.scene.IsValid())
                .Select(gameObject => Find(gameObject))
                .ToList();

#if false
        // 仮のログ出力
        foreach (var findResult in findResults)
        {
            foreach (var info in findResult.ReferenceInfos)
            {
                var format = "{0} is referenced from [{1}]";
                Debug.Log(string.Format(format, findResult.SelectedGameObject.name, info), info.ResultComponent);
            }
        }
#endif

            var window = GetWindow<FindObjectReferenceInScene>();
            window.SetResults(findResults);
        }

        private static FindResult Find(GameObject selectedGameObject)
        {
            var findObjects = new FindResult(selectedGameObject);

            // 検索対象が所属しているシーンからのみ検索する
            // プレイ中のDontDestoroyOnLoad内のあるオブジェクトが
            // あるシーンのオブジェクトの参照を持っているという特殊な状態は考慮しない
            var scene = selectedGameObject.scene;
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                var findObjectsInGameObject = FindReferenceInGameObjectAndItsChildren(rootGameObject, selectedGameObject);
                findObjects.AddRange(findObjectsInGameObject);
            }

            return findObjects;
        }

        private static IEnumerable<FindResult.ReferenceInfo> FindReferenceInGameObjectAndItsChildren(GameObject gameObject, GameObject selectedGameObject)
        {
            var findInfos = new List<FindResult.ReferenceInfo>();
            var cacheComponents = new List<Component>();
            gameObject.GetComponents(cacheComponents);
            foreach (var component in cacheComponents)
            {
                if (component == null)
                {
                    continue;
                }

                var serializeObject = new SerializedObject(component);
                var iterator = serializeObject.GetIterator();

                while (iterator.NextVisible(true))
                {
                    if (iterator.isArray && iterator.type != "string")
                    {
                        var findObjectsInArray = FindReferenceInArray(iterator, selectedGameObject, component);
                        findInfos.AddRange(findObjectsInArray);
                    }
                    else if (CompareReference(iterator, selectedGameObject))
                    {
                        findInfos.Add(new FindResult.ReferenceInfo(component, iterator.displayName));
                    }
                }
            }

            // 子階層に対して再帰的処理で検索していく
            for (var i = 0; i < gameObject.transform.childCount; ++i)
            {
                var child = gameObject.transform.GetChild(i).gameObject;
                var findObjectsInChildren = FindReferenceInGameObjectAndItsChildren(child, selectedGameObject);
                findInfos.AddRange(findObjectsInChildren);
            }

            return findInfos;
        }

        private static IEnumerable<FindResult.ReferenceInfo> FindReferenceInArray(SerializedProperty serializedProperty, GameObject selectedGameObject, Component component)
        {
            var findInfos = new List<FindResult.ReferenceInfo>();
            var arrayName = serializedProperty.displayName;
            var arraySize = serializedProperty.arraySize;

            serializedProperty.NextVisible(true);

            for (var i = 0; i < arraySize; ++i)
            {
                if (!serializedProperty.NextVisible(true))
                {
                    break;
                }

                if (CompareReference(serializedProperty, selectedGameObject))
                {
                    findInfos.Add(new FindResult.ReferenceInfo(component, arrayName, i));
                }
            }

            return findInfos;
        }

        private static bool CompareReference(SerializedProperty serializedProperty, Object selectedGameObject)
        {
            if (serializedProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                return false;
            }

            if (serializedProperty.objectReferenceValue is GameObject || serializedProperty.objectReferenceValue is ScriptableObject)
            {
                return serializedProperty.objectReferenceValue == selectedGameObject;
            }

            var component = serializedProperty.objectReferenceValue as Component;
            if (component != null)
            {
                return component.gameObject == selectedGameObject;
            }

            return false;
        }

        private class FindResult
        {
            public GameObject SelectedGameObject { get; private set; }
            public List<ReferenceInfo> ReferenceInfos { get; private set; }

            public FindResult(GameObject selectedGameObject)
            {
                SelectedGameObject = selectedGameObject;
                ReferenceInfos = new List<ReferenceInfo>();
            }

            public void AddRange(IEnumerable<ReferenceInfo> infos)
            {
                if (infos == null)
                {
                    throw new System.ArgumentNullException("infos");
                }

                ReferenceInfos.AddRange(infos);
            }

            public class ReferenceInfo
            {
                public Component ResultComponent { get; private set; }
                public string FieldName { get; private set; }
                public int ArrayIndex { get; private set; }

                public ReferenceInfo(Component component, string fieldName)
                {
                    ResultComponent = component;
                    FieldName = fieldName;
                    ArrayIndex = -1;
                }

                public ReferenceInfo(Component component, string fieldName, int arrayIndex)
                {
                    ResultComponent = component;
                    FieldName = fieldName;
                    ArrayIndex = arrayIndex;
                }

                public override string ToString()
                {
                    if (ArrayIndex != -1)
                    {
                        const string format = "GameObject[{0}], Component[{1}], ArrayField[{2}], Index[{3}]";
                        return string.Format(format, ResultComponent.gameObject.name, ResultComponent.GetType(), FieldName, ArrayIndex);
                    }
                    else
                    {
                        const string format = "GameObject[{0}], Component[{1}], Field[{2}]";
                        return string.Format(format, ResultComponent.gameObject.name, ResultComponent.GetType(), FieldName);
                    }
                }
            }
        }

        #region EditorWindow

        private List<FindResultContent> _findObjects;
        private Vector2 _scrollPosition;
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        private void SetResults(List<FindResult> findObjects)
        {
            if (findObjects == null)
            {
                throw new System.ArgumentNullException("findObjects");
            }

            _findObjects = findObjects
                .Select(x => new FindResultContent(x))
                .ToList();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            _findObjects = null;
        }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            if (_findObjects == null)
            {
                return;
            }

            using (var svScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = svScope.scrollPosition;

                foreach (var group in _findObjects)
                {
                    DrawFindObjects(group);
                }
            }
        }

        private void DrawFindObjects(FindResultContent resultContent)
        {
            using (var _ = new EditorGUILayout.HorizontalScope("box", GUILayout.Width(position.width * 0.96f)))
            {
                DrawSelectedGameObject(resultContent.SelectedGameObjectGuiContent);
                PingObjectIfOnMouseDown(resultContent.SelectedGameObject);

                DrawFindResults(resultContent);
            }
        }

        private void DrawSelectedGameObject(GUIContent selectedContent)
        {
            var iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(Vector2.one * 32);

            GUILayout.Label(selectedContent, GUILayout.Width(position.width * 0.45f), GUILayout.ExpandWidth(false));

            EditorGUIUtility.SetIconSize(iconSize);
        }

        private void DrawFindResults(FindResultContent resultContent)
        {
            var iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(Vector2.one * 16);

            var width = position.width * 0.45f;
            using (var _ = new EditorGUILayout.VerticalScope(GUILayout.Width(width)))
            {
                foreach (var infoContentGroup in resultContent.ReferenceInfoGuiContents.GroupBy(x => x.ReferenceGameObject, (gameObject, contents) =>
                {
                    var referenceInfoContents = contents.ToList();
                    return new
                    {
                        KeyObject = gameObject,
                        KeyGuiContent = referenceInfoContents.First().ReferenceGameObjectGuiContent,
                        Value = referenceInfoContents
                    };
                }))
                {
                    var id = resultContent.SelectedGameObject.GetInstanceID() + "_" + infoContentGroup.KeyObject.GetInstanceID();
                    if (!_foldouts.ContainsKey(id))
                    {
                        _foldouts.Add(id, false);
                    }

                    _foldouts[id] = EditorGUILayout.Foldout(_foldouts[id], infoContentGroup.KeyGuiContent, true);

                    if (_foldouts[id])
                    {
                        EditorGUI.indentLevel++;
                        foreach (var infoContent in infoContentGroup.Value)
                        {
                            EditorGUILayout.LabelField(infoContent.GetComponentInfoText(), EditorStyles.miniLabel, GUILayout.Width(width), GUILayout.ExpandWidth(true));
                            PingObjectIfOnMouseDown(infoContentGroup.KeyObject);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUIUtility.SetIconSize(iconSize);
        }

        private static void PingObjectIfOnMouseDown(GameObject pingObject)
        {
            if (Event.current.type != EventType.MouseDown)
            {
                return;
            }

            if (!GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                return;
            }

            Selection.activeGameObject = pingObject;
            EditorGUIUtility.PingObject(pingObject);
        }

        private class FindResultContent
        {
            private readonly FindResult _findResult;

            public FindResultContent(FindResult findResult)
            {
                if (findResult == null)
                {
                    throw new System.ArgumentNullException("findResult");
                }

                _findResult = findResult;
                SelectedGameObjectGuiContent = new GUIContent(EditorGUIUtility.ObjectContent(SelectedGameObject, typeof(GameObject)));
                ReferenceInfoGuiContents = ReferenceInfos.Select(x => new ReferenceInfoContent(x)).ToList();
            }

            public GameObject SelectedGameObject
            {
                get { return _findResult.SelectedGameObject; }
            }

            private List<FindResult.ReferenceInfo> ReferenceInfos
            {
                get { return _findResult.ReferenceInfos; }
            }

            public GUIContent SelectedGameObjectGuiContent { get; private set; }

            public List<ReferenceInfoContent> ReferenceInfoGuiContents { get; private set; }

            public class ReferenceInfoContent
            {
                private readonly FindResult.ReferenceInfo _referenceInfo;

                public ReferenceInfoContent(FindResult.ReferenceInfo referenceInfo)
                {
                    if (referenceInfo == null)
                    {
                        throw new System.ArgumentNullException("referenceInfo");
                    }

                    _referenceInfo = referenceInfo;
                    ReferenceGameObjectGuiContent = new GUIContent(EditorGUIUtility.ObjectContent(referenceInfo.ResultComponent.gameObject, referenceInfo.ResultComponent.GetType()));
                }

                public GameObject ReferenceGameObject
                {
                    get { return _referenceInfo.ResultComponent.gameObject; }
                }

                public GUIContent ReferenceGameObjectGuiContent { get; private set; }

                public string GetComponentInfoText()
                {
                    if (_referenceInfo.ArrayIndex != -1)
                    {
                        const string format = "{0}.{1}[{2}]";
                        return string.Format(format, _referenceInfo.ResultComponent.GetType().Name, _referenceInfo.FieldName, _referenceInfo.ArrayIndex);
                    }
                    else
                    {
                        const string format = "{0}.{1}";
                        return string.Format(format, _referenceInfo.ResultComponent.GetType().Name, _referenceInfo.FieldName);
                    }
                }
            }
        }

        #endregion
    }

#endif
}
