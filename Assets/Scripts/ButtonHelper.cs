using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHelper : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler
{
	public bool skipHover;

	public void OnPointerDown(PointerEventData eventData)
	{
		ReactionController.instance.InvokeReaction("UiButtonPress");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!skipHover)
		{
			ReactionController.instance.InvokeReaction("UiButtonPlay");
		}
	}
}
