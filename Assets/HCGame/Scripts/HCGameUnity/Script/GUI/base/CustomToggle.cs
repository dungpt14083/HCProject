using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : Toggle
{
    public Action OnCustomToggleClicked;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnCustomToggleClicked?.Invoke();
    }
}