using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public delegate void Buy(int id);


[Serializable]
public class NeiGouData
{
    public int id;
    public int coin;
    public float rmb;
}


public class NeiGouPanel : MonoBehaviour {
    //数据
    public NeiGouData[] Data;
    //定义委托
    public Buy a;
    //单例
    public static NeiGouPanel _ins;

    /// ////
    /// 
    /// 
    public GameObject item;
    //

    void Awake()
    {
        //单例
        _ins = this;

        //添加委托
        a += Buy;
    }
    void Start()
    {
        if (Data.Length == 0)
            return;

        //实例化
        GameObject gameitem1 = Instantiate(item);

        //获取脚本
        NeiGouItem gameJiaoBen1 = gameitem1.GetComponent<NeiGouItem>();

        //设置属性
        gameJiaoBen1.SetItem("2000", "免费", 999);

        //设置父级
        gameitem1.transform.parent = this.gameObject.transform;

        //设置大小
        gameitem1.transform.localScale = new Vector3(1 * 1.7f, 1 * 1.7f, 1);

        for (int i = 0; i < Data.Length; i++)
        {
            //实例化
            GameObject gameitem = Instantiate(item);

            //获取脚本
            NeiGouItem gameJiaoBen = gameitem.GetComponent<NeiGouItem>();

            //设置属性
            gameJiaoBen.SetItem(Data[i].coin.ToString(), Data[i].rmb.ToString(), Data[i].id);

            //设置父级
            gameitem.transform.parent= this.gameObject.transform;

            //设置大小
            gameitem.transform.localScale = new Vector3(1*1.7f, 1*1.7f, 1);
        }
    }

    public void Buy(int id){
        //Debug.Log(id+"写内购代码");
        if(id==999){
            GameSdkUnity.Instance.showVideoAds("free", () =>
            {
                //StoreManager.AddCoins(2000);
            });
            return;
        }
        GameSdkUnity.Instance.payGoods(id, (int obj) =>
        {
            //StoreManager.AddCoins(Data[obj-1].coin);
        });
        //StoreManager.AddCoins(3000);
    }
}



