using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : TriggerObject
{
	[Serializable]
	public class OrderMenu
	{
		public List<OrdedItem> orderItems = new List<OrdedItem>();
	}

	[Serializable]
	public class OrdedItem
	{
		public int objectID;

		public bool wantLettuce;

		public bool wantCheddar;

		public bool wantKetchup;

		public bool wantMustard;
	}

	public int customerID;

	public Animator characterAnimator;

	public Transform headRotate;

	private Transform destination;

	private NavMeshAgent navMeshAgent;

	private Coroutine tempCor;

	private bool ordered;

	public int[] dayTextDialogID;

	public OrderMenu[] dayOrders;

	private bool followWithTheHead;

	private float lookAtWeight = 0.5f;

	private void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		DialogController.instance.dialogDoneEvent.AddListener(DialogEventDone);
		if (GetComponent<ReactionObject>() != null)
		{
			GetComponent<ReactionObject>().SetupReactionObject();
		}
		canInteract = false;
	}

	public void OnAnimatorIK(int layerIndex)
	{
		if (followWithTheHead)
		{
			characterAnimator.SetLookAtPosition(PlayerController.instance.mainCamera.transform.position + new Vector3(0f, -0.2f, 0f));
			characterAnimator.SetLookAtWeight(lookAtWeight);
		}
		else
		{
			characterAnimator.SetLookAtWeight(0f);
		}
	}

	private void Update()
	{
		if (Vector3.Distance(destination.position, base.transform.position) < 0.2f && characterAnimator.GetBool("walk"))
		{
			characterAnimator.SetBool("walk", value: false);
			if (tempCor != null)
			{
				StopCoroutine(tempCor);
			}
			tempCor = StartCoroutine(rotateToTarget(180f));
		}
	}

	private IEnumerator rotateToTarget(float newAngle)
	{
		float lerpValue2 = 0f;
		float time = 0.6f;
		Vector3 startRot = base.transform.localEulerAngles;
		Vector3 endRot = new Vector3(0f, newAngle, 0f);
		while (lerpValue2 <= time)
		{
			base.transform.localEulerAngles = AngleLerp(startRot, endRot, lerpValue2 / time);
			lerpValue2 += Time.deltaTime;
			yield return null;
		}
		base.transform.localEulerAngles = endRot;
		tempCor = null;
		followWithTheHead = true;
		if (!ordered)
		{
			ReactionController.instance.InvokeReaction("ServiceBellSound");
		}
		canInteract = true;
		lerpValue2 = 0f;
		lookAtWeight = 0f;
		while (lerpValue2 <= time)
		{
			lookAtWeight = Mathf.Lerp(0f, 1f, lerpValue2 / time);
			lerpValue2 += Time.deltaTime;
			yield return null;
		}
		lookAtWeight = 1f;
	}

	private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{
		float x = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float y = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float z = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		return new Vector3(x, y, z);
	}

	public override void OnTriggerOrCollision(GrabObject tempGrabObj)
	{
		base.OnTriggerOrCollision(tempGrabObj);
		if (tempGrabObj != null && ordered && orderCompleted(tempGrabObj))
		{
			DialogController.instance.StartDialog(dayTextDialogID[GameManager.instance.currentDay] + 1, onlyLastText: false, setSkipFirstTimeTrue: false);
			PlayerController.instance.FocusViewOn(headRotate);
			canInteract = false;
		}
	}

	private bool orderCompleted(GrabObject currentGrabObj)
	{
		int grabObjectID = currentGrabObj.grabObjectID;
		UnityEngine.Object.Destroy(currentGrabObj.gameObject);
		int num = 32;
		bool flag = false;
		for (int num2 = dayOrders[GameManager.instance.currentDay].orderItems.Count - 1; num2 >= 0; num2--)
		{
			flag = false;
			if (dayOrders[GameManager.instance.currentDay].orderItems[num2].objectID == grabObjectID)
			{
				if ((currentGrabObj.grabObjectID == 0 || currentGrabObj.grabObjectID == 1) && currentGrabObj.GetComponent<Bun>() != null)
				{
					if (currentGrabObj.GetComponent<Bun>().meatType == Bun.MeatType.None)
					{
						flag = true;
					}
					else if (currentGrabObj.GetComponent<Bun>().meatType == Bun.MeatType.Burned)
					{
						num = 33;
						flag = true;
					}
					else if (currentGrabObj.GetComponent<Bun>().meatType == Bun.MeatType.Raw)
					{
						num = 34;
						flag = true;
					}
					else if (dayOrders[GameManager.instance.currentDay].orderItems[num2].wantCheddar && !currentGrabObj.GetComponent<Bun>().hasCheddar)
					{
						num = 35;
						flag = true;
					}
					else if (dayOrders[GameManager.instance.currentDay].orderItems[num2].wantLettuce && !currentGrabObj.GetComponent<Bun>().hasLettuce)
					{
						num = 36;
						flag = true;
					}
					else if (!dayOrders[GameManager.instance.currentDay].orderItems[num2].wantCheddar && currentGrabObj.GetComponent<Bun>().hasCheddar)
					{
						num = 37;
						flag = true;
					}
					else if (!dayOrders[GameManager.instance.currentDay].orderItems[num2].wantLettuce && currentGrabObj.GetComponent<Bun>().hasLettuce)
					{
						num = 38;
						flag = true;
					}
					else if (dayOrders[GameManager.instance.currentDay].orderItems[num2].wantKetchup && !currentGrabObj.GetComponent<Bun>().hasKetchup)
					{
						num = 39;
						flag = true;
					}
					else if (dayOrders[GameManager.instance.currentDay].orderItems[num2].wantMustard && !currentGrabObj.GetComponent<Bun>().hasMustard)
					{
						num = 40;
						flag = true;
					}
					else if (!dayOrders[GameManager.instance.currentDay].orderItems[num2].wantKetchup && currentGrabObj.GetComponent<Bun>().hasKetchup)
					{
						num = 41;
						flag = true;
					}
					else if (!dayOrders[GameManager.instance.currentDay].orderItems[num2].wantMustard && currentGrabObj.GetComponent<Bun>().hasMustard)
					{
						num = 42;
						flag = true;
					}
					if (flag)
					{
						UnityEngine.Debug.Log("Wrong order!");
						if (num == 32 && currentGrabObj.grabObjectID == 8)
						{
							KnifeDialog();
							return false;
						}
						DialogController.instance.StartDialog(num, onlyLastText: false, setSkipFirstTimeTrue: false);
						PlayerController.instance.FocusViewOn(headRotate);
						return false;
					}
				}
				flag = false;
				dayOrders[GameManager.instance.currentDay].orderItems.RemoveAt(num2);
				break;
			}
			flag = true;
		}
		if (flag)
		{
			if (num == 32 && currentGrabObj.grabObjectID == 8)
			{
				KnifeDialog();
				return false;
			}
			UnityEngine.Debug.Log("Wrong order!");
			DialogController.instance.StartDialog(num, onlyLastText: false, setSkipFirstTimeTrue: false);
			PlayerController.instance.FocusViewOn(headRotate);
			return false;
		}
		ReactionController.instance.InvokeReaction("positiveFeedback");
		return dayOrders[GameManager.instance.currentDay].orderItems.Count <= 0;
	}

	public void GoAway(bool nextCustomer = true)
	{
		canInteract = false;
		UpdateDestination(CustomersController.instance.endPos);
		if (nextCustomer)
		{
			CustomersController.instance.NextCustomer();
		}
		followWithTheHead = false;
	}

	public void UpdateDestination(Transform newDestination)
	{
		if (navMeshAgent != null)
		{
			destination = newDestination;
			characterAnimator.SetBool("walk", value: true);
			navMeshAgent.SetDestination(destination.position);
		}
		else
		{
			navMeshAgent = GetComponent<NavMeshAgent>();
			UpdateDestination(newDestination);
		}
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		DialogController.instance.StartDialog(dayTextDialogID[GameManager.instance.currentDay]);
		PlayerController.instance.FocusViewOn(headRotate);
	}

	private void DialogEventDone(int doneDialogIndex, int doneChildDialogIndex, bool lastText)
	{
		if (lastText)
		{
			string text = "dialog" + doneDialogIndex.ToString();
			ReactionController.instance.InvokeReaction(text);
			if (doneDialogIndex == dayTextDialogID[GameManager.instance.currentDay] + 1)
			{
				if (text == "dialog20")
				{
					canInteract = false;
					if (GetComponent<JumpScareObject>() != null)
					{
						GetComponent<JumpScareObject>().enabled = true;
					}
				}
				else if (text == "dialog2" || text == "dialog8" || text == "dialog16" || text == "dialog31")
				{
					GoAway(nextCustomer: false);
				}
				else
				{
					GoAway();
				}
			}
			else if (doneDialogIndex == dayTextDialogID[GameManager.instance.currentDay])
			{
				ordered = true;
			}
		}
		else
		{
			ReactionController.instance.InvokeReaction("dialog" + doneDialogIndex.ToString() + "step" + doneChildDialogIndex.ToString());
		}
	}

	public void DestroyCurrentCustomer()
	{
		CustomersController.instance.DestroyCurrentCustomer();
		CustomersController.instance.NextCustomer();
	}

	public void PlayInteractAnim()
	{
		characterAnimator.CrossFade("Give", 0.2f);
		CallShowCoins();
	}

	private void CallShowCoins()
	{
		GameManager.instance.ShowCoins(1.4f);
		DialogController.blockSkipDialog = true;
	}

	public void KnifeDialog()
	{
		DialogController.instance.StartDialog(47, onlyLastText: false, setSkipFirstTimeTrue: false);
		PlayerController.instance.FocusViewOn(headRotate);
	}

	public bool DidOrder()
	{
		return ordered;
	}
}
