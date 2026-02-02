using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class NonEnglishOnlyUtility : MonoBehaviour
{
	private void Awake()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnEnable()
	{
		CheckLanguage();
	}

	private void OnDestroy()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}


	private void CheckLanguage()
	{
		// If the active language is english, disable this game object
		var locale = LocalizationSettings.SelectedLocale;
		if (locale == null)
			return;

		// assumes English locale code starts with "en" (e.g., "en", "en-US")
		bool isEnglish = locale.Identifier.Code.StartsWith("en");
		gameObject.SetActive(!isEnglish);
	}


	private void OnLocaleChanged(Locale locale) => CheckLanguage();
}