using UnityEngine;

public class JumpScareObject : MonoBehaviour
{
	public string reactionName;

	protected bool eventInvoked;

	public bool needsToBeClose;

	public float closeDistance = 4f;

	public bool checkPointColliders;

	public bool reversed;

	public bool hasToBeInTheBox;

	public Vector3 boxOffset = Vector3.zero;

	public Vector3 boxSize = Vector3.one;

	public Transform[] points;

	private void CheckReverse(Vector3 vpPos, bool checkNormal = false)
	{
		if (points.Length != 0)
		{
			bool flag = false;
			for (int i = 0; i < points.Length; i++)
			{
				Vector3 vpPos2 = PlayerController.instance.mainCamera.WorldToViewportPoint(points[i].position);
				if (!canSeeObject(vpPos2))
				{
					continue;
				}
				if (checkPointColliders)
				{
					Vector3 direction = points[i].position - PlayerController.instance.mainCamera.transform.position;
					if (Physics.Raycast(PlayerController.instance.mainCamera.transform.position, direction, out RaycastHit hitInfo) && hitInfo.collider.gameObject == points[i].gameObject)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag && checkNormal)
			{
				InvokeReaction();
			}
			else if (!flag && !checkNormal)
			{
				InvokeReaction();
			}
		}
		else if (canSeeObject(vpPos))
		{
			InvokeReaction();
		}
	}

	private void CheckNormalHit(Vector3 vpPos)
	{
		if (canSeeObject(vpPos))
		{
			Vector3 direction = base.transform.position - PlayerController.instance.mainCamera.transform.position;
			if (Physics.Raycast(PlayerController.instance.mainCamera.transform.position, direction, out RaycastHit hitInfo) && hitInfo.collider.gameObject == base.gameObject)
			{
				InvokeReaction();
			}
		}
	}

	private bool canSeeObject(Vector3 vpPos)
	{
		if (vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f)
		{
			return vpPos.z > 0f;
		}
		return false;
	}

	private void Update()
	{
		if (!eventInvoked)
		{
			Vector3 vpPos = PlayerController.instance.mainCamera.WorldToViewportPoint(base.transform.position);
			if (reversed)
			{
				CheckReverse(vpPos);
			}
			else if (points.Length != 0)
			{
				CheckReverse(vpPos, checkNormal: true);
			}
			else
			{
				CheckNormalHit(vpPos);
			}
		}
	}

	protected void ResetObj()
	{
		eventInvoked = false;
		base.enabled = true;
	}

	public virtual void InvokeReaction(bool invokeReaction = true)
	{
		if ((!needsToBeClose || !(Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) > closeDistance)) && (!hasToBeInTheBox || Physics.OverlapBox(base.transform.position + boxOffset, boxSize / 2f, Quaternion.identity, GameManager.instance.playerMask).Length >= 1))
		{
			ReactionController.instance.InvokeReaction(reactionName);
			eventInvoked = true;
			base.enabled = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (hasToBeInTheBox)
		{
			Gizmos.DrawWireCube(base.transform.position + boxOffset, boxSize);
		}
	}
}
