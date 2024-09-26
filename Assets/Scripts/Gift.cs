public class Gift : Interactable
{
	public override void PressVirtual()
	{
		base.PressVirtual();
		canInteract = false;
		ReactionController.instance.InvokeReaction("GiftBomb");
		Invoke("EndCredit", 0.7f);
	}

	private void EndCredit()
	{
		ReactionController.instance.InvokeReaction("EndCredit");
		UIController.instance.EndCredit();
	}
}
