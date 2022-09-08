using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: DashCostUpgrade
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
 * 29/07/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class DashCostUpgrade : PowerUpParent
{
    [SerializeField]
    [Tooltip ( "How much to reduce the cost of dash by" )]
    private float m_costReduction; // Amount to reduce the cost by

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Upgrades dash cost
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    Contents moved from Pickup
    *                        Function changed from override of Effect from Pickup
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Reduce dash cost
        m_player.SetDashCost( m_player.GetDashCost() - m_costReduction );

    }

}
