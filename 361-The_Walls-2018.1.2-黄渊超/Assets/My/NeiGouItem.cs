using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NeiGouItem : MonoBehaviour {
    public int id;

    public Text coin, rmb;

    public Button Buy;

    public void SetItem(string coin,string rmb,int id){
        this.coin.text = "X " + coin;
        this.rmb.text = "¥ " + rmb;
        this.id = id;


        Buy.onClick.AddListener(() =>
        {
            NeiGouPanel._ins.a(id);
        });
    }
}
