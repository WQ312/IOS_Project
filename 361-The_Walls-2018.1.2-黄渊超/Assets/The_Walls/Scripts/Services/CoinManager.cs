using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 处理星星
/// </summary>

namespace OnefallGames
{
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance;
		//星星个数
        public int Coins
        { 
            get { return currentCoins; }
            private set { currentCoins = value; }
        }

        public static event Action<int> CoinsUpdated = delegate {};

        [SerializeField]
        int initialCoins = 0;

        // Show the current coins value in editor for easy testing
        [SerializeField]
        int currentCoins;

        // key name to store high score in PlayerPrefs
        const string PPK_COINS = "ONEFALL_COINS";


        void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            // Initialize coins
            Coins = PlayerPrefs.GetInt(PPK_COINS, initialCoins);
        }
		//增加星星数目
        public void AddCoins(int amount)
        {
            Coins += amount;


            // Store new coin value
            PlayerPrefs.SetInt(PPK_COINS, Coins);

            // Fire event
            CoinsUpdated(Coins);
        }
		//减少星星数目
        public void RemoveCoins(int amount)
        {
            Coins -= amount;

            // Store new coin value
            PlayerPrefs.SetInt(PPK_COINS, Coins);

            // Fire event
            CoinsUpdated(Coins);
        }
    }
}
