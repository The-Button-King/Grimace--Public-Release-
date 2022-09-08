using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

namespace NodeCanvas.Tasks.Actions
{
	// BB Tasks 
	[Category( " Custom Brute Attack" )]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: GroundSlamTask
	*
	* Author: Joseph Gilmore
	*
	* Purpose: Brute attack one 
	* 
	* Functions:		protected override string OnInit()
	*					protected override void OnExecute()
	*					private void  SetUpAttack()
	*					private void Attack()
	*					protected override void OnStop()
	*					private IEnumerator ApplyAttackRate()
	* References:
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 19/07/2022    JG			1.00		- Created class shell
	* 20/07/2022    JG			1.01        - Created a gradual shockwave might move at somepoint and needs cleaning
	* 21/07/2022    JG			2.00		- Moved code to a different class  
	* 26/07/2022    JG			2.01		- Sound 
	* 01/08/2022    JG          2.02		- delayed 
	* 02/08/2022    JG			2.03		- Minor improvements 
	* 05/08/2022    JG			3.00		- Animation overhaul
	* 11/08/2022    JG			4.00		- Overhaul 2.0
	* 13/08/2022    JG			4.01		- Cleaning 
	* 15/08/2022    JG			4.02		- More cleaning
	****************************************************************************************************/
	public class GroundSlamTask : ActionTask
	{
		private ForceWave		m_forceWave;				// Reference to forcewave class 
		private float			m_radius = 5.0f;			// Size of attack radius
		private const float		k_knockBackForce = 10.0f;   // Player knock back force	
		private UnityEvent		m_animationEventListerner;  // Used to listen to Animation events
		private NavMeshAgent	m_agent;                    // Reference to AI agent 
		private int				an_slam;					// String to hash for slam
		/***************************************************
		*   Function        : OnInit()
		*   Purpose         : Set up task before execution  
		*   Parameters      : N/A
		*   Returns         : void   
		*   Date altered    : 11/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Setup force wave
			m_forceWave = agent.transform.gameObject.AddComponent<ForceWave>();
			m_forceWave.SetAll( agent.transform.GetComponentInChildren<LineRenderer>().transform.position, agent.transform.GetComponentInChildren<LineRenderer>(), k_knockBackForce, m_radius, agent.GetComponent<AIData>().GetBaseDamage(), Layers.Player );

			// Get Reference to Agent 
			m_agent = agent.GetComponent<NavMeshAgent>();

			// If agent is a animation trigger 
			if( m_agent.GetComponent<IAnimationTrigger>() != null )
            {
				// Store event reference 
				m_animationEventListerner = m_agent.GetComponent<IAnimationTrigger>().GetAnimationEvent();

			}
			// Set string to hash for anim
			an_slam = Animator.StringToHash( "GroundSlam" );
			return null;
		}


		/***************************************************
		*   Function        : OnExecute   
		*   Purpose         : Start a ground slam   
		*   Parameters      : N/A
		*   Returns         : void   
		*   Date altered    : 11/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
			// If animation event listerner has been set
			if( m_animationEventListerner != null )
            {	
				// Make attack listen 
				m_animationEventListerner.AddListener( Attack );
            }
            else
            {
				//	Task requries animation ( could do without but little point)
				EndAction(false);
            }

			// Setup AI for attack 
			SetUpAttack();
			
		}
		/***************************************************
		*   Function        : SetUpAttack
		*   Purpose         :  Set up the AI and play anim   
		*   Parameters      : N/A
		*   Returns         : void   
		*   Date altered    : 11/08/2022 
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private void  SetUpAttack()
		{

			// Stop agent 
			m_agent.isStopped = true;

			// Reset damage incase AI buffed 
			m_forceWave.SetDamage( m_agent.transform.GetComponent<AIData>().GetBaseDamage() );

			// play  attack anim
			m_agent.transform.GetComponent<Animator>().SetTrigger( an_slam );

		}
		/***************************************************
		*   Function        : Attack
		*   Purpose         : Apply shockwave 
		*   Parameters      : N/A
		*   Returns         : void   
		*   Date altered    : 11/08/2022 
		*   Contributors    : JG
		*   Notes           : Called by unity event   
		*   See also        :    
		******************************************************/
		private void Attack()
		{
			// Update the shock wave pos the the current position 
			m_forceWave.SetShockWavePosition( agent.transform.GetComponentInChildren<LineRenderer>().transform.position );

			// Start Force wave 
			m_forceWave.StartForceWave();

			// Play  attack Sound 
			m_agent.transform.GetComponent<AIData>().GetAudioPool().PlaySound( m_agent.transform.GetComponent<AIData>().GetAudioPool().m_attack2 );

			// Apply AI attack rate before ending task
			StartCoroutine( ApplyAttackRate() );
		}
		/***************************************************
		*   Function        : OnStop  
		*   Purpose         : On task stop reset it   
		*   Parameters      : N/A
		*   Returns         : void   
		*   Date altered    : 12/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{
			// Check if agent is still alive 
			if( m_agent.isActiveAndEnabled )
            {
				// Reset vars
				m_agent.isStopped = false;
			}
			
			m_animationEventListerner.RemoveAllListeners();
		}
		/***************************************************
		*   Function        : ApplyAttackRate
		*   Purpose         : Delay the end of the task by using attack rate  
		*   Parameters      : N/A
		*   Returns         : yield return wait  
		*   Date altered    : 11/08/2022 
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private IEnumerator ApplyAttackRate()
       {
			// Apply attack rate 
			WaitForSeconds wait = new WaitForSeconds( m_agent.GetComponent<AIData>().GetAttackRate() );
			yield return wait;

			// Rest var
			EndAction( true );
		}
	}
	


	
}