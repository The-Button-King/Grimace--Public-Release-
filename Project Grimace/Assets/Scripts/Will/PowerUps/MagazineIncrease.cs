using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: MagazineIncrease
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
 * 25/07/22     WH          1.2         Added individual gun mag increases and cleaned
 * 17/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class MagazineIncrease : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The gun mode to give ammo for" )]
    private GunManager.Guns m_gun;          // Gun to effect

    [SerializeField]
    [Tooltip( "The increase to the magazine capacity" )]
    private int             m_magIncrease;  // Amount to increase mag by

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases magazine size
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

        // Increase magazine capacity
        gun.SetMagCap( gun.GetMagCap() + m_magIncrease );
    }

}
