using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithScript : EnemyBaseScript {

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Abyssal Calling settings")]
    public GameObject skeletonSoldierPrefab;
    public float abyssalCallingWaitCost = 60;

    [Header("Foul Grip settings")]
    public float foulGripDamageLower = 10.0f;
    public float foulGripDamageHigher = 18.0f;
    public float foulGripAccuracy = 80.0f;
    public float foulGripWaitCost = 56;

    [Header("Shadowbolt settings")]
    public float shadowboltDamageLower = 9.0f;
    public float shadowboltDamageHigher = 14.0f;
    public float shadowboltAccuracy = 85.0f;
    public float shadowboltWaitCost = 39;

    [Header("Evil Unleashed settings")]
    public float evilUnleashedWaitCost = 30;

    // This flag is used to determine if abyssal calling can be used
    public bool abyssalCallingFlag = true;

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
            StartCoroutine(EvilUnleashed());
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

    // By default, Use abyssal calling if available, then foul grip if not already banished, then shadowbolt
    private void ExecuteStandardActions()
    {
        // See if can abyssal calling
        if (abyssalCallingFlag == true)
        {
            StartCoroutine(AbyssalCalling());
        }
        else
        {
            // See if player has banish
            if (playerReference.HasAugment(AugmentType.BANISH) == true)
            {
                StartCoroutine(Shadowbolt());
            }
            else
            {
                StartCoroutine(FoulGrip());
            }
        }
    }

    // First action, spawns spooks
    private IEnumerator AbyssalCalling()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Wraith performs an abyssal calling", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("AbyssalCalling");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Spawn the skeletons
        combatManagerReference.SpawnEnemyDuringCombat(skeletonSoldierPrefab, true);
        combatManagerReference.SpawnEnemyDuringCombat(skeletonSoldierPrefab, true);

        // Change description
        combatManagerReference.DisplayCombatDescription("Skeleton soldiers have arisen...", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Set flag to false
        abyssalCallingFlag = false;

        // Increase wait cost
        IncreaseWaitTime(abyssalCallingWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, damages and banishes
    private IEnumerator FoulGrip()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Wraith reaches a foul grip", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("FoulGrip");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(foulGripAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(foulGripDamageLower, foulGripDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();

            // Change description
            combatManagerReference.DisplayCombatDescription(combatManagerReference.previousUsedEquipment.equipmentName + " is banished!", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Apply banish to player
            combatManagerReference.ApplyAugmentToPlayer(AugmentType.BANISH);
        }
        else
        {
            print("It misses...");

            // Change description
            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(foulGripWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // First action, standard damaging attack
    private IEnumerator Shadowbolt()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Wraith shoots a shadowbolt", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("ShadowBolt");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits (78% ACC)
        if (TestAccuracy(shadowboltAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(shadowboltDamageLower, shadowboltDamageHigher);

            // Tell combat manager to inflict damage
            combatManagerReference.InflictDamagePlayer(damage);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Remove combat description
            combatManagerReference.RemoveCombatDescription();
        }
        else
        {
            print("It misses...");

            // Change description
            combatManagerReference.DisplayCombatDescription("It misses...", 1.5f);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(shadowboltWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Lash action, kills skeletons and heals their remaining health
    private IEnumerator EvilUnleashed()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Wraith unleashes its evil", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("EvilUnleashed");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if skeletons are alive
        if (combatManagerReference.enemiesFront.Count > 0)
        {
            // Calculate health to heal
            float healing = 0;
            for (int i = 0; i < combatManagerReference.enemiesFront.Count; i++)
            {
                healing += combatManagerReference.enemiesFront[i].health;
            }

            // Damage the front row
            combatManagerReference.InflictDamageEnemy(TargetType.FrontLine, 999f, 999f, this);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Change description
            combatManagerReference.DisplayCombatDescription("It consumes the souls of the dead", 1.5f, false);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            combatManagerReference.RestoreHealthEnemy(3, healing);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            // Damage the front row
            combatManagerReference.InflictDamageEnemy(TargetType.FrontLine, 999f, 999f, this);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Change description
            combatManagerReference.DisplayCombatDescription("It consumes itself!", 1.5f, false);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Enable abyssal calling
        abyssalCallingFlag = true;

        // Increase wait cost
        IncreaseWaitTime(evilUnleashedWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
