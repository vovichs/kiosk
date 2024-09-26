using UnityEngine;

public class TriggerBox : MonoBehaviour
{
	public string reactionName = "";

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == PlayerController.instance.gameObject)
		{
			ReactionController.instance.InvokeReaction(reactionName);
			base.gameObject.SetActive(value: false);
		}
	}
}
