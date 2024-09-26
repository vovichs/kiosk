using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionBar : SettingsBar
{
	private TMP_Dropdown dropDownObj;

	private List<Resolution> resolutions = new List<Resolution>();

	private void Start()
	{
		resolutions = new List<Resolution>(Screen.resolutions);
		resolutions.Reverse();
		for (int num = resolutions.Count - 1; num >= 0; num--)
		{
			if (resolutions[num].width < 780)
			{
				resolutions.RemoveAt(num);
			}
		}
		settingsValuesGroup = new string[resolutions.Count];
		dropDownObj = GetComponent<TMP_Dropdown>();
		dropDownObj.ClearOptions();
		for (int i = 0; i < resolutions.Count; i++)
		{
			if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
			{
				currentSettingsGroupID = i;
			}
			settingsValuesGroup[i] = resolutions[i].ToString();
		}
		LateStart();
		dropDownObj.AddOptions(new List<string>(settingsValuesGroup));
		dropDownObj.value = currentSettingsGroupID;
	}

	public override void StateChanged(int currentID)
	{
		base.StateChanged(currentID);
		SetResolution(currentID);
	}

	private void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetRes(bool next = true)
	{
		int value = dropDownObj.value;
		value += (next ? 1 : (-1));
		if (value >= resolutions.Count)
		{
			value = 0;
		}
		else if (value < 0)
		{
			value = resolutions.Count - 1;
		}
		dropDownObj.value = value;
		valueText.text = settingsValuesGroup[value];
		SetResolution(value);
	}
}
