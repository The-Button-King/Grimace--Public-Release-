using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: MaxHealthIncrease
 *
 * Author: Will Harding
 *
 * Purpose: Override for PowerUpParent
 * 
 * Functions:   public override void PowerUpEffect()
 * 
 * References: 
 * 
 * See Also: Pickup
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 20/06/22     WH          1.0         Initial creation
 * 18/07/22     WH          1.1         Changed parent for use with shop
 * 25/07/22     WH          1.2         Cleaned
 * 17/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class MaxHealthIncrease : PowerUpParent
{
    [SerializeField]
    [Tooltip( "Increase to the player's max helth" )]
    private int m_maxHealthIncrease = 10; // Amount to increase the max health by

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases max health
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    WH
    *   Notes           :    Contents moved from Pickup
    *                        Function changed from override of Effect from Pickup
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Increase max health
        m_player.SetMaxHealth( m_player.GetMaxHealth() + m_maxHealthIncrease );
    }

}
