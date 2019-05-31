using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using OnefallGames;
using UnityEngine.UI;
using System;

/// <summary>
/// 处理游戏UI控件
/// </summary>

public class UIManager : MonoBehaviour {

	//单例模式
    public static UIManager Instance { private set; get; }

	public PlayerController playerController;
    //Gameplay UI
    [SerializeField] private GameObject gameplayUI;				//开始游戏顶部界面
    [SerializeField] private Text scoreTxt;						//当前分数
    [SerializeField] private Text coinTxt;						//星星个数

	[SerializeField] private Text coinTxt1;						//星星个数（主界面）

    //Revive UI
    [SerializeField] private GameObject reviveUI;               //复活界面
    [SerializeField] private Image reviveCoverImg;              //复活心形图片背景

    //Gameover UI
    [SerializeField] private GameObject gameOverUI;				//结束界面
    [SerializeField] private Text bestScoreTxt;					//最高分数数字
    [SerializeField] private GameObject gameName;				//主界面游戏名字
    //[SerializeField] private RawImage shareImg;
    [SerializeField] private GameObject playBtn;				//开始游戏按钮
    [SerializeField] private GameObject restartBtn;				//重完游戏按钮
    [SerializeField] private Button dailyRewardBtn;             //每天奖励按钮
    [SerializeField] private Text dailyRewardTxt;				//每天获得奖励数字
    [SerializeField] private GameObject watchAdForCoinsBtn;		//观看视频按钮
    [SerializeField] private GameObject MoreBtn;				//更多游戏按钮
    [SerializeField] private GameObject soundOnBtn;				//声音开启按钮
    [SerializeField] private GameObject soundOffBtn;			//声音关闭按钮
    [SerializeField] private GameObject musicOnBtn;				//音乐开启按钮
    [SerializeField] private GameObject musicOffBtn;			//音乐关闭按钮
    [SerializeField] private Animator servicesUIAnim;			
    [SerializeField] private Animator settingUIAnim;
    [SerializeField] private AnimationClip servicesUI_Show;
    [SerializeField] private AnimationClip servicesUI_Hide;
    [SerializeField] private AnimationClip settingUI_Show;
    [SerializeField] private AnimationClip settingUI_Hide;

	[SerializeField] private GameObject pauseOnBtn;				//暂停按钮
	[SerializeField] private GameObject startOnBtn;				//继续游戏按钮
	[SerializeField] private GameObject remainOnBtn;			//返回主界面按钮
	[SerializeField] private GameObject buyOnBtn;				//内购开启按钮
	[SerializeField] private GameObject storeUI;				//内购界面
    [SerializeField] private GameObject buyOffBtn;
    [SerializeField] private GameObject rankOnBtn;

    public  bool isClick;										//判断暂停后是否开启继续游戏



    //Reward coins control UI
    [SerializeField] private RewardedCoinsController rewardCoinsControl;        //每日获得奖励UI脚本


    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }


    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

	//改变游戏状态
    private void GameManager_GameStateChanged(GameState obj)
    {
        
        if (obj == GameState.GameOver)          //死亡状态
        {
            StartCoroutine(ShowGameOverUI());
        }
        else if (obj == GameState.Revive)       //复活状态
        {
            StartCoroutine(ShowReviveUI());
        }
        else if (obj == GameState.Playing)      //游戏开始状态
        {
			Debug.Log ("c");
            gameplayUI.SetActive(true);
			pauseOnBtn.SetActive (true);
            gameOverUI.SetActive(false);
            reviveUI.SetActive(false);
            rewardCoinsControl.gameObject.SetActive(false);

			buyOnBtn.SetActive (false);
            rankOnBtn.SetActive(false);
        }
    }

    void Awake()
    {
		//如果创建的单例对象为空，则重新创建
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }




    // Use this for initialization
    void Start () {
        //PlayerPrefs.DeleteAll();

        //DateTime dateTime = DateTime.Now;
        //DateTime d2 = new DateTime(2020, 8, 31);
        //TimeSpan t = d2.Subtract(dateTime);
        //DateTime d3 = new DateTime();

        //Debug.Log("dateTime===" + dateTime);
        //Debug.Log("dateTime2===" + d2);
        //Debug.Log("dateTime3===" + t);

        //第一次加载
        if (!GameManager.isRestart) //This is the first load
        {
            rankOnBtn.SetActive(true);
			gameplayUI.SetActive(false);
            reviveUI.SetActive(false);
            rewardCoinsControl.gameObject.SetActive(false);
            gameOverUI.SetActive(true);

            restartBtn.SetActive(false);
            bestScoreTxt.gameObject.SetActive(false);
            //shareImg.gameObject.SetActive(false);
            watchAdForCoinsBtn.SetActive(false);
            dailyRewardBtn.gameObject.SetActive(false);
			//MoreBtn.SetActive(false);
			pauseOnBtn.SetActive(false);
			startOnBtn.SetActive (false);

			DontDestroyOnLoad(this);

			isClick = true;
            GameSdkUnity.Instance.showBanner();

        }
    }
	
	// Update is called once per frame
	void Update () {

        UpdateMusicButtons();
        UpdateMuteButtons();

        scoreTxt.text = ScoreManager.Instance.Score.ToString();
		bestScoreTxt.text = Localization.Get(this.gameObject, "BEST") + ScoreManager.Instance.BestScore.ToString();
		if (coinTxt) 
		{
			coinTxt.text = CoinManager.Instance.Coins.ToString();
		}
		if (coinTxt1) 
		{
			coinTxt1.text = CoinManager.Instance.Coins.ToString();
		}

        if(DailyRewardManager.Instance.CanRewardNow())
        {
            dailyRewardTxt.text = Localization.Get(this.gameObject, "CLAIM");
            dailyRewardBtn.interactable = true;
        }
        else
        {
            string hours = DailyRewardManager.Instance.TimeUntilNextReward.Hours.ToString();
            string minutes = DailyRewardManager.Instance.TimeUntilNextReward.Minutes.ToString();
            string seconds = DailyRewardManager.Instance.TimeUntilNextReward.Seconds.ToString();
			if (dailyRewardTxt) 
			{
				dailyRewardTxt.text = hours + ":" + minutes + ":" + seconds;
			}
            
            dailyRewardBtn.interactable = false;
        }


	}


    //////////////////////////// 
    //每次点击有点击音乐
    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
    }

    public void ReviveBtn()
    {
        reviveUI.SetActive(false);
        //AdManager.Instance.ShowRewardedVideoAd();
    }

    public void SkipBtn()
    {
        reviveUI.SetActive(false);
        GameManager.Instance.GameOver();
    }
	//开始游戏
    public void PlayBtn()
    {
        rankOnBtn.SetActive(false);
        playBtn.SetActive(false);
        gameOverUI.SetActive(false);
		buyOnBtn.SetActive (false);
        GameManager.Instance.PlayingGame();
		Debug.Log ("4");
    }
	//重新开始游戏
    public void RestartBtn(float delay)
    {
        isClick = true;
        Debug.Log("RestarBtn");
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name, delay);
		pauseOnBtn.SetActive (false);
		startOnBtn.SetActive (false);
        rankOnBtn.SetActive(false);
        Debug.Log("RestarBtn=" + GameManager.isRestart);
    }
	//每天获取游戏奖励
    public void DailyRewardBtn()
    {
        DailyRewardManager.Instance.ResetNextRewardTime();
        StartReward(0.5f, DailyRewardManager.Instance.GetRandomReward());
    }

    //观看视频获得奖励
    public void WatchAdForCoinsBtn()
    {
        GameSdkUnity.Instance.showVideoAds("1", () => {
            WatchAdForCoins();
        });
    }
    //观看视频后回调增加金币函数
    public void WatchAdForCoins()
    {
        CoinManager.Instance.AddCoins(50);
    }
	//更多游戏
    public void NativeShareBtn()
    {
		Debug.Log ("moregame");
        ShareManager.Instance.NativeShare();
		GameSdkUnity.Instance.openMoreGames ();
    }
    public void RateAppBtn()
    {
        Application.OpenURL(ShareManager.Instance.AppUrl);
    }
	//游戏商店
    public void CharacterBtn()
    {
        GameManager.Instance.LoadScene("Character", 0.5f);
		Debug.Log ("buy");
    }
	//游戏设置
    public void SettingBtn()
    {
        settingUIAnim.Play(settingUI_Show.name);
        servicesUIAnim.Play(servicesUI_Hide.name);
        buyOnBtn.SetActive(false);
    }
	//声音设置
    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();

    }
	//音乐设置
    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }

    public void FacebookShareBtn()
    {
        ShareManager.Instance.FacebookShare();
    }
	//返回设置
    public void BackBtn()
    {
        settingUIAnim.Play(settingUI_Hide.name);
        servicesUIAnim.Play(servicesUI_Show.name);
        buyOnBtn.SetActive(true);
    }
	//游戏菜单
	public void showGameBtn()
	{
		GameSdkUnity.Instance.openGameMenu ();
		Debug.Log ("游戏菜单");
	}
	//暂停
	public void pauseBtn()
	{
        //		Vector3 mouse = Input.mousePosition;
        //		if (mouse.x > 0.2 && mouse.y > 0.2) 
        //		{
        //			
        //		}
        //		Debug.Log (mouse.x + "   " + mouse.y);

        //playerController.enabled = false;
        GameManager.isRestart = false;
        isClick = false;
        //GameManager.Instance.pause();
		Time.timeScale = 0;
		startOnBtn.SetActive (true);
		pauseOnBtn.SetActive (false);
		remainOnBtn.SetActive (true);
		GameSdkUnity.Instance.showInterstitial ();
		

	}

	//继续开始  
	public void startBtn()
	{
        //playerController.enabled = true;
        GameManager.isRestart = true;
		GameManager.Instance.start ();
		Debug.Log ("5");
		Time.timeScale = 1;
		pauseOnBtn.SetActive (true);
		startOnBtn.SetActive (false);
		remainOnBtn.SetActive (false);
        //buyOnBtn.SetActive(false);
		isClick = true;

	}

	//返回主界面
	public void remainBtn()
	{
        if(!isClick && !GameManager.isRestart)
        {
            gameOverUI.SetActive(true);
            Debug.Log("remainBtn="+GameManager.Instance.GameState);
            SceneManager.LoadScene("Gameplay");
            Time.timeScale = 1;
            buyOnBtn.SetActive(true);
            //this.gameObject.SetActive (true);
        }

	}

	//内购
	public void store()
	{
		storeUI.SetActive (true);
		buyOnBtn.SetActive (false);
        buyOffBtn.SetActive(true);
        rankOnBtn.SetActive(false);

	}

	//关闭内购
	public void close_store()
	{
		storeUI.SetActive (false);
		buyOnBtn.SetActive (true);
        buyOffBtn.SetActive(false);
        rankOnBtn.SetActive(true);
	}

    //排行榜
    public void rank()
    {
        GameSdkUnity.Instance.openRank();
    }

  
    /////////////////////////////Private functions
    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
			if (soundOnBtn.gameObject && soundOffBtn.gameObject) 
			{
				soundOnBtn.gameObject.SetActive(false);
				soundOffBtn.gameObject.SetActive(true);
			}
            
        }
        else
        {
			if (soundOnBtn.gameObject) 
			{
				soundOnBtn.gameObject.SetActive(true);
			}

			if(soundOffBtn.gameObject)
			{
				soundOffBtn.gameObject.SetActive(false);
			}
            
        }
    }


    void UpdateMusicButtons()
    {
        if (SoundManager.Instance.IsMusicOff())
        {
			if (musicOffBtn.gameObject) 
			{
				musicOffBtn.gameObject.SetActive(true);
			}
            
			if (musicOnBtn.gameObject) 
			{
				musicOnBtn.gameObject.SetActive(false);
			}
            
        }
        else
        {
			if(musicOffBtn.gameObject && musicOnBtn.gameObject)
			{
				musicOffBtn.gameObject.SetActive(false);
				musicOnBtn.gameObject.SetActive(true);
			}
            
        }
    }


    IEnumerator ShowGameOverUI()
    {
        while (Time.timeScale != 1)
        {
            yield return new WaitForSecondsRealtime(0.02f);
        }

        yield return new WaitForSeconds(0.5f);

        GameSdkUnity.Instance.submitScoreToRank(1, ScoreManager.Instance.Score);
		gameplayUI.SetActive(false);
        reviveUI.SetActive(false);
        rewardCoinsControl.gameObject.SetActive(false);

		GameSdkUnity.Instance.showInterstitial ();
		pauseOnBtn.SetActive (false);
		startOnBtn.SetActive (false);

        gameOverUI.SetActive(true);

        bestScoreTxt.gameObject.SetActive(true);
        gameName.SetActive(false);

        playBtn.SetActive(false);
        restartBtn.SetActive(true);

        //shareImg.gameObject.SetActive(true);
        //shareImg.texture = GameManager.Instance.LoadedScrenshot();

        watchAdForCoinsBtn.SetActive(true);
        dailyRewardBtn.gameObject.SetActive(true);
        //watchAdForCoinsBtn.SetActive(AdManager.Instance.IsRewardedVideoAdReady());
        //buyOnBtn.SetActive(true);

    }


    IEnumerator ShowReviveUI()
    {
        while (Time.timeScale != 1)
        {
            yield return new WaitForSecondsRealtime(0.02f);
        }

        yield return new WaitForSeconds(0.5f);
        reviveUI.SetActive(true);
        StartCoroutine(ReviveCountDown());
    }


    IEnumerator ReviveCountDown()
    {
        float t = 0;
        while (t < GameManager.Instance.reviveCountDownTime)
        {
            if (!reviveUI.activeInHierarchy)
                yield break;
            t += Time.deltaTime;
            float factor = t / GameManager.Instance.reviveCountDownTime;
            reviveCoverImg.fillAmount = Mathf.Lerp(1f, 0f, factor);
            yield return null;
        }

        reviveUI.SetActive(false);
        GameManager.Instance.GameOver();
    }





    public void StartReward(float delay, int coins)
    {
        StartCoroutine(RewardingCoins(delay, coins));
    }

    IEnumerator RewardingCoins(float delay, int coins)
    {
        yield return new WaitForSeconds(delay);
        rewardCoinsControl.gameObject.SetActive(true);
        rewardCoinsControl.StartReward(coins);
    }
}
