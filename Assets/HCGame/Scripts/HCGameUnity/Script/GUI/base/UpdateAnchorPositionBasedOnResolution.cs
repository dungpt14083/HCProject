using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAnchorPositionBasedOnResolution : MonoBehaviour
{
    [Serializable]
    public class AnchorPositionOnResolution
    {
        public Vector2 anchorPosition;
        public float resolutionAspect;
    }

    private RectTransform _rectTransform;
    private Vector2 _currentResolution = new Vector2();

    [SerializeField] private List<AnchorPositionOnResolution> anchorPositions;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _currentResolution.x = Screen.width;
        _currentResolution.y = Screen.height;
        UpdateSize();
    }

    private void FixedUpdate()
    {
        if (_currentResolution.x != Screen.width || _currentResolution.y != Screen.height)
        {
            UpdateSize();

            _currentResolution.x = Screen.width;
            _currentResolution.y = Screen.height;
        }
    }

    public void UpdateSize()
    {
        if (null == _rectTransform)
        {
            Debug.LogError("UpdateSizeBaseOnResolution!!! Null rect transform!!!");
            return;
        }
        float currentAspect = ((float)Screen.width) / ((float)Screen.height);
        //Round currentAspect to 2 decimal place
        currentAspect = Mathf.Round(currentAspect * 100) / 100;

        foreach (AnchorPositionOnResolution anchorPosition in anchorPositions)
        {
            if (currentAspect == anchorPosition.resolutionAspect)
            {
                _rectTransform.anchoredPosition = anchorPosition.anchorPosition;
                return;
            }
        }
    }
}
