using UnityEngine;

public class ShadowGuy : JumpScareObject
{
	public bool invokeWhenSeen = true;

	public ParticleSystem shadowParticles;

	public void Explode()
	{
		shadowParticles.transform.SetParent(null);
		shadowParticles.Play();
		base.gameObject.SetActive(value: false);
	}

	private void ReturnToNormal()
	{
		base.gameObject.SetActive(value: true);
		ResetObj();
	}

	public override void InvokeReaction(bool invokeReaction = true)
	{
		base.InvokeReaction(invokeReaction: false);
		if (invokeWhenSeen)
		{
			Invoke("Explode", 0.15f);
		}
	}
}
