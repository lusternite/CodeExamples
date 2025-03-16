using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is solely used for steam achievements

public class AchievementManagerScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    private List<Steamworks.Data.Achievement> achievements;

    [Header("Stat Trackers")]
    public int morningstarStuns = 0;
    public int greataxeHealing = 0;
    public int debuffsAvoided = 0;
    public float slowAddedWT = 0;

    public bool initialised = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitialiseAchievementManager()
    {
        if (Steamworks.SteamUserStats.RequestCurrentStats())
        {
            achievements = new List<Steamworks.Data.Achievement>();

            foreach (Steamworks.Data.Achievement ach in Steamworks.SteamUserStats.Achievements)
            {
                achievements.Add(ach);
            }

            // Check for demo
            if (GetComponent<GameManagerScript>().IsDemo() == false)
            {
                initialised = true;
                print("Achievements set up and ready to go!");
            }
            else
            {
                print("Achievements unavailable in demo");
            }
        }
        else
        {
            Debug.Log("Steamworks cannot request current stats, achievements cannot be set up");
        }
    }

    public void UnlockAchievement(string achievementIdentifier)
    {
        if (initialised)
        {
            for (int i = 0; i < achievements.Count; i++)
            {
                if (achievements[i].Identifier == achievementIdentifier)
                {
                    achievements[i].Trigger();
                    break;
                }
            }
        }
    }

    public void CountStunMorningstar()
    {
        if (initialised)
        {
            morningstarStuns += 1;
            if (morningstarStuns == 3)
            {
                UnlockAchievement("ACH_MORNINGSTAR");
            }
        }
    }

    public void ResetMorningstarStun()
    {
        if (initialised)
        {
            if (morningstarStuns > 0)
            {
                morningstarStuns = 0;
            }
        }
    }

    public void CountEnemyBleedDefeat()
    {
        if (initialised)
        {
            if (Steamworks.SteamUserStats.GetStatInt("stat_bleedcount") == 4)
            {
                UnlockAchievement("ACH_ESTOC");
            }
            else if (Steamworks.SteamUserStats.GetStatInt("stat_bleedcount") < 4)
            {
                Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_ESTOC", Steamworks.SteamUserStats.GetStatInt("stat_bleedcount") + 1, 5);
            }
            Steamworks.SteamUserStats.AddStat("stat_bleedcount", 1);
        }
    }

    public void CheckStats()
    {
        if (initialised)
        {
            print("Bleed count: " + Steamworks.SteamUserStats.GetStatInt("stat_bleedcount"));
            print("Extra flag staff damage: " + Steamworks.SteamUserStats.GetStatInt("stat_dmgbuffbonus"));
        }
    }

    public void CountFlagStaffDamageBonus(int extraDamage)
    {
        if (initialised)
        {
            // Check if overflow
            if (extraDamage + Steamworks.SteamUserStats.GetStatInt("stat_dmgbuffbonus") >= 200)
            {
                // Unlock achievement
                UnlockAchievement("ACH_FLAG_STAFF");

                // Set the stat
                Steamworks.SteamUserStats.SetStat("stat_dmgbuffbonus", 200);
            }
            else
            {
                print(Steamworks.SteamUserStats.AddStat("stat_dmgbuffbonus", extraDamage));
                print("Damage added: " + extraDamage);
                print("Current stat value: " + Steamworks.SteamUserStats.GetStatInt("stat_dmgbuffbonus"));
            }
        }
    }

    public void CountGreataxeHealing(int healing)
    {
        if (initialised)
        {
            greataxeHealing += healing;

            if (greataxeHealing >= 60)
            {
                UnlockAchievement("ACH_GREATAXE");
            }
        }
    }

    public void ResetGreataxeHealing()
    {
        if (initialised)
        {
            greataxeHealing = 0;
        }
    }

    public void CountLongSwordVictory()
    {
        if (initialised)
        {
            if (Steamworks.SteamUserStats.GetStatInt("stat_longswordvictory") == 3)
            {
                UnlockAchievement("ACH_LONG_SWORD");
            }
            else if (Steamworks.SteamUserStats.GetStatInt("stat_longswordvictory") < 3)
            {
                Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_LONG_SWORD", Steamworks.SteamUserStats.GetStatInt("stat_longswordvictory") + 1, 4);
            }
            Steamworks.SteamUserStats.AddStat("stat_longswordvictory", 1);
        }
    }

    public void CountPoleAxeDamage(int damage)
    {
        if (initialised)
        {
            // Check if overflow
            if (damage + Steamworks.SteamUserStats.GetStatInt("stat_poleaxedamage") >= 250)
            {
                // Unlock achievement
                UnlockAchievement("ACH_POLE_AXE");

                // Set the stat
                Steamworks.SteamUserStats.SetStat("stat_poleaxedamage", 200);
            }
            else
            {
                print(Steamworks.SteamUserStats.AddStat("stat_poleaxedamage", damage));
                print("Damage added: " + damage);
                print("Current stat value: " + Steamworks.SteamUserStats.GetStatInt("stat_poleaxedamage"));
            }
        }
    }

    public void ResetPoleAxeDamage()
    {
        if (initialised)
        {
            if (Steamworks.SteamUserStats.GetStatInt("stat_poleaxedamage") != 250)
            {
                Steamworks.SteamUserStats.SetStat("stat_poleaxedamage", 0);
            }
        }
    }

    public void CountDebuffAvoided()
    {
        if (initialised)
        {
            debuffsAvoided += 1;
            if (debuffsAvoided == 4)
            {
                UnlockAchievement("ACH_RECURVE_BOW");
            }
        }
    }

    public void ResetDebuffAvoided()
    {
        if (initialised)
        {
            debuffsAvoided = 0;
        }
    }

    public void CountWarhammerWTSlow(float wtAdded)
    {
        if (initialised)
        {
            slowAddedWT += wtAdded;
            if (slowAddedWT >= 100.0f)
            {
                UnlockAchievement("ACH_WARHAMMER");
            }
        }
    }

    public void ResetWarhammerWTSlow()
    {
        if (initialised)
        {
            slowAddedWT = 0.0f;
        }
    }

    public void ResetAllStatsAndAchievements()
    {
        if (initialised)
        {
            Steamworks.SteamUserStats.ResetAll(true);
            for (int i = 0; i < achievements.Count; i++)
            {
                achievements[i].Clear();
            }
        }
    }
}
