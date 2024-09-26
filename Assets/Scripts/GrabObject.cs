using System.Collections;
using UnityEngine;

public class GrabObject : Interactable
{
	public int grabObjectID;

	public float throwForce = 40f;

	public bool breakable;

	public Vector3 localPosOffset = Vector3.zero;

	private Vector3 localStartScale;

	private Vector3 dragOffset = Vector3.zero;

	private Transform grabObjTransform;

	private Renderer objectRenderer;

	private Rigidbody myRb;

	private bool equiped;

	private float defaultMass;

	private GameObject lettuceHelpObj;

	public ParticleSystem breakParticle;

	public AudioObject breakSound;

	private bool started;

	private bool alreadyCut;

	private Coroutine tempCor;

	private Coroutine movingToPos;

	private Coroutine rotatingToPos;

	public void EnableGrabObject()
	{
		canInteract = true;
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = true;
		}
	}

	public void DisableGrabObject()
	{
		canInteract = false;
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = false;
		}
	}

	public void SetOnPlace(Transform placeTransform, GrabObject otherGrabObj, bool setParentOfGrabObj = true)
	{
		DisableGrabObject();
		Equip(placeTransform, resetRot: true);
		if (setParentOfGrabObj)
		{
			base.transform.SetParent(otherGrabObj.transform);
		}
		ResetScale();
		if (myRb != null)
		{
			myRb.useGravity = false;
			myRb.interpolation = RigidbodyInterpolation.None;
			myRb.isKinematic = true;
			myRb.constraints = RigidbodyConstraints.FreezeAll;
		}
	}

	public override void SecondStart()
	{
		if (!started)
		{
			SetupObject();
		}
	}

	private void SetupObject()
	{
		started = true;
		base.SecondStart();
		localStartScale = base.transform.localScale;
		myRb = GetComponent<Rigidbody>();
		grabObjTransform = PlayerController.instance.grabHolder;
		GetChildMeshes();
		objectRenderer = GetComponent<Renderer>();
		if (objectRenderer == null)
		{
			objectRenderer = GetComponentInChildren<Renderer>();
		}
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		PlayerController.instance.EquipGrabObj(this);
	}

	public void InstantEquip()
	{
		if (!started)
		{
			SetupObject();
		}
		Press();
	}

	public virtual void Throw(bool addForce = true, bool addTorque = true)
	{
		if (tempCor != null)
		{
			PlayerController.instance.StopCoroutine(tempCor);
		}
		equiped = false;
		myRb.mass = defaultMass;
		base.transform.SetParent(GameManager.instance.currentGrabObjects.transform);
		myRb.constraints = RigidbodyConstraints.None;
		myRb.useGravity = true;
		myRb.interpolation = RigidbodyInterpolation.Interpolate;
		if (addForce)
		{
			myRb.AddForce(PlayerController.instance.mainCamera.transform.forward * throwForce * Time.fixedDeltaTime, ForceMode.Impulse);
			if (addTorque)
			{
				myRb.AddTorque((PlayerController.instance.mainCamera.transform.forward * Random.Range((0f - throwForce) / 2f, throwForce / 2f) + PlayerController.instance.mainCamera.transform.right * throwForce) * Time.fixedDeltaTime, ForceMode.Impulse);
			}
		}
		base.gameObject.layer = 6;
	}

	public virtual void CutVirtual()
	{
	}

	public void Cut()
	{
		if (!alreadyCut)
		{
			alreadyCut = true;
			CutVirtual();
			if (breakParticle != null)
			{
				Break();
			}
		}
	}

	private void ReturnToInteractable()
	{
		base.gameObject.layer = 6;
	}

	public virtual void Equip(Transform parentTransform = null, bool resetRot = false)
	{
		if (rotatingToPos != null)
		{
			PlayerController.instance.StopCoroutine(rotatingToPos);
		}
		if (movingToPos != null)
		{
			PlayerController.instance.StopCoroutine(movingToPos);
		}
		if (myRb != null)
		{
			FreezeObj();
			if (resetRot)
			{
				base.transform.SetParent(parentTransform);
				myRb.constraints = RigidbodyConstraints.FreezeAll;
				rotatingToPos = PlayerController.instance.StartCoroutine(resetRotation());
				movingToPos = PlayerController.instance.StartCoroutine(resetPosition());
				return;
			}
			base.transform.SetParent(grabObjTransform);
			myRb.constraints = RigidbodyConstraints.None;
			myRb.isKinematic = false;
			equiped = true;
			myRb.interpolation = RigidbodyInterpolation.None;
			base.gameObject.layer = 7;
			defaultMass = myRb.mass;
			myRb.mass = 0.01f;
		}
	}

	private void FixedUpdate()
	{
		if (equiped)
		{
			Vector3 velocity = (grabObjTransform.position + grabObjTransform.right * dragOffset.x + grabObjTransform.up * dragOffset.y - base.transform.position) * 700f * Time.fixedDeltaTime;
			myRb.velocity = velocity;
		}
	}

	public void FreezeObj()
	{
		myRb.useGravity = false;
	}

	private IEnumerator resetPosition()
	{
		float lerpValue = 0f;
		float time = 0.15f;
		Vector3 startPos = base.transform.localPosition;
		Vector3 endPos = Vector3.zero + localPosOffset;
		AudioController.instance.SpawnCombineSoundAtPos(base.transform.position);
		while (lerpValue <= time)
		{
			base.transform.localPosition = Vector3.Lerp(startPos, endPos, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		base.transform.localPosition = endPos;
		movingToPos = null;
		if (lettuceHelpObj != null)
		{
			if (tempCor != null)
			{
				StopCoroutine(tempCor);
			}
			if (rotatingToPos != null)
			{
				StopCoroutine(rotatingToPos);
			}
			lettuceHelpObj.SetActive(value: true);
			base.gameObject.SetActive(value: false);
		}
		tempCor = null;
	}

	private IEnumerator resetRotation()
	{
		float lerpValue = 0f;
		float time = 0.25f;
		Vector3 startRot = base.transform.localEulerAngles;
		Vector3 endRot = Vector3.zero;
		while (lerpValue <= time)
		{
			base.transform.localEulerAngles = AngleLerp(startRot, endRot, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		base.transform.localEulerAngles = endRot;
		rotatingToPos = null;
	}

	private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{
		float x = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float y = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float z = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		return new Vector3(x, y, z);
	}

	public Rigidbody getMyRb()
	{
		if (myRb == null)
		{
			return myRb = GetComponent<Rigidbody>();
		}
		return myRb;
	}

	public bool isEquiped()
	{
		return equiped;
	}

	public void SetDragOffset(Vector3 desiredOffset)
	{
		dragOffset = desiredOffset;
	}

	public void ResetScale()
	{
		base.transform.localScale = localStartScale;
	}

	public void SetHelpLettuceObj(GameObject newHelpObj)
	{
		lettuceHelpObj = newHelpObj;
	}

	public void OnCollisionEnter(Collision other)
	{
		if (getMyRb().velocity.magnitude > 2.4f && breakable && ((movingToPos == null) & (rotatingToPos == null)) && !equiped)
		{
			if (other.gameObject.GetComponent<Customer>() == null)
			{
				Break();
			}
		}
		else if (movingToPos == null && rotatingToPos == null && getMyRb().velocity.magnitude > 0.35f && other.gameObject.GetComponent<TriggerObject>() == null)
		{
			AudioController.instance.SpawnDropSound(base.transform.position, getMyRb().velocity.magnitude / 13f);
		}
	}

	public void Break()
	{
		if (breakParticle != null)
		{
			breakParticle.gameObject.layer = 0;
			breakParticle.Play(withChildren: true);
			breakParticle.transform.SetParent(null);
			PlayBreakSound();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void PlayBreakSound()
	{
		if (breakSound != null)
		{
			breakSound.transform.SetParent(null);
			breakSound.PlayAudioOnThisObject();
			UnityEngine.Object.Destroy(breakSound, 2f);
		}
	}
}
