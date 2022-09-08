using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: MaxRegenStat
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
public class MaxRegenStat : PowerUpParent
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
    [Tooltip( "The amount to increase the stat by" )]
    private float       m_maxIncrease;  // Increase to stat


    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases max regen stat
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
        statToUpgrade.SetMaxValue( statToUpgrade.GetMaxValue() + m_maxIncrease );
    }

}
