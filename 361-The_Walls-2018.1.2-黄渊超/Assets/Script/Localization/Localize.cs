using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Localize : MonoBehaviour {

	public string Key;
	[HideInInspector]
	public string filePath;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake() {
		LoadKey();
	}

	public void LoadKey()
	{
		if (this.GetComponent<Text>() != null) 
		{
			Debug.Log(Localization.Get(this.gameObject, Key));
            this.GetComponent<Text>().text = Localization.Get(this.gameObject, Key);
		}
		else if (this.GetComponent<Image>() != null)
		{
			filePath = Localization.Get (this.gameObject, "FilePath");
			Sprite tempSprite = Resources.Load<Sprite>(filePath + Localization.Get(this.gameObject, Key));
			this.GetComponent<Image>().sprite = tempSprite;
		}
		else if (this.GetComponent<RawImage>() != null) 
		{
			filePath = Localization.Get (this.gameObject, "FilePath");
			Texture tempTexture = Resources.Load<Texture>(filePath + Localization.Get(this.gameObject, Key));
			this.GetComponent<RawImage>().texture = tempTexture;
		}
	}

}
