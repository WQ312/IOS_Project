using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class GameSdkUnity : MonoBehaviour {
	
    private Action _videoCallback;
    private Action<int> _payCallback;
    #region CSharpCallOC
    [DllImport("__Internal")]
    private static extern void initSdk();
    [DllImport("__Internal")]
    private static extern void payProp(int goodId);
    [DllImport("__Internal")]
    private static extern void showInterstitialAds(int randomNum);
    [DllImport("__Internal")]
    private static extern void delayShowInterstitialAds();
    [DllImport("__Internal")]
    private static extern void showRewardVideoAds(string key);
    [DllImport("__Internal")]
    private static extern void showBannerAds();
    [DllImport("__Internal")]
    private static extern void hideBannerAds();
    [DllImport("__Internal")]
    private static extern void moreGames();
    [DllImport("__Internal")]
    private static extern void rateGame();
    [DllImport("__Internal")]
    private static extern void openGameCenter();
    [DllImport("__Internal")]
    private static extern void submitScoreToGameCenter(int rankId, int score);
    [DllImport("__Internal")]
    private static extern void privacyUrl();
    [DllImport("__Internal")]
    private static extern void shareGame();
    [DllImport("__Internal")]
    private static extern void gameMenu();
    [DllImport("__Internal")]
    private static extern void showMessage(string msg);
    [DllImport("__Internal")]
    private static extern void showAlert(string msg);
    [DllImport("__Internal")]
    private static extern bool isIpad();
    [DllImport("__Internal")]
    private static extern bool isIphoneX();

    #endregion

    private float pTimeScale = 1.0f;


    void Awake()
    {
        instance = this;

        //DontDestroyOnLoad(transform.gameObject);
		StartCoroutine(WaitInitSdk(5.0F));

#if UNITY_EDITOR
        gameObject.AddComponent<GameSdkTestUI>();
#endif
	}

	IEnumerator WaitInitSdk(float waitTime)  
	{  
		yield return new WaitForSeconds(waitTime);
#if UNITY_IPHONE && !UNITY_EDITOR
		initSdk();
#endif
    }  

    /// <summary>
    /// 启动内购
    /// </summary>
    /// <param name="goodsId">商品id</param>
    /// <param name="callBack">内购成功回掉函数</param>
    public void payGoods(int goodsId, Action<int> callBack)
    {
        _payCallback = callBack;
        Debug.Log("购买物品:" + goodsId + "=========");
#if UNITY_IPHONE && !UNITY_EDITOR
		payProp(goodsId);
#else
        IosCallPaySuccess(goodsId.ToString());
#endif
    }

    /// <summary>
    /// 显示插页广告
    /// </summary>
    /// <param name="randomNum">广告几率 一般默认就好</param>
    public void showInterstitial(int randomNum = 0)
    {
        Debug.Log("显示插页广告");
#if UNITY_IPHONE && !UNITY_EDITOR
        showInterstitialAds(randomNum);
#else
        gameObject.GetComponent<GameSdkTestUI>().testShowAds();
#endif
    }

    /// <summary>
    /// 显示奖励视频广告
    /// </summary>
    /// <param name="key">传"free"就好</param>
    /// <param name="callBack">观看视频成功回掉函数</param>
    public void showVideoAds(string key, Action callback = null)
    {
        _videoCallback = callback;
        Debug.Log("观看视频插页广告" + key);
#if UNITY_IPHONE && !UNITY_EDITOR
		showRewardVideoAds(key);
#else
        IosCallVideoSuccess();
#endif
    }

    /// <summary>
    /// 显示横幅广告
    /// </summary>
    public void showBanner()
    {
        
            Debug.Log("showBanner");
#if UNITY_IPHONE && !UNITY_EDITOR
        showBannerAds();
#else
		gameObject.GetComponent<GameSdkTestUI>().testShowBanner();
#endif
       

    }

    /// <summary>
    /// 隐藏横幅广告
    /// </summary>
    public void hideBanner()
    {
        Debug.Log("hideBanner");
#if UNITY_IPHONE && !UNITY_EDITOR
		hideBannerAds();
#else
        gameObject.GetComponent<GameSdkTestUI>().testHideBanner();

#endif
    }

    /// <summary>
    /// 打开更多游戏
    /// </summary>
    public void openMoreGames()
    {
        Debug.Log("更多游戏");
#if UNITY_IPHONE && !UNITY_EDITOR
        moreGames();
#endif
    }

    /// <summary>
    /// 打开游戏评价
    /// </summary>
    public void openReteGame()
    {
        Debug.Log("游戏评价");
#if UNITY_IPHONE && !UNITY_EDITOR
		rateGame();
#endif
    }

    /// <summary>
    /// 打开游戏排行榜
    /// </summary>
    public void openRank() 
    {
        Debug.Log("游戏排行榜");
#if UNITY_IPHONE && !UNITY_EDITOR
		openGameCenter();
#endif
    }

    /// <summary>
    /// 提交数据到游戏排行榜
    /// </summary>
    /// <param name="rankId">排行榜id</param>
    /// <param name="score">要提交的分数</param>
    public void submitScoreToRank(int rankId, int score) 
    {
        Debug.Log("提交数据到游戏排行榜 分数:" + score);
#if UNITY_IPHONE && !UNITY_EDITOR
		submitScoreToGameCenter(rankId, score);
#endif
    }

    /// <summary>
    /// 打开隐私政策
    /// </summary>
    public void openPrivacyUrl()
    {
        Debug.Log("打开隐私政策");
#if UNITY_IPHONE && !UNITY_EDITOR
        privacyUrl();
#endif
    }

    /// <summary>
    /// 打开分享游戏
    /// </summary>
    public void openShareGame()
    {
        Debug.Log("打开分享游戏");
#if UNITY_IPHONE && !UNITY_EDITOR
        shareGame();
#endif
    }

    /// <summary>
    /// 打开游戏菜单
    /// </summary>
    public void openGameMenu()
    {
        Debug.Log("打开游戏菜单");
#if UNITY_IPHONE && !UNITY_EDITOR
        gameMenu();
#endif
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    /// <param name="msg">信息</param>
    public void showGameMessage(string msg)
    {
        Debug.Log("打开游戏菜单");
#if UNITY_IPHONE && !UNITY_EDITOR
        showMessage(msg);
#endif
    }

    /// <summary>
    /// 弹窗提示信息
    /// </summary>
    /// <param name="msg">信息</param>
    public void showGameAlert(string msg)
    {
        Debug.Log("打开游戏菜单");
#if UNITY_IPHONE && !UNITY_EDITOR
        showAlert(msg);
#endif
    }

    /// <summary>
    /// 设备是否是ipad
    /// </summary>
    public bool deviceIsIpad()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return isIpad();
#else
        return false;
#endif
    }

    /// <summary>
    /// 设备是否是iphoneX
    /// </summary>
    public bool deviceIsIphoneX()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return isIphoneX();
#else
        return false;
#endif
    }


#region OCCallCSharp

    void IosCallPauseGame()
    {
        Debug.Log("SDK 调用暂停游戏!!!!!");
        // pTimeScale = Time.timeScale;
        // Time.timeScale = 0.0f;
    }

    void IosCallResumeGame()
    {
        Debug.Log("SDK 调用继续游戏!!!!!");
        // Time.timeScale = pTimeScale;
    }

    void IosCallVideoSuccess()
    {
        Debug.Log("观看成功!!!!!");
        if (_videoCallback != null)
        {
            _videoCallback();
            _videoCallback = null;
        }
    }
    
	void IosCallPaySuccess(string goodsId)
    {
        Debug.Log("内购成功!!!!!");
        if (_payCallback != null)
        {
            _payCallback(int.Parse(goodsId));
            _payCallback = null;
        }
    }

    void IosCallShareSuccess()
    {
        Debug.Log("分享成功!!!!!");
    }

    void IosCallShareFail()
    {
        Debug.Log("分享失败!!!!!");
    }
#endregion

#region Singleton instance
    protected static GameSdkUnity instance = null;
    public static GameSdkUnity Instance
    {
        get
        {
            if(instance==null){
               
                GameObject sdkobj = new GameObject("Sdk");
                DontDestroyOnLoad(sdkobj);
                instance = sdkobj.AddComponent<GameSdkUnity>();
            }
            return instance;
        }
    }
#endregion
}