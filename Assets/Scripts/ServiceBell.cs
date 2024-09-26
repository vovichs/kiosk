using UnityEngine;

public class ServiceBell : Interactable
{
	public override void PressVirtual()
	{
		base.PressVirtual();
		BellRing();
	}

	private void OnCollisionEnter(Collision other)
	{
		BellRing();
	}

	public void BellRing()
	{
		ReactionController.instance.InvokeReaction("ServiceBellSound");
	}

	private void Update()
	{
	}
}
