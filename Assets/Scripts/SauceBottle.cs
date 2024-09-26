using UnityEngine;

public class SauceBottle : GrabObject
{
	public Vector3 goodAngleTest;

	public Vector3 offsetPos;

	public ParticleSystem sauceParticle;

	public GameObject decal;

	public AudioSource bottleSqueezeSound;

	private float randomTorqueValue = 4f;

	private float moveSpeed = 15f;

	private Vector3 lastPointPos = Vector3.zero;

	private bool startedUsing;

	public override void StartUseVirtual()
	{
		base.StartUseVirtual();
		sauceParticle.Play();
		SetDragOffset(offsetPos);
		startedUsing = true;
	}

	public override void UseVirtual()
	{
		base.UseVirtual();
		if (!startedUsing)
		{
			return;
		}
		GameObject gameObject = PlayerController.instance.ShootRay();
		bottleSqueezeSound.volume = 0.18f;
		if (gameObject != null && gameObject.GetComponent<Bun>() != null && (gameObject.GetComponent<Bun>().grabObjectID == 0 || gameObject.GetComponent<Bun>().grabObjectID == 1) && gameObject.GetComponent<Bun>().meatType != 0)
		{
			if (grabObjectID == 10)
			{
				gameObject.GetComponent<Bun>().AddKetchupValue();
			}
			else if (grabObjectID == 9)
			{
				gameObject.GetComponent<Bun>().AddMustardValue();
			}
		}
		RaycastHit raycastHit = PlayerController.instance.shootPoint();
		if (raycastHit.collider != null && Vector3.Distance(lastPointPos, raycastHit.point) > 0.02f)
		{
			lastPointPos = raycastHit.point;
			Object.Instantiate(decal, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal), GameManager.instance.transform).transform.localScale /= Random.Range(0.9f, 1.7f);
		}
		getMyRb().angularVelocity = Vector3.zero;
		base.transform.localEulerAngles = AngleLerp(base.transform.localEulerAngles, goodAngleTest, moveSpeed * Time.deltaTime);
	}

	public override void StopUseVirtual()
	{
		base.StopUseVirtual();
		if (startedUsing)
		{
			StopUsing();
			getMyRb().angularVelocity = new Vector3(Random.Range(0f - randomTorqueValue, randomTorqueValue), Random.Range(0f - randomTorqueValue, randomTorqueValue), Random.Range(0f - randomTorqueValue, randomTorqueValue));
		}
	}

	public override void Throw(bool addForce = true, bool addTorque = true)
	{
		base.Throw(addForce, addTorque);
		StopUsing();
	}

	private void StopUsing()
	{
		bottleSqueezeSound.volume = 0f;
		startedUsing = false;
		SetDragOffset(Vector3.zero);
		sauceParticle.Stop();
		sauceParticle.Clear();
	}

	private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{
		float x = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float y = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float z = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		return new Vector3(x, y, z);
	}
}
