using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GreadeEnums;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: AddGrenades
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
 * 04/08/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class AddGrenades : PowerUpParent
{
    [SerializeField]
    [Tooltip( "The grenade type to give more of" )]
    private GrenadeTypes    m_type;             // The grenade type to give

    [SerializeField]
    [Tooltip( "Number of grenades to add" )]
    [Min(1)]
    private int             m_grenadesToAdd;    // Number of grenades to add

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Adds grenades
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Get current grenade counts
        List<int> counts = m_grenadeManager.GetCounts();

        // Add to the specified grenade count
        counts[ ( int )m_type ] += m_grenadesToAdd;

        // Set the counts
        m_grenadeManager.SetCounts( counts );

        // Update UI
        m_grenadeManager.UpdateCurrentGrenade();

    }
}
