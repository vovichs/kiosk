using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
	public static AudioController instance;

	public AudioMixer myMixer;

	public AudioObject grabObjectCombineSound;

	public AudioObject dialogLetterTick;

	public AudioObject dropSound;

	private AudioSource[] allAudios;

	private Coroutine tempAudioCor;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		allAudios = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	public void SpawnCombineSoundAtPos(Vector3 audioPos)
	{
		AudioObject audioObject = UnityEngine.Object.Instantiate(grabObjectCombineSound, audioPos, Quaternion.identity, null);
		audioObject.PlayAudioOnThisObject();
		UnityEngine.Object.Destroy(audioObject.gameObject, 1f);
	}

	public void SpawnDialogLetterTick()
	{
		AudioObject audioObject = UnityEngine.Object.Instantiate(dialogLetterTick, base.transform.position, Quaternion.identity, null);
		audioObject.PlayAudioOnThisObject();
		UnityEngine.Object.Destroy(audioObject.gameObject, 1f);
	}

	public void SpawnDropSound(Vector3 audioPos, float tempVol)
	{
		AudioObject audioObject = UnityEngine.Object.Instantiate(dropSound, audioPos, Quaternion.identity, null);
		audioObject.GetComponent<AudioSource>().volume = tempVol;
		audioObject.PlayAudioOnThisObject();
		UnityEngine.Object.Destroy(audioObject.gameObject, 1f);
	}

	public void SetMusic(float value)
	{
		myMixer.SetFloat("uiSounds", value);
	}

	public void SetSound(float value)
	{
		myMixer.SetFloat("sound", value);
	}

	public void SetMaster(float value)
	{
		myMixer.SetFloat("master", value);
	}

	public void TurnOffAllSound()
	{
		for (int i = 0; i < allAudios.Length; i++)
		{
			allAudios[i].Stop();
		}
	}

	public void FadeMasterTo(float value, float fadeTime = 0.4f)
	{
		if (tempAudioCor != null)
		{
			StopCoroutine(tempAudioCor);
		}
		tempAudioCor = StartCoroutine(FadeInMasterCor(value, fadeTime));
	}

	private IEnumerator FadeInMasterCor(float fullValue = 0f, float newTime = 0.4f)
	{
		float lerpValue = 0f;
		float startValue = GetMasterLevel();
		SetSound(startValue);
		while (lerpValue <= newTime)
		{
			SetSound(Mathf.Lerp(startValue, fullValue, lerpValue / newTime));
			lerpValue += Time.deltaTime;
			yield return null;
		}
		SetSound(fullValue);
		tempAudioCor = null;
	}

	public float GetMasterLevel()
	{
		if (myMixer.GetFloat("sound", out float value))
		{
			return value;
		}
		return 0f;
	}
}
