using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace NBCore
{
    public class GUIManager : SingletonMono<GUIManager>
    {

        private GameObject guiSystem;

        public static string GuiEditorResources {
            get
            {
                if (NBCoreSettings.instance.isCore)
                {
                    return "Assets/NBCore/GUI/Resources/";
                } // if
                return "Assets/LumiGame/GameResources/";
            }
        }


        public static string GuiPathPrefab {
            get {
                if (NBCoreSettings.instance.isCore) {
                    return "Prefab/GUI/";
                } // if
                return "GUIData/Dialog/";
            } // get
        }

        private List<GUIDialogBase> listShowDialog = new List<GUIDialogBase> ();

        public Vector2 DefaultReferenceSolution { get; set; }

        // Use this for initialization
        void Start ()
        {
            //get Scaler mode
            CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler> ();
            DefaultReferenceSolution = canvasScaler.referenceResolution;
        }

        public void CloseAllWindow ()
        {

        }
        
        public bool IsShowDialog(GUIDialogBase panelController)
        {
            return listShowDialog.Contains(panelController);
        }


        public void ShowPanel (GUIDialogBase panelController)
        {
            Debug.Log ("show panel : " + panelController.GetType ().ToString ());
            ShowPanel (panelController, null);
        }

        public async void ShowPanel (GUIDialogBase panelController, params object[] parameter)
        {
            if (!listShowDialog.Contains (panelController)) {
                listShowDialog.Add (panelController);
            } // if
            await panelController.TryShow(parameter);
        }

        public void HidePanelAfterTime (GUIDialogBase panelController, float _time)
        {
            StartCoroutine (CoroutineHidePanel (panelController, _time));
        }

        private IEnumerator CoroutineHidePanel (GUIDialogBase panelController, float _time)
        {
            yield return new WaitForSeconds (_time);
            HidePanel (panelController);
        }

        public void HidePanel (GUIDialogBase panelController)
        {
            HidePanel (panelController, null);
        }

        public void HidePanel (GUIDialogBase panelController, object parameter)
        {
            panelController.Hide (parameter);
            if (listShowDialog.Contains (panelController)) {
                listShowDialog.Remove (panelController);
            } // if
        }
        // HidePanel ()


        public bool HideLastShowPanel ()
        {
            if (listShowDialog.Count <= 0) {
                return false;
            } // if
            GUIDialogBase prevPanel = listShowDialog [0];
            if (prevPanel == null) {
                listShowDialog.RemoveAt (0);
                return HideLastShowPanel ();
            } // if
            HidePanel (prevPanel);
            return true;
        }
        // HideLastShowPanel ()

    }
    // GuiManager

} // NBCore
