using UnityEngine;

public class TrashCan : TriggerObject
{
	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		UnityEngine.Object.Destroy(tempGrabObj.gameObject);
	}
}
