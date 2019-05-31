using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class GameSdkTestUI : MonoBehaviour
{

    private GameObject ads_test_interstitial;
    private GameObject ads_test_banner;

    const string FONT_PATH = "Assets/Resources/Fonts/Millennia1.ttf";

    void Awake()
    {
    }

    private static GameObject createBanner()
    {

		int bannerW = 320;
		int bannerH = 50;
		//if (Screen.width >= 728 && Screen.height >= 90)
		//{
		//	bannerW = 728;
		//	bannerH = 90;
		//}
		//else if (Screen.width >= 468 && Screen.height >= 60)
		//{
		//	bannerW = 468;
		//	bannerH = 60;
		//}
		//else if (Screen.width >= 320 && Screen.height >= 50)
		//{
		//	bannerW = 320;
		//	bannerH = 50;
		//}

		// 横幅广告
		GameObject bannerImg = new GameObject("bannerImg", typeof(Image));
        bannerImg.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (Screen.width - bannerW) / 2, bannerW);
        bannerImg.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, bannerH);
        bannerImg.GetComponent<Image>().color = new Color(253.0f / 255.0f, 137.0f / 255.0f, 128.0f / 255.0f);

        GameObject bannerText = new GameObject("bannerText", typeof(Text));
        bannerText.GetComponent<Text>().text = "横幅测试";
        bannerText.GetComponent<Text>().fontSize = 25;
        bannerText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        //bannerText.GetComponent<Text>().font = AssetDatabase.LoadAssetAtPath<Font>(FONT_PATH);
        bannerText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (Screen.width - bannerW) / 2, bannerW);
        bannerText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, bannerH);
        bannerText.transform.SetParent(bannerImg.transform);

        return bannerImg;
    }

    private static GameObject createInterAds()
    {
        // 插页广告
        GameObject adsImg = new GameObject("adsImg", typeof(Image));
        adsImg.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, Screen.width);
        adsImg.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, Screen.height);
        adsImg.GetComponent<Image>().color = new Color(97.0f / 255.0f, 215.0f / 255.0f, 255.0f / 255.0f);

        GameObject adsCloseBtn = new GameObject("adsCloseBtn", typeof(Button));
        adsCloseBtn.AddComponent<Image>();
        adsCloseBtn.GetComponent<Button>().targetGraphic = adsCloseBtn.GetComponent<Image>();
        adsCloseBtn.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Screen.width - 100, 50);
        adsCloseBtn.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Screen.height - 100, 50);
        
        GameObject closeText = new GameObject("closeText", typeof(Text));
        closeText.GetComponent<Text>().text = "X";
        closeText.GetComponent<Text>().fontSize = 25;
        closeText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        //closeText.GetComponent<Text>().font = AssetDatabase.LoadAssetAtPath<Font>(FONT_PATH);
        closeText.GetComponent<Text>().color = new Color(0, 0, 0);
        closeText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Screen.width - 100, 50);
        closeText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Screen.height - 100, 50);
        closeText.transform.SetParent(adsCloseBtn.transform);

        GameObject adsText = new GameObject("adsText", typeof(Text));
        adsText.GetComponent<Text>().text = "插页广告模拟测试";
        adsText.GetComponent<Text>().fontSize = 40;
        adsText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        //adsText.GetComponent<Text>().font = AssetDatabase.LoadAssetAtPath<Font>(FONT_PATH);
        adsText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (Screen.width - 350) / 2, 350);
        adsText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, (Screen.height - 50) / 2, 50);

        adsText.transform.SetParent(adsImg.transform);
        adsCloseBtn.transform.SetParent(adsImg.transform);
        return adsImg;
    }

    private static GameObject getCanvas()
    {
        GameObject canvas = GameObject.Find("SdkTestCanvas");
        if (!canvas) {
            canvas = new GameObject("SdkTestCanvas", typeof(Canvas));
            canvas.AddComponent<GraphicRaycaster>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.GetComponent<Canvas>().sortingOrder = 999;
			DontDestroyOnLoad(canvas);
		}

        GameObject eventSystem = GameObject.Find("SdkTestEventSystem");
        if (!eventSystem) {
            eventSystem = new GameObject("SdkTestEventSystem", typeof(EventSystem));
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        return canvas;
    }

    public void testShowBanner()
    {
        if (ads_test_banner)
        {
            return;
        }
        if (!ads_test_banner)
        {
            ads_test_banner = createBanner();
            ads_test_banner.transform.SetParent(getCanvas().transform);
        }
        ads_test_banner.transform.SetAsLastSibling();
    }

    public void testHideBanner()
    {
        if (ads_test_banner)
        {
            DestroyObject(ads_test_banner);
        }
    }

    public void testShowAds()
    {
        if (ads_test_interstitial)
        {
            return;
        }

        ads_test_interstitial = createInterAds();
        ads_test_interstitial.transform.SetParent(getCanvas().transform);
        ads_test_interstitial.transform.SetAsLastSibling();
        GameObject.Find("adsCloseBtn").GetComponent<Button>().onClick.AddListener(delegate () {
            DestroyObject(ads_test_interstitial);
            ads_test_interstitial = null;
        });
    }
}