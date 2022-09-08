using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
/****************************************************************************************************
* Type: Class
* 
* Name: Mage
*
* Author: Joseph Gilmore
*
* Purpose: Mage AI class 
* 
* Functions:            protected override void Start()
*                       public void SetHitByEntitie( GameObject gameObject )
*                       public GameObject GetHitByEntitie()
*                       private void Update()
*                       public void AnimationEvent()
*                       public UnityEvent GetAnimationEvent()
*                       protected override void DelayDeath()
*                       
* 
* References:
* 
* See Also:
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* UK            JG           1.00       - Created class
* 02/08/2022    JG           2.00       - Restructured 
* 11/08/2022    Jg           3.00       - Added interface for new animatio system 
* 12/08/2022    JG           3.01       - Delayed death
* 15/08/2022    JG           3.02       - Cleaning 
****************************************************************************************************/
public class Mage : AgentAI , IAnimationTrigger
{
    private GameObject m_hitByEntitie;      // Stores what AI has taken damage from 
    private UnityEvent m_animationEvent;    // Store event as animation
    private int        an_movement;         // Animation string to hash 
     /***************************************************
     *   Function        : Start
     *   Purpose         : Setup script and call parent 
     *   Parameters      : N/A
     *   Returns         : Void  
     *   Date altered    : 11/08/2022 
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Create animation event 
        m_animationEvent = new UnityEvent();

        // Set animation hash
        an_movement = Animator.StringToHash( "Movement" );
    }
    /***************************************************
     *   Function        : SetHitByEntite   
     *   Purpose         : Set the hit by entite    
     *   Parameters      : GameObject gameObject  
     *   Returns         : N/A   
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void SetHitByEntitie( GameObject gameObject )
    {
       m_hitByEntitie = gameObject;
    }
    /***************************************************
     *   Function        : GetHitByEntitie() 
     *   Purpose         : Gethitbyentite     
     *   Parameters      : return hitby entite 
     *   Returns         : N/A   
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public GameObject GetHitByEntitie()
    {
        return m_hitByEntitie;
    }
    /***************************************************
    *   Function        : Update
    *   Purpose         : Update the mage & animations 
    *   Parameters      : N/A
    *   Returns         : Void  
    *   Date altered    : 12/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        // If not dead 
        if(m_isDead == false )
        {
            // Set the movement value to the agent velocity 
            m_animator.SetFloat( an_movement, m_agent.velocity.magnitude );
        }
       
    }
    /***************************************************
    *   Function        : Animation Event 
    *   Purpose         : Trigger the unity event 
    *   Parameters      : N/A
    *   Returns         : Void  
    *   Date altered    : 11/08/2022 
    *   Contributors    : JG
    *   Notes           : Called by an animation event 
    *   See also        : IAnimationTrigger, Animation tree    
    ******************************************************/
    public void AnimationEvent()
    {
        m_animationEvent.Invoke();
    }
    /***************************************************
    *   Function        : GetAnimationEvent
    *   Purpose         : Return unity event used as animation event  
    *   Parameters      : N/A
    *   Returns         : m_animationEvent  
    *   Date altered    : 11/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public UnityEvent GetAnimationEvent()
    {
        return m_animationEvent;
    }
    /***************************************************
    *   Function        : DelayedDeath
    *   Purpose         : Make object not active while in death anim
    *   Parameters      : N/A
    *   Returns         : 
    *   Date altered    : 12/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : AIData   
    ******************************************************/
    protected override void DelayDeath()
    {
        base.DelayDeath();

        // Disable AI
        GetComponent<NavMeshAgent>().enabled =false;
        GetComponent<BehaviourTreeOwner>().enabled =false;


    }
}
