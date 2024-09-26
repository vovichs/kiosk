using System;
using UnityEngine;

public class MakingStation : TriggerObject
{
	[Serializable]
	public class ObjectPosition
	{
		public Transform objectPos;

		public GrabObject grabObjOnPos;
	}

	private bool canMake = true;

	public ObjectPosition[] objectPositions;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (canMake)
		{
			if (isPosFreeWithID(0) && tempGrabObj.grabObjectID == 0)
			{
				SetObj(tempGrabObj, 0);
			}
			if (isPosFreeWithID(1) && tempGrabObj.grabObjectID == 1)
			{
				SetObj(tempGrabObj, 1);
			}
			else if (isPosFreeWithID(2) && tempGrabObj.grabObjectID == 2)
			{
				SetObj(tempGrabObj, 2);
				canMake = false;
				Invoke("MakeHotdog", 0.3f);
			}
		}
	}

	private void SetObj(GrabObject tempGrabObj, int index)
	{
		tempGrabObj.DisableGrabObject();
		objectPositions[index].grabObjOnPos = tempGrabObj;
		tempGrabObj.Equip(objectPositions[index].objectPos, resetRot: true);
	}

	private bool isPosFreeWithID(int index)
	{
		for (int i = 0; i < index; i++)
		{
			if (objectPositions[i].grabObjOnPos == null)
			{
				return false;
			}
		}
		return objectPositions[index].grabObjOnPos == null;
	}

	private void ClearAll()
	{
		for (int i = 0; i < objectPositions.Length; i++)
		{
			objectPositions[i].grabObjOnPos = null;
		}
	}

	private void MakeHotdog()
	{
		GameObject gameObject = new GameObject("CompleteHotdog");
		gameObject.transform.position = objectPositions[1].grabObjOnPos.transform.position;
		GrabObject grabObject = gameObject.AddComponent<GrabObject>();
		grabObject.interactableName = "Hotdog";
		grabObject.throwForce = 60f;
		grabObject.grabObjectID = 3;
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		Rigidbody component = objectPositions[1].grabObjOnPos.GetComponent<Rigidbody>();
		rigidbody.mass = 0.13f;
		rigidbody.drag = component.drag;
		rigidbody.angularDrag = component.angularDrag;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = false;
		rigidbody.interpolation = component.interpolation;
		rigidbody.collisionDetectionMode = component.collisionDetectionMode;
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(0.37f, 0.17f, 0.218f);
		boxCollider.center = new Vector3(0f, -0.015f, 0f);
		objectPositions[0].grabObjOnPos.transform.SetParent(grabObject.transform);
		objectPositions[1].grabObjOnPos.transform.SetParent(grabObject.transform);
		objectPositions[2].grabObjOnPos.transform.SetParent(grabObject.transform);
		for (int i = 0; i < objectPositions.Length; i++)
		{
			UnityEngine.Object.Destroy(objectPositions[i].grabObjOnPos.GetComponent<Rigidbody>());
			UnityEngine.Object.Destroy(objectPositions[i].grabObjOnPos.GetComponent<Collider>());
			UnityEngine.Object.Destroy(objectPositions[i].grabObjOnPos.GetComponent<GrabObject>());
		}
		objectPositions[1].grabObjOnPos = grabObject;
	}

	public override void OnGrabObjEquiped(GrabObject tempGrabObj)
	{
		base.OnGrabObjEquiped(tempGrabObj);
		if (objectPositions[1].grabObjOnPos == tempGrabObj)
		{
			canMake = true;
			ClearAll();
		}
	}
}
