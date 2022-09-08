using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Money
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
 * 17/08/22     WH          1.3         Cleaning and fixed base not calling
 ****************************************************************************************************/
public class Money : PowerUpParent
{
    [SerializeField]
    [Tooltip( "Amount of money to give" )]
    private int m_moneyIncrease = 10; // Money to give

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases money
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    Contents moved from Pickup
    *                        Function changed from override of Effect from Pickup
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Increase money
        m_player.SetMoney( m_player.GetMoney() + m_moneyIncrease );
    }

}
