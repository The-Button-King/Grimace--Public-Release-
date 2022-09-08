using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
namespace NodeCanvas.Tasks.Actions
{

	[Category( "Custom Desitnation Task" )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: WalkToEchoLocation
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: Used to go to last desitnation a sound was dectected 
	 * 
	 * Functions:			protected override string OnInit()
	 *						protected override void OnUpdate()
	 *						protected override void OnStop()
	 *						private void SetPath()
	 *						private void CheckPathStatus()
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
	 * 10/06/2022    JG			1.00		- class created 
	 * 04/08/2022    JG			1.01		- Extra Defensive and cleaning 
	 * 15/08/2022    JG			1.02		- Cleaning and fixed name spelling error 
	****************************************************************************************************/
	public class WalkToEchoLocationTask : ActionTask
	{
			
		private NavMeshAgent				m_agent;						// Reference to Agent 
		private bool						m_pathSet = false;				// Path been set 
		private float						m_stoppingDistance = 2.0f;      // Is close to desitnation 
		/***************************************************
		*   Function        : OnInIt    
		*   Purpose         : Used to setup task for the first time 
		*   Parameters      : N/A   
		*   Returns         : Null   
		*   Date altered    : UK
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get AI reference 
			m_agent = agent.transform.GetComponent<NavMeshAgent>();
			return null;
		}


		/***************************************************
		*   Function        : OnUpdate
		*   Purpose         : Used to update the task
		*   Parameters      : N/A   
		*   Returns         : Void    
		*   Date altered    : UK
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnUpdate()
		{
			// Has path been set 
			if( m_pathSet == false )
            {
				
				SetPath();
            }
            else
            {
				// Check the status of the path 
				CheckPathStatus();
            }

			
        }

		/***************************************************
		*   Function        : OnStop
		*   Purpose         : reset vars 
		*   Parameters      : N/A   
		*   Returns         : Void    
		*   Date altered    : UK
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{
			// Reset
			m_pathSet = false;
		}
		/***************************************************
		*   Function        : SetPath
		*   Purpose         : Used to set the path to the last known position of the player 
		*   Parameters      : N/A   
		*   Returns         : Void    
		*   Date altered    : 04/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private void SetPath()
        {
			// If a search position been set 
			if ( blackboard.GetVariable<Vector3>( "b_searchPosition" ).GetValue() != null )
			{
				// Set desitnation 
				m_agent.SetDestination( blackboard.GetVariable<Vector3>( "b_searchPosition" ).GetValue() );
				m_pathSet = true;
			}
			else
			{
				EndAction( false );
			}
		}
		/***************************************************
		*   Function        : CheckPathStatus
		*   Purpose         : check if path been complete 
		*   Parameters      : N/A   
		*   Returns         : Void    
		*   Date altered    : 04/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private void CheckPathStatus()
        {
			// If path been set 
			if ( m_pathSet )
			{
				// If reached desination 
				if ( m_agent.remainingDistance <= m_stoppingDistance )
				{
					EndAction( true );
				}

				// If location no longer viabale 
				if ( m_agent.pathStatus == NavMeshPathStatus.PathInvalid )
				{
					EndAction( false );
				}
			}
		}
	
	}
}