using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: ReloadSpeed
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
public class ReloadSpeed : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The gun mode to give ammo for" )]
    private GunManager.Guns m_gun; // Gun to effect

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Decreases reload time
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

        // The gun to give ammo to, defult to assault rifle to not error
        GunStats gun = m_assaultRifle;

        // Switch to the actual selection
        switch ( m_gun )
        {
            case GunManager.Guns.Assault:
            {
                gun = m_assaultRifle;
                break;
            }
            case GunManager.Guns.Charge:
            {
                gun = m_chargeRifle;
                break;
            }
            case GunManager.Guns.Shotgun:
            {
                gun = m_shotgun;
                break;
            }
        }

        // Upgrade reload time
        gun.UpgradeReloadTime();
    }

}
