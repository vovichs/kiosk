using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public int currentDay;

	public int currentCustomer;

	public LayerMask playerMask;

	public GameObject[] allObjects;

	public GameObject coins;

	public Material cookedHotdogMat;

	public Material burnedHotdogMat;

	public Material cookedBurgerMat;

	public Material burnedBurgerMat;

	public Transform startingPos;

	public Transform mainMenuPos;

	public GameObject currentGrabObjects;

	public GameObject grabObjectsPrefab;

	private void Awake()
	{
		instance = this;
	}

	public void ShowCoins(float delay = 0.5f)
	{
		if (delay > 0f)
		{
			Invoke("ShowCoinInvoke", delay);
		}
		else
		{
			ShowCoinInvoke();
		}
	}

	private void ShowCoinInvoke()
	{
		DialogController.blockSkipDialog = false;
		coins.SetActive(value: true);
	}

	public void SpawnNewGrabObjects()
	{
		if (currentGrabObjects != null)
		{
			UnityEngine.Object.Destroy(currentGrabObjects);
		}
		currentGrabObjects = UnityEngine.Object.Instantiate(grabObjectsPrefab, ReactionController.instance.reactionObjectHolder.transform.position, Quaternion.identity, ReactionController.instance.reactionObjectHolder.transform);
	}
}
