using UnityEngine;

public class Hotdog : Meat
{
	public float cookValue;

	private MeshRenderer hotdogMesh;

	public ParticleSystem sizzlingParticle;

	public AudioSource sizzlingSound;

	private float maxSizzleVolume = 0.7f;

	private float randomPitchValue = 0.15f;

	public override void SecondStart()
	{
		base.SecondStart();
		hotdogMesh = GetComponentInChildren<MeshRenderer>(includeInactive: true);
		Renderer[] array = allObjectMeshes = new MeshRenderer[1]
		{
			hotdogMesh
		};
		sizzlingSound.pitch += Random.Range(0f - randomPitchValue, randomPitchValue);
	}

	public void AddCookValue(float newValue)
	{
		cookValue += newValue;
		if (!sizzlingSound.isPlaying)
		{
			sizzlingSound.Play();
		}
		sizzlingSound.volume = maxSizzleVolume;
		if (cookValue < 150f)
		{
			sizzlingParticle.Play(withChildren: true);
		}
		if (cookValue >= 100f && meatType == Bun.MeatType.Raw)
		{
			meatType = Bun.MeatType.Done;
			interactableName = "Cooked hotdog";
			PlayerController.instance.UpdateHoverName();
			hotdogMesh.material = GameManager.instance.cookedHotdogMat;
		}
		else if (cookValue >= 150f && meatType == Bun.MeatType.Done)
		{
			meatType = Bun.MeatType.Burned;
			interactableName = "Burned hotdog";
			PlayerController.instance.UpdateHoverName();
			hotdogMesh.material = GameManager.instance.burnedHotdogMat;
			maxSizzleVolume = 0.25f;
		}
	}

	public override void PressVirtual()
	{
		base.PressVirtual();
		StopSizzle();
	}

	public void StopSizzle()
	{
		sizzlingSound.volume = 0f;
		sizzlingSound.Stop();
	}
}
