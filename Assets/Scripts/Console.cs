using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
	public static Console instance;

	public InputField inputField;

	public ScrollRect scrollRect;

	public GameObject consoleHolder;

	public GameObject textPrefab;

	public GameObject contentObj;

	public Text[] suggestionsTextObjects;

	public List<string> suggestions = new List<string>();

	private int counter;

	private Text selectedSuggestedText;

	private int suggestTextSelectID = -1;

	private void Awake()
	{
		if ((bool)instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
		}
		consoleHolder.SetActive(value: false);
	}

	private void Update()
	{
		if ((UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter)) && consoleHolder.activeSelf)
		{
			if ((bool)selectedSuggestedText)
			{
				SuggestionClicked(selectedSuggestedText);
				if (!inputField.text.Contains(" "))
				{
					Submit();
				}
			}
			else
			{
				Submit();
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
		{
			Select(1);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
		{
			Select(-1);
		}
	}

	public void Submit()
	{
		CheckCommand(inputField.text);
		ClearInputText();
		FocusInput();
		SearchSuggestions();
	}

	public void OnValueChanged()
	{
		SearchSuggestions();
		ResetSelect();
		Select(1);
	}

	private void ResetSelect()
	{
		selectedSuggestedText = null;
		suggestTextSelectID = -1;
	}

	private void Select(int index)
	{
		if (counter != 0)
		{
			if ((bool)selectedSuggestedText)
			{
				selectedSuggestedText.transform.GetChild(0).gameObject.SetActive(value: false);
			}
			suggestTextSelectID += index;
			if (suggestTextSelectID >= counter)
			{
				suggestTextSelectID = 0;
			}
			else if (suggestTextSelectID < 0)
			{
				suggestTextSelectID = counter - 1;
			}
			selectedSuggestedText = suggestionsTextObjects[suggestTextSelectID];
			selectedSuggestedText.transform.GetChild(0).gameObject.SetActive(value: true);
		}
	}

	private void SearchSuggestions()
	{
		counter = 0;
		if (inputField.text == "")
		{
			for (int i = counter; i < suggestionsTextObjects.Length; i++)
			{
				suggestionsTextObjects[i].gameObject.SetActive(value: false);
			}
			return;
		}
		for (int j = 0; j < suggestions.Count; j++)
		{
			if (suggestions[j].ToLower().Contains(inputField.text.ToLower()) && suggestions[j] != inputField.text)
			{
				suggestionsTextObjects[counter].text = suggestions[j];
				suggestionsTextObjects[counter].gameObject.SetActive(value: true);
				suggestionsTextObjects[counter].transform.GetChild(0).gameObject.SetActive(value: false);
				counter++;
				if (counter >= suggestionsTextObjects.Length)
				{
					return;
				}
			}
		}
		for (int k = counter; k < suggestionsTextObjects.Length; k++)
		{
			suggestionsTextObjects[k].gameObject.SetActive(value: false);
		}
	}

	public void SuggestionClicked(Text textObj)
	{
		inputField.text = textObj.text;
		FocusInput();
		ResetSelect();
	}

	public void ToggleConsole()
	{
		consoleHolder.SetActive(!consoleHolder.activeInHierarchy);
		Cursor.lockState = ((!consoleHolder.activeInHierarchy) ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = consoleHolder.activeInHierarchy;
		if (consoleHolder.activeInHierarchy)
		{
			FocusInput();
		}
		ClearInputText();
	}

	private void ClearInputText()
	{
		inputField.text = "";
	}

	public void FocusInput()
	{
		inputField.ActivateInputField();
		StartCoroutine(FocusInputCor());
	}

	private IEnumerator FocusInputCor()
	{
		yield return new WaitForEndOfFrame();
		inputField.caretPosition = inputField.text.Length;
		inputField.ForceLabelUpdate();
	}

	private void CheckCommand(string command)
	{
		if (CheckIfCommandWithValue(command) || CheckIfCommandWithTwoValue(command))
		{
			return;
		}
		command = command.Trim(' ');
	}

	private bool CheckIfCommandWithValue(string command)
	{
		try
		{
			string[] array = command.Split(' ');
			if (array.Length > 2)
			{
				return false;
			}
			if (array.Length != 2)
			{
				return false;
			}
			float num = float.Parse(array[1]);
			switch (array[0])
			{
			case "walkSpeed":
				ConsoleLog("Walk speed set to = " + num.ToString());
				break;
			case "runSpeed":
				ConsoleLog("Run speed set to = " + num.ToString());
				break;
			case "crouchSpeed":
				ConsoleLog("Crouch speed set to = " + num.ToString());
				break;
			case "jumpSpeed":
				ConsoleLog("Jump speed set to = " + num.ToString());
				break;
			case "gravity":
				ConsoleLog("Gravity set to = " + num.ToString());
				break;
			case "sensitivity":
				ConsoleLog("Sensitivity set to = " + num.ToString());
				break;
			default:
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private bool CheckIfCommandWithTwoValue(string command)
	{
		try
		{
			string[] array = command.Split(' ');
			if (array.Length > 3)
			{
				return false;
			}
			if (array.Length != 3)
			{
				return false;
			}
			float.Parse(array[1]);
			float.Parse(array[2]);
			if (!(array[0] == "add"))
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void ConsoleLog(string text)
	{
		if (contentObj.transform.childCount > 70)
		{
			ClearConsole();
		}
		UnityEngine.Object.Instantiate(textPrefab, base.transform.position, Quaternion.identity, contentObj.transform).GetComponent<Text>().text = text;
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ForceScrollDown());
		}
	}

	private IEnumerator ForceScrollDown()
	{
		yield return new WaitForEndOfFrame();
		scrollRect.verticalNormalizedPosition = 0f;
	}

	private void ClearConsole()
	{
		for (int num = contentObj.transform.childCount; num > 0; num--)
		{
			UnityEngine.Object.Destroy(contentObj.transform.GetChild(num - 1).gameObject);
		}
	}

	private void Start()
	{
		Application.logMessageReceived += HandleLog;
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		ConsoleLog(logString);
	}
}
