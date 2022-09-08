using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: RegenRechargeRate
 *
 * Author: Will Harding
 *
 * Purpose: Override for PowerUpParent
 * 
 * Functions:   public override void PowerUpEffect()
 * 
 * References: 
 * 
 * See Also: Pickup, PowerUpParent
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 27/07/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class RegenRechargeRate : PowerUpParent
{
    private enum        RegenStats      // Regen stats
    {
        Stamina,
        Shield
    };

    [SerializeField]
    [Tooltip( "The stat to effect" )]
    private RegenStats  m_stat;         // The stat to effect

    [SerializeField]
    [Tooltip( "The amount to increase the regen rate by" )]
    private float       m_rechargeRate; // New max value


    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Upgrades recharge rate
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    27/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Stat to increase, defaulting to stamina
        RegenStat statToUpgrade = m_stamina;

        // Switch to the actual selection
        switch ( m_stat )
        {
            case RegenStats.Stamina:
            {
                statToUpgrade = m_stamina;
                break;
            }

            case RegenStats.Shield:
            {
                statToUpgrade = m_shield;
                break;
            }

        }

        // Upgrade max value of the stat
        statToUpgrade.SetRechargeRate( statToUpgrade.GetRechargeRate() + m_rechargeRate );
    }

}
