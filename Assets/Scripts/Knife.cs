using UnityEngine;

public class Knife : GrabObject
{
	private bool canStab;

	public Transform knifePoint;

	public AudioObject knifeAudio;

	private bool stabbed;

	public override void Equip(Transform parentTransform = null, bool resetRot = false)
	{
		base.Equip(parentTransform, resetRot);
		canStab = false;
		Invoke("Rotate", 0.08f);
	}

	private void Rotate()
	{
		if (stabbed)
		{
			float x = Random.Range(0f, 0f);
			float y = Random.Range(-180f, 180f);
			float z = Random.Range(-180f, 180f);
			getMyRb().AddTorque(new Vector3(x, y, z), ForceMode.Impulse);
		}
		stabbed = false;
	}

	public override void Throw(bool addForce = true, bool addTorque = true)
	{
		if (addForce)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-5f, 5f));
		}
		canStab = addForce;
		base.Throw(addForce, addTorque: false);
		if (addForce)
		{
			getMyRb().freezeRotation = true;
		}
	}

	private void Update()
	{
		if (canStab)
		{
			CheckForCutObjects();
		}
	}

	private new void OnCollisionEnter(Collision other)
	{
		if (canStab && getMyRb() != null && !getMyRb().isKinematic && other != null && !isEquiped() && other.gameObject != PlayerController.instance.gameObject)
		{
			CheckForCutObjects();
			if (other.gameObject.GetComponent<Customer>() != null && !other.gameObject.GetComponent<Customer>().DidOrder())
			{
				other.gameObject.GetComponent<Customer>().KnifeDialog();
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (other.gameObject.GetComponent<GrabObject>() == null)
			{
				stabbed = true;
				getMyRb().freezeRotation = false;
				getMyRb().isKinematic = true;
				canStab = false;
				base.transform.position += base.transform.forward / 10f;
				knifeAudio.PlayAudioOnThisObject();
			}
		}
	}

	private void CheckForCutObjects()
	{
		Collider[] array = Physics.OverlapSphere(knifePoint.position, 0.15f, PlayerController.instance.interactableMask);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject != base.gameObject && array[i].GetComponent<GrabObject>() != null)
			{
				array[i].GetComponent<GrabObject>().Cut();
			}
		}
	}
}
