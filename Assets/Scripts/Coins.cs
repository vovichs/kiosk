using UnityEngine;

public class Coins : Interactable
{
	public GameObject[] coinObjects;

	public GameObject[] paperBill;

	public AudioObject moneyTakeSoundObject;

	public override void SecondStart()
	{
		base.SecondStart();
		GetChildMeshes();
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		moneyTakeSoundObject.PlayAudioOnThisObject();
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		RandomizeCoins();
		RandomziePaperBills();
	}

	private void OnDisable()
	{
		for (int i = 0; i < coinObjects.Length; i++)
		{
			coinObjects[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < paperBill.Length; j++)
		{
			paperBill[j].gameObject.SetActive(value: false);
		}
	}

	private void RandomizeCoins()
	{
		for (int i = 0; i < coinObjects.Length; i++)
		{
			coinObjects[i].gameObject.SetActive(Random.Range(0, 2) == 0);
		}
	}

	private void RandomziePaperBills()
	{
		for (int i = 0; i < paperBill.Length; i++)
		{
			paperBill[i].gameObject.SetActive(Random.Range(0, 2) == 0);
		}
		paperBill[Random.Range(0, paperBill.Length)].SetActive(value: true);
	}
}
