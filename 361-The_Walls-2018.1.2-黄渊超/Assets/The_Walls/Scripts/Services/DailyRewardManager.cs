using UnityEngine;
using System.Collections;
using System;

namespace OnefallGames
{

    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        private DateTime NextRewardTime
        {
            get
            {
                return GetNextRewardTime();
            }
        }

        //6个小时
        public TimeSpan TimeUntilNextReward
        { 
            get
            {
                //下个时间 - 当前时间 = 6个小时
                return NextRewardTime.Subtract(DateTime.Now);
            }
        }

        [Header("Daily Reward Config")]
        [SerializeField] private int rewardHours = 6;
        [SerializeField] private int rewardMinutes = 0;
        [SerializeField] private int rewardSeconds = 0;
        [SerializeField] private int minRewardValue = 50;
        [SerializeField] private int maxRewardValue = 100;

        private const string NextRewardTimePPK = "ONEFALLGAMES_DAILY_REWARD_TIME";

        void Awake()
        {
            //DateTime next = DateTime.Now.Add(new TimeSpan(rewardHours, rewardMinutes, rewardSeconds));
            //Debug.Log("next===" + next);

            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Is the waiting time has passed and can reward now.
        /// </summary>
        /// <returns><c>true</c> if this instance can reward now; otherwise, <c>false</c>.</returns>
        /// 时间结束，可以获取奖励
        public bool CanRewardNow()
        {
            return TimeUntilNextReward <= TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the random reward.
        /// </summary>
        /// <returns>The random reward.</returns>
        /// 获取奖励
        public int GetRandomReward()
        {
            int amount = 100;
            return amount;
        }

        /// <summary>
        /// Reset the reward time
        /// </summary>
        public void ResetNextRewardTime()
        {
            //当前时间 + 6小时
            DateTime next = DateTime.Now.Add(new TimeSpan(rewardHours, rewardMinutes, rewardSeconds));
            SaveNextRewardTime(next);
        }

        //保存下个时间
        void SaveNextRewardTime(DateTime time)
        {
            //序列化
            PlayerPrefs.SetString(NextRewardTimePPK, time.ToBinary().ToString());
            PlayerPrefs.Save();
        }


        /// <summary>
        /// Get next reward time with Datetime format
        /// </summary>
        /// <returns></returns>
        /// 获取完整下个时间
        DateTime GetNextRewardTime()
        {
            string storedTime = PlayerPrefs.GetString(NextRewardTimePPK, string.Empty);

            if (!string.IsNullOrEmpty(storedTime))
            {
                Debug.Log("qqqqqq====1");
                //反序列化为  DateTime
                return DateTime.FromBinary(Convert.ToInt64(storedTime));
            }
            else
            {
                Debug.Log("qqqqqqqq====2");
                return DateTime.Now;
            }
                
        }
    }
}
