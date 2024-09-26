using System.Collections;
using UnityEngine;

public class CoffeeStation : TriggerObject
{
	public GameObject greenLight;

	private bool hasCoffeeCapsule;

	public bool hasPower = true;

	public Cup currentCraftingObj;

	public Animator coffeePourObj;

	public Transform cupPos;

	public GameObject fakeCapsuleObj;

	public Transform capsuleStartPos;

	public Transform capsuleEndPos;

	public ParticleSystem steamParticle;

	private Coroutine tempCor;

	private bool coffeePoured;

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (currentCraftingObj != null && currentCraftingObj == tempGrabObj)
		{
			return;
		}
		if (tempGrabObj.grabObjectID == 4 && currentCraftingObj == null && tempGrabObj.GetComponent<Cup>() != null)
		{
			currentCraftingObj = tempGrabObj.GetComponent<Cup>();
			currentCraftingObj.GetComponent<Rigidbody>().isKinematic = true;
			currentCraftingObj.Equip(cupPos, resetRot: true);
			currentCraftingObj.ResetScale();
			currentCraftingObj.canInteract = false;
			if (alreadyHasCoffeeCapsule())
			{
				SetCanMakeCoffee();
			}
		}
		else if (currentCraftingObj != null && tempGrabObj.grabObjectID == 5 && hasCoffeeCapsule && coffeePoured)
		{
			tempGrabObj.SetOnPlace(currentCraftingObj.lidPosition, currentCraftingObj, setParentOfGrabObj: false);
			currentCraftingObj.grabObjectID = 7;
			currentCraftingObj.canInteract = true;
			currentCraftingObj.GetChildMeshes();
			currentCraftingObj.CoffeeCompleted();
			greenLight.SetActive(value: false);
			SetCoffeeCapsule(b: false);
			coffeePoured = false;
		}
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		UnityEngine.Debug.Log("Make coffee");
	}

	private void SetCanMakeCoffee()
	{
		if (hasPower)
		{
			tempCor = StartCoroutine(pouringCoffee());
		}
	}

	public void SetPower(bool b)
	{
		hasPower = b;
		if (b && currentCraftingObj != null && alreadyHasCoffeeCapsule())
		{
			SetCanMakeCoffee();
		}
	}

	private IEnumerator pouringCoffee()
	{
		yield return new WaitForSeconds(0.3f);
		float lerpValue2 = 0f;
		float time2 = 1f;
		Vector3 startPos = capsuleStartPos.position;
		Vector3 endPos = capsuleEndPos.position;
		while (lerpValue2 <= time2)
		{
			fakeCapsuleObj.transform.position = Vector3.Lerp(startPos, endPos, lerpValue2 / time2);
			lerpValue2 += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		greenLight.SetActive(value: true);
		fakeCapsuleObj.transform.position = endPos;
		fakeCapsuleObj.gameObject.SetActive(value: false);
		coffeePourObj.gameObject.SetActive(value: true);
		coffeePourObj.Play("CoffeeAnimPour", -1, 0f);
		steamParticle.Play(withChildren: true);
		ReactionController.instance.InvokeReaction("CoffeeSoundPlay");
		time2 = 8f;
		lerpValue2 = 0f;
		while (lerpValue2 <= time2)
		{
			currentCraftingObj.SetCoffeeValue(lerpValue2 / time2 * 100f);
			lerpValue2 += Time.deltaTime;
			yield return null;
		}
		steamParticle.Stop();
		currentCraftingObj.SetCoffeeValue(100f);
		coffeePourObj.Play("CoffeePourEnd", -1, 0f);
		yield return new WaitForSeconds(1f);
		coffeePourObj.gameObject.SetActive(value: false);
		greenLight.SetActive(value: false);
		tempCor = null;
		coffeePoured = true;
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

	public void SetCoffeeCapsule(bool b = true)
	{
		if (b && currentCraftingObj != null)
		{
			SetCanMakeCoffee();
		}
		if (b)
		{
			fakeCapsuleObj.transform.position = capsuleStartPos.transform.position;
			fakeCapsuleObj.SetActive(value: true);
		}
		hasCoffeeCapsule = b;
	}

	public bool alreadyHasCoffeeCapsule()
	{
		return hasCoffeeCapsule;
	}
}
