using System.Collections;
using UnityEngine;

public class DecalSelfDestroy : MonoBehaviour
{
	public GameObject decalChild;

	private void Start()
	{
		StartCoroutine(selfDestruct());
	}

	private IEnumerator selfDestruct()
	{
		float lerpValue = 0f;
		float time = 0.1f;
		yield return new WaitForSeconds(7f);
		Vector3 startScale = base.transform.localScale;
		Vector3 endScale = Vector3.zero;
		while (lerpValue <= time)
		{
			decalChild.transform.localScale = Vector3.Lerp(startScale, endScale, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		decalChild.transform.localScale = endScale;
		UnityEngine.Object.Destroy(base.gameObject, 0.1f);
	}
}
