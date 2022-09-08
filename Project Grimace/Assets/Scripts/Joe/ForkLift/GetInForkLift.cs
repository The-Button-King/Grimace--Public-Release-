using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: GetInForkLift 
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Used to get into the forklift 
 * 
 * Functions:       private void Start()
 *                  public override void Interact()
 *                  public override void DisplayLookAtText( )
 *                  
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * UK            JG          1.00        - Created class 
 * 15/08/2022    JG          1.01        - Cleaning 
 ****************************************************************************************************/
public class GetInForkLift : Interactable
{
    private ForkLift        m_forkLift;         // Reference to forklift 

    public PlayerManager    m_player;           // Reference to player 

    /***************************************************
     *   Function        : Start  
     *   Purpose         : Setup Class
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : 
     ******************************************************/
    private void Start()
    {
        // References 
        m_forkLift = GetComponent<ForkLift>();  
        m_player = GameObject.FindWithTag( "Player" ).GetComponent<PlayerManager>();
    }

    /***************************************************
     *   Function        : Interact    
     *   Purpose         : Ride forklift
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : 31/07/2022
     *   Contributors    : JG, WH
     *   Notes           :    
     *   See also        : Interactable, PlayerManager 
     ******************************************************/
    public override void Interact()
    {
        // If not in forklift and forklift certified
        if( m_forkLift.IsInForkLift() == false && m_player.GetForkliftCertification() )
        { 
            m_forkLift.ActivateForkLift();
            
        }
        // Change interact text
        else
        {
            m_useAltText = true;
        }
        
       
    }
    /***************************************************
    *   Function        : DisplayLookAText   
    *   Purpose         : Displays text
    *   Parameters      : None   
    *   Returns         : Void   
    *   Date altered    : 31/07/2022
    *   Contributors    : JG, WH
    *   Notes           :    
    *   See also        : Interactable
    ******************************************************/
    public override void DisplayLookAtText( )
    {
        // Change text when you are forklift certified
        if ( m_player.GetForkliftCertification() )
        {
            m_useAltText = false;
        }

        base.DisplayLookAtText();
    }
}
