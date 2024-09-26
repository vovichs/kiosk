using System;
using UnityEngine;

public class CustomersController : MonoBehaviour
{
	[Serializable]
	public class Day
	{
		public Customer[] customerPrefabs;
	}

	public static CustomersController instance;

	public Transform startPos;

	public Transform kioskPos;

	public Transform endPos;

	public Day[] days;

	private Customer currentCustomer;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public void DestroyCurrentCustomer(bool andCancelInvoke = false)
	{
		if (currentCustomer != null)
		{
			UnityEngine.Object.Destroy(currentCustomer.gameObject);
		}
		if (andCancelInvoke)
		{
			CancelInvoke();
		}
	}

	public void SpawnCustomer()
	{
		DestroyCurrentCustomer();
		currentCustomer = UnityEngine.Object.Instantiate(days[GameManager.instance.currentDay].customerPrefabs[GameManager.instance.currentCustomer], startPos.position, Quaternion.identity);
		currentCustomer.UpdateDestination(kioskPos);
	}

	public void ForceSpawnNextCustomer(float delay = 3f)
	{
		Invoke("SpawnCustomer", delay);
	}

	public void NextCustomer()
	{
		GameManager.instance.currentCustomer++;
		if (GameManager.instance.currentCustomer >= days[GameManager.instance.currentDay].customerPrefabs.Length)
		{
			if (GameManager.instance.currentDay >= 2)
			{
				UnityEngine.Debug.Log("<color=green>Game over!</color>");
				ReactionController.instance.InvokeReaction("EndGame");
			}
			else
			{
				ReactionController.instance.InvokeReaction("Clock");
			}
		}
		else
		{
			Invoke("SpawnCustomer", UnityEngine.Random.Range(8f, 15f));
		}
	}

	public Customer GetCurrentCustomer()
	{
		return currentCustomer;
	}
}
