using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: DamageIncrease
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
 * 28/07/22     WH          1.0         Initial creation
 * 29/07/22     WH          1.1         Name change for consistency
 * 17/08/22     WH          1.2         Cleaning
 ****************************************************************************************************/
public class DamageIncrease : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The gun mode to give ammo for" )]
    private GunManager.Guns m_gun;              // Gun to effect

    [SerializeField]
    [Tooltip( "The amount to increase the damage by" )]
    private int             m_damageIncrease;   // Amount of damage to increase

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases damage
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    28/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // The gun to increase damage to, defult to assault rifle to not error
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

        // Increase damage
        gun.SetBaseDamage( gun.GetBaseDamage() + m_damageIncrease );
    }

}
