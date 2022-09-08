using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PlayerSpawnTeleport
 *
 * Author: Will Harding
 *
 * Purpose: Teleports player from position in scene to it's position at the initial load. This let's 
 *          any and all references with the player be made while it exists before runtime, but moves it
 *          to wherever this script is attached to at the very beginning of runtime.
 * 
 * Functions:   void Start()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 06/07/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class PlayerSpawnTeleport : MonoBehaviour
{
    private GameObject m_player; // Player

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Finds and teleports player
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    06/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    void Start()
    {
        // Find player
        m_player = GameObject.FindWithTag( "Player" );

        // Teleports player
        m_player.transform.position = transform.position;
    }
}
