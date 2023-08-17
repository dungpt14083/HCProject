using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenShotGame : MonoBehaviour
{
    // chup man hinh game
#if UNITY_EDITOR

    [MenuItem("Tools/CMDEV/ScreenShotGame")]
    static void DoubleMass()
    {
        string name = "ScreenShot_" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second + ".png";
        ScreenCapture.CaptureScreenshot(name);
    }
#endif

}
