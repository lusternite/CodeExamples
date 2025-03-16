using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseScript : EntityBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    protected PlayerBehaviourScript playerReference;
    protected AudioManagerScript audioManagerReference;

    public bool deathFlag = false;

    public bool currentlyTargeted = false;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();
        playerReference = FindObjectOfType<PlayerBehaviourScript>();
        audioManagerReference = FindObjectOfType<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Used to determine what this enemy does during its turn
    public virtual void HandleTurn()
    {

    }

    // Used to determine if an action hits or not
    protected virtual bool TestAccuracy(float accuracy)
    {
        float realAccuracy = accuracy;

        // Check if the enemy is blind
        if (HasAugment(AugmentType.BLIND))
        {
            // Reduce accuracy by 120
            realAccuracy -= 120.0f;
        }

        // Check if enemy has accuracy modifiers
        if (HasModifier(StatType.ACC))
        {
            realAccuracy += GetModifier(StatType.ACC).modifierValue;
        }

        // Check if hits with accuracy modifiers
        return CombatManagerScript.TestAccuracy(realAccuracy);
    }

    protected void IncreaseWaitTime(float waitCost)
    {
        if (HasModifier(StatType.SPD))
        {
            if (GetModifier(StatType.SPD).modifierDuration != 100)
            {
                wait += waitCost * ((100.0f - GetModifier(StatType.SPD).modifierValue) / 100.0f);

                // Check slows for achievement
                if (GetModifier(StatType.SPD).modifierValue < 0.0f)
                {
                    FindObjectOfType<AchievementManagerScript>().CountWarhammerWTSlow(wait - waitCost);
                }
            }
            else
            {
                wait += waitCost;
            }
        }
        else
        {
            wait += waitCost;
        }
    }

    // Checks modifiers and then calculates damage
    protected virtual float CalculateDamage(float damageLower, float damageHigher)
    {
        float damage = Mathf.Round(Random.Range(damageLower, damageHigher));
        if (HasModifier(StatType.DMG))
        {
            damage *= 1 + GetModifier(StatType.DMG).modifierValue / 100.0f;
        }
        return damage;
    }

    // Decide whether to unleash a lash action depending on Revenge meter
    protected bool ShouldILash()
    {
        // Look at revenge meter and if it is full then return true
        return true;
    }

    public void HandleDeath()
    {
        deathFlag = true;
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
            transform.GetChild(transform.childCount - 1).SetParent(null);
        }
        // Also kill adjusters as they arent children
        DestroyAdjusterIcons();
    }

    public void DestroyAdjusterIcons()
    {
        // Delete any adjuster icons
        for (int i = 0; i < adjusterIcons.Count; i++)
        {
            Destroy(adjusterIcons[i].gameObject);
        }
    }

    public void SetTargetedAnimation(bool shouldPlay)
    {
        // Check if should start playing the targeted animation or stop it and go back to idle
        if (shouldPlay == true)
        {
            GetComponent<Animator>().Play("EntityTargeted");
            currentlyTargeted = true;
        }
        else
        {
            GetComponent<Animator>().Play("EntityIdle");
            currentlyTargeted = false;
        }
    }
}
