public class StickyNote : Interactable
{
	public int dialogID;

	public AudioObject noteSound;

	public override void PressVirtual()
	{
		base.PressVirtual();
		ReadNote();
	}

	private void ReadNote()
	{
		noteSound.PlayAudioOnThisObject();
		DialogController.instance.StartDialog(dialogID);
		PlayerController.instance.FocusViewOn(base.transform);
	}
}
