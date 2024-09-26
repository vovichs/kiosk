using UnityEngine;

public class GarbageCan : TriggerObject
{
	public AudioObject trashAudioObject;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (trashAudioObject != null)
		{
			trashAudioObject.PlayAudioOnThisObject();
		}
		UnityEngine.Object.Destroy(tempGrabObj.gameObject);
	}
}
