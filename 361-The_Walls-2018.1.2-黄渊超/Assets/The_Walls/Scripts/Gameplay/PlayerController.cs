using OnefallGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 主角操作
/// </summary>

//游戏状态
public enum PlayerState
{
    Prepare,	//准备
    Living,		//存活
    Die,		//死亡
}

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { private set; get; }
    public static event System.Action<PlayerState> PlayerStateChanged = delegate { };

    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }

        private set
        {
            if (value != playerState)
            {
                value = playerState;
                PlayerStateChanged(playerState);
            }
        }
    }


    private PlayerState playerState = PlayerState.Die;


    [Header("Player Config")]
    [SerializeField]
	private float rotattingTime = 0.5f;

    [Header("Player References")]
    [SerializeField] private GameObject playerExplore;			//玩家死亡特效
    [SerializeField] private MeshRenderer meshRender = null;
    [SerializeField] private BoxCollider boxCollider = null;
	[SerializeField] private UIManager uiManager;


    [HideInInspector] public bool stopRotate = false;


    private bool isRotating = false;



    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(GameState obj)
    {
        if (obj == GameState.Playing)
        {
			Debug.Log ("b");
			//uiManager.isClick = true;
            PlayerLiving();
        }
        else if (obj == GameState.Prepare)
        {
            PlayerPrepare();
        }
    }    



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

	private void Start()
	{
        uiManager.isClick = true;
	}


	// Update is called once per frame
	void Update () {
		//触发在UI按钮上
		if (EventSystem.current.currentSelectedGameObject!=null&&EventSystem.current.currentSelectedGameObject.name == "PauseBtn")
		{
			//Debug.Log ("EventSystem.current.currentSelectedGameObject===="+EventSystem.current.currentSelectedGameObject.name);
			return;
		}
		else    //触发在屏幕上
		{
            Debug.Log("控制1"+GameManager.Instance.GameState);
            Debug.Log("isClick=" + uiManager.isClick);
			//Debug.Log ("restart");	游戏状态为Playing,且游戏继续
			if (GameManager.Instance.GameState == GameState.Playing && uiManager.isClick==true) 
			{
                Debug.Log("控制2"+GameManager.Instance.GameState);
				//Debug.Log ("当前没有触摸在UI上");	
				//鼠标左键点击且物体不在旋转
				if (Input.GetMouseButton (0) && !isRotating && !stopRotate) {
                    //向屏幕发射射线
					Vector2 viewportPos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
                    //点击左边
					if (viewportPos.x < 0.5f) 
                    {
						float x = (float)System.Math.Round (transform.position.x, 1);

						if (x > -2.5f)
							DoRotateLeft ();
					} 
                    //点击右边
                    else 
                    {
						float x = (float)System.Math.Round (transform.position.x, 1);
						if (x < 2.5f)
							DoRotateRight ();
					}
				}
			}
		}

//		if (EventSystem.current.currentSelectedGameObject != null) {
//			Debug.Log ("UI");
//		} else 
//		{
//			Debug.Log ("not");
//			if (GameManager.Instance.GameState == GameState.Playing)
//			{
//
//				if (Input.GetMouseButton(0) && !isRotating && !stopRotate && IsClick)
//				{
//					Vector2 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
//					if (viewportPos.x < 0.5f)
//					{
//						float x = (float)System.Math.Round(transform.position.x, 1);
//						if (x > -2.5f)
//							DoRotateLeft();
//					}
//					else
//					{
//						float x = (float)System.Math.Round(transform.position.x, 1);
//						if (x < 2.5f)
//							DoRotateRight();
//					}
//				}
//			}
//		}

		/*if (EventSystem.current.IsPointerOverGameObject ()) {
			Debug.Log ("当前触摸在UI上");
		} 
		else 
		{
			if (GameManager.Instance.GameState == GameState.Playing) 
			{
				Debug.Log ("当前没有触摸在UI上");
				if (Input.GetMouseButton (0) && !isRotating && !stopRotate) {
					Vector2 viewportPos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
					if (viewportPos.x < 0.5f) {
						float x = (float)System.Math.Round (transform.position.x, 1);
						if (x > -2.5f)
							DoRotateLeft ();
					} else {
						float x = (float)System.Math.Round (transform.position.x, 1);
						if (x < 2.5f)
							DoRotateRight ();
					}
				}
			}
		}*/
				

//		Vector3 mouse = Input.mousePosition;
//		float x1 = mouse.x - Screen.width / 2f;
//		float y1 = mouse.y - Screen.height / 2f;
//		if (x1 > 0.5 && Input.mousePosition.y > Screen.height / 5) 
//		{
//			
//		}
        
    }

    void PlayerPrepare()
    {
        //Fire event
        PlayerState = PlayerState.Prepare;
        playerState = PlayerState.Prepare;

        //Add another actions here
        meshRender.enabled = false;
    }

    void PlayerLiving()
    {
        //Fire event
        PlayerState = PlayerState.Living;
        playerState = PlayerState.Living;

        //Add another actions here
        meshRender.enabled = true;
        StartCoroutine(WaitAndResetPosition(0));
        stopRotate = false;
    }

    void PlayerDie()
    {
        //Fire event
        PlayerState = PlayerState.Die;
        playerState = PlayerState.Die;

        //Add another actions here

    }
		

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (other.transform.parent != null)
            {
                GameObject parent = other.transform.parent.gameObject;
                if (parent.CompareTag("Finish"))
                {
                    ShareManager.Instance.CreateScreenshot();
                    PlayerDie();
                    SetGamestate();
                    GameManager.Instance.AdjustTimescale();
                    StartCoroutine(Flashing(other.gameObject));
                }
            }
        }
    }

    private void SetGamestate()
    {
        //if (!GameManager.Instance.IsRevived)
        //{
        //    if (AdManager.Instance.IsRewardedVideoAdReady())
        //    {
        //        GameManager.Instance.Revive();
        //    }
        //    else
        //    {
        //        GameManager.Instance.GameOver();
        //    }
        //}
       // else
        //{
            GameManager.Instance.GameOver();
        //}
    }



    //Do rotate player left 
    private void DoRotateLeft()
    {
        Vector3 pivotPoint = Vector3.zero;

        RaycastHit down;
        RaycastHit left;
        Ray rayDown = new Ray(transform.position, Vector3.down);
        Ray rayLeft = new Ray(transform.position, Vector3.left);
        bool hitDown = Physics.Raycast(rayDown, out down, 1f);
        bool hitLeft = Physics.Raycast(rayLeft, out left, 1f);

        if (hitLeft)
        {
            Transform leftOb = left.collider.transform;
            RaycastHit hitUp;
            Ray rayUp = new Ray(leftOb.position, Vector3.up);
            if (Physics.Raycast(rayUp, out hitUp, 1f))
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.up * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.up * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 180, rotattingTime));
            }
        }
        else if (hitDown)
        {

            Transform downOb = down.collider.transform;
            Ray rayLeftTemp = new Ray(down.transform.position, Vector3.left);
            if (Physics.Raycast(rayLeftTemp, out left, 1f))
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 180, rotattingTime));
            }
        }
        else
        {
            float y = Mathf.RoundToInt(transform.position.y);
            if (y > 0)
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.forward, 90, rotattingTime));
            }
        }
    }


    //Do rotate player right 
    private void DoRotateRight()
    {

        Vector3 pivotPoint = Vector3.zero;

        RaycastHit down;
        RaycastHit right;
        Ray rayDown = new Ray(transform.position, Vector3.down);
        Ray rayRight = new Ray(transform.position, Vector3.right);
        bool hitDown = Physics.Raycast(rayDown, out down, 1f);
        bool hitRight = Physics.Raycast(rayRight, out right, 1f);

        if (hitRight)
        {
            RaycastHit hitUp;
            Ray rayUp = new Ray(right.transform.position, Vector3.up);
            if (Physics.Raycast(rayUp, out hitUp, 1f))
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 180, rotattingTime));
            }
        }
        else if (hitDown)
        {
            Ray rayRightTemp = new Ray(down.transform.position, Vector3.right);
            if (Physics.Raycast(rayRightTemp, out down, 1f))
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 180, rotattingTime));
            }
        }
        else
        {
            float y = Mathf.RoundToInt(transform.position.y);
            if (y > 0)
            {
                pivotPoint = transform.position + Vector3.left * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 90, rotattingTime));
            }
            else
            {
                pivotPoint = transform.position + Vector3.right * 0.5f + Vector3.down * 0.5f;
                StartCoroutine(Rotating(pivotPoint, Vector3.back, 90, rotattingTime));
            }
        }
    }

    //Do rotate player
    private IEnumerator Rotating(Vector3 pivotPoint, Vector3 axis, float angle, float rotateTime)
    {
        isRotating = true;

        float tStep = Mathf.Ceil(rotateTime * 30f);
        float tAngle = angle / tStep;

        for (int i = 0; i < tStep; i++)
        {
            if (stopRotate)
            {
                isRotating = false;
                yield break;
            }
            transform.RotateAround(pivotPoint, axis, tAngle);
            yield return null;
        }


        //Snaping the position 
        double x = System.Math.Round(transform.position.x, 1);
        double y = System.Math.Round(transform.position.y, 1);
        transform.position = new Vector3((float)x, (float)y, 0);

        // Snaping the rotation to 90 degrees.    
        Vector3 angles = transform.eulerAngles;
        double angleZ = System.Math.Round(transform.eulerAngles.z, 0);
        transform.eulerAngles = new Vector3(0, 0, (float)angleZ);

        isRotating = false;
    }

    //Reset player position , rotation, collider and mesh with delay time
    private IEnumerator WaitAndResetPosition(float time)
    {
        meshRender.enabled = false;
        yield return new WaitForSeconds(time);
        transform.eulerAngles = Vector3.zero;
        transform.position = GameManager.Instance.GetFreePos();
        boxCollider.enabled = true;
        meshRender.enabled = true;
    }


    //Flashing the tile that player hit 
    private IEnumerator Flashing(GameObject ob)
    {
        Renderer ren = ob.GetComponent<Renderer>();
        Texture currentMainTex = ren.material.mainTexture;
        while (true)
        {
            if (!ob.activeInHierarchy)
                yield break;

            ren.material.mainTexture = GameManager.Instance.collisionTexture;
            yield return new WaitForSecondsRealtime(0.05f);
            ren.material.mainTexture = currentMainTex;
            yield return new WaitForSecondsRealtime(0.05f);

            if (Time.timeScale == 1)
            {
                PlayExplode();
                GameManager.Instance.HideAllActiveCoin();
                GameManager.Instance.HideCurrentTiles();
                yield break;
            }
        }     
    }


    //Play player explode particle
    private void PlayExplode()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
        meshRender.enabled = false;
        GameObject par = Instantiate(playerExplore, transform.position, Quaternion.identity);
        ParticleSystem parSys = par.GetComponent<ParticleSystem>();
        parSys.Play();
        Destroy(par, parSys.main.startLifetimeMultiplier);
    }



    /////////////////////////////////////////////public functions

    
    /// <summary>
    /// Create new wall and tiles, reset all coins, reset player position
    /// </summary>
    public void ResetWallHoles()
    {
        stopRotate = true;
        boxCollider.enabled = false;
        GameManager.Instance.HideCurrentTiles();
        GameManager.Instance.HideAllActiveCoin();
        GameManager.Instance.CreateWallHoleAndCoins();
        StartCoroutine(WaitAndResetPosition(Time.deltaTime));
    }

}
