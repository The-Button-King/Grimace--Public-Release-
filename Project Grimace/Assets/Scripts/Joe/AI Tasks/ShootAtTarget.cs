using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{
	// Behaviour Tree details
	[Category("Custom Attack")]
	[Description("basic shooting at target")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: ShootAtPlayer
	*
	* Author: Joseph Gilmore
	*
	* Purpose: Make the AI shoot towards a target 
	* 
	* Functions:			protected override string OnInit()
	*						protected override void OnExecute()
	*						protected override void OnUpdate()
	*						protected override void OnStop()
	*						protected override void OnPause()
	*						private IEnumerator ApplyFireRate()
	*						Vector3 InacuracyDirection()
	*						private void Shoot()
	*						
	* 
	* References: N/A
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* N/A			JG			1.00		Set up class 
	* 26/06/2022    JG		    2.00		- Change clas name from ShootAtplayer to shoot at target to have more flexabiilty 
	* 05/07/2022    JG			2.01		- Use AI Data to apply stats 
	* 19/07/2022    JG			2.02		- Added slight inaccuracy
	* 23/07/2022    JG			2.03		- Fixed AI shooting itself with new module 
	* 11/08/2022    JG			3.00        - Overhauled class to work with anims(I recived anims late this is what I could do in the time)
	* 15/08/2022    JG			3.01		- Cleaning 
	****************************************************************************************************/
	public class ShootAtTarget : ActionTask
	{

		
		private BasicGun	m_gun;							// Gun reference 
		private float		m_timer = 0f;					// Timer for fireRate 
		private float		m_fireRate;						// FireRate for AI using task 
		private float		m_inacuracyMutiplier = 0.02f;   // Make the AI not 100% accurate
		private UnityEvent	m_shootEvent;                   // Store unity event and use it as an animation event.
		private const float k_failSafe = 50.0f;				// If task excuting for too long 
		private NavMeshAgent m_agent;						// Reference to AI	
		/***************************************************
		*   Function        : OnInit
		*   Purpose         : Use for initialization of task. This is called only once in the lifetime of the task. Set up gun
		*   Parameters      : N/A   
		*   Returns         : null   
		*   Date altered    : 16/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get the attacted gun
			m_gun = agent.transform.Find( "Gun" ).GetComponent<BasicGun>();

			// Set Agent
			m_agent = agent.GetComponent<NavMeshAgent>();

			// If agent is a an animation trigger
			if( agent.GetComponent<IAnimationTrigger>() != null )
            {
				// Now has reference to event handler 
				m_shootEvent =  agent.GetComponent<IAnimationTrigger>().GetAnimationEvent();
			}

			return null;
		}
		/***************************************************
		*   Function        : OnExecute
		*   Purpose         : Called once each time task enabled 
		*   Parameters      : N/A   
		*   Returns         : void  
		*   Date altered    : 11/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnUpdate
		******************************************************/
		protected override void OnExecute()
		{
			// Get current AI stats (do every time execute incase buff being applyied )
			m_fireRate = m_agent.GetComponent<AIData>().GetAttackRate();

			// Get AI damage
			m_gun.SetBaseDamage( m_agent.GetComponent<AIData>().GetBaseDamage() );

			// If event has been set 
			if(m_shootEvent != null )
            {
				// Assign listner 
				m_shootEvent.AddListener( Shoot );
            }

			// Apply firerate and shoot
			StartCoroutine(ApplyFireRate());
		}
		/***************************************************
		*   Function        : ApplyFireRate
		*   Purpose         : Apply firerate of AI and play animation if it has one 
		*   Parameters      : N/A   
		*   Returns         : void  
		*   Date altered    : 16/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnUpdate
		******************************************************/
		private IEnumerator ApplyFireRate()
        {
			// While not hit fire rate
			while(m_timer < m_fireRate )
            {
				// Increase timer 
				m_timer += Time.deltaTime;

				yield return null;
			}

			// If it has an animatior 
			if (m_agent.GetComponent<AIData>().GetAnimator() != null )
			{
				// Set animation to play
		        m_agent.GetComponent<AIData>().GetAnimator().SetTrigger( "Shoot" );
			}
			else
			{
				// Shoot 
				Shoot();
			}
		}

		/***************************************************
		*   Function        : OnUpdate
		*   Purpose         : Called once per frame while task active. Look towards target
		*   Parameters      : N/A   
		*   Returns         : void  
		*   Date altered    : 16/08/2022 
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnUpdate
		******************************************************/
		protected override void OnUpdate()
		{
			// Look at target 
			m_agent.transform.LookAt(blackboard.GetVariable<GameObject>( "b_target" ).value.transform);

			// If task is executing for too long kill task (Extra defensive)
			if( elapsedTime > k_failSafe )
            {
				Debug.Log( "Shooting failed" );
				EndAction(false);
            }
		}

		/***************************************************
		*   Function        : OnStop
		*   Purpose         : Called task when disabled 
		*   Parameters      : N/A   
		*   Returns         : Void  
		*   Date altered    : 11/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnPause 
		******************************************************/
		protected override void OnStop()
		{
			// Remove all event listeners 
			if ( m_shootEvent != null )
			{
				m_shootEvent.RemoveAllListeners();
			}
			
		}

		/***************************************************
		*   Function        : OnPause 
		*   Purpose         : Called when task pasused 
		*   Parameters      : N/A   
		*   Returns         : Void  
		*   Date altered    : N/A
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnStop
		******************************************************/
		protected override void OnPause()
		{
			
		}
		/***************************************************
		*   Function        : InacuracyDirection
		*   Purpose         : Add slight inacuracy to the direction of AI shooting 
		*   Parameters      : N/A   
		*   Returns         : void  
		*   Date altered    : 19/07/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : 
		*   See also        : basic gun
		******************************************************/
		Vector3 InacuracyDirection()
		{
			// Get normal shooting direction
			Vector3 defaultDirection = agent.transform.TransformDirection( Vector3.forward );

			// Apply a small offset on each axis to create a spread
			defaultDirection.x += Random.Range( -m_inacuracyMutiplier, m_inacuracyMutiplier);
			defaultDirection.y += Random.Range( -m_inacuracyMutiplier, m_inacuracyMutiplier );
			defaultDirection.z += Random.Range( -m_inacuracyMutiplier, m_inacuracyMutiplier );

			// Normalize spread direction and return
			return defaultDirection.normalized;
		}
		/***************************************************
		*   Function        : Shoot
		*   Purpose         : Make the AI shoot 
		*   Parameters      : N/A   
		*   Returns         : Void  
		*   Date altered    : 16/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        : OnStop
		******************************************************/
		private void Shoot()
        {
			// Shoot Gun at with inacuracy
			m_gun.Shoot( m_gun.GetFirePos(), InacuracyDirection() );

			// Play  shoot sound 
			m_agent.GetComponent<EnemyAudioPool>().PlaySound( m_agent.GetComponent<EnemyAudioPool>().m_attack1 );

			// Reset timer 
			m_timer = 0f;

			EndAction( true );
			
		}
	}
}