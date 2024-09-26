public class Cookbook : Interactable
{
	public AudioObject bookAudioObject;

	public override void PressVirtual()
	{
		base.PressVirtual();
		UIController.instance.OpenCookBook();
		if (bookAudioObject != null)
		{
			bookAudioObject.PlayAudioOnThisObject();
		}
	}
}
