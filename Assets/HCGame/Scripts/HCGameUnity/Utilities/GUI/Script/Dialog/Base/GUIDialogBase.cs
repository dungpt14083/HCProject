using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NBCore
{

    public enum GUIPanelHideAction
    {
        Disable = 0,
        Destroy,
    }


    public enum ShowDialogType
    {
        None,
        Move,
        Alpha,
        Pumb
    } // ShowDialogType


    public class GUIDialogBase : MonoBehaviour
    {
        public enum GUIPanelStatus
        {
            Invalid,
            Ok,
            Showing,
            Showed,
            Hiding,
            Hidden
        }

        public const string SHOW_TWEEN_NAME = "show";
        public const string HIDE_TWEEN_NAME = "hide";
        protected const string NBCORE_PRELOAD_PATH = "Assets/NBCore/GUI/Resources/Prefab/GUI/ZPreloadGUI.prefab";

        // Hide action
        public string dialogPrefab = "";
        public string locationName = "UIContent";
        public int layer = 0;
        public GUIPanelHideAction hideAction;
        public float destroyTimeout = 120;
        public ShowDialogType showDialogType;

        // Show/Hide tween

        public bool useBlackBolder = false;
        public iTween.EaseType tweenType;
        public float showDelay;
        public Vector2 startPostition = new Vector2 (0, 0);

        // Active state
        private Dictionary<GameObject, bool> activeSave;

        [HideInInspector]
        public GUIPanelStatus status = GUIPanelStatus.Invalid;
        private bool isHasAlpha = false;
        public bool isSetupStartLocation = false;
        public bool resetAnchor = true;


        [HideInInspector]
        public GameObject guiControlDlg;

        [HideInInspector]
        UIDialogController uiBaseDialogHandler;        

        public virtual async UniTask<GameObject> OnInit ()
        {
            try {
                Debug.Log ("Init Dialog : " + GUIManager.GuiPathPrefab + dialogPrefab);
                //var sample = await BGameResource.LoadAssetAsync<GameObject>(GUIManager.GuiPathPrefab + dialogPrefab + ".prefab");
                // var sample = await LGameResource.LoadAssetFromResources<GameObject>(GUIManager.GuiPathPrefab + dialogPrefab);
                var sample = await HCGameResource.LoadAssetAsync<GameObject>(GUIManager.GuiPathPrefab + dialogPrefab+ ".prefab");

                GameObject obj = Instantiate(sample, gameObject.transform) as GameObject;
                obj.name = dialogPrefab;

                if(true == isSetupStartLocation)
                {
                    obj.GetComponent<RectTransform>().anchoredPosition = startPostition;
                }

                // have to change pos by zero first
                //TODO : review
                //obj.transform.localPosition = Vector3.zero;
                //obj.transform.localScale = Vector3.one;
                //if (resetAnchor) 
                //{
                //    obj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                //    obj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                //}                

                //obj.transform.localPosition = new Vector3 (vectorSetupLocation.x, vectorSetupLocation.y, 0);
                //startPostition = obj.transform.position;

                uiBaseDialogHandler = gameObject.GetComponentInChildren<UIDialogController> ();
                return obj;
            } catch (Exception ex) {
                Debug.LogError ("Error:" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        public async UniTask<bool> Init ()
        {
            if (status >= GUIPanelStatus.Ok)
                return true;
            status = GUIPanelStatus.Invalid;

            try {
                guiControlDlg = await OnInit();
                if (guiControlDlg != null)
                {
                    status = GUIPanelStatus.Ok;
                }
                return true;
            } catch (System.Exception ex) {
                Debug.LogError ("Init GUI panel - Init " + this.GetType ().Name + "Exception - " + ex);
            }

            return false;
        }


        /// <summary>
        /// Tries the show.
        /// </summary>
        public async UniTask<bool> TryShow (params object[] parameter)
        {
            if (status == GUIPanelStatus.Invalid) {
                await Init ();
            }
            StopCoroutine ("WaitForDestroy");

            // End hiding
            if (status == GUIPanelStatus.Hiding) {
                StopTween ();
                OnEndHide (hideAction == GUIPanelHideAction.Destroy);
                status = GUIPanelStatus.Hidden;
            }

            if (status == GUIPanelStatus.Hidden || status == GUIPanelStatus.Ok) {
                status = GUIPanelStatus.Showing;

                RestoreActiveState (true);
                if (isHasAlpha) {
                    isHasAlpha = false;
                }
                StartCoroutine (DoShow (parameter));
                //Debug.LogWarning("3");
                return true;
            }

            if (status == GUIPanelStatus.Showed || status == GUIPanelStatus.Showing) {
                StartCoroutine (DoShow (parameter));
                //Debug.LogWarning("4");
                return true;
            }
            //Debug.LogWarning("5");
            return false;
        }

        private IEnumerator DoShow (params object[] parameter)
        {
            yield return new WaitForSeconds (showDelay);

            float wait = OnBeginShow (parameter);

            if (wait > 0)
                yield return new WaitForSeconds (wait);
            else
                yield return null;
            if (status == GUIPanelStatus.Showing) {
                OnEndShow ();
                status = GUIPanelStatus.Showed;
            }
        }


        /// <summary>
        /// Hides the specified after time out.
        /// </summary>
        public void Hide (object parameter)
        {
            if (gameObject == null)
                return;

            // End showing
            if (status == GUIPanelStatus.Showing) {
                //      iTween.Stop(guiControlLocation);
                StopTween ();
                OnEndShow ();
                status = GUIPanelStatus.Showed;
            }

            if (status == GUIPanelStatus.Showed) {
                status = GUIPanelStatus.Hiding;

                SaveActiveState ();

                StartCoroutine (DoHide (parameter));
            }
            //    GuiManager.instance.CheckShowBorder();
        }

        /// <summary>
        /// Does the hide.
        /// </summary>
        private IEnumerator DoHide (object parameter)
        {
            float wait = OnBeginHide (parameter);
            if (wait > 0)
                yield return new WaitForSeconds (wait);
            else
                yield return null;

            if (status == GUIPanelStatus.Hiding) {
                OnEndHide (hideAction == GUIPanelHideAction.Destroy);

                switch (hideAction) {
                case GUIPanelHideAction.Disable:
                    guiControlDlg.SetActive (false);
                    status = GUIPanelStatus.Hidden;
                    break;

                case GUIPanelHideAction.Destroy:
                    guiControlDlg.SetActive (false);
                    status = GUIPanelStatus.Hidden;
                    StartCoroutine ("WaitForDestroy");
                    break;
                }
            }
        }

        IEnumerator WaitForDestroy ()
        {
            yield return new WaitForSeconds (destroyTimeout);
            Destroy (guiControlDlg);
            status = GUIPanelStatus.Invalid;
        }


        #region Call to controller

        /// <summary>
        /// Called when [begin show].
        /// </summary>
        protected virtual float OnBeginShow (params object[] parameter)
        {
            if (uiBaseDialogHandler != null) {
                uiBaseDialogHandler.OnBeginShow (parameter);
            }// if
            //        GUIManager.Instance.CheckShowBorder();
            return DoTween (SHOW_TWEEN_NAME);
        }

        /// <summary>
        /// Called when [end show].
        /// </summary>
        protected virtual void OnEndShow ()
        {
            //        GUIManager.Instance.CheckShowBorder();
        }

        /// <summary>
        /// Called when [begin hide].
        /// </summary>
        protected virtual float OnBeginHide (object parameter)
        {
            if (uiBaseDialogHandler != null) {
                uiBaseDialogHandler.OnBeginHide (parameter);
            }
            return DoTween (HIDE_TWEEN_NAME);
        }

        #endregion

        #region Tween

        private void StopTween ()
        {
            iTween.Stop (uiBaseDialogHandler.gameObject);
            return;
        }
        // StopTween ()


        /// <summary>
        /// Does the tween.
        /// </summary>
        protected float DoTween (string tweenName)
        {
            float timeMove = 0.5f;

            //            Debug.LogWarning("base pos : " + this.transform.position);
            switch (showDialogType) {
            case ShowDialogType.None:
                uiBaseDialogHandler.transform.position = this.transform.position;
                break;
            //    return doMoveTween (tweenName, 0);
            case ShowDialogType.Move:
                return doMoveTween (tweenName, timeMove);
            }
            return 0;
        }



        private float doMoveTween (string tweenName, float timeMove)
        {

            switch (tweenName) {
            case SHOW_TWEEN_NAME:
                //uiBaseDialogHandler.transform.DOMove (this.transform.position, timeMove);
                    uiBaseDialogHandler.gameObject.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, timeMove, true);
                    break;
            case HIDE_TWEEN_NAME:
                uiBaseDialogHandler.gameObject.GetComponent<RectTransform>().DOAnchorPos(startPostition, timeMove, true);
                break;
            } // switch
            return timeMove;

        }// doMoveTween ()

        #endregion



        /// <summary>
        /// Called when [end hide].
        /// </summary>
        protected virtual void OnEndHide (bool isDestroy)
        {
        }

        /// <summary>
        /// Saves the state of the active.
        /// </summary>
        protected void SaveActiveState ()
        {
            if (guiControlDlg == null)
                return;

            //Debug.LogError("save");
            if (activeSave == null)
                activeSave = new Dictionary<GameObject, bool> ();
            else
                activeSave.Clear ();

            activeSave [guiControlDlg] = gameObject.activeSelf;

            Transform [] children = gameObject.GetComponentsInChildren<Transform> (true);
            foreach (var child in children) {
                activeSave[child.gameObject] = child.gameObject.activeSelf;
            }
        }

        /// <summary>
        /// Restores the state of the active.
        /// </summary>
        protected void RestoreActiveState (bool defaultState)
        {
            if (gameObject == null || activeSave == null)
                return;

            //Debug.LogError("restore");
            bool isActive = false;
            if (activeSave.TryGetValue (gameObject, out isActive))
                gameObject.SetActive (isActive);

            Transform [] children = gameObject.GetComponentsInChildren<Transform> (true);
            foreach (var child in children) {
                if (activeSave.TryGetValue (child.gameObject, out isActive))
                {
                    child.gameObject.SetActive(isActive);
                }
                //else
                //{
                //    child.gameObject.SetActive(defaultState);
                //}
            }
        }

        public void ApplyDepthPosition (GameObject guiObject)
        {
            if (guiObject == null)
                return;
            if (layer >= 15) {
                layer = 15;
            }
            Vector3 pos = guiObject.transform.localPosition;
            if (layer >= 0) {
                pos.z = -20 - layer * 10;
            } else {
                pos.z = -20 - 150;
            }
            guiObject.transform.localPosition = pos;
        }

        #region load dialog to handler

#if UNITY_EDITOR
        public void Load ()
        {
            //GameObject root = addRootUI ();
            GameObject root = this.gameObject;

            Transform _rootTran = root.transform;
            if (!_rootTran.Find (dialogPrefab)) {
                string prefabPath = GUIManager.GuiEditorResources + GUIManager.GuiPathPrefab + dialogPrefab + ".prefab";
                Debug.Log ("prefab path : " + prefabPath);
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

                //GameObject prefab = BGameResource.LoadAssetFromResources<GameObject>(prefabPath);
                GameObject obj = null;

                obj = (GameObject)PrefabUtility.InstantiatePrefab (prefab, _rootTran);

                obj.name = dialogPrefab;
                //obj.transform.SetParent (_rootTran);
                //obj.transform.localScale = Vector3.one;
                //obj.transform.localPosition = Vector3.zero;
                //obj.GetComponent<RectTransform> ().offsetMin = Vector2.zero;
                //obj.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
            } // if
        }
        // Load ()

        public void UnLoad ()
        {
            //if (GameObject.Find ("ZPreloadGUI") == null) {
            //    GameObject _root = new GameObject ();
            //    _root.name = "ZPreloadGUI";
            //    //      _root.AddComponent<UIRootBase>();
            //}
            //Transform _rootTran = GameObject.Find ("ZPreloadGUI").transform;
            //if (_rootTran.Find (dialogPrefab)) {
            //    GameObject obj = _rootTran.Find (dialogPrefab).gameObject;
            //    GameObject.DestroyImmediate (obj);
            //}

            var dialog = this.transform.Find(dialogPrefab);
            if(dialog != null)
            {
                GameObject.DestroyImmediate(dialog.gameObject);
            }
        }


        public void Save ()
        {
            //            Transform root = addRootUI().transform;
            //
            //            if (root.Find(dialogPrefab)) {
            //                GameObject obj = root.Find(dialogPrefab).gameObject;
            //                PrefabUtility.CreatePrefab("Assets/NBCore/Resources/" + GUI_PATH_PREFAB + dialogPrefab + ".prefab", obj);
            //                GameObject.DestroyImmediate(obj);
            //            } // if
        }
        // Save ()


        private GameObject addRootUI ()
        {
            GameObject result = GameObject.Find ("ZPreloadGUI");
            if (result == null) {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath (NBCORE_PRELOAD_PATH, typeof (GameObject));
                result = (GameObject)PrefabUtility.InstantiatePrefab (prefab);
                //      result = new GameObject ();
                //      result.name = "ZPreloadGUI";
                //      Canvas canvas = result.AddComponent<Canvas>();
                //      canvas.pixelPerfect = true;
                //      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                //      _root.AddComponent<UIRootBase>();
            } // if
              //            Debug.Log("position :" + result.transform.position + "===");
            return result;
        }
        // addRootUI ()
#endif

        #endregion
    }
} // NBCore

