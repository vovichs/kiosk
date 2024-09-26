using UnityEngine;

public class Grill : TriggerObject
{
	public float grillSpeed = 2f;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (tempGrabObj.grabObjectID == 2)
		{
			tempGrabObj.GetComponent<Hotdog>().AddCookValue(Time.deltaTime * grillSpeed);
		}
		if (tempGrabObj.grabObjectID == 12)
		{
			tempGrabObj.GetComponent<Burger>().AddCookValue(Time.deltaTime * grillSpeed);
		}
	}

	public override void OnTriggerExitEvent(GrabObject tempGrabObj)
	{
		base.OnTriggerExitEvent(tempGrabObj);
		if (tempGrabObj.grabObjectID == 2)
		{
			tempGrabObj.GetComponent<Hotdog>().StopSizzle();
		}
		if (tempGrabObj.grabObjectID == 12)
		{
			tempGrabObj.GetComponent<Burger>().StopSizzle();
		}
	}
}
