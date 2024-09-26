using System.Collections;
using UnityEngine;

public class CabinetDoor : Interactable
{
	public enum Coordinate
	{
		X,
		Y,
		Z
	}

	public bool locked;

	public float openPos = 90f;

	public float lockedPos = 3f;

	public float getOutValue = 1.1f;

	public Coordinate targetCoordinate;

	public float openLittleAngle = 1f;

	public AudioObject audioObject;

	public AudioObject closeAudioObject;

	private Coroutine tempCor;

	private Coroutine tempCorLittle;

	private Vector3 startRot;

	private Vector3 endRot;

	private bool doorOpened;

	private Collider doorCollider;

	public Vector2 pitchValues;

	public override void SecondStart()
	{
		base.SecondStart();
		startRot = base.transform.localEulerAngles;
		endRot = new Vector3((targetCoordinate == Coordinate.X) ? (startRot.x + openPos) : startRot.x, (targetCoordinate == Coordinate.Y) ? (startRot.y + openPos) : startRot.y, (targetCoordinate == Coordinate.Z) ? (startRot.z + openPos) : startRot.z);
		SetInteractableName();
		doorCollider = GetComponent<Collider>();
	}

	public override void PressVirtual()
	{
		ToggleDoor();
	}

	private void SetInteractableName()
	{
		interactableName = ((!doorOpened) ? "Open" : "Close");
	}

	public void ToggleDoor()
	{
		if (tempCor != null)
		{
			StopCoroutine(tempCor);
		}
		if (tempCorLittle != null)
		{
			StopCoroutine(tempCorLittle);
		}
		tempCor = StartCoroutine(ToggleDoorAnim());
	}

	private IEnumerator ToggleDoorAnim()
	{
		bool wasConvexBefore = false;
		if (doorCollider.GetComponent<MeshCollider>() != null && doorCollider.GetComponent<MeshCollider>().convex)
		{
			wasConvexBefore = true;
		}
		if (!locked)
		{
			doorOpened = !doorOpened;
			if (audioObject != null)
			{
				if (doorOpened)
				{
					audioObject.PlayWithPitch(pitchValues.x);
				}
				else if (closeAudioObject != null)
				{
					closeAudioObject.PlayAudioOnThisObject();
				}
				else
				{
					audioObject.PlayWithPitch(pitchValues.y);
				}
			}
			SetInteractableName();
			if (doorCollider.GetComponent<MeshCollider>() != null)
			{
				doorCollider.GetComponent<MeshCollider>().convex = true;
			}
			doorCollider.isTrigger = true;
		}
		else
		{
			interactableName = "Locked";
		}
		float time = 0.35f;
		float lerpValue2 = 0f;
		Vector3 finalStartPos2 = base.transform.localEulerAngles;
		Vector3 finalEndPos3 = doorOpened ? endRot : startRot;
		if (locked)
		{
			finalEndPos3 = new Vector3((targetCoordinate == Coordinate.X) ? (startRot.x + lockedPos) : startRot.x, (targetCoordinate == Coordinate.Y) ? (startRot.y + lockedPos) : startRot.y, (targetCoordinate == Coordinate.Z) ? (startRot.z + lockedPos) : startRot.z);
			float newTime = time / 5f;
			while (lerpValue2 <= newTime)
			{
				base.transform.localEulerAngles = AngleLerp(finalStartPos2, finalEndPos3, lerpValue2 / newTime);
				lerpValue2 += Time.deltaTime;
				yield return null;
			}
			base.transform.localEulerAngles = finalEndPos3;
			finalStartPos2 = base.transform.localEulerAngles;
			finalEndPos3 = startRot;
			lerpValue2 = 0f;
			while (lerpValue2 <= newTime)
			{
				base.transform.localEulerAngles = AngleLerp(finalStartPos2, finalEndPos3, lerpValue2 / newTime);
				lerpValue2 += Time.deltaTime;
				yield return null;
			}
			base.transform.localEulerAngles = finalEndPos3;
		}
		else
		{
			while (lerpValue2 <= time)
			{
				base.transform.localEulerAngles = AngleLerp(finalStartPos2, finalEndPos3, lerpValue2 / time);
				lerpValue2 += Time.deltaTime;
				yield return null;
			}
			base.transform.localEulerAngles = finalEndPos3;
		}
		while (Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < getOutValue)
		{
			UnityEngine.Debug.Log(Vector3.Distance(base.transform.position, PlayerController.instance.transform.position));
			yield return null;
		}
		doorCollider.isTrigger = false;
		if (!wasConvexBefore && doorCollider.GetComponent<MeshCollider>() != null)
		{
			doorCollider.GetComponent<MeshCollider>().convex = false;
		}
	}

	private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{
		float x = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float y = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float z = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		return new Vector3(x, y, z);
	}

	public void SetLocked(bool b)
	{
		locked = b;
	}

	public void UnlockALittle()
	{
		locked = false;
		tempCorLittle = StartCoroutine(UnlockOpenALittle());
		interactableName = "Open";
	}

	private IEnumerator UnlockOpenALittle()
	{
		float time = 0.2f;
		float lerpValue = 0f;
		startRot = base.transform.localEulerAngles;
		Vector3 finalStartPos = base.transform.localEulerAngles;
		Vector3 finalEndPos = new Vector3((targetCoordinate == Coordinate.X) ? (startRot.x + openLittleAngle) : startRot.x, (targetCoordinate == Coordinate.Y) ? (startRot.y + openLittleAngle) : startRot.y, (targetCoordinate == Coordinate.Z) ? (startRot.z + openLittleAngle) : startRot.z);
		while (lerpValue <= time)
		{
			base.transform.localEulerAngles = AngleLerp(finalStartPos, finalEndPos, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		base.transform.localEulerAngles = finalEndPos;
	}
}
