using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: ForkliftCertification
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
 * 31/07/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class ForkliftCertification : PowerUpParent
{

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Vertifies player for forklift usage
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        m_player.SetForkliftCertification( true );

    }

}
