using System;
using UnityEngine;
using UnityEngine.Events;

public class ReactionController : MonoBehaviour
{
	[Serializable]
	public class ReactionEvent : UnityEvent<string>
	{
	}

	public static ReactionController instance;

	public ReactionEvent reactionEvent;

	public GameObject reactionObjectHolder;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		ReactionObject[] componentsInChildren = reactionObjectHolder.GetComponentsInChildren<ReactionObject>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetupReactionObject();
		}
	}

	public void InvokeReaction(string reactionId)
	{
		UnityEngine.Debug.Log("<color=green>Reaction with id = " + reactionId + " invoked!</color>");
		reactionEvent.Invoke(reactionId);
	}
}
