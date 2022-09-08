using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Ammo
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
 * 20/06/22     WH          1.0         Initial creation
 * 18/07/22     WH          1.1         Changed parent for use with shop
 * 25/07/22     WH          1.2         Added indivudial gun ammo and cleaned
 * 17/07/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class Ammo : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The gun mode to give ammo for" )]
    private GunManager.Guns m_gun;  // Gun to effect

    [SerializeField]
    [Tooltip( "The amount of ammo to give" )]
    private int             m_ammo; // Amount of ammo to give

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases Ammo
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

        // Increase ammo
        gun.SetCurrentAmmo( gun.GetCurrentAmmo() + m_ammo );
    }

}
