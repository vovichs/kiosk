using UnityEngine;

public class ExplodeGrabObj : GrabObject
{
	public GrabObject[] allExplodePieces;

	public float explosionForce = 0.4f;

	public ParticleSystem cutParticle;

	public override void CutVirtual()
	{
		base.CutVirtual();
		base.gameObject.SetActive(value: false);
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = false;
		}
		for (int i = 0; i < allExplodePieces.Length; i++)
		{
			allExplodePieces[i].gameObject.SetActive(value: true);
			allExplodePieces[i].transform.SetParent(GameManager.instance.currentGrabObjects.transform);
			if (allExplodePieces[i].GetComponent<Collider>() != null)
			{
				allExplodePieces[i].GetComponent<Collider>().enabled = true;
			}
			allExplodePieces[i].getMyRb().isKinematic = false;
			allExplodePieces[i].getMyRb().useGravity = true;
			float x = Random.Range(0f, 35f);
			float y = Random.Range(0f, 35f);
			float z = Random.Range(0f, 35f);
			allExplodePieces[i].transform.rotation = Quaternion.Euler(x, y, z);
			allExplodePieces[i].getMyRb().AddExplosionForce(explosionForce, base.transform.position, 0f, explosionForce, ForceMode.Impulse);
			if (cutParticle != null)
			{
				cutParticle.transform.SetParent(null);
				cutParticle.Play();
			}
		}
		PlayBreakSound();
		UnityEngine.Object.Destroy(base.gameObject, 0.2f);
	}
}
