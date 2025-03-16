using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMageScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Unholy Strength settings")]
    public float unholyStrengthDmgIncreaseValue = 30.0f;
    public float unholyStrengthWaitCost = 55;

    [Header("Unholy Aid settings")]
    public float unholyAidHealPercentage = 35;
    public float unholyAidHealThreshold = 35;
    public float unholyAidWaitCost = 70;

    [Header("Unending Night settings")]
    public float unendingNightWaitCost = 56.0f;
    public GameObject skeletonSoldierPrefab;
    public GameObject skeletonArcherPrefab;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HandleTurn()
    {
        base.HandleTurn();

        // Decide whether to act or lash

        if (combatManagerReference.revengeMeter == 100.0f)
        {
            StartCoroutine(UnendingNight());
        }
        else
        {
            // Different action use percentages depending on if damaged before this turn
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {

        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // By default, check if any allies are below 35% hp to heal them, otherwise start buffing the backline then frontline
    private void ExecuteStandardActions()
    {
        bool usingUnholyAid = false;

        // Check if should use unholy aid

        // First go through rear line
        List<EnemyBaseScript> currentLine = combatManagerReference.enemiesRear;
        for (int i = 0; i < currentLine.Count; i++)
        {
            if (currentLine[i].health < maxHealth * (unholyAidHealThreshold / 100.0f))
            {
                StartCoroutine(UnholyAid(i + 3));
                usingUnholyAid = true;
                break;
            }
        }

        // Then go through front line if no suitable target found
        if (usingUnholyAid == false)
        {
            currentLine = combatManagerReference.enemiesFront;
            for (int i = 0; i < currentLine.Count; i++)
            {
                if (currentLine[i].health < maxHealth * (unholyAidHealThreshold / 100.0f))
                {
                    StartCoroutine(UnholyAid(i + 1));
                    usingUnholyAid = true;
                    break;
                }
            }

            // Continue to unholy strength/speed logic if no aid target found
            if (usingUnholyAid == false)
            {
                // Check if there is another unit in the rear line
                if (combatManagerReference.enemiesRear.Count >= 2)
                {
                    // Check if that unit has a damage buff and if it is negative
                    // This relies on the skeleton mage being in position 2
                    if (combatManagerReference.enemiesRear[0].HasModifier(StatType.DMG))
                    {
                        if (combatManagerReference.enemiesRear[0].GetModifier(StatType.DMG).modifierValue < 0.0f)
                        {
                            // Rear unit has negative damage buff so use unholy strength
                            StartCoroutine(UnholyStrength(3));
                        }
                        else
                        {
                            // Rear unit has positive damage buff so check front row
                            UnholyCheckFront();
                        }
                    }
                    // Rear unit has no damage buff so use unholy strength
                    else
                    {
                        StartCoroutine(UnholyStrength(3));
                    }
                }
                else
                {
                    // No other rear target so check front row
                    UnholyCheckFront();
                }
            }
        }
    }

    // Check the front row for targets of unholy strength
    private void UnholyCheckFront()
    {
        bool frontBuffed = false;
        for (int i = 0; i < combatManagerReference.enemiesFront.Count; i++)
        {
            // Check if that unit has a damage buff and if it is negative
            if (combatManagerReference.enemiesFront[i].HasModifier(StatType.DMG))
            {
                if (combatManagerReference.enemiesFront[i].GetModifier(StatType.DMG).modifierValue < 0.0f)
                {
                    // Unit has negative damage buff so use unholy strength
                    StartCoroutine(UnholyStrength(i + 1));
                    frontBuffed = true;
                    break;
                }
            }
            // Unit has no damage buff so use unholy strength
            else
            {
                StartCoroutine(UnholyStrength(i + 1));
                frontBuffed = true;
                break;
            }
        }

        if (frontBuffed == false)
        {
            // All targets unapplicable, just buff rear row
            StartCoroutine(UnholyStrength(3));
        }
    }

    // First action, grants damage buff to target ally
    private IEnumerator UnholyStrength(int enemyIndex)
    {
        EnemyBaseScript targetEnemy = combatManagerReference.GetEnemyFromIndex(enemyIndex);

        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Mage empowers " + targetEnemy.entityName, 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("UnholyStrength");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        targetEnemy.AddModifier(StatType.DMG, unholyStrengthDmgIncreaseValue);

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(unholyStrengthWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, heals an ally
    private IEnumerator UnholyAid(int enemyIndex)
    {
        EnemyBaseScript targetEnemy = combatManagerReference.GetEnemyFromIndex(enemyIndex);

        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton Mage restores vitality to " + targetEnemy.entityName, 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("UnholyAid");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Heal target enemy
        combatManagerReference.RestoreHealthEnemy(enemyIndex, Mathf.Round(targetEnemy.maxHealth * (unholyAidHealPercentage / 100.0f)));
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(unholyAidWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, revives an ally
    private IEnumerator UnendingNight()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("An unending night awaits as Skeleton Mage calls back the dead", 3.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("UnendingNight");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Tell combat manager to spawn back a dead companion
        // Try to spawn soldier first, otherwise spawn archer

        if (combatManagerReference.enemiesFront.Count <= 1)
        {
            combatManagerReference.SpawnEnemyDuringCombat(skeletonSoldierPrefab, true);
        }
        else if (combatManagerReference.enemiesRear.Count <= 1)
        {
            combatManagerReference.SpawnEnemyDuringCombat(skeletonArcherPrefab, false);
        }
        else
        {
            combatManagerReference.DisplayCombatDescription("But nothing happened...", 1.5f, false);
            yield return new WaitForSeconds(0.1f);
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(unendingNightWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
