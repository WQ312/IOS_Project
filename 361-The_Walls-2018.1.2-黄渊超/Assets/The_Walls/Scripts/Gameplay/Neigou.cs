using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OnefallGames;

/// <summary>
/// 内购
/// </summary>

public class Neigou : MonoBehaviour {

	[SerializeField] 
	 private Text t1;		//内购第一项
	[SerializeField] 
	 private Text t2;		//内购第二项
    [SerializeField]
    private Text t3;
 	[SerializeField]
	 private Text t;			//星星总数

	//购买第一项
	public void item1()
	{
        GameSdkUnity.Instance.showVideoAds ("1", ()=>{
            WatchAdForCoinsItem1();
        });
	}

    public void WatchAdForCoinsItem1()
    {
        CoinManager.Instance.AddCoins(int.Parse(t1.text));  //将购买的数字加入总数中
    }

	//购买第二项
	public void item2()
	{
		GameSdkUnity.Instance.payGoods(2, (int i)=>{
			Debug.Log(t1.text);
			Debug.Log(t2.text);
			Debug.Log(t.text);
			CoinManager.Instance.AddCoins(int.Parse(t2.text));
		});
	}	

    //购买第三项
    public void item3()
    {
        GameSdkUnity.Instance.payGoods(3, (int i)=>{
            Debug.Log(t1.text);
            Debug.Log(t2.text);
            Debug.Log(t.text);
            CoinManager.Instance.AddCoins(int.Parse(t3.text));
        });
    }   
}


