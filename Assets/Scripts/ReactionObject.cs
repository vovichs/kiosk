using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactionObject : MonoBehaviour
{
	[Serializable]
	public class ReactionEvent
	{
		public string reactionID;

		public float delay;

		public UnityEvent reactionEvent;
	}

	public List<ReactionEvent> allReactions = new List<ReactionEvent>();

	public void SetupReactionObject()
	{
		ReactionController.instance.reactionEvent.AddListener(ReactionInvokedWithId);
	}

	private void ReactionInvokedWithId(string reactionId)
	{
		for (int i = 0; i < allReactions.Count; i++)
		{
			if (allReactions[i].reactionID == reactionId)
			{
				ReactionController.instance.StartCoroutine(DelayEvent(allReactions[i].reactionEvent, allReactions[i].delay));
			}
		}
	}

	public void ForceInvokeReactionWithID(string reactionId)
	{
		ReactionController.instance.InvokeReaction(reactionId);
	}

	private IEnumerator DelayEvent(UnityEvent eventToInvoke, float delay)
	{
		yield return new WaitForSeconds(delay);
		eventToInvoke.Invoke();
	}

	public void StartDialog(int index)
	{
		DialogController.instance.StartDialog(index);
	}

	public void FocusOnTransform(Transform focusPoint)
	{
		PlayerController.instance.FocusAndRelease(focusPoint);
	}

	public void NextCustomer(float delay)
	{
		CustomersController.instance.ForceSpawnNextCustomer(delay);
	}

	public void NextCustomerReal()
	{
		CustomersController.instance.NextCustomer();
	}
}
