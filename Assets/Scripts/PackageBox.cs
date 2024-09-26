using UnityEngine;

public class PackageBox : Interactable
{
	public int packageID;

	public Transform spawnPos;

	public override void PressVirtual()
	{
		base.PressVirtual();
		Object.Instantiate(GameManager.instance.allObjects[packageID], spawnPos.position, Quaternion.identity, GameManager.instance.currentGrabObjects.transform).GetComponent<GrabObject>().InstantEquip();
	}
}
