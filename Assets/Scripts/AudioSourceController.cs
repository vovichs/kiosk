using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
	public Transform soundSourcePosition;

	public float divideValue = 2f;

	public float maxVolume = 1f;

	public float minVolume = 0.2f;

	public float distance;

	private float helpVolume;

	private AudioSource myAudioSource;

	private void Start()
	{
		myAudioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (myAudioSource != null)
		{
			distance = Vector3.Distance(PlayerController.instance.transform.position, soundSourcePosition.position);
			helpVolume = distance / divideValue;
			myAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(helpVolume));
		}
	}
}
