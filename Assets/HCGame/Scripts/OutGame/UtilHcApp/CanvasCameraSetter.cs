using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    public Camera mainCamera; // Camera chính
    private Canvas canvas; // Canvas cần được gán camera

    private void Awake()
    {
        // Tìm và lấy canvas
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        // Nếu canvas không có camera thì tìm camera được gán vào mặc định trong Unity
        canvas.worldCamera = Camera.main;


        // Nếu canvas vẫn không có camera thì tìm camera được chỉ định
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = mainCamera;
        }

        // Nếu canvas vẫn không có camera thì tạo một camera mới và gán vào canvas
        if (canvas.worldCamera == null)
        {
            GameObject cameraObject = new GameObject("Canvas Camera");
            cameraObject.transform.SetParent(canvas.transform);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.depth = 0;
            canvas.worldCamera = camera;
        }
    }
}