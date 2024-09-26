using UnityEngine;

public class ElectricBox : Interactable
{
	public bool gameEnded;

	public MeshRenderer objectOne;

	public MeshRenderer objectTwo;

	public override void SecondStart()
	{
		base.SecondStart();
		Renderer[] array = allObjectMeshes = new MeshRenderer[2]
		{
			objectOne,
			objectTwo
		};
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		ReactionController.instance.InvokeReaction("TurnOnLights" + GameManager.instance.currentDay.ToString());
		ReactionController.instance.InvokeReaction("TurnOnLights");
		if (gameEnded)
		{
			ReactionController.instance.InvokeReaction("EndGameCutscene");
		}
	}

	public void SetGameEnded(bool b)
	{
		gameEnded = b;
	}
}
