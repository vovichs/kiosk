using UnityEngine;

public class CraftingStation : TriggerObject
{
	public GrabObject currentCraftingObj;

	public Transform bunPos;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (currentCraftingObj != null && currentCraftingObj == tempGrabObj)
		{
			return;
		}
		if ((tempGrabObj.grabObjectID == 1 || tempGrabObj.grabObjectID == 0) && currentCraftingObj == null)
		{
			currentCraftingObj = tempGrabObj;
			currentCraftingObj.GetComponent<Rigidbody>().isKinematic = true;
			currentCraftingObj.Equip(bunPos, resetRot: true);
			currentCraftingObj.ResetScale();
		}
		else
		{
			if (!(currentCraftingObj != null) || !(currentCraftingObj.GetComponent<Bun>() != null))
			{
				return;
			}
			if (currentCraftingObj.GetComponent<Bun>().meatType == Bun.MeatType.None && ((tempGrabObj.grabObjectID == 2 && currentCraftingObj.grabObjectID == 1) || (tempGrabObj.grabObjectID == 12 && currentCraftingObj.grabObjectID == 0)))
			{
				tempGrabObj.SetOnPlace(bunPos, currentCraftingObj);
				if (currentCraftingObj.grabObjectID == 1)
				{
					currentCraftingObj.interactableName = "Hotdog";
					currentCraftingObj.GetComponent<Bun>().bunSkinMesh.SetBlendShapeWeight(0, 0f);
				}
				else if (currentCraftingObj.grabObjectID == 0)
				{
					currentCraftingObj.interactableName = "Burger";
					currentCraftingObj.GetComponent<Bun>().bunSkinMesh.SetBlendShapeWeight(1, 35f);
				}
				if (tempGrabObj.GetComponent<Meat>() != null)
				{
					currentCraftingObj.GetComponent<Bun>().meatType = tempGrabObj.GetComponent<Meat>().GetMeatType();
				}
			}
			else if (tempGrabObj.grabObjectID == 13 && !currentCraftingObj.GetComponent<Bun>().hasCheddar && currentCraftingObj.grabObjectID == 0 && currentCraftingObj.GetComponent<Bun>().meatType != 0)
			{
				currentCraftingObj.GetComponent<Bun>().hasCheddar = true;
				tempGrabObj.SetOnPlace(bunPos, currentCraftingObj);
				tempGrabObj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 62f);
				if (currentCraftingObj.grabObjectID == 0)
				{
					currentCraftingObj.GetComponent<Bun>().bunSkinMesh.SetBlendShapeWeight(1, currentCraftingObj.GetComponent<Bun>().bunSkinMesh.GetBlendShapeWeight(1) + 14f);
				}
			}
			else if (tempGrabObj.grabObjectID == 14 && !currentCraftingObj.GetComponent<Bun>().hasLettuce && currentCraftingObj.GetComponent<Bun>().meatType != 0)
			{
				currentCraftingObj.GetComponent<Bun>().hasLettuce = true;
				if (currentCraftingObj.grabObjectID == 1 && currentCraftingObj.GetComponent<Bun>().lettuceObj != null)
				{
					tempGrabObj.SetHelpLettuceObj(currentCraftingObj.GetComponent<Bun>().lettuceObj);
				}
				tempGrabObj.SetOnPlace(bunPos, currentCraftingObj);
				if (currentCraftingObj.grabObjectID == 0)
				{
					currentCraftingObj.GetComponent<Bun>().bunSkinMesh.SetBlendShapeWeight(1, currentCraftingObj.GetComponent<Bun>().bunSkinMesh.GetBlendShapeWeight(1) + 14f);
				}
			}
			currentCraftingObj.GetChildMeshes();
		}
	}

	public override void OnGrabObjEquiped(GrabObject tempGrabObj)
	{
		base.OnGrabObjEquiped(tempGrabObj);
		if (currentCraftingObj == tempGrabObj)
		{
			currentCraftingObj.ResetScale();
			currentCraftingObj = null;
		}
	}
}
