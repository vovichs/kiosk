using UnityEngine;

public class Clock : Interactable
{
	public Animator alarmAnim;

	public override void SecondStart()
	{
		base.SecondStart();
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		GameManager.instance.currentCustomer = 0;
		GameManager.instance.currentDay++;
		PlayerPrefs.SetInt("currentDay", GameManager.instance.currentDay);
		canInteract = false;
		ReactionController.instance.InvokeReaction("ClockClicked");
		UIController.instance.NextDayScreen();
	}

	public void PlayClockAnim()
	{
		alarmAnim.enabled = true;
		alarmAnim.Play("AlarmAnim", -1, 0f);
	}
}
