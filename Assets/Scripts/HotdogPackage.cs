using UnityEngine;

public class HotdogPackage : GrabObject
{
	public GrabObject[] allHotdogs;

	public float explosionForce = 100f;

	public override void CutVirtual()
	{
		base.CutVirtual();
		GetComponent<Collider>().enabled = false;
		for (int i = 0; i < allHotdogs.Length; i++)
		{
			allHotdogs[i].transform.SetParent(GameManager.instance.currentGrabObjects.transform);
			allHotdogs[i].GetComponent<Collider>().enabled = true;
			allHotdogs[i].getMyRb().isKinematic = false;
			allHotdogs[i].getMyRb().useGravity = true;
			allHotdogs[i].getMyRb().AddExplosionForce(explosionForce, base.transform.position, 0f, explosionForce, ForceMode.Impulse);
		}
		base.gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(base.gameObject, 0.2f);
	}
}
