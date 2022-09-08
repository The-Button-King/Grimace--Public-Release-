using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: AmmoCapIncrease
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
public class AmmoCapIncrease : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The gun mode to give ammo for" )]
    private GunManager.Guns m_gun; // Gun to effect

    [SerializeField]
    [Tooltip( "The increase to the ammo capacity" )]
    private int             m_cap; // Ammo cap increase

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases Ammo Cap
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    27/07/22
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

        // Increase ammo cap
        gun.SetAmmoCap( gun.GetAmmoCap() + m_cap );
    }

}
