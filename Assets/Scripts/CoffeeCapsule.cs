using UnityEngine;

public class CoffeeCapsule : TriggerObject
{
	public CoffeeStation coffeeStation;

	public Transform capsuleParent;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (tempGrabObj.grabObjectID == 6 && !coffeeStation.alreadyHasCoffeeCapsule())
		{
			coffeeStation.SetCoffeeCapsule();
			AudioController.instance.SpawnCombineSoundAtPos(base.transform.position);
			UnityEngine.Object.Destroy(tempGrabObj.gameObject);
		}
	}
}
