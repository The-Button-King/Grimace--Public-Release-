using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: RandomIdleBehaviour
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Randomly play another idle animaion
 * 
 * Functions:       override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
 *                  override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
 *                  private void ResetIdle(Animator animator)
 *                  private void CheckState( Animator animator, AnimatorStateInfo stateInfo )
 * 
 * References: Help from:  www.youtube.com. Ketra Games (n.d.). Add Random ‘Bored’ Idle Animations to Your Character (Unity Tutorial). [online] Available at: https://www.youtube.com/watch?v=OCd7terfNxk [Accessed 7 Aug. 2022].
?
 * 
 * See Also: StateMachineBehaviour
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 07/08/2022    JG          1.00        - Created class
 * 12/08/2022    JG          1.01        - Bug fix
 * 14/08/2022   JG           1.02        - Cleaned
 ****************************************************************************************************/
public class RandomIdleBehaviour : StateMachineBehaviour
{
  
    [Header("Idle length before random")]
    private float            m_changeTime;                                              // Time between idle animation changes    
    [SerializeField][Tooltip("Min Amount of time for idle to play to change to eye")]
    private float            m_minChangeTime = 1.0f;                                    // Min amount of time to switch anims
    [SerializeField]
    [Tooltip( "Max Amount of time for idle to play to chnage to eye" )]
    private float            m_maxChangeTime = 8.0f;                                    // Max amount of time to change anims
    private float            m_idleTime;                                                // Time spent idle 
    private bool             m_eyeIdle = false;                                         // Is Eye idle playing 
    private float            m_blendScale = 0.0f;                                       // Current blend scale of idle animation
    private const float      k_dampAmount = 0.2f;                                       // Amount to damp blend tree transition
    private const float      k_startFreshHold = 0.02f;                                  // Freshold to start new anim
    private const float      k_endFreshHold = 0.98f;                                    // Freshold to end anim
    private const float      k_endBlendScale = 1.0f;                                    // End blend scale
    private int              an_idle;                                                   // Used to string to has animator 
    /***************************************************
     *   Function        : OnStateEnter   
     *   Purpose         : When enter animation state setup idle  
     *   Parameters      : Animator animator, AnimatorStateInfo stateInfo, int layerIndex   
     *   Returns         : void   
     *   Date altered    : 12/08/2022
     *   Contributors    : JG
     *   Notes           : Inhereited from unity class   
     *   See also        :    
     ******************************************************/
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // Set string to hash to reduce type errors 
        an_idle = Animator.StringToHash( "Idle" );

        // Reset values since last idle
        ResetIdle(animator);
    }

    /***************************************************
     *   Function        : OnStateUpdate  
     *   Purpose         : while animator inside state change state  
     *   Parameters      : Animator animator, AnimatorStateInfo stateInfo, int layerIndex   
     *   Returns         : void   
     *   Date altered    : 14/08/2022
     *   Contributors    : JG
     *   Notes           : Inhereited from unity class   
     *   See also        :    
     ******************************************************/
    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // Check state of animation to see if it needs changing 
        CheckState(animator,stateInfo);
        
       // Update blend tree with blend scale and damp the transition
       animator.SetFloat( an_idle, m_blendScale, k_dampAmount, Time.deltaTime );
        
    }
    /***************************************************
     *   Function        : ResetIdle   
     *   Purpose         : Reset to default idle  
     *   Parameters      : Animator animator 
     *   Returns         : Void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void ResetIdle( Animator animator )
    {
       
        // Reset values
        m_eyeIdle =false;
        m_idleTime = 0.0f;
        m_blendScale= 0.0f;

        // Reset animator 
        animator.SetFloat( an_idle, m_blendScale );


        // Randomise timer 
        m_changeTime = Random.Range( m_minChangeTime, m_maxChangeTime );
       
    }
    /***************************************************
     *   Function        : CheckState   
     *   Purpose         : Check if animation needs changing 
     *   Parameters      : Animator animator, AnimatorStateInfo stateInfo 
     *   Returns         : Void   
     *   Date altered    : 14/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void CheckState( Animator animator, AnimatorStateInfo stateInfo )
    {
        // If eye idle not being played
        if ( m_eyeIdle == false )
        {
            // Add to timer 
            m_idleTime += Time.deltaTime;

            // If idle time reached and close to the begining of current animation
            if ( m_idleTime > m_changeTime && stateInfo.normalizedTime % 1 < k_startFreshHold )
            {
                // Set to change anim to eye idle 
                m_eyeIdle = true;

                // Set the the scale needed to blend 
                m_blendScale = k_endBlendScale;
            }

        }
        else if ( stateInfo.normalizedTime % 1 > k_endFreshHold )
        {
            // If close to completion reset back to normal ideal 
            ResetIdle( animator );
        }
    }
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
