using UnityEngine;

public class Bun : GrabObject
{
	public enum MeatType
	{
		None,
		Raw,
		Done,
		Burned
	}

	public MeatType meatType;

	public SkinnedMeshRenderer bunSkinMesh;

	public bool hasCheddar;

	public bool hasLettuce;

	public bool hasKetchup;

	public bool hasMustard;

	public float ketchupValue;

	public float mustardValue;

	public GameObject ketchupObj;

	public GameObject mustardObj;

	public GameObject lettuceObj;

	private float valueToAdd = 0.2f;

	private float maxSauceSize = 8f;

	public void AddKetchupValue()
	{
		ketchupValue += valueToAdd;
		if (!hasKetchup && ketchupValue > maxSauceSize)
		{
			hasKetchup = true;
			AudioController.instance.SpawnCombineSoundAtPos(base.transform.position);
			ketchupValue = maxSauceSize;
			ketchupObj.SetActive(value: true);
		}
	}

	public void AddMustardValue()
	{
		mustardValue += valueToAdd;
		if (!hasMustard && mustardValue > maxSauceSize)
		{
			hasMustard = true;
			AudioController.instance.SpawnCombineSoundAtPos(base.transform.position);
			mustardValue = maxSauceSize;
			mustardObj.SetActive(value: true);
		}
	}
}
