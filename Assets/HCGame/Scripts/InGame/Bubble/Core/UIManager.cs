using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BubblesShot
{
    public class QueuedScreen
    {
        public UIScreen screen;
        public ScreenInfo info;
        public bool popIt;
        public string screenName;

        public QueuedScreen(UIScreen screen, ScreenInfo info, bool popIt)
        {
            this.screen = screen;
            this.info = info;
            this.popIt = popIt;
        }
    }

    public partial class UIManager : Singleton<UIManager>
    {
        protected List<UIScreen> pseudoStack = new List<UIScreen>();
        protected Queue<QueuedScreen> queue = new Queue<QueuedScreen>();
        protected Dictionary<System.Type, UIScreen> cache = new Dictionary<Type, UIScreen>();

        private string currentScreen;

        [HideInInspector]
        public bool isScreenBusy = false;

        public string CurrentScreen { get => currentScreen; set => currentScreen = value; }

        /// <summary>
        /// Returns the top most screen on the screen stack. Will return null if the screen stack is empty.
        /// </summary>
        public UIScreen TopScreen()
        {
            if (this.pseudoStack != null && this.pseudoStack.Count > 0)
            {
                return this.pseudoStack[0];
            }
            else
            {
                return null;
            }
        }

        public UIScreen PendingTopScreen()
        {
            if (this.pseudoStack.Count > 1)
            {
                return this.pseudoStack[1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the second top most screen on the screen stack. Will return null if the screen stack is empty.
        /// </summary>
        public UIScreen UnderTopScreen()
        {
            if (this.pseudoStack.Count > 1)
            {
                return this.pseudoStack[1];
            }
            else
            {
                return null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Push next screen in queue if there is no screen that is busy
            if (!isScreenBusy && queue.Count > 0)
            {
                QueuedScreen queuedScreen = queue.Dequeue();
                if (queuedScreen != null && queuedScreen.screen != null)
                {
                    if (queuedScreen.popIt)
                    {
                        StartCoroutine(PopEnumerator(queuedScreen.screen));
                    }
                    else
                    {
                        StartCoroutine(PushEnumerator(queuedScreen));
                    }
                }
            }
        }

        protected IEnumerator PushEnumerator(QueuedScreen queuedScreen)
        {
            this.isScreenBusy = true;

            System.Type screenType = queuedScreen.screen.GetType();
            bool isCached = this.cache.ContainsKey(screenType);

            UIScreen topScreen = this.TopScreen();
            UIScreen screen = null;

            // Figure out if the screen is already on top of the stack or not.
            bool alreadyOnTop;
            if (topScreen != null && topScreen.GetType() == screenType)
            {
                alreadyOnTop = true;
            }
            else
            {
                alreadyOnTop = false;
            }

            if (isCached)
            {
                // Turn cached screen back on.
                screen = this.cache[screenType];

                // prevent multiple intro tween when screen is pushed multiple times
                if (!alreadyOnTop)
                {
                    screen.gameObject.SetActive(true);

                    // refresh screen content when bringing it back from cache, since it could be outdated
                    screen.OnFocus();
                }
            }
            else
            {
                screen = UIScreen.Create(queuedScreen.screen, screenName: queuedScreen.screenName);
                this.cache.Add(screenType, screen);
            }

            // Push the new screen on to the psuedo stack.
            if (!alreadyOnTop)
            {
                if (isCached)
                {
                    this.pseudoStack.Remove(screen);
                }

                this.pseudoStack.Insert(0, screen);
            }

            // Wait for screen's PushRoutine to return...
            yield return StartCoroutine(screen.PushRoutine(queuedScreen.info));

            this.isScreenBusy = false;

            // Tell the screen on top of the stack that it has lost focus.
            if (topScreen != null && !alreadyOnTop)
            {
                topScreen.OnLostFocus();
            }

            yield return null;
        }

        protected IEnumerator PopEnumerator(UIScreen screen)
        {
            this.isScreenBusy = true;

            // Wait for the screen's PopRoutine to return...
            yield return StartCoroutine(screen.PopRoutine());

            if (screen.destroyOnPop)
            {
                this.cache.Remove(screen.GetType());

                //In some cases, the gameObject has already been destroyed by this point.  So we'll double-check and make sure the object still exista before trying to destroy it.
                if ((screen != null) && (screen.gameObject != null))
                {
                    Object.Destroy(screen.gameObject);
                }
            }
            else
            {
                screen.gameObject.SetActive(false);
            }

            // NOTE: The above command to Remove from the top is not correct, as there is a chance
            //  another screen could be added to the pseudoStack, from the time the 'then' top most screen was marked to be popped, to 'now', when it is actually popped
            // 'screen' variable is correctly the right screen that needs to be popped, so use that to find it in the pseudoStack.
            this.pseudoStack.Remove(screen);

            // Tell the screen on top of the stack that it has gained focus again.
            UIScreen topScreen = this.TopScreen();
            if (topScreen != null)
            {
                topScreen.OnFocus();
            }

            this.isScreenBusy = false;
        }

        public List<UIScreen> GetUIScreens()
        {
            return this.pseudoStack;
        }

        public void ShowPopup(string name, ScreenInfo info = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Logger.Log($"ShowPopup: {name}");

            GameObject obj = ResourceManager.Instance.Load<GameObject>(name);
            if (obj == null)
            {
                Logger.LogError("UIManager - ShowPopup - obj null");
                return;
            }

            UIScreen screen = obj.GetComponent<UIScreen>();
            if (screen == null)
            {
                Logger.LogError("UIManager - ShowPopup - screen null");
                return;
            }

            QueuedScreen queuedScreen = new QueuedScreen(screen, info, false);
            if (name.Contains("ToastMessage"))
            {
                queuedScreen.screenName = "ToastMessage";
            }

            this.queue.Enqueue(queuedScreen);
            currentScreen = name;
        }

        /// <summary>
        /// Pops the top most screen from the screen stack and adds it to the screen action queue.
        /// </summary>
        public UIScreen HidePopup()
        {
            UIScreen screen = this.GetNextUnpoppedScreen();

            if (screen == null)
            {
                return null;
            }

            this.queue.Enqueue(new QueuedScreen(screen, null, true));

            return screen;
        }

        /// <summary>
        /// Will continue to pop screens until a popped screen matches the specified type.
        /// </summary>
        public UIScreen HidePopup(System.Type type)
        {
            if (!this.cache.ContainsKey(type))
            {
                return null;
            }

            UIScreen screen = HidePopup();
            while (screen.GetType() != type)
            {
                screen = HidePopup();
            }

            return screen;
        }

        /// <summary>
        /// Will continue to pop screens until the screen on top of the stack matches the specified type.
        /// </summary>
        public void HidePopupToScreen(System.Type type)
        {
            if (!this.cache.ContainsKey(type))
            {
                return;
            }

            var screen = this.GetNextUnpoppedScreen();
            while (screen != null && screen.GetType() != type)
            {
                this.HidePopup();
                screen = this.GetNextUnpoppedScreen();
            }
        }

        public UIScreen CloseToastPopup()
        {
            UIScreen screen = null;
            for (int i = 0; i < pseudoStack.Count; i++)
            {
                if (!string.IsNullOrEmpty(pseudoStack[i].screenName) && pseudoStack[i].screenName.Contains("ToastMessage"))
                {
                    screen = pseudoStack[i];
                    continue;
                }
            }

            if (screen == null)
            {
                return null;
            }

            this.queue.Enqueue(new QueuedScreen(screen, null, true));

            return screen;
        }

        public UIScreen HidePopupByName(string popName)
        {
            UIScreen screen = null;
            for (int i = 0; i < pseudoStack.Count; i++)
            {
                if (!string.IsNullOrEmpty(pseudoStack[i].name) && pseudoStack[i].name.Contains(popName))
                {
                    screen = pseudoStack[i];
                    continue;
                }
            }

            if (screen == null)
            {
                return null;
            }

            this.queue.Enqueue(new QueuedScreen(screen, null, true));

            return screen;
        }

        /// <summary>
        /// Since we push and pop lazily. We need to count how many screens are currently in the queue to be popped.
        /// </summary>
        protected UIScreen GetNextUnpoppedScreen()
        {
            int lazyIndex = 0;
            foreach (QueuedScreen queuedScreen in this.queue)
            {
                if (queuedScreen.popIt)
                    lazyIndex++;
            }

            if (lazyIndex >= this.pseudoStack.Count)
                return null;

            return this.pseudoStack[lazyIndex];
        }

        public T GetScreen<T>() where T : UIScreen
        {
            System.Type type = typeof(T);

            if (this.cache.ContainsKey(type))
            {
                return this.cache[type] as T;
            }

            return null;
        }

        //Move it to ToastManager
        //public void ShowToastMessage(string message) {
        //  ScreenInfo info = new ScreenInfo();
        //  info.Add("Message", message);
        //    ShowPopup("AssetBundles/GameScene/Prefabs/windows/toast/ToastMessage", info);
        //}
    }
}