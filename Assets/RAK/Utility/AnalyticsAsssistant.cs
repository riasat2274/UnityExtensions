using RAK;
//using LionStudios.Suite.Analytics;
using System.Collections.Generic;

#if SW_STAGE_STAGE1_OR_ABOVE
using GameAnalyticsSDK;
using SupersonicWisdomSDK;
#endif
using UnityEngine;
public static class AnalyticsAssistant
{
    static bool isHardLevel => false;
    static int levelNumber;
    //static int levelNumber => LevelManager.instance ? LevelManager.instance.GetCurrentLevelIndex() + 1 : 0;
    static int? AttemptCountForCurrentLevel 
    {
        get
        {
            return null;
            //LevelStat ls = LevelStatData.Data.FindStat(levelNumber);
            //if(ls) return ls.totalAttemptCount;
            //else return null;
        }
    }
    public static void LevelStarted()
    {
        //LionAnalytics.MissionStarted(false, "Main", $"Main_{levelNumber}", levelNumber.ToString(),missionAttempt:AttemptCountForCurrentLevel);
        
        $"Level Started : {levelNumber}, attempt {AttemptCountForCurrentLevel}".Debug(Color.green);
    }
    public static void LevelCompleted(int coin)
    {
        //Product product = new Product();
        //List<VirtualCurrency> virtualCurrencies = new List<VirtualCurrency>{ new VirtualCurrency("coins", "soft", coin) };
        //product.virtualCurrencies = virtualCurrencies;
        //Reward reward = new Reward(product);
        //LionAnalytics.MissionCompleted(false, "Main", $"Main_{levelNumber}", levelNumber.ToString(),0, missionAttempt:AttemptCountForCurrentLevel, reward: reward);
        
        $"Level Completed : {levelNumber}, attempt {AttemptCountForCurrentLevel} with coin: {coin}".Debug(Color.green);        
    }
    public static void LevelFailed()
    {
        //LionAnalytics.MissionFailed(false, "Main", $"Main_{levelNumber}", levelNumber.ToString(),0,missionAttempt:AttemptCountForCurrentLevel);
        
        $"Level Failed : {levelNumber}, attempt {AttemptCountForCurrentLevel}".Debug(Color.green);
    }
    public static void LevelReset()
    {
        //LionAnalytics.MissionAbandoned(false, "Main", $"Main_{levelNumber}", levelNumber.ToString(),0,missionAttempt:AttemptCountForCurrentLevel);
        $"Level Reset : {levelNumber}, attempt {AttemptCountForCurrentLevel}".Debug(Color.green);
    }
    public static void LevelStep(int step)
    {
        //LionAnalytics.MissionStep(false, "Main", $"Main_{levelNumber}", levelNumber.ToString(),0,missionAttempt:AttemptCountForCurrentLevel, stepName: $"Step{step}");
        $"Step{step} : Level {levelNumber}, attempt {AttemptCountForCurrentLevel}".Debug(Color.green);
    }


    static void DesignEvent(string eventString)
    {
#if SW_STAGE_STAGE1_OR_ABOVE
        GameAnalytics.NewDesignEvent(eventString);
    #endif
        $"Design Event : {eventString}".Debug(Color.green);


    }
}
