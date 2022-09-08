using CameraShake;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.Events;
/****************************************************************************************************
* Type: Class
* 
* Name: Brute 
*
* Author: Joseph Gilmore
*
* Purpose: for the brute AI
* 
* Functions:        protected override void Start()
*                   void Update()
*                   public Transform GetHazradTransform()
*                   public void AnimationEvent()
*                   public UnityEvent GetAnimationEvent()
*                   protected override void DelayDeath()
*                   private void WalkScreenShake()
*                   
*                   
* 
* References:
* 
* See Also:
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 02/08/2022    JG           1.00       - Created class 
* 05/08/2022    JG           1.01       - Dirty animation calling 
* 12/08/2022    JG           1.50       - Anim improvements  & delayed death
* 15/08/2022    JG           1.51       - Cleaning 
* 16/08/2022    JG           1.52       - Bugs
****************************************************************************************************/
public class Brute : AgentAI ,IAnimationTrigger
{
    [Header("Camera shake for walking")]
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("The speed of the brute to trigger screen shake ")]
    private float                        m_minScreenShakeSpeed = 2.0f;      // Min Amount speed requried to toggle screen shake 
    [SerializeField]
    [Range( 1.0f, 10.0f )]
    [Tooltip( "TThe max range screen shake can be applied to the player  " )]
    private float                       m_shakeRange = 5.0f;                // The max range of the screen shake in relation to the playe
    [SerializeField]
    [Range( 1.0f, 5.0f )]
    [Tooltip( "How rough screen shake is " )]
    private float                       m_shakeIntensity = 2.0f;            // How intense the screen shake 
    [SerializeField]
    [Range( 0.01f, 1.0f )]
    [Tooltip( "How rough screen shake is " )]
    private float                       m_shakeTime = 0.1f;                 // How long the screen shake is applied for 
    [SerializeField][Tooltip("Transform for the brute to hold its hazard in attacks")]
    private Transform                   m_hazardTransform;                  // Hazard position for attacks
    private UnityEvent                  m_animationEvent;                   // Unity event as animation event for attacks
    private int                         an_movement;                        // String to hash for animation
    /***************************************************
     *   Function        : Start
     *   Purpose         : setup class 
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    protected override void Start()
    {
        base.Start();
      
      
        // Create new Unity event 
        m_animationEvent = new UnityEvent();

        // Set string to hash 
        an_movement = Animator.StringToHash( "Movement" );
    }
    /***************************************************
     *   Function        : Update  
     *   Purpose         : Check if screen needs shaking    
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    void Update()
    {
        // Check if screen needs shaking 
        WalkScreenShake();

        // Apply anim speed 
        m_animator.SetFloat( an_movement, m_agent.velocity.magnitude );
       
    
    }
    /***************************************************
     *   Function        : GetHazardTransform
     *   Purpose         : Return transform where the hazard is position for anims
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 02/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public Transform GetHazradTransform()
    {
        return m_hazardTransform;
    }
    /***************************************************
     *   Function        : AnimationEvent
     *   Purpose         :  Called via animation events to call unity event  
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 12/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : IAminamtionTrigger    
     ******************************************************/
    public void AnimationEvent()
    {
        // Call unity event as animation 
        m_animationEvent.Invoke();
    }
    /***************************************************
     *   Function        : GetAnimationEvent()
     *   Purpose         :  Return unity event acting as anim event 
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 12/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : IAminamtionTrigger    
     ******************************************************/
    public UnityEvent GetAnimationEvent()
    {
        return m_animationEvent;
    }
    /***************************************************
     *   Function        : DelayDeath
     *   Purpose         :  Delay the actual death by disabling 
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 16/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : 
     ******************************************************/
    protected override void DelayDeath()
    {
        base.DelayDeath();

        // Disable AI componets 
        m_agent.enabled = false;
        GetComponent<BehaviourTreeOwner>().enabled = false;

        //  delete hazard on parents death
         if(m_hazardTransform.GetComponentInChildren<EnviromentInteractble>() != null )
         {
            m_hazardTransform.GetComponentInChildren<EnviromentInteractble>().gameObject.SetActive(false);
         }
        
       
    }
    /***************************************************
     *   Function        : WalkScreenShake
     *   Purpose         : Calculate amount of screen shake for brute walking 
     *   Parameters      : N/A  
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : 
     ******************************************************/
    private void WalkScreenShake()
    {
        // If brute is going over x speed 
        if ( m_agent.velocity.magnitude > m_minScreenShakeSpeed )
        {
            // Shake screen over distance
            ScreenShake.Instance.ShakeOverDistanceFromPoint( transform.position, GameObject.FindGameObjectWithTag( "Player" ).transform.position, m_shakeRange, m_shakeIntensity, m_shakeTime );
        }
    }
}
