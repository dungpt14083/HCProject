using UnityEngine;

public class FPSDisplay : MonoBehaviour
{

    private const float INTERVAL_TIME = 1.0f;

    float lastInterval = 0.0f;
    float fps = 0.0f;
    float msec = 0.0f;
    int frames =0;
    void Start () {
        lastInterval = Time.realtimeSinceStartup;
    } // Start ()

    void Update()
    {
        frames++;
        var timeNow = Time.realtimeSinceStartup;

        if (timeNow >= lastInterval + INTERVAL_TIME) {
            fps = frames/(timeNow - lastInterval);
            msec = 1000.0f / fps;
            frames = 0;
            lastInterval = timeNow;
        } // if
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.0f, 0.0f, 1.0f, 1.0f);

        string text = string.Format("{0:0.0} ms ({1:0.} fps)", (int)msec, (int)fps);

        GUI.Label(rect, text, style);


//        //Device info
        //Rect graphicsInfoRect = new Rect(0, h*3 /100, w, h*2/100);
//        GUI.Label(graphicsInfoRect, SystemInfo.graphicsDeviceVersion, style);

        //drawCall
//        Rect drawCallRect = new Rect(0, h*6 /100, w, h*2/100);
//        GUI.Label(drawCallRect, string.Format("Draw Call : {0} ",UnityStats.drawCalls), style);
        //
        //map name
//        Rect mapNameRect = new Rect(0, h*6 /100, w, h*2/100);
//        GUI.Label(mapNameRect, string.Format("map name : {0}", GameData.Instance.mapName), style);
    }
}