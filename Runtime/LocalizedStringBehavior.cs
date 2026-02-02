using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Localized String_ New", menuName = "Behaviors/Localized String")]
public class LocalizedStringBehavior : ScriptableObject
{
	public LocalizedString localizedString;


	public void SetStringOnText(TMP_Text text)
	{
		localizedString.GetLocalizedStringAsync().Completed += handle =>
		{
			text.text = handle.Result;
		};
	}
}