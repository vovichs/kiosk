using UnityEngine;

public class AudioObject : MonoBehaviour
{
	private AudioSource myAudioSource;

	public float randomPitchRange = 0.2f;

	private float startPitch = 1f;

	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		myAudioSource = GetComponent<AudioSource>();
		if (myAudioSource != null)
		{
			startPitch = myAudioSource.pitch;
		}
	}

	public void PlayAudioOnThisObject()
	{
		if (myAudioSource != null)
		{
			if (randomPitchRange > 0f || randomPitchRange < 0f)
			{
				myAudioSource.pitch = startPitch + UnityEngine.Random.Range(0f - randomPitchRange, randomPitchRange);
			}
			myAudioSource.Play();
		}
		else
		{
			Setup();
			PlayAudioOnThisObject();
		}
	}

	public void PlayWithPitch(float pitchValue)
	{
		if (myAudioSource != null)
		{
			myAudioSource.pitch = pitchValue;
			myAudioSource.Play();
		}
	}

	public void PlayAsAChild()
	{
	}
}
