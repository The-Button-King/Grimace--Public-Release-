using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: RecoilUpgrade
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
public class RecoilUpgrade : PowerUpParent
{

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Reduces Recoil
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

        // Reduce recoil
        m_assaultRifle.UpgradeRecoil();
    }

}
