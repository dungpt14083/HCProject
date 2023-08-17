using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleButton : MonoBehaviour, IPointerDownHandler
{
	[SerializeField] Image indicator,background;
	[SerializeField] Color activeColor, inActiveColor;
	public bool active = false; 

	private void Start()
	{
		
	}

	public void Toggle()
	{
	
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		// khi click vào toggle thì chuyển trạng thái và đổi màu
		active = !active;
		indicator.color = active ? activeColor : inActiveColor;
		background.color = active ? activeColor : inActiveColor;
	}
}