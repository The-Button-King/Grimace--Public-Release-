using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: EnemyAudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Enemy audio pool child
 * 
 * Functions:   
 * 
 * References: 
 * 
 * See Also: AudioPool
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 10/06/22     WH          1.0         Initial creation
 * 26/07/22     WH          1.1         Added more variables
 * 14/08/22     WH          1.2         Cleaning
 ****************************************************************************************************/
public class EnemyAudioPool : AudioPool
{
    [Header("Enemy Audios")]

    [Tooltip( "Audio for idling" )]
    public Audio m_idle;

    [Tooltip( "Audio for attack 1" )]
    public Audio m_attack1;
    [Tooltip( "Audio for attack 2" )]
    public Audio m_attack2;

    [Tooltip( "Audio for being damaged" )]
    public Audio m_damaged;

    [Tooltip( "Audio for critical damage" )]
    public Audio m_critDamage;

    [Tooltip( "Audio for dying" )]
    public Audio m_death;

}
