using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Languages : MonoBehaviour {

	private static bool isInitLanguages = false;

	private void Awake()
    {
		Languages.Init();
	}

	public static void Init()
	{
		if (isInitLanguages) {
			return;
		}
		Debug.Log("[Languages] Init called.");

		string selectedLanguage = "ChineseSimplified"; // The default language.

   //     switch (Application.systemLanguage)
   //     {
   //         case SystemLanguage.ChineseSimplified:
   //             selectedLanguage = "ChineseSimplified";
   //             break;
   //         case SystemLanguage.ChineseTraditional:
   //             selectedLanguage = "ChineseTraditional";
   //             break;
			//case SystemLanguage.Chinese:
   //             selectedLanguage = "ChineseSimplified";
   //             break;
   //     }
		
        Localization.language = selectedLanguage;
		Debug.Log("[Languages] Language set to: " + Localization.language);
		Debug.Log("[Languages] Language set to: " + Application.systemLanguage);
		isInitLanguages = true;
	}

	void Start () {
	}
	
	void Update () {
		
	}
}
