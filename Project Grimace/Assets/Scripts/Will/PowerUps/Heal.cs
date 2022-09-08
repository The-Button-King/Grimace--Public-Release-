using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: Heal
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
 * 25/07/22     WH          1.2         Changed to healing by an amount and cleaned
 * 26/07/22     WH          1.3         Made it so it actually heals instead of setting health
 * 13/08/22     WH          1.4         Fixed overheal bug
 * 17/08/22     WH          1.5         Cleaning
 ****************************************************************************************************/
public class Heal : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The amount to heal the player" )]
    [Min(0)]
    private float   m_health;               // The amount of health to give

    [SerializeField]
    [Tooltip( "Should you ignore the health value and fully heal the player?" )]
    private bool    m_fullHeal = false;     // Should you fully heal the player?

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Heals player
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

        if ( m_fullHeal )
        {
            // Set the health to be the player's max health when fully healing
            m_health = m_player.GetMaxHealth();
        }
        else
        {
            // Add the extra health, clamping between 0 and the max health
            m_health = Mathf.Clamp( m_health + m_player.GetHealth(), 0, m_player.GetMaxHealth() );
        }

        // Heal player
        m_player.SetHealth( m_health );

    }

}
