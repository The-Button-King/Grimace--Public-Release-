using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{
	[Category( "Custom Brute Attack" )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: ThrowHazard 
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: Throw a hazard toward the player 
	 * 
	 * Functions:		protected override string OnInit()
	 *					protected override void OnExecute()
	 *					private IEnumerator SetUpThrow()
	 *					public void EventHandle()
	 *					private IEnumerator ReleaseHazard()
	 *					protected override void OnStop()
	 *					private void SetUpRigidbody( Rigidbody body)
	 *					
	 *					
	 * 
	 * References:
	 * 
	 * See Also: PickUpHazard
	 *
	 * Change Log:
	 * Date          Initials    Version     Comments
	 * ----------    --------    -------     ----------------------------------------------
	 * 25/07/2022    JG			 1.00		 - Created class with basic functions
	 * 26/07/2022    JG			 1.01		 - Audio
	 * 06/08/2022    JG			 1.02		 - Animation overhaul 
	 * 12/08/2022    JG			 2.00		 - Animation overhaul 2.0
	 * 15/08/2022    JG			 2.01		 - Clean
	 * 16/08/2022    JG			 2.02	     - Bug fix
	 ****************************************************************************************************/
	public class ThrowHazard : ActionTask
	{
		
		private FieldOfView			m_fieldOfView;								// Reference to fov
		private GameObject			m_hazard;									// Reference to hazard
		private float				m_maxDistance = 20.0f;						// Max amount of distance to throw
		private float				m_maxForce = 2.2f;							// Max amount of force that can be applied 	
		private float				m_throwDelays = 0.5f;						// Used to delay throw actions 
		private float				m_rotationAngle = 5.0f;						// Rotation value 
		private float				m_rotationSpeed = 2.0f;                     // Speed to rotate towards the player 
		private float				m_angularSpeed;								// Store AI angular speed
		private const float			k_minAmountOfForce = 1.0f;					// Min amount of force to throw hazard 
		private const float		    k_collisionDelay = 0.01f;                   // Wait a small amount of time for collsions 
		private UnityEvent			m_animationEvent;							// Reference to unity event used as animation event 
		private NavMeshAgent		m_agent;                                    // Reference to Agent 
		private int					an_throw;									// String to has for attack anim
		/***************************************************
		*   Function        : OnInit
		*   Purpose         : Set up task for first time use 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 15/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get agent reference
			m_agent = agent.GetComponent<NavMeshAgent>();

			// Get FOV of agent 
			m_fieldOfView = m_agent.GetComponent<AIData>().GetFieldOfView();

			// If Agent is an animation trigger 
			if(m_agent.GetComponent<IAnimationTrigger>() != null )
            {
				// Store  unity event as its used as an animation event 
				m_animationEvent = m_agent.GetComponent<IAnimationTrigger>().GetAnimationEvent();
            }

			// Set string to hash
			an_throw = Animator.StringToHash( "ThrowAttack" );
			return null;
		}

		/***************************************************
		*   Function        : OnExecute
		*   Purpose         : When task runs check for player
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{

			// Add event listerner 
			m_animationEvent.AddListener( EventHandle );

			// Store hazard 
			m_hazard = blackboard.GetVariable<GameObject>( "b_hazard" ).value;

			// If null task cannot run
			if (m_hazard == null )
            {
				EndAction(false);
            }

			// Store agent anuglar speed
			m_angularSpeed = m_agent.angularSpeed;

			// Set it to 0 so can override 
			m_agent.angularSpeed = 0;

			// Check FOV to throw hazard at player 
			StartCoroutine( m_fieldOfView.CheckFOV(agent.transform) );

			// Start the throwing routine 
			StartCoroutine( SetUpThrow() );
		}

		/***************************************************
		*   IEnumerator     : SetUpThrow
		*   Purpose         : Throw hazard towards player with a nice amount of force 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 15/08/2022
		*   Contributors    : JG
		*   Notes           : Named changed from throw
		*   See also        :    
		******************************************************/
		private IEnumerator SetUpThrow()
        {
				// If player in fov
			if ( m_fieldOfView.GetTargetInFOV() == true )
			{

				// Stop Agent 
				m_agent.isStopped = true;

				// Wait for 
				yield return new WaitForSeconds( m_throwDelays );

				// Work out direction
				Vector3 direction = ( m_fieldOfView.GetTarget().position - agent.transform.position ).normalized;

				// Rotate AI to look at target 
				while ( Vector3.Angle( direction, agent.transform.forward ) > m_rotationAngle )
				{
					// Rotate AI towards last know pos 
					m_agent.transform.rotation = Quaternion.LookRotation( Vector3.RotateTowards( m_agent.transform.forward, direction, m_rotationSpeed * Time.deltaTime, 0 ), Vector3.up );
				}

				yield return new WaitForSeconds( m_throwDelays );
				
				// Set who thrown the hazard so it cannot collide with it 
				m_hazard.GetComponent<EnviromentInteractble>().SetThrownBy( m_agent.transform.root.gameObject );

				// Trigger animation
				m_agent.GetComponent<Animator>().SetTrigger( an_throw );

				// Play  attack sound sound 
				m_agent.transform.GetComponent<AIData>().GetAudioPool().PlaySound( m_agent.transform.GetComponent<AIData>().GetAudioPool().m_attack1 );
				
			}
			else
			{
				EndAction( false );
			}
		
			
			
		}
		/***************************************************
		*   Function		: EventHandle 
		*   Purpose         :  Handle unity event to trigger attack 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           : Named changed from throw
		*   See also        :    
		******************************************************/
		public void EventHandle()
        {
			// Release hazard from AI hands
			StartCoroutine(ReleaseHazard());
        }
		/***************************************************
		*   Function		: ReleaseHazard 
		*   Purpose         :  trigger attack by releasing and throwing hazard 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :
		*   See also        :    
		******************************************************/
		private IEnumerator ReleaseHazard()
        {

			while(m_agent.GetComponent<AIData>().GetIsDead() ==false )
            {
				// Remove hazzard parenting 
				m_hazard.transform.parent = null;
				m_hazard.transform.rotation = Quaternion.identity;

				// Store rb
				Rigidbody body = m_hazard.GetComponent<Rigidbody>();

				// Setup the rigidbody to be thrown
				SetUpRigidbody( body );

				// Get the direction towards the player 
				Vector3 direction = ( m_fieldOfView.GetTarget().position - m_agent.transform.position ).normalized;

				// Work out how much force required for the distance 
				float forceMult = m_maxDistance - Vector3.Distance( m_agent.transform.position, m_fieldOfView.GetTarget().position );
				float force = m_maxForce * forceMult;

				// Ensure its there is some force to apply 
				if ( force < k_minAmountOfForce )
				{
					force = k_minAmountOfForce;
				}

				// Apply force to rb 
				body.AddForce( direction * force, ForceMode.Impulse );


				// Disable collisions 
				body.detectCollisions = false;

				yield return new WaitForSeconds( k_collisionDelay );

				// Enable 
				body.detectCollisions = true;

				// Set thrown to to true so it can accept collisions 
				m_hazard.GetComponent<EnviromentInteractble>().SetThrown( true );


				// Apply attack rate 
				yield return new WaitForSeconds( m_agent.GetComponent<AIData>().GetAttackRate() );

				EndAction( true );
				break;
			}
            
				EndAction( false );
            
		}
		/***************************************************
		*   Function        : OnStop()
		*   Purpose         : When task has stop reset vars
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 15/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{

			if( m_agent.isActiveAndEnabled )
            {
				// Reset vars
				m_agent.isStopped = false;
				m_agent.angularSpeed = m_angularSpeed;
			}
			


			// Reset event 
			m_animationEvent.RemoveAllListeners();
			
		}
		/***************************************************
		*   Function        : SetUpRigidbody
		*   Purpose         : Make sure rigidbody can be thrown 
		*   Parameters      : N/A    
		*   Returns         : Void    
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private void SetUpRigidbody( Rigidbody body)
        {
			// Setup body to be thrown 
			body.velocity = Vector3.zero;
			body.useGravity = true;
			body.isKinematic = false;
		}
	}
}