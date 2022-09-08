using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{
	[Category("Custom Brute Attack")]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: PickUpHazard
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: Pick Up a enviromental hazard for throw task 
	 * 
	 * Functions:   protected override string OnInit()
	 *				protected override void OnExecute()
	 *				private IEnumerator PickUp()
	 *				private void SetUpHazard()
	 *				protected override void OnStop()
	 *				protected override void OnUpdate()
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
	 * 25/07/2022    JG			 1.00		 - Created class with basic functions
	 * 05/08/2022    JG			 1.01		- started anims 
	 * 06/08/2022    JG			 1.02		_ Improved anims 
	 * 12/08/2022    JG			 1.50		- Animations redone 
	 * 15/08/2022    JG			 1.51		- Clean
	 * 24/08/2022     JG	     1.52       - More bug fixes 
	 ****************************************************************************************************/
	public class PickUpHazard : ActionTask
	{
		private FieldOfView m_fieldOfViewEnvironment;				// Detect Enviroment in FOV
		private float		m_enviromentHazardFovRange = 200.0f;	// FOV range to see hazards 
		private GameObject	m_hazard;								// Reference to hazard 
		private float		m_pickUpDistance = 2.2f;                // Distance requried to pick up
		private UnityEvent	m_animationEvent;                       // Store unity event triggered by anim event 
		private const float k_failSafe = 150.0f;                    // Used to exit the task if time elasped 
		private NavMeshAgent m_agent;                               // Reference the agent 
		private int			an_grab;								// String to hash animation
		/***************************************************
		 *   Function        : OnInit    
		 *   Purpose         : Setup class   
		 *   Parameters      : N/A  
		 *   Returns         : null   
		 *   Date altered    : 12/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override string OnInit()
		{
			// Set up field of view for enviroment 
			m_fieldOfViewEnvironment = new FieldOfView( agent.transform, m_enviromentHazardFovRange, Layers.EnviromentInteractable );

			// Set Agent 
			m_agent = agent.GetComponent<NavMeshAgent>();

			// If agent uses animation triggers
			if(m_agent.GetComponent<IAnimationTrigger>() != null )
            {
				// Store event 
				m_animationEvent = m_agent.GetComponent<IAnimationTrigger>().GetAnimationEvent();

			}
			// Set string to hash
			an_grab = Animator.StringToHash( "Grab" );
			return null;
		}

		 /***************************************************
		 *   Function        : OnExecute   
		 *   Purpose         : When task is execute run code    
		 *   Parameters      : N/A    
		 *   Returns         : Void    
		 *   Date altered    : 24/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnExecute()
		{
			// Add event listener 
			m_animationEvent.AddListener(SetUpHazard);

			// Start looking for hazards 
			StartCoroutine( m_fieldOfViewEnvironment.CheckFOV( m_agent.transform ) );
			StartCoroutine( PickUp() );
		}

		/***************************************************
		*  IEnumerator      : PickUp 
		*   Purpose         : Pick up a hazard if in view 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private IEnumerator PickUp()
        {
			
			// Check if target is in FOV
			if ( m_fieldOfViewEnvironment.GetTargetInFOV() == true  )
			{ 
				// Set hazard
				m_hazard = m_fieldOfViewEnvironment.GetTarget().gameObject;
				if(m_hazard.GetComponent<EnviromentInteractble>().IsPickedUp() == false )
                {
					// Set desitnation of agent to hazard position
					m_agent.SetDestination( m_hazard.transform.position );

					// Hazard has now been picked up
					m_hazard.GetComponent<EnviromentInteractble>().SetPickedUp( true );

					// Check if active 
					if ( m_agent.isActiveAndEnabled )
					{
						// If close enough to hazard to pickup
						while ( m_agent.remainingDistance > m_pickUpDistance )
						{

							yield return null;
						}
					}
					else
					{
						EndAction( false );
					}


					// Play grab animation
					m_agent.GetComponent<Animator>().SetTrigger( an_grab );

				}
                else
                {
					EndAction(false );
                }

			}
			else
			{
				// If no hazard in view end task
				EndAction( false );
			}
		}
		/***************************************************
		*   Function        : SetUpHazard
		*   Purpose         : Set up the hazard to be held by AI
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 25/07/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private void SetUpHazard()
        {
			// Reparent hazard to be inside AI
			m_hazard.transform.parent = m_agent.GetComponent<Brute>().GetHazradTransform();
			
			// Update rigidbody to carried
			m_hazard.GetComponent<Rigidbody>().useGravity = false;
			m_hazard.GetComponent<Rigidbody>().isKinematic = true;

			// Set local position to zero to be in the held positon 
			m_hazard.transform.localPosition = Vector3.zero;
			m_hazard.transform.localEulerAngles = Vector3.zero;

			// Store hazard in blackboard for throw task
			blackboard.SetVariableValue("b_hazard",m_hazard);

			EndAction(true);
		}
		/***************************************************
		*   Function        : OnStop
		*   Purpose         : When task stops running 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{
			m_animationEvent.RemoveAllListeners();
			
		}
		/***************************************************
		*   Function        : OnUpdate
		*   Purpose         : Kill task if ran for too long. being extra defensive 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnUpdate()
        {
		  // Stop task if ran for too long 
          if(elapsedTime > k_failSafe )
          {
				EndAction(false);
          }
			if ( m_agent.isActiveAndEnabled == false )
			{

				EndAction( false );
			}
		}
       
	}
}