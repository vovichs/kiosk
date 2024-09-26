using System.Runtime.CompilerServices;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public string interactableName;

	public bool canInteract = true;

	public bool noOutline;

	public Renderer[] allObjectMeshes;

	private void Start()
	{
		base.gameObject.layer = 6;
		SecondStart();
	}

	public virtual void SecondStart()
	{
	}

	public void GetChildMeshes()
	{
		allObjectMeshes = GetComponentsInChildren<Renderer>(includeInactive: true);
	}

	public void SetInteractableName(string newName)
	{
		interactableName = newName;
	}

	public void Press()
	{
		if (canInteract)
		{
			PressVirtual();
		}
	}

	public void Release()
	{
		if (canInteract)
		{
			ReleaseVirtual();
		}
	}

	public void Drag()
	{
		if (canInteract)
		{
			DragVirtual();
		}
	}

	public void StartUse()
	{
		if (canInteract)
		{
			StartUseVirtual();
		}
	}

	public void Use()
	{
		if (canInteract)
		{
			UseVirtual();
		}
	}

	public void StopUse()
	{
		if (canInteract)
		{
			StopUseVirtual();
		}
	}

	public void SetCanInteract(bool b)
	{
		canInteract = b;
	}

	public bool isInteractable()
	{
		return canInteract;
	}

	public virtual void PressVirtual()
	{
	}

	public virtual void ReleaseVirtual()
	{
	}

	public virtual void DragVirtual()
	{
	}

	public virtual void UseVirtual()
	{
	}

	public virtual void StopUseVirtual()
	{
	}

	public virtual void StartUseVirtual()
	{
	}
}
