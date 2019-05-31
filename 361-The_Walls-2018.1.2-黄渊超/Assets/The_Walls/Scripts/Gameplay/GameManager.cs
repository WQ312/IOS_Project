using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnefallGames;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// 游戏管理
/// </summary>

public enum GameState
{
    Prepare,
    Playing,    //游戏中
    Revive,     //重玩
    GameOver,   //死亡
	//Pause,
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance { private set; get; }
    public static event System.Action<GameState> GameStateChanged = delegate { };
    public static bool isRestart = false;

    public GameState GameState
    {
        get
        {
            return gameState;
        }
        private set
        {
            if (value != gameState)
            {
                gameState = value;
                GameStateChanged(gameState);
            }
        }
    }

    //序列化类
    [System.Serializable]
    class WallHoles
    {
        public GameObject Wall = null;
        public GameObject Tiles = null;
    }


    [Header("Gameplay Config")]
    [SerializeField] private float originalWallSpeed = 15f;
    [SerializeField] private float maxWallSpeed = 21f;
    [SerializeField] private float wallSpeedIncreaseAmount = 1f;
    [SerializeField] private int scoreToIncreaseWallSpeed = 5;
    public float wallScaleUpTime = 0.5f;
    public float boxScaleUpTime = 1f;
    [SerializeField] private float slowMotionTimeScale = 0.1f;
    [SerializeField] private float slowMotionDelay = 0.1f;
    public float wallSpeedAfterSlowMotion = 15f;
    [SerializeField] private float backgroundColorLerpTime = 15f;
    [SerializeField] private int maxCoinNumber = 3;
    public float reviveCountDownTime = 4f;
    [SerializeField] private Color[] backgroundColors;

    [Header("Gameplay References")]
    public Texture collisionTexture;
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject coinExplorePrefab;
    [SerializeField]
    private Material skybox;
    [SerializeField]
    private Vector3[] pathPos;
    [SerializeField]
    private List<WallHoles> listWallHolesPrefabs = new List<WallHoles>();

    public bool IsRevived { private set; get; }

    private GameState gameState = GameState.GameOver;
    private List<WallHoles> listWallHoles = new List<WallHoles>();
    private List<GameObject> listCoin = new List<GameObject>();
    private List<GameObject> listCoinExplore = new List<GameObject>();
    private int previousWallHoleIndex = -1;
    private float currentWallSpeed = 0;
    private int previousScore = 0;
    void Awake()
    {
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
    void Start() {

        Application.targetFrameRate = 60;
        ScoreManager.Instance.Reset();

        //Fire event
        GameState = GameState.Prepare;
        gameState = GameState.Prepare;

        //Add another actions here


        IsRevived = false;
        currentWallSpeed = originalWallSpeed;

        //Set the main tex for prefabs and instantiate it
        Texture mainTex = CharacterManager.Instance.characters[CharacterManager.Instance.SelectedIndex].GetComponent<Renderer>().sharedMaterial.mainTexture;


        foreach (WallHoles o in listWallHolesPrefabs)
        {
            GameObject tile = o.Tiles;
            for (int i = 0; i < tile.transform.childCount; i++)
            {
                tile.transform.GetChild(i).GetComponent<Renderer>().sharedMaterial.mainTexture = mainTex;
            }
            GameObject wallHole = o.Wall;
            for (int i = 0; i < wallHole.transform.childCount; i++)
            {
                wallHole.transform.GetChild(i).GetComponent<Renderer>().sharedMaterial.mainTexture = mainTex;
            }
        }

        //Instantiate walls prefab;
        //实例化墙预制体 
        foreach (WallHoles o in listWallHolesPrefabs)
        {
            WallHoles wallHole = new WallHoles();
            wallHole.Tiles = Instantiate(o.Tiles);
            wallHole.Tiles.SetActive(false);
            wallHole.Wall = Instantiate(o.Wall);
            wallHole.Wall.SetActive(false);
            listWallHoles.Add(wallHole);
        }

        //Instantiate coins prefab;
        for (int i = 0; i < 10; i++)
        {
            GameObject coin = Instantiate(coinPrefab, Vector3.zero, Quaternion.identity);
            coin.SetActive(false);
            listCoin.Add(coin);

            GameObject coinExplore = Instantiate(coinExplorePrefab, Vector3.zero, Quaternion.identity);
            coinExplore.SetActive(false);
            listCoinExplore.Add(coinExplore);
        }

        RenderSettings.fogColor = skybox.GetColor("_Tint");

		if (isRestart) {

			PlayingGame();
			Debug.Log ("1");
		}
            
    }

    /// <summary>
    /// Actual start the game
    /// </summary>
    public void PlayingGame()
    {
        Debug.Log("PlayingGame");
        //Fire event
        GameState = GameState.Playing;
        gameState = GameState.Playing;

        //Add another actions here
        if (IsRevived)
        {
            ResumeBGMusic(0.5f);
        }
        else
        {
            PlayBGMusic(0.5f);
            StartCoroutine(IncreaseWallSpeed());
            StartCoroutine(LerpBackgroundColor());
        }

        CreateWallHoleAndCoins();
    }

    /// <summary> 
    /// Call Revive event
    /// </summary>
    public void Revive()
    {
        Debug.Log("Revive");
        //Fire event
        GameState = GameState.Revive;
        gameState = GameState.Revive;

        //Add another actions here
        PauseBGMusic(0.5f);
    }


    /// <summary>
    /// Call GameOver event
    /// </summary>
    public void GameOver()
    {
        Debug.Log("GameOver");
        //Fire event
        GameState = GameState.GameOver;
        gameState = GameState.GameOver;

        //Add another actions here
        isRestart = true;
        StopBGMusic(0.5f);
    }

	//public void pause()
	//{
 //       Debug.Log("pause");
	//	GameState = GameState.Pause;
	//	gameState = GameState.Pause;
	//}

	public void start()
	{
        Debug.Log("start");
		GameState = GameState.Playing;
		gameState = GameState.Playing;
	}


    public void LoadScene(string sceneName, float delay)
    {
        StartCoroutine(LoadingScene(sceneName, delay));
    }

    private IEnumerator LoadingScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    void PlayBGMusic(float delay)
    {
        StartCoroutine(CRPlayBGMusic(delay));
    }

    private IEnumerator CRPlayBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SoundManager.Instance.background != null)
            SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
    }

    void StopBGMusic(float delay)
    {
        StartCoroutine(CRStopBGMusic(delay));
    }

    private IEnumerator CRStopBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SoundManager.Instance.background != null)
            SoundManager.Instance.StopMusic();
    }

    void PauseBGMusic(float delay)
    {
        StartCoroutine(CRPauseBGMusic(delay));
    }

    private IEnumerator CRPauseBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SoundManager.Instance.background != null)
            SoundManager.Instance.PauseMusic();
    }

    void ResumeBGMusic(float delay)
    {
        StartCoroutine(CRResumeBGMusic(delay));
    }

    private IEnumerator CRResumeBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SoundManager.Instance.background != null)
            SoundManager.Instance.ResumeMusic();
    }

    private IEnumerator IncreaseWallSpeed()
    {
        while (true)
        {
            if (gameState == GameState.Playing)
            {
                if (currentWallSpeed < maxWallSpeed)
                {
                    if (ScoreManager.Instance.Score > 0 && ScoreManager.Instance.Score % scoreToIncreaseWallSpeed == 0)
                    {
                        if (ScoreManager.Instance.Score != previousScore)
                        {
                            currentWallSpeed += wallSpeedIncreaseAmount;
                            previousScore = ScoreManager.Instance.Score;
                        }
                    }
                }
                else
                    yield break;
            }
            yield return null;
        }
    }


    private IEnumerator LerpBackgroundColor()
    {
        int index = Random.Range(0, backgroundColors.Length);
        while (true)
        {
            Color startColor = backgroundColors[index];
            index++;
            if (index == backgroundColors.Length)
                index = 0;
            Color endColor = backgroundColors[index];

            float t = 0;
            while (t < backgroundColorLerpTime)
            {
                if (gameState == GameState.GameOver)
                    yield break;
                t += Time.deltaTime;
                float factor = t / backgroundColorLerpTime;
                Color lerpColor = Color.Lerp(startColor, endColor, factor);
                skybox.SetColor("_Tint", lerpColor);
                RenderSettings.fogColor = lerpColor;
                yield return null;
            }
        }
    }



    //Create a list of position base on the original list position with a number
    private List<Vector3> GetRandomPos(List<Vector3> originalPos, int number)
    {
        List<Vector3> finalList = new List<Vector3>();
        while (finalList.Count < number)
        {
            Vector3 pos = originalPos[Random.Range(0, originalPos.Count)];
            if (!finalList.Contains(pos))
                finalList.Add(pos);
        }
        return finalList;
    }



    //Get a coin from list
    private GameObject GetCoin()
    {
        foreach (GameObject o in listCoin)
        {
            if (!o.activeInHierarchy)
            {
                o.SetActive(true);
                return o;
            }
        }
        return null;
    }

    private List<Vector3> ListFreePos()
    {
        List<Vector3> freePos = new List<Vector3>();
        RaycastHit hit;
        foreach (Vector3 o in pathPos)
        {
            Ray rayUp = new Ray(o, Vector3.up);
            if (!Physics.Raycast(rayUp, out hit, 1f))
                freePos.Add(o + Vector3.up);
        }
        return freePos;
    }


    private IEnumerator AdjustingTimeScale()
    {
        Time.timeScale = slowMotionTimeScale;
        yield return new WaitForSeconds(slowMotionDelay);
        Time.timeScale = 1;
    }


    private IEnumerator PlayCoinExplore(ParticleSystem par)
    {
        par.Play();
        yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
        par.gameObject.SetActive(false);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////public functions


    /// <summary>
    /// Set the game back to Playing state
    /// </summary>
    public void SetContinueGame()
    {
        IsRevived = true;
        PlayingGame();
		Debug.Log ("3");
    }


    /// <summary>
    /// Reduce Timescale, wait, then set Timescale back to 1
    /// </summary>
    public void AdjustTimescale()
    {
        StartCoroutine(AdjustingTimeScale());
    }

    /// <summary>
    /// Create wall hole, then create coins
    /// </summary>
    public void CreateWallHoleAndCoins()
    {
        //Create wallhole
        int currentIndex = Random.Range(0, listWallHoles.Count);

        while (currentIndex == previousWallHoleIndex)
        {
            currentIndex = Random.Range(0, listWallHoles.Count);
        }

        previousWallHoleIndex = currentIndex;
        WallHoles wallHole = listWallHoles[currentIndex];
        wallHole.Tiles.transform.position = Vector3.down * 0.5f;
        wallHole.Tiles.SetActive(true);
        wallHole.Wall.transform.position = Vector3.forward * 50f + Vector3.down * 0.5f;
        wallHole.Wall.GetComponent<WallController>().movingSpeed = currentWallSpeed;
        wallHole.Wall.SetActive(true);

        //Create coins
        List<Vector3> coinPos = new List<Vector3>();
        for (int i = 0; i < wallHole.Tiles.transform.childCount; i++)
        {
            Transform trans = wallHole.Tiles.transform.GetChild(i).transform;
            RaycastHit hit;
            Ray ray = new Ray(trans.position, Vector3.up);
            if (!Physics.Raycast(ray, out hit, 1f))
            {
                coinPos.Add(trans.position + Vector3.up * 0.5f);
            }
        }

        if (coinPos.Count <= maxCoinNumber)
        {
            foreach (Vector3 o in coinPos)
            {
                GetCoin().transform.position = o;
            }
        }
        else
        {
            List<Vector3> pos = GetRandomPos(coinPos, maxCoinNumber);
            foreach (Vector3 o in pos)
            {
                GetCoin().transform.position = o;
            }
        }
    }

    /// <summary>
    /// Hide all the active coins
    /// </summary>
    public void HideAllActiveCoin()
    {
        foreach (GameObject o in listCoin)
        {
            if (o.activeInHierarchy)
                o.SetActive(false);
        }
    }


    /// <summary>
    /// Hide the current tile
    /// </summary>
    public void HideCurrentTiles()
    {
        foreach (WallHoles o in listWallHoles)
        {
            if (o.Tiles.activeInHierarchy)
            {
                o.Tiles.SetActive(false);
                break;
            }
        }
    }


    /// <summary>
    /// Get the the position for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFreePos()
    {
        List<Vector3> freePos = ListFreePos();
        return freePos[Random.Range(0, freePos.Count)];
    }


    /// <summary>
    /// Create coin explore particle
    /// </summary>
    /// <param name="pos"></param>
    public void CreateCoinExplore(Vector3 pos)
    {
        foreach (GameObject o in listCoinExplore)
        {
            if (!o.activeInHierarchy)
            {
                o.SetActive(true);
                o.transform.position = pos;
                ParticleSystem par = o.GetComponent<ParticleSystem>();
                StartCoroutine(PlayCoinExplore(par));
                break;
            }
        }
    }

    /// <summary>
    /// Load the saved screenshot
    /// </summary>
    /// <returns></returns>
    public Texture LoadedScrenshot()
    {
        byte[] bytes = File.ReadAllBytes(ShareManager.Instance.ScreenshotPath);
        Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        tx.LoadImage(bytes);
        return tx;
    }

}
