using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.CheckCameraCanvas();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        this.CheckCameraCanvas();
    }

    private void CheckCameraCanvas()
    {
        if (this.gameObject.TryGetComponent<Canvas>(out Canvas canvasGame))
        {
            canvasGame.worldCamera = Camera.main;
            if (canvasGame.worldCamera == null)
            {
                GameObject cameraObject = new GameObject("Canvas Camera");
                cameraObject.transform.SetParent(canvasGame.transform);
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Depth;
                camera.depth = 0;
                canvasGame.worldCamera = camera;
            }
        }
        
    }
}