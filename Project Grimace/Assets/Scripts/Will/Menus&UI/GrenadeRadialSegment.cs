using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GreadeEnums;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: GrenadeRadialSegment
 *
 * Author: Will Harding
 *
 * Purpose: Segment of radial menu
 * 
 * Functions:   private void Start()
 *              public override void Select()
 * 
 * References: 
 * 
 * See Also: RadialSegment
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 04/07/22     WH          1.0         Initial creation
 * 16/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class GrenadeRadialSegment : RadialSegment
{
    [Tooltip( "The grenade type to give the player" )]
    public GrenadeTypes     m_grenadeType;  // Grenade to give to player

    [Tooltip( "The grenade manager" )]
    public GrenadeManager   m_manager;      // Grenade manager

    /***************************************************
    *   Function        : Start
    *   Purpose         : Sets grenade manager
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 16/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get grenade manager
        GameObject player = GameObject.FindWithTag( "Player" );
        m_manager = player.GetComponent<GrenadeManager>();
    }

    /***************************************************
    *   Function        :    Select
    *   Purpose         :    Does something when segment is selected
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void Select()
    {
        base.Select();

        // Sets grenade
        m_manager.SetGrenade( m_grenadeType, m_iconImage );
    }
}
