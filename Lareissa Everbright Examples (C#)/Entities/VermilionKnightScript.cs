using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VermilionKnightScript : EnemyBaseScript
{

    //**~~~~~~~~VARIABLES~~~~~~~~**//

    // ACTION STATS
    [Header("Replicate Arsenal settings")]
    public float replicateArsenalWaitCost = 25f;
    public bool replicateArsenalUsed = false;

    [Header("Heavy Crossbow settings")]
    public float heavyCrossbowDamageLower = 28.0f;
    public float heavyCrossbowDamageHigher = 32.0f;
    public float heavyCrossbowHealingMultiplier = 0.8f;
    public float heavyCrossbowAccuracy = 90.0f;
    public float heavyCrossbowWaitCost = 37;
    public bool heavyCrossbowEnabled = false;
    public int heavyCrossbowCD = 0;

    [Header("Halberd settings")]
    public float halberdDamageLower = 20.0f;
    public float halberdDamageHigher = 28.0f;
    public float halberdDefReductionAmount = -25.0f;
    public float halberdAccuracy = 85.0f;
    public float halberdWaitCost = 44;
    public bool halberdEnabled = false;
    public int halberdCD = 0;

    [Header("Javelin settings")]
    public float javelinDamageLower = 6.0f;
    public float javelinDamageHigher = 13.0f;
    public float javelinAccReductionAmount = -20.0f;
    public float javelinAccuracy = 200.0f;
    public float javelinWaitCost = 21;
    public bool javelinEnabled = false;
    public int javelinCD = 0;

    [Header("Maul settings")]
    public float maulDamageLower = 23.0f;
    public float maulDamageHigher = 30.0f;
    public float maulSpdReductionAmount = -20.0f;
    public float maulAccuracy = 82.0f;
    public float maulWaitCost = 46;
    public bool maulEnabled = false;
    public int maulCD = 0;

    [Header("Rapier settings")]
    public float rapierDamageLower = 10.0f;
    public float rapierDamageHigher = 16.0f;
    public float rapierSpdBuffAmount = 20.0f;
    public float rapierAccBuffAmount = 20.0f;
    public float rapierAccuracy = 120.0f;
    public float rapierWaitCost = 22;
    public bool rapierEnabled = false;
    public int rapierCD = 0;

    [Header("Greatsword settings")]
    public float greatswordDamageLower = 20.0f;
    public float greatswordDamageHigher = 36.0f;
    public float greatswordHealingModifier = 7.0f;
    public float greatswordAccuracy = 85.0f;
    public float greatswordWaitCost = 36;
    public bool greatswordEnabled = false;
    public int greatswordCD = 0;

    [Header("Greatshield settings")]
    public float greatshieldWaitCost = 33f;
    public bool greatshieldEnabled = false;
    public int greatshieldCD = 0;

    [Header("Unholy Symbol settings")]
    public float unholySymbolDamageMultiplier = 5.0f;
    public float unholySymbolHealingMultiplier = 5.0f;
    public float unholySymbolWaitCost = 45;
    public bool unholySymbolEnabled = false;
    public int unholySymbolCD = 0;

    [Header("Hellish Banner settings")]
    public float hellishBannerSpdReductionAmount = -20.0f;
    public float hellishBannerDmgReductionAmount = -20.0f;
    public float hellishBannerDmgBuffAmount = 20.0f;
    public float hellishBannerWaitCost = 34;
    public bool hellishBannerEnabled = false;
    public int hellishBannerCD = 0;

    [Header("Falchion settings")]
    public float falchionDamageLower = 24.0f;
    public float falchionDamageHigher = 26.0f;
    public float falchionDefBuffAmount = 35.0f;
    public float falchionHealingMultiplier = 0.5f;
    public float falchionAccuracy = 300.0f;
    public float falchionWaitCost = 39;

    List<EquipmentInitialisationData> playerInventory;
    List<int> enabledActionCooldowns;
    int actionLowLimit = -5;
    int actionCooldownIncrease = 3;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        playerInventory = FindObjectOfType<PlayerManagerScript>().combatInventory;
        enabledActionCooldowns = new List<int>();
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
            StartCoroutine(Falchion());
        }
        else
        {
            ExecuteStandardActions();
        }
    }

    private void HandleEndTurn()
    {
        // Inform the combat manager that turn has ended
        combatManagerReference.NotifyTurnComplete();
    }

    // First replicate arms, then look at use conditions from a priority list and check if enabled.
    private void ExecuteStandardActions()
    {
        if (replicateArsenalUsed == false)
        {
            StartCoroutine(ReplicateArsenal());
            replicateArsenalUsed = true;
        }
        else
        {
            bool actionDecided = false;

            // Check that none of the actions are at or below -5 cd

            // Check greatshield
            if (greatshieldEnabled == true && greatshieldCD <= actionLowLimit)
            {
                StartCoroutine(Greatshield());
                greatshieldCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check hellish banner
            else if (hellishBannerEnabled == true && hellishBannerCD <= actionLowLimit)
            {
                StartCoroutine(HellishBanner());
                hellishBannerCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check unholy symbol
            else if (unholySymbolEnabled == true && unholySymbolCD <= actionLowLimit)
            {
                StartCoroutine(UnholySymbol());
                unholySymbolCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check heavy crossbow
            else if (heavyCrossbowEnabled == true && heavyCrossbowCD <= actionLowLimit)
            {
                StartCoroutine(HeavyCrossbow());
                heavyCrossbowCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check greatsword
            else if (greatswordEnabled == true && greatswordCD <= actionLowLimit)
            {
                StartCoroutine(Greatsword());
                greatswordCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check rapier
            else if (rapierEnabled == true && rapierCD <= actionLowLimit)
            {
                StartCoroutine(Rapier());
                rapierCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check halberd
            else if (halberdEnabled == true && halberdCD <= actionLowLimit)
            {
                StartCoroutine(Halberd());
                halberdCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check javelin
            else if (javelinEnabled == true && javelinCD <= actionLowLimit)
            {
                StartCoroutine(Javelin());
                javelinCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Check maul
            else if (maulEnabled == true && maulCD <= actionLowLimit)
            {
                StartCoroutine(Maul());
                maulCD = actionCooldownIncrease;
                actionDecided = true;
            }

            // Loop through actions and their conditions to determine right action to use
            while (actionDecided == false)
            {
                // First look at actions that have specific activation conditions

                // Check greatshield counter
                if (playerReference.HasAugment(AugmentType.BLOCK) && greatshieldEnabled == true && greatshieldCD <= 0)
                {
                    StartCoroutine(Greatshield());
                    greatshieldCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Check hellish banner counter
                if (hellishBannerEnabled == true && hellishBannerCD <= 0)
                {
                    if (playerReference.HasModifier(StatType.DMG))
                    {
                        if (playerReference.GetModifier(StatType.DMG).modifierValue > 0)
                        {
                            StartCoroutine(HellishBanner());
                            hellishBannerCD = actionCooldownIncrease;
                            actionDecided = true;
                            break;
                        }
                    }
                    else if (playerReference.HasModifier(StatType.SPD))
                    {
                        if (playerReference.GetModifier(StatType.SPD).modifierValue > 0)
                        {
                            StartCoroutine(HellishBanner());
                            hellishBannerCD = actionCooldownIncrease;
                            actionDecided = true;
                            break;
                        }
                    }
                }

                // Check unholy symbol
                if (unholySymbolEnabled == true && unholySymbolCD <= 0)
                {
                    // Check gwen adjusters
                    if ((playerReference.GetModifierCount() + playerReference.GetAugmentCount()) >= 2)
                    {
                        StartCoroutine(UnholySymbol());
                        unholySymbolCD = actionCooldownIncrease;
                        actionDecided = true;
                        break;
                    }
                    // Check self adjusters
                    else if ((GetModifierCount() + GetAugmentCount()) >= 2)
                    {
                        StartCoroutine(UnholySymbol());
                        unholySymbolCD = actionCooldownIncrease;
                        actionDecided = true;
                        break;
                    }
                }

                // Now check for healing actions

                // Check for heavy crossbow
                if (heavyCrossbowEnabled == true && heavyCrossbowCD <= 0)
                {
                    StartCoroutine(HeavyCrossbow());
                    heavyCrossbowCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Check for greatsword
                if (greatswordEnabled == true && greatswordCD <= 0)
                {
                    StartCoroutine(Greatsword());
                    greatswordCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // And now the rest

                // Check rapier
                if (rapierEnabled == true && rapierCD <= 0)
                {
                    StartCoroutine(Rapier());
                    rapierCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Check for halberd
                if (halberdEnabled == true && halberdCD <= 0)
                {
                    StartCoroutine(Halberd());
                    halberdCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Check for javelin
                if (javelinEnabled == true && javelinCD <= 0)
                {
                    StartCoroutine(Javelin());
                    javelinCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Check for maul
                if (maulEnabled == true && maulCD <= 0)
                {
                    StartCoroutine(Maul());
                    maulCD = actionCooldownIncrease;
                    actionDecided = true;
                    break;
                }

                // Nothing has been found so far, so decrease cd and continue searching
                ReduceActionCooldowns();
            }

            // Make sure to reduce cooldowns after action decided
            ReduceActionCooldowns();
        }
    }

    private void ReduceActionCooldowns()
    {
        // Have to do this the omega manual way..
        heavyCrossbowCD -= 1;
        halberdCD -= 1;
        javelinCD -= 1;
        maulCD -= 1;
        rapierCD -= 1;
        greatswordCD -= 1;
        greatshieldCD -= 1;
        unholySymbolCD -= 1;
        hellishBannerCD -= 1;
    }

    // Initial action, determines available actions
    private IEnumerator ReplicateArsenal()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight replicates Gwen's equipment", 3.0f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("ReplicateArsenal");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check for heavy crossbow
        if (PlayerInventoryContains("Recurve Bow") || PlayerInventoryContains("Crossbow"))
        {
            heavyCrossbowEnabled = true;
            enabledActionCooldowns.Add(heavyCrossbowCD);
        }

        // Check for halberd
        if (PlayerInventoryContains("Pole Axe"))
        {
            halberdEnabled = true;
            enabledActionCooldowns.Add(halberdCD);
        }

        // Check for javelin
        if (PlayerInventoryContains("Spear"))
        {
            javelinEnabled = true;
            enabledActionCooldowns.Add(javelinCD);
        }

        // Check for maul
        if (PlayerInventoryContains("Morningstar") || PlayerInventoryContains("Warhammer"))
        {
            maulEnabled = true;
            enabledActionCooldowns.Add(maulCD);
        }

        // Check for rapier
        if (PlayerInventoryContains("Estoc"))
        {
            rapierEnabled = true;
            enabledActionCooldowns.Add(rapierCD);
        }
         
        // Check for greatsword
        if (PlayerInventoryContains("Claymore") || PlayerInventoryContains("Greataxe") || PlayerInventoryContains("Long Sword"))
        {
            greatswordEnabled = true;
            enabledActionCooldowns.Add(greatswordCD);
        }

        // Check for greatshield
        if (PlayerInventoryContains("Shield"))
        {
            greatshieldEnabled = true;
            enabledActionCooldowns.Add(greatshieldCD);
        }

        // Check for unholy symbol
        if (PlayerInventoryContains("Medicinal Remedy") || PlayerInventoryContains("Rosary") || PlayerInventoryContains("Incense"))
        {
            unholySymbolEnabled = true;
            enabledActionCooldowns.Add(unholySymbolCD);
        }

        // Check for hellish banner
        if (PlayerInventoryContains("Flag Staff"))
        {
            hellishBannerEnabled = true;
            enabledActionCooldowns.Add(hellishBannerCD);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(replicateArsenalWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Searches the player combat inventory for specific type of equipment
    bool PlayerInventoryContains(string equipmentName)
    {
        for (int i = 0; i < playerInventory.Count; i++)
        {
            if (playerInventory[i].equipmentName == equipmentName)
            {
                return true;
            }
        }
        return false;
    }

    // First action, lifesteal attack
    private IEnumerator HeavyCrossbow()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight launches a vampiric stake from her heavy crossbow", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("CrossbowShoot");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(heavyCrossbowAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(heavyCrossbowDamageLower, heavyCrossbowDamageHigher);

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

            // Heal for half damage dealt

            // Figure out def mods
            if (playerReference.HasModifier(StatType.DEF))
            {
                damage *= (100.0f - playerReference.GetModifier(StatType.DEF).modifierValue) / 100.0f;
            }

            // Make sure to round the damage
            damage = Mathf.Round(damage);

            // Change description
            combatManagerReference.DisplayCombatDescription("The vampiric stake glows red", 1.5f, false);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Figure out healing
            float healing = Mathf.Floor(damage * heavyCrossbowHealingMultiplier);

            combatManagerReference.RestoreHealthEnemy(1, healing);

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
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
        IncreaseWaitTime(heavyCrossbowWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Second action, def reduction attack
    private IEnumerator Halberd()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight smashes with her halberd", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("PowerSwing");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(halberdAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(halberdDamageLower, halberdDamageHigher);

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

            // Reduce def
            if (combatManagerReference.ApplyModifierToPlayer(StatType.DEF, halberdDefReductionAmount))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen's armour is shattered!", 1.5f, false);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen prevents her armour from being shattered!", 1.5f, false);
            }
            
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
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
        IncreaseWaitTime(halberdWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Third action, acc dropping attack
    private IEnumerator Javelin()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight tosses her javelin", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("SpearThrow");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(javelinAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(javelinDamageLower, javelinDamageHigher);

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

            // Reduce acc
            if (combatManagerReference.ApplyModifierToPlayer(StatType.ACC, javelinAccReductionAmount))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen's vision blurs!", 1.5f, false);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen prevents her vision from blurring!", 1.5f, false);
            }
            
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
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
        IncreaseWaitTime(javelinWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Fourth action, spd reduction attack
    private IEnumerator Maul()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight clobbers with her maul", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("MassiveImpact");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(maulAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(maulDamageLower, maulDamageHigher);

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

            // Reduce spd
            if (combatManagerReference.ApplyModifierToPlayer(StatType.SPD, maulSpdReductionAmount))
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen's breathing is heavy!", 1.5f, false);
            }
            else
            {
                // Change description
                combatManagerReference.DisplayCombatDescription("Gwen prevents her breathing from becoming heavy!", 1.5f, false);
            }
            
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
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
        IncreaseWaitTime(maulWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Fifth action, spd and acc buff attack
    private IEnumerator Rapier()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight pierces with her rapier", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("DrillThrust");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(rapierAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(rapierDamageLower, rapierDamageHigher);

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

            // Buff spd and acc
            AddModifier(StatType.SPD, rapierSpdBuffAmount);
            AddModifier(StatType.ACC, rapierAccBuffAmount);

            // Change description
            combatManagerReference.DisplayCombatDescription("Vermilion Knight focuses her concentration", 1.5f, false);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
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
        IncreaseWaitTime(rapierWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Sixth action, negative mod healing attack
    private IEnumerator Greatsword()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight slashes with her greatsword", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("HeavySwordSwingSpecial");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if hits
        if (TestAccuracy(greatswordAccuracy))
        {
            // It hits, calculate damage
            float damage = CalculateDamage(greatswordDamageLower, greatswordDamageHigher);

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

            // Determine healing amount
            float healing = 0;

            int negativeModifiers = playerReference.GetNegativeModifierCount();

            // Check if negative modifiers exist
            if (negativeModifiers > 0)
            {
                healing = greatswordHealingModifier * negativeModifiers;

                // Change description
                combatManagerReference.DisplayCombatDescription("Vermilion Knight siphons Gwen's weaknesses", 1.5f, false);
                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                combatManagerReference.RestoreHealthEnemy(1, healing);
                yield return new WaitForSeconds(0.1f);

                // Wait until turn can proceed
                while (combatManagerReference.CanTurnProceed() == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
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
        IncreaseWaitTime(halberdWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Seventh action, grant counter
    private IEnumerator Greatshield()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight readies a counter with her greatshield", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("ShieldReady");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        AddAugment(AugmentType.COUNTER);

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(greatshieldWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Eighth action, removes adjusters and damages/heals
    private IEnumerator UnholySymbol()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight displays an unholy symbol", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("UnholySymbol");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check gwen's adjusters
        if (playerReference.HasAnyAugments() || playerReference.HasAnyModifiers())
        {
            // Change description
            combatManagerReference.DisplayCombatDescription("Gwen's adjusters rupture!", 1.5f, false);

            // Play sfx
            audioManagerReference.PlayCombatSFX("AdjusterRupture");

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Figure out damage
            int adjusterCount = playerReference.GetAugmentCount() + playerReference.GetModifierCount();

            // Remove adjusters
            playerReference.RemoveAllAdjusters();

            float damage = adjusterCount * unholySymbolDamageMultiplier;

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

        // Do karmillion's adjusters
        if (HasAnyAugments() || HasAnyModifiers())
        {
            // Change description
            combatManagerReference.DisplayCombatDescription("Vermilion Knight's adjusters rupture!", 1.5f, false);

            // Play sfx
            audioManagerReference.PlayCombatSFX("AdjusterRupture");

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Figure out healing
            int vkAdjusterCount = GetAugmentCount() + GetModifierCount();

            // Remove adjusters
            RemoveAllAdjusters();

            float healing = vkAdjusterCount * unholySymbolHealingMultiplier;

            combatManagerReference.RestoreHealthEnemy(1, healing);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(unholySymbolWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Ninth action, debuffs spd and dmg and buffs own dmg
    private IEnumerator HellishBanner()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight flourishes a hellish banner", 1.5f, false);

        // Play sfx
        audioManagerReference.PlayWeaponSFX("FlagStaffInspire");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Debuff player
        combatManagerReference.ApplyModifierToPlayer(StatType.SPD, hellishBannerSpdReductionAmount);
        combatManagerReference.ApplyModifierToPlayer(StatType.DMG, hellishBannerDmgReductionAmount);

        // Buff self
        AddModifier(StatType.DMG, hellishBannerDmgBuffAmount);

        // Change description
        combatManagerReference.DisplayCombatDescription("A sickening pressure looms", 1.5f, false);
        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(hellishBannerWaitCost);

        // End the turn
        HandleEndTurn();

        yield return null;
    }

    // Revenge attack, buffs def and heals from judgement
    private IEnumerator Falchion()
    {
        // Change description
        combatManagerReference.DisplayCombatDescription("Vermilion Knight paints a crimson sky with her falchion", 2.0f, false);

        // Play sfx
        audioManagerReference.PlayEntitySFX("CrimsonSky");

        yield return new WaitForSeconds(0.1f);

        // Wait until turn can proceed
        while (combatManagerReference.CanTurnProceed() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        bool judgementThresholdCheck = combatManagerReference.judgementMeter > 25.0f;
        float healing = combatManagerReference.judgementMeter * falchionHealingMultiplier;

        // It hits, calculate damage
        float damage = CalculateDamage(falchionDamageLower, falchionDamageHigher);

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

        // increase def
        AddModifier(StatType.DEF, falchionDefBuffAmount);

        // Check judgement
        if (judgementThresholdCheck == true)
        {
            // Kill judgement and heal self
            if (combatManagerReference.judgementMeter == 100.0f)
            {
                combatManagerReference.NotifyJudgementUsed();
            }
            combatManagerReference.judgementMeter = -1000.0f;

            // Change description
            combatManagerReference.DisplayCombatDescription("Gwen's judgement is eviscerated", 2.5f, false);

            // Play sfx
            audioManagerReference.PlayCombatSFX("JudgementShatter");

            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            combatManagerReference.RestoreHealthEnemy(1, healing);
            yield return new WaitForSeconds(0.1f);

            // Wait until turn can proceed
            while (combatManagerReference.CanTurnProceed() == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Remove combat description
        combatManagerReference.RemoveCombatDescription();

        // Increase wait cost
        IncreaseWaitTime(falchionWaitCost);

        // Notify the combat manager that lash was used
        combatManagerReference.NotifyRevengeUsed();

        // End the turn
        HandleEndTurn();

        yield return null;
    }
}
