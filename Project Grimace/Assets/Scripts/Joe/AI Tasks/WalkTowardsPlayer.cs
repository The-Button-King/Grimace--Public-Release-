using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace NodeCanvas.Tasks.Actions
{
	// Behaviour Tree details
	[Category("Custom Desitnation Task")]
	[Description("AI walks towards player until a certain distance apart")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: WalkTowardsPlayer
	*
	* Author: Joseph
	*
	* Purpose: Walk towards the player to a certain distance 
	* 
	* Functions:		protected override string OnInit()
	*					protected override void OnExecute()
	*					protected override void OnUpdate()
	*					protected override void OnStop()
	*					protected override void OnPause()
	* 
	* References:
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 01/04/2022	JG			1.00		Got the AI to follow the player 
	* 3/04/2022     JG			1.01		Got AI to move back if too close to player 
	* 11/04/2022    JG          1.02        Cleaning 
	* 10/07/2022    JG			1.03		Testing bomber fixes 
	* 11/07/2022    JG			1.04        Remove walk away from player to fix bomber 
	* 15/08/2022    JG			1.05		Cleaning 
	* 16/08/2022    JG			1.06		Bug Fixes
	****************************************************************************************************/
	public class WalkTowardsPlayer : ActionTask
	{

		private NavMeshAgent m_agent;								// Reference to the agent 
		private GameObject	 m_target;								// Player target reference 

		[SerializeField]
		public float		m_stoppingDistance = 5.0f;				// Distance to stop (black board vars need to be public)  
		/***************************************************
		*   Function        : OnInit
		*   Purpose         : Use for initialization of task. This is called only once in the lifetime of the task. Get references   
		*   Parameters      : N/A
		*   Returns         : Null   
		*   Date altered    : 11/04/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin.    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get a reference to agent 
			 m_agent = agent.GetComponent<NavMeshAgent>();

			// Get a refernce to the player 
			m_target = GameObject.FindWithTag( "Player" ); 

			return null;
		}
		/***************************************************
		*   Function        : OnExecute
		*   Purpose         : Used on each task execution
		*   Parameters      : N/A
		*   Returns         : void  
		*   Date altered    : 11/04/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin.    
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
            if ( m_agent.isActiveAndEnabled )
            {
				// Set desitnation 
				m_agent.destination = m_target.transform.position;

            }
		}

		/***************************************************
		*   Function        : OnUpdate
		*   Purpose         : Move towards /away from player to maintain a certain distance 
		*   Parameters      : N/A
		*   Returns         : void
		*   Date altered    : 16/08/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin.    
		*   See also        :    
		******************************************************/
		protected override void OnUpdate()
		{
			// Look at target
			m_agent.transform.LookAt( m_target.transform.position );

			// Was getting call order erros check if active first
			if ( m_agent.isActiveAndEnabled )
            {
				// If got close to the target 
				if ( m_agent.remainingDistance < m_stoppingDistance )
				{
					EndAction( true );
				}
			}
			
          
		}
		
		/***************************************************
		*   Function        : OnStop   
		*   Purpose         : Called when Task Stops   
		*   Parameters      : N/A  
		*   Returns         : Void   
		*   Date altered    : 
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		****************************************************/
		protected override void OnStop() 
		{
			
		}

		/***************************************************
		*   Function        : OnPause   
		*   Purpose         : Called when Task pasues   
		*   Parameters      : N/A  
		*   Returns         : Void   
		*   Date altered    : 
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		****************************************************/
		protected override void OnPause()
		{
			
		}
	}
}