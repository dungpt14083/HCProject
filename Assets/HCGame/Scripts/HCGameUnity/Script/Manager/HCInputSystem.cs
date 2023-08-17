using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HCInputSystem
{
    HCInputAction inputActions;
    public Action<bool, Vector2> OnClick;
    public Action OnRHold;
    public Action<float, string> OnScroll;
    public Action<Vector2, Vector2> OnRDrag;
    public Action<Vector2, Vector2> OnLDrag;
    public Action<Vector2, Vector2> OnTouchDrag;

    public bool IsHold
    {
        get
        {
            return isDelayTouchHold || isLHold;
        }
    }

    private bool _isTouchControl = false;

    bool isScroll = false;
    bool clickOverUI = false;

    //touch 
    private bool isTouchHold = false;
    private bool isDelayTouchHold = false; // use for plant tree
    private Vector2 touchPos;
    private Vector2 touchDelta;

    //mouse
    bool isRHold = false;
    private bool isLHold = false;
    Vector2 mousePos;
    Vector2 mouseDelta;

    //zoom
    private bool isZoom = false;
    private Vector2 secondaryTouchPos;

    public Vector2 ScreenPosition
    {
        get
        {
            // Debug.Log("get screen position : " + isTouchHold +"===" + touchPos + "====" + mousePos);
            if (_isTouchControl)
            {
                return touchPos;
            }
            return mousePos;
        }
    }
    public Vector3 MouseDelta { get => mouseDelta; }


    // Start is called before the first frame update
    public void Init()
    {
        inputActions = new HCInputAction();

#if UNITY_ANDROID && !UNITY_EDITOR
        _isTouchControl = true;
#endif

        inputActions.PlayerInput.Click.started += (i) =>
        {
            // clickOverUI = checkIsOverUI(Input.mousePosition);
        };
        
        inputActions.PlayerInput.Click.performed += (i) =>
        {
            Debug.Log("input click performed : " + Input.mousePosition);
            clickOverUI = checkIsOverUI(Input.mousePosition);
            OnClick?.Invoke(clickOverUI, mousePos);
            clickOverUI = false;
            isLHold = false;
        };

        inputActions.PlayerInput.LeftHold.performed += (i) =>
        {
            isLHold = true;
            // clickOverUI = EventSystem.current.IsPointerOverGameObject();
        };
        
        
        inputActions.PlayerInput.LeftHold.canceled += i =>
        {
            // Debug.LogWarning("end touch====" );
            isLHold = false;
            OnLDrag?.Invoke(Vector2.zero, Vector2.zero);
        };
        
        
        inputActions.PlayerInput.Click.canceled += Click_canceled;

        inputActions.PlayerInput.Scroll.performed += (i) =>
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                isScroll = true;
        };
        inputActions.PlayerInput.Scroll.canceled += (i) =>
        {
            isScroll = false;
            OnScroll?.Invoke(0f, "stop");
        };

        inputActions.PlayerInput.RClick.performed += i =>
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                isRHold = true;
        };
        inputActions.PlayerInput.RClick.canceled += i =>
        {
            isRHold = false;
            OnRDrag?.Invoke(Vector2.zero, Vector2.zero);
        };

        inputActions.PlayerInput.TouchPress.performed += i =>
        {
            clickOverUI = checkIsOverUI(inputActions.PlayerInput.PrimaryFingerPosition.ReadValue<Vector2>());
            //Debug.LogWarning("shpt: perform tap " + "===" + EventSystem.current.IsPointerOverGameObject() + "===" + clickOverUI);
            OnClick?.Invoke(clickOverUI, touchPos);
        };

        inputActions.PlayerInput.TouchHold.started += i =>
        {
            //if (!checkIsOverUI(inputActions.PlayerInput.PrimaryFingerPosition.ReadValue<Vector2>()))
            //{
            //    Debug.LogWarning("shpt: start touch hold" + "at frame = " + Time.frameCount);

            //}
        };

        inputActions.PlayerInput.TouchHold.performed += i =>
        {
            //Debug.LogWarning("shpt: perform touch hold");
            isTouchHold = true;
            isDelayTouchHold = true;
        };

        inputActions.PlayerInput.TouchHold.canceled += i =>
        {
            // Debug.LogWarning("end touch====" );
            isTouchHold = isDelayTouchHold = false;
            OnTouchDrag?.Invoke(Vector2.zero, Vector2.zero);
        };

        inputActions.PlayerInput.SecondaryTouchContact.started += i =>
        {
            isZoom = true;
            isTouchHold = isDelayTouchHold = false;
            touchPos = inputActions.PlayerInput.PrimaryFingerPosition.ReadValue<Vector2>();
            secondaryTouchPos = inputActions.PlayerInput.SecondaryFingerPosition.ReadValue<Vector2>();

            previousDistance = distance = Vector2.Distance(touchPos, secondaryTouchPos);

            Debug.LogWarning("secondary finger started");
        };
        inputActions.PlayerInput.SecondaryTouchContact.canceled += i =>
        {
            isZoom = false;
            isTouchHold = isDelayTouchHold = true;
            previousDistance = distance = 0;
            Debug.LogWarning("secondary finger ended");
        };
        inputActions.Enable();
    }

    private void Click_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log($"SHPT click canceled");
        // OnClick?.Invoke(clickOverUI, mousePos);
        // clickOverUI = false;
        // isLHold = false;
        // Debug.Log($"SHPT canceled click ui = {clickOverUI}");
    }

    private void handleScroll()
    {
        if (isScroll)
            OnScroll?.Invoke(inputActions.PlayerInput.Scroll.ReadValue<Vector2>().y * 0.6f, "");
    }

    private void handleRDrag()
    {
        if (isRHold)
        {
            //Debug.Log($"SHPT mouseDelta = {mouseDelta}, normalize X = {mouseDelta.x / Screen.width}, normalizeY = {mouseDelta.y / Screen.height}");
            OnRDrag?.Invoke(mousePos, mouseDelta);
        }
        if (isLHold)
        {
            OnLDrag?.Invoke(mousePos, mouseDelta);
        }
        if (isTouchHold )//&& !checkIsOverUI(inputActions.PlayerInput.PrimaryFingerPosition.ReadValue<Vector2>()))
        {
            OnTouchDrag?.Invoke(touchPos, touchDelta);
        }
    }

    private float previousDistance = 0f;
    private float distance = 0f;
    private void handleZoom()
    {
        if (isZoom)
        {
            var normalizeTouch = new Vector2(touchPos.x / Screen.width, touchPos.y / Screen.height);
            var normalize2Touch = new Vector2(secondaryTouchPos.x / Screen.width, secondaryTouchPos.y / Screen.height);
            distance = Vector2.Distance(normalizeTouch, normalize2Touch);
            //Debug.Log("handle Zoom : " + distance );
            var testValue = distance - previousDistance;
            //var info = $"SHPT normalizeTouch = {normalizeTouch} -- normalizeTouch2 = {normalize2Touch} -- distance = {distance} -- value = {testValue}";
            if (distance > previousDistance)
            {
                //zoom in
                // OnScroll?.Invoke(distance);
                OnScroll?.Invoke(testValue, "");

            }
            else
            {
                //zoom out
                // OnScroll?.Invoke(-1);
                OnScroll?.Invoke(testValue, "");
            }
            previousDistance = distance;
        }
    }

    public void UpdateTouchData()
    {
        touchPos = inputActions.PlayerInput.PrimaryFingerPosition.ReadValue<Vector2>();
        
        // Debug.Log("touch pos: " + touchPos);
        touchDelta = inputActions.PlayerInput.PrimaryFingerDelta.ReadValue<Vector2>();
        secondaryTouchPos = inputActions.PlayerInput.SecondaryFingerPosition.ReadValue<Vector2>();
    }

    // Update is called once per frame
    public void Tick()
    {
        mousePos = inputActions.PlayerInput.MousePos.ReadValue<Vector2>();
        mouseDelta = inputActions.PlayerInput.MouseDelta.ReadValue<Vector2>();

        UpdateTouchData();
        if (isDisabledEvent)
            return;
        handleScroll();
        handleRDrag();
        handleZoom();
    }

    private bool checkIsOverUI(Vector2 pos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool isDisabledEvent = false;
    public void DisableEvent()
    {
        isDisabledEvent = true;
    }

    public void EnableEvent()
    {
        isDisabledEvent = false;
    }
}
