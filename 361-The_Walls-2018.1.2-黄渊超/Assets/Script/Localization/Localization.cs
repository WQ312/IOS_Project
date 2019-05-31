using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Localization : MonoBehaviour {

	// path to the folder the text files are in
	public static string filePath = "Localization/";
	// Dictionary contains the localization information
	static Dictionary<string, string> dictionary = new Dictionary<string, string>();
	// Boolean to check if the localization is loaded yet
	public static bool localizationLoaded = false;
	// Keeps track of which language to use
	static string currentLanguage = "Example";

	static List<GameObject> localizeGameObjects = new List<GameObject>();

	public static string language
	{
		get { return currentLanguage; }
		set
		{
			currentLanguage = value;
			// Load the new file
			localizationLoaded = false;
			Load ();
			// Re load all the objects xLocalize
			foreach(GameObject obj in localizeGameObjects)
			{
				if (obj != null) obj.GetComponent<Localize>().LoadKey();
			}
		}
	}

	public static void LoadLocalization()
	{
		Debug.Log("LoadLocalization Load");
		if (!localizationLoaded)
		{
			dictionary.Clear();
			// split text file on ENTER
			string[] separator = new string[] {"\n"};
			// Load text file
			TextAsset textAsset = (TextAsset)Resources.Load(filePath+currentLanguage, typeof(TextAsset));
			// Check if textfile is succesfully loaded
			if (textAsset == null)
			{
				Debug.LogWarning("[Localization] Localization not found: " + filePath+currentLanguage);
				return;
			}
			// Save textfile into list
			List<string> tempString = textAsset.text.Split(separator, System.StringSplitOptions.None).ToList();
			// split on ' = '
			separator = new string[] {" = "};
			// A list to check if the key doesn't exist yet
			List<string> doubleKeyCheck = new List<string>();
			// enter the list into the dictionary
			foreach (string str in tempString)
			{
				// split each list on the separator
				string[] tString = str.Split(separator, System.StringSplitOptions.None).ToArray();
				if (tString.Length > 2)
				{
					foreach (string tempStr in tString)
					{
						if (tempStr != tString[0] && tempStr != tString[1]) 
						{
							tString[1] = tString[1] +" = "+ tempStr;
						}
					}
				}
				// Takes care of empty lines
				if (tString.Length > 1)
				{
					tString[1] = tString[1].Replace("\\n", System.Environment.NewLine);
					// seperated list items go into dictionary
					if (!doubleKeyCheck.Contains(tString[0])) 
					{
						doubleKeyCheck.Add(tString[0]);
						dictionary.Add(tString[0], tString[1]);
					}
					else Debug.LogWarning("[Localization] Identical keys ("+tString[0]+") found while loading localization");
				}
			}
			// Localization is loaded
			localizationLoaded = true;
		}
	}

	public static void Load()
	{
		// Loads file and adds to dictionary
		LoadLocalization();
	}

	public static string Get(string key) 
	{ 
		// Checks if localization is loaded yet
		if (!localizationLoaded) Debug.LogWarning("[Localization] Localization hasn't been loaded yet.");//Load ();
		if (localizationLoaded)
		{
			// Returns the value from the dictionary
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key];
			}
			Debug.LogWarning("[Localization] Could not find the key "+key);
		}
		return key;
	}

	public static string Get(GameObject currentGameObject, string key)
	{
		Languages.Init();

		// Stores all objects that should be updated when the language changes
		// bool objCheck = true;
		// foreach (GameObject obj in localizeGameObjects) if (obj == currentGameObject) objCheck = false;
		// if (objCheck) localizeGameObjects.Add(currentGameObject);

		// Checks if localization is loaded yet
		if (!localizationLoaded) Debug.LogWarning("[Localization] Localization hasn't been loaded yet.");//Load ();
		if (localizationLoaded)
		{
			// Returns the value from the dictionary
			if (dictionary.ContainsKey(key))
			{
				// Returns the value
				return dictionary[key];
			}
			Debug.LogWarning("[Localization] Could not find the key "+key);
		}
		return key;
	}

	public static void Set(string key, string value)
	{
		// Checks if localization is loaded yet
		if (!localizationLoaded) Load ();
		// Checks if dictionary has the key
		if (dictionary.ContainsKey(key))
		{
			// Saves dictionary into temporary dictionary
			Dictionary<string, string> tempDic = new Dictionary<string, string>(dictionary);
			// Dictionary keys go in to temporary list
			List<string> tempList = new List<string>(dictionary.Keys);
			// clear the main dictionary
			dictionary.Clear();
			// aplly list into dictionary
			foreach(string str in tempList)
			{
				// Adds the new key and value to the dictionary And restores the keys and values that shouldn't be changed
				if (str == key) dictionary.Add(key, value);
				else dictionary.Add(str, tempDic[str]);
			}
			// Returns if done
			return;
		}
		// Adds a new key and value to the dictionary
		dictionary.Add(key, value);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
