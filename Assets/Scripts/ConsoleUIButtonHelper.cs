using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConsoleUIButtonHelper : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent onClick;

	public UnityEvent onDrag;

	public UnityEvent onPress;

	public UnityEvent onRelease;

	public UnityEvent onEnter;

	public UnityEvent onExit;

	public bool draggable;

	public void OnDrag(PointerEventData eventData)
	{
		if (draggable)
		{
			Console.instance.consoleHolder.GetComponent<RectTransform>().anchoredPosition += eventData.delta / Console.instance.transform.parent.GetComponent<Canvas>().scaleFactor;
			Vector2 anchoredPosition = Console.instance.consoleHolder.GetComponent<RectTransform>().anchoredPosition;
			anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, -700f, 700f);
			anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, -700f, 45f);
			Console.instance.consoleHolder.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		}
		onDrag.Invoke();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		onClick.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		onPress.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		onEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onExit.Invoke();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		onRelease.Invoke();
		Console.instance.FocusInput();
	}
}
