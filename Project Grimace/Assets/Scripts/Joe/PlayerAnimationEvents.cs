using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: PlayerAnimationEvents
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Due to the animatior not being on the gameobject that handles the player I use this script to handle animation events for the player 
 * 
 * Functions:   void Start()
 *              public void AnimationEvent(string eventToTrigger )
 * 
 * References:
 * 
 * See Also: PlayerManager & its animators
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 07/08/2022     JG         1.00         - Created class 
 ****************************************************************************************************/
public class PlayerAnimationEvents : MonoBehaviour
{
    
    private PlayerManager m_playerManager;    // Reference to player manager 
   
    /***************************************************
     *   Function        : Start  
     *   Purpose         : Setup the class by getting references 
     *   Parameters      : N/A 
     *   Returns         : void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    void Start()
    {
        // Get reference to manager 
        m_playerManager = transform.root.GetComponentInChildren<PlayerManager>();
    }
    /***************************************************
     *   Function        : AnimationEvent   
     *   Purpose         : Call correct function depending on event    
     *   Parameters      : string eventToTrigger   
     *   Returns         : void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void AnimationEvent( string eventToTrigger )
    
    {
        switch ( eventToTrigger )
        {
            case "GrenadeThrow":
                {
                    // Throw grenade 
                    m_playerManager.ThrowGrenade();
                }
                break;
        }
    }
}
