using UnityEngine;
using UnityEngine.UI;

public class SettingsBar : MonoBehaviour
{
	public int currentSettingsGroupID;

	public string[] settingsValuesGroup;

	[SerializeField]
	protected Text valueText;

	private void Start()
	{
		LateStart();
	}

	public virtual void LateStart()
	{
		SetValueText(settingsValuesGroup[currentSettingsGroupID]);
	}

	public void ClickNext()
	{
		currentSettingsGroupID++;
		if (currentSettingsGroupID >= settingsValuesGroup.Length)
		{
			currentSettingsGroupID = 0;
		}
		StateChanged(currentSettingsGroupID);
	}

	public void ClickPrevious()
	{
		currentSettingsGroupID--;
		if (currentSettingsGroupID < 0)
		{
			currentSettingsGroupID = settingsValuesGroup.Length - 1;
		}
		StateChanged(currentSettingsGroupID);
	}

	public virtual void StateChanged(int currentID)
	{
		SetValueText(settingsValuesGroup[currentID]);
	}

	protected void SetValueText(string textValue)
	{
		valueText.text = textValue;
	}
}
