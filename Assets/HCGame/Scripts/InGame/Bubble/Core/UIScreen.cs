using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BubblesShot
{
    public class ScreenInfo
    {
        private Dictionary<string, object> dict;

        public ScreenInfo()
        {
            this.dict = new Dictionary<string, object>();
        }

        public void Add(string key, object obj)
        {
            if (!this.dict.ContainsKey(key))
            {
                this.dict.Add(key, obj);
            }
            else
            {
                throw new System.Exception("ScreenInfo already contains an entry with the key: " + key);
            }
        }

        public void Replace(string key, object obj)
        {
            if (this.dict.ContainsKey(key))
            {
                this.dict[key] = obj;
            }
            else
            {
                throw new System.Exception("ScreenInfo does not contain an entry with the key: " + key);
            }
        }

        public bool KeyExists(string key)
        {
            return this.dict.ContainsKey(key);
        }

        public T TryGet<T>(string key)
        {
            return this.Get<T>(key, false);
        }

        public T Get<T>(string key)
        {
            return this.Get<T>(key, true);
        }

        private T Get<T>(string key, bool assert)
        {
            if (this.dict.ContainsKey(key))
                return (T)this.dict[key];
            else
            {
                if (assert)
                    throw new System.Exception("ScreenInfo does not contain an entry for key: " + key);

                return default(T);
            }
        }
    }

    abstract public class UIScreen : MonoBehaviour
    {
        public bool destroyOnPop = true;
        public bool hideOnFocusLost = false;

        public string screenName;

        [HideInInspector]
        public bool hasBeenSetup;

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }


        public static UIScreen Create(UIScreen screen, string screenName, bool deactivate = false)
        {
            GameObject obj = Instantiate(screen.gameObject);
            UIScreen newScreen = obj.GetComponent<UIScreen>();
            newScreen.screenName = screenName;

            Canvas sceenCanvas = newScreen.gameObject.GetComponent<Canvas>();
            if (sceenCanvas != null && UICameraManager.Instance != null)
            {
                sceenCanvas.worldCamera = UICameraManager.Instance.HomeUICamera;
                if (sceenCanvas.worldCamera)
                {
                    if (sceenCanvas.gameObject.layer != UICameraManager.Instance.HomeUICamera.gameObject.layer)
                    {
                        sceenCanvas.gameObject.ChangeLayer(UICameraManager.Instance.HomeUICamera.gameObject.layer);
                    }
                }
            }

            if (!deactivate)
            {
                newScreen.Setup();
                newScreen.hasBeenSetup = true;
            }
            else
            {
                newScreen.hasBeenSetup = false;
            }

            return newScreen;
        }

        /// <summary>
        /// Use this to run inital setup on the screen. This is only ever called once when the screen is created.
        /// If the screen was cached and is being reactivated, this is not called again... use PushRoutine
        /// to setup cached screens if that is necessary.
        /// </summary>
        virtual public void Setup()
        {
            //Call only once
        }

        /// <summary>
        /// This is called after the UIManager activates or creates the screen during the pushing process.
        /// Note that this is called AFTER Setup during the instantiation process.
        /// Use this to run anything like animations or other scripted sequences.
        /// </summary>
        virtual public IEnumerator PushRoutine(ScreenInfo info)
        {
            yield return null;
        }

        /// <summary>
        /// This is called just before the UIManager deactivates or destroys the screen during the popping process.
        /// Use this to run anything like animations or other scripted sequences before the UIManager deals with it.
        /// </summary>
        virtual public IEnumerator PopRoutine()
        {
            yield return null;
        }

        virtual public void CloseScreen()
        {
            UIManager.Instance.HidePopup();
        }

        /// <summary>
        /// This is called when the screen is back on the top of the screen stack.
        /// </summary>
        virtual public void OnFocus()
        {
            //TODO update tween here
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// This is called when the screen is pushed down the screen stack by another screen.
        /// </summary>
        virtual public void OnLostFocus()
        {
            // outro tween is played in UIManager::QueuePush
            // this is called by UIManager::Update > PushEnumerator after outro tween finishes
            if (this.hideOnFocusLost)
            {
                //TODO add tween here later
                Logger.Log($"OnLostFocus 1: {gameObject.name}");
                gameObject.SetActive(false);
            }
        }
    }


}