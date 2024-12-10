using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameStats : MonoBehaviour
{
   public static float completionTime {get;set;} = 0;
   public static bool gameWon {get;set;} = false;
   public static int shrinesCleansed {get;set;} = 0;
   public static int totalShrines {get;set;} = 0;
   public static int enemiesKilled = 0;
   public static int bonusDamage {get;set;} = 0;
   public static int totalHealth {get;set;} = 0;
   public static int healingDone {get;set;} = 0;
   public static int armor {get;set;} = 0;
   public static int soulPowerCollected {get;set;} = 0;
   public static int soulPowerSpent {get;set;} = 0;

   public enum GameDifficulty
   {
        tutorial,
        normal,
        hard
   }

   public static GameDifficulty gameDifficulty {get;set;} = GameDifficulty.normal;

   public static void IncreaseEnemiesKilled()
   {
        enemiesKilled++;
   }
   public static void IncreaseHealingDone(int amount)
   {
        healingDone += amount;
   }

    public static void IncreaseShrinesCleansed()
    {
        shrinesCleansed++;
    }

    public static void ResetDefaults()
    {
        completionTime = 0;
        gameWon = false;
        shrinesCleansed = 0;
        totalShrines =0;
        enemiesKilled = 0;
        bonusDamage = 0;
        totalHealth = 0;
        healingDone = 0;
        armor = 0;
        soulPowerCollected = 0;
        soulPowerSpent = 0;

    }

    public static void SetGameDifficulty(int diff)
    {
        gameDifficulty = (GameDifficulty)diff;
    }
}
