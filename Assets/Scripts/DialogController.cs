using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
	[Serializable]
	public class DialogDoneEvent : UnityEvent<int, int, bool>
	{
	}

	[Serializable]
	public class CharacterInfo
	{
		public string characterName;

		public Color characterTextColor;
	}

	[Serializable]
	public class DialogObject
	{
		public bool playOnRandom;

		private bool dialogShown;

		public List<DialogTextObject> dialogsChildTexts = new List<DialogTextObject>();

		public void SetDialogShown(bool b)
		{
			dialogShown = b;
		}

		public bool GetDialogShown()
		{
			return dialogShown;
		}
	}

	[Serializable]
	public class DialogTextObject
	{
		public bool mainText;

		public string dialogText;

		public int characterID;
	}

	public static DialogController instance;

	public static bool blockSkipDialog;

	public Text dialogTextNormal;

	public float letterShowSpeed = 0.002f;

	public DialogDoneEvent dialogDoneEvent;

	public CharacterInfo[] charactersInfo;

	public List<DialogObject> allDialogs = new List<DialogObject>();

	private Coroutine tempCor;

	private bool dialogActive;

	private bool skipFirstTime;

	private int childTextCounter;

	private int currentDialog = -1;

	private void Awake()
	{
		instance = this;
	}

	public void StartDialog(int index, bool onlyLastText = false, bool setSkipFirstTimeTrue = true)
	{
		PlayerController.blockGlobal = true;
		UIController.instance.SetCrosshair(b: false);
		UIController.instance.SetInteractableInfoTextActive(b: false);
		if (onlyLastText)
		{
			childTextCounter = allDialogs[index].dialogsChildTexts.Count - 1;
		}
		else
		{
			childTextCounter = 0;
		}
		currentDialog = index;
		if (allDialogs[index].GetDialogShown() && !allDialogs[index].dialogsChildTexts[childTextCounter].mainText)
		{
			childTextCounter = GetCurrentDialogFirstMainText(index);
		}
		skipFirstTime = setSkipFirstTimeTrue;
		dialogDoneEvent.Invoke(currentDialog, childTextCounter, arg2: false);
		ShowDialog(index);
		UIController.instance.SetInteractableInfoTextActive(b: false);
	}

	private int GetCurrentDialogFirstMainText(int index)
	{
		for (int i = 0; i < allDialogs[index].dialogsChildTexts.Count; i++)
		{
			if (allDialogs[index].dialogsChildTexts[i].mainText)
			{
				return i;
			}
		}
		return 0;
	}

	private void ShowDialog(int index)
	{
		if (allDialogs.Count > index)
		{
			if (tempCor != null)
			{
				StopCoroutine(tempCor);
			}
			tempCor = StartCoroutine(showDialogLettersOneByOne(getDialogText(index), index));
		}
	}

	private string getDialogText(int index)
	{
		if (allDialogs[index].playOnRandom)
		{
			return allDialogs[index].dialogsChildTexts[UnityEngine.Random.Range(0, allDialogs[index].dialogsChildTexts.Count)].dialogText;
		}
		return allDialogs[index].dialogsChildTexts[childTextCounter].dialogText;
	}

	private IEnumerator showDialogLettersOneByOne(string textToShow, int index)
	{
		yield return new WaitForEndOfFrame();
		UIController.instance.SetInteractableInfoTextActive(b: false);
		UIController.instance.ForceDisableInteractText();
		dialogActive = true;
		string originalText = "";
		string emptyString = textToShow;
		char[] wordToShow = textToShow.ToCharArray();
		Color white = Color.white;
		Color textColor = (allDialogs[index].dialogsChildTexts[childTextCounter].characterID < 0) ? charactersInfo[CustomersController.instance.GetCurrentCustomer().customerID].characterTextColor : charactersInfo[allDialogs[index].dialogsChildTexts[childTextCounter].characterID].characterTextColor;
		for (int i = 0; i < wordToShow.Length; i++)
		{
			AudioController.instance.SpawnDialogLetterTick();
			originalText += wordToShow[i].ToString();
			if (emptyString.Length > 0)
			{
				emptyString = emptyString.Remove(0, 1);
			}
			dialogTextNormal.text = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + originalText + "</color><color=#00000000>" + emptyString + "</color>";
			dialogTextNormal.gameObject.SetActive(value: true);
			yield return new WaitForSecondsRealtime(letterShowSpeed);
		}
		tempCor = null;
	}

	private bool isLastChildText(int index)
	{
		if (allDialogs[index].playOnRandom || allDialogs[index].dialogsChildTexts.Count - 1 == childTextCounter)
		{
			return true;
		}
		return false;
	}

	private void JustShowText(string textToShow, int index)
	{
		string text = "";
		string text2 = textToShow;
		char[] array = textToShow.ToCharArray();
		Color white = Color.white;
		white = ((allDialogs[index].dialogsChildTexts[childTextCounter].characterID < 0) ? charactersInfo[CustomersController.instance.GetCurrentCustomer().customerID].characterTextColor : charactersInfo[allDialogs[index].dialogsChildTexts[childTextCounter].characterID].characterTextColor);
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString();
			if (text2.Length > 0)
			{
				text2 = text2.Remove(0, 1);
			}
			dialogTextNormal.text = "<color=#" + ColorUtility.ToHtmlStringRGB(white) + ">" + text + "</color><color=#00000000>" + text2 + "</color>";
			dialogTextNormal.gameObject.SetActive(value: true);
		}
	}

	private void StopDialog()
	{
		if (!dialogActive)
		{
			return;
		}
		if (!isLastChildText(currentDialog))
		{
			childTextCounter++;
			if (allDialogs[currentDialog].GetDialogShown() && !allDialogs[currentDialog].dialogsChildTexts[childTextCounter].mainText)
			{
				StopDialog();
				return;
			}
			ShowDialog(currentDialog);
			dialogDoneEvent.Invoke(currentDialog, childTextCounter, arg2: false);
			return;
		}
		allDialogs[currentDialog].SetDialogShown(b: true);
		dialogTextNormal.gameObject.SetActive(value: false);
		if (tempCor != null)
		{
			StopCoroutine(tempCor);
		}
		StartCoroutine(waitForFrameAndUnblock());
		dialogDoneEvent.Invoke(currentDialog, childTextCounter, arg2: true);
		dialogActive = false;
	}

	private IEnumerator waitForFrameAndUnblock()
	{
		yield return new WaitForEndOfFrame();
		PlayerController.instance.ForceStopFocus();
		PlayerController.blockGlobal = false;
		UIController.instance.SetCrosshair(b: true);
	}

	private void Update()
	{
		if (!blockSkipDialog && (Input.GetMouseButtonDown(0) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)))
		{
			if (skipFirstTime)
			{
				skipFirstTime = false;
			}
			else
			{
				StopDialog();
			}
		}
	}
}
