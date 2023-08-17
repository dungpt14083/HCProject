using UnityEngine;
using TMPro;
public class FpsDisplay : MonoBehaviour
{
    private float[] _deltaTimes = new float[600];
    private int _deltaTimeIndex = 0;

  

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(50, 250, 100,100),"FPS: "+ (1.0f / Time.smoothDeltaTime).ToString(),style);
    }


}