public class Phone : Interactable
{
	public int dialogID;

	public override void SecondStart()
	{
		base.SecondStart();
		DialogController.instance.dialogDoneEvent.AddListener(DialogEventDone);
	}

	private void DialogEventDone(int doneDialogIndex, int doneChildDialogIndex, bool lastText)
	{
		if (doneDialogIndex < 1)
		{
			if (lastText)
			{
				ReactionController.instance.InvokeReaction("dialog" + doneDialogIndex.ToString());
			}
			else
			{
				ReactionController.instance.InvokeReaction("dialog" + doneDialogIndex.ToString() + "step" + doneChildDialogIndex.ToString());
			}
		}
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		DialogController.instance.StartDialog(dialogID);
		PlayerController.instance.FocusViewOn(base.transform);
		ReactionController.instance.InvokeReaction("PhoneAnswer");
	}

	public void PhoneCall()
	{
		ReactionController.instance.InvokeReaction("PhoneCallReceived");
	}

	public void SetDialogID(int index)
	{
		dialogID = index;
	}
}
