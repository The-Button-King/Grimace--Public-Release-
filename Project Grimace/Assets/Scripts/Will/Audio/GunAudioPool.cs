using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: GunAudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Gun audio pool child
 * 
 * Functions:   
 * 
 * References: 
 * 
 * See Also: AudioPool
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 13/06/22     WH          1.0         Initial creation
 * 26/07/22     WH          1.1         Added more variables
 * 27/07/22     JG          1.2         Added more variables
 * 14/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class GunAudioPool : AudioPool
{
    [Header( "Gun Audios" )]

    [Tooltip( "Audio for shooting" )]
    public Audio m_shoot;

    [Tooltip( "Audio for reloading" )]
    public Audio m_reload;
    
    [Tooltip( "Audio for equipping the gun" )]
    public Audio m_equip;
    
    [Tooltip( "Audio for shooting with an empty magazine" )]
    public Audio m_emptyShoot;
    
    [Tooltip( "Audio for charging the gin" )]
    public Audio m_charge;
    
    [Tooltip( "Audio for overcharging the gun" )]
    public Audio m_overHeat;
}
