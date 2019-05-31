using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnefallGames;

/// <summary>
/// 控制星星生产消除
/// </summary>

public class CoinController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        transform.eulerAngles += new Vector3(0, 200 * Time.deltaTime, 0);
	}

    private void OnTriggerEnter(Collider other)
    {
        //游戏中
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            //主角碰到金币，增加 1
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.CreateCoinExplore(transform.position);
                CoinManager.Instance.AddCoins(1);
                SoundManager.Instance.PlaySound(SoundManager.Instance.coin);
                gameObject.SetActive(false);
            }
        }
       
    }
}
