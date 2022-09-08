using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PelletCountIncrease
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
 * 29/07/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class PelletCountIncrease : PowerUpParent
{
    [SerializeField]
    [Tooltip( "Number of pellets to add to shotgun shot" )]
    private int m_pelletsToAdd; // Amount of pellets to add

    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Increases pellet count
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void PowerUpEffect()
    {
        base.PowerUpEffect();

        // Increase shotgun pellets
        m_shotgun.SetPelletCount( m_shotgun.GetPelletCount() + m_pelletsToAdd );
    }

}
