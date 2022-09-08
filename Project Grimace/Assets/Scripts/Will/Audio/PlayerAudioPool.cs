using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PlayerAudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Player audio pool child
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
 * 14/08/22     WH          1.2         Cleaning
 ****************************************************************************************************/
public class PlayerAudioPool : AudioPool
{
    [Header( "Player Audios" )]

    [Tooltip( "Audio for walking" )]
    public Audio m_walk;

    [Tooltip( "Audio for being damaged" )]
    public Audio m_damaged;

    [Tooltip( "Audio for dying" )]
    public Audio m_death;

    [Tooltip( "Audio for interacting with a non-interactable" )]
    public Audio m_interactFail;

    [Tooltip( "Audio for interacting with an interactable" )]
    public Audio m_interactSuccess;

    [Tooltip( "Audio for your shield being damaged" )]
    public Audio m_shieldHit;

    [Tooltip( "Audio for jumping" )]
    public Audio m_jump;

    [Tooltip( "Audio for throwing a grenade" )]
    public Audio m_throwGrenade;

    [Tooltip( "Audio for dashing" )]
    public Audio m_dash;
}
