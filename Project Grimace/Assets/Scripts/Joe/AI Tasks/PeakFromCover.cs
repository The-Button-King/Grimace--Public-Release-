using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{

	[Category( "Custom Desitnation Task" )]
	[Description( "Peak from assigned cover " )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: PeakFromCover
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: Get the AI to shoot at player from cover then return 
	 * 
	 * Functions:		
	 *					protected override string OnInit() 
	 *					protected override void OnExecute()
	 *					protected override void OnUpdate()
	 *					private IEnumerator Peak()
	 *					protected override void OnStop()
	 *					rivate void CheckPath( Vector3 direction )
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
	 * 24/06/2022    JG		     1.00		- Got the AI to peak from marked cover 
	 * 12/07/2022    JG		     2.00	    - Started reworking entire script 
	 * 13/07/2022    JG		     2.01		- Improved peaking by restructuring cover data to Cover.cs
	 * 14/07/2022    JG		     2.02		- Messed with rotation of agent while doing task 
	 * 17/07/2022    JG		     2.03		- Added a check to see if AI path is vaild 
	 * 04/08/2022    JG		     2.04		- Improved defensiveness 
	 * 15/08/2022    JG		     2.08		- Cleaning 
	 ****************************************************************************************************/
	public class PeakFromCover : ActionTask
	{
		private GameObject		m_cover;										// Reference to marked cover 
		private Vector3			m_targetLastPos;								// The target hiding from last known position
		private float			m_rotationAngle = 3.0f;							// Angle of rotation used to peak at the right angle
		private Cover			m_coverInfo;									// Reference to script attached to cover
		private Vector3			m_peakPos;										// Current position to peak from 
		private float			m_angularSpeed;									// Stores AI angluar speed
		private Vector2			m_peakTimerRange = new Vector2( 1.0f, 4.0f );   // Timer range for peaking 
		private float			m_stopingDistance = 0.8f;                       // Distance to stop peaking 
		private float			m_rotationSpeed = 2.0f;                         // Rotation speed of AI 
		private bool			m_pathSet = false;                              // Has path been sey
		private const float		k_failSafe = 60.0f;								// If task has executed for too long 
		private NavMeshAgent	m_agent;                                        // Reference to Agent 
		/***************************************************
		*   Function        : OnInit 
		*   Purpose         : Setup task for first execution   
		*   Parameters      : N/A   
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           : Overrides bt task   
		*   See also        :    
		******************************************************/
		protected override string OnInit() 
		{
			// Set agent 
			m_agent = agent.GetComponent<NavMeshAgent>();

			return null;
		}
		/***************************************************
		*   Function        : OnExecute   
		*   Purpose         : Get updated blackboard vars on task execution   
		*   Parameters      : N/A   
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           : Overrides bt task   
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
			// Get vars blackboard 
			m_cover = blackboard.GetVariable<GameObject>( "b_cover" ).value;
			m_targetLastPos = blackboard.GetVariable<Vector3>( "b_searchPosition" ).value;
			
			// If cover does not have script attacthed 
			if(m_cover.GetComponent<Cover>() == null )
            {
				// Add script to cover
				m_cover.gameObject.AddComponent<Cover>();
            }

			// Give information to cover
			m_coverInfo = m_cover.GetComponent<Cover>();
			m_coverInfo.SetAgent( m_agent );
			m_coverInfo.SetPeakPositions( m_targetLastPos );

			// Get a peak position from the assigned cover 
			m_peakPos = m_coverInfo.GetPeakPoint();

			// Get angular speed of AI
			m_angularSpeed = m_agent.angularSpeed;

			// Set to 0 to take control of rotation
			m_agent.angularSpeed = 0;

		
			// Peak
			StartCoroutine( Peak() );
		}

		/***************************************************
		*   Function        : OnUpdate   
		*   Purpose         : update when the task is running  
		*   Parameters      : N/A   
		*   Returns         : Void   
		*   Date altered    : 15/08/2022
		*   Contributors    : JG
		*   Notes           : Overrides bt task   
		*   See also        :    
		******************************************************/
		protected override void OnUpdate()
		{
			
			// Get the direction from the AI to the last know position 
			Vector3 direction = m_targetLastPos - agent.transform.position;

			// Rotate AI towards last know pos 
			m_agent.transform.rotation = Quaternion.LookRotation( Vector3.RotateTowards( m_agent.transform.forward, direction, m_rotationSpeed * Time.deltaTime, 0 ), Vector3.up );

			// Check status of path
			CheckPath( direction );
			
		}


		/***************************************************
		*   IEnumerator     : Peak 
		*   Purpose         : Peak from selected cover 
		*   Parameters      : N/A   
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           :  
		*   See also        :    
		******************************************************/
		private IEnumerator Peak()
        {
			// If no peak position assigned 
			if ( m_peakPos == Vector3.zero )
			{
			   // Fail task 
			   EndAction( false );
			}
				
			// Set peak timer 
			float peak = Random.Range(m_peakTimerRange.x ,m_peakTimerRange.y);

			WaitForSeconds hideTime = new WaitForSeconds(peak);
			yield return hideTime;

			// Check if still alive 
			if( m_agent != null )
            {
				// Set peak pos
				m_agent.SetDestination(m_peakPos);

            }
            else
            {
				EndAction( false );
            }

			// Path has now been set
			m_pathSet = true;

			yield return null;
		}


		/***************************************************
		*   Function        : OnStop   
		*   Purpose         : When task stops make used cover pos avaiabile again
		*   Parameters      : N/A   
		*   Returns         : Void   
		*   Date altered    : 04/08/2022
		*   Contributors    : JG
		*   Notes           :  
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{
			// Reset angluar speed
			m_agent.angularSpeed = m_angularSpeed;

			// Return peak position to cover 
			m_coverInfo.ReturnPeakPoint( m_peakPos );

			// No Path 
			m_pathSet= false;
			
		}
		/***************************************************
		*   Function        : CheckPath 
		*   Purpose         : Check status of the path
		*   Parameters      : Vector3 direction
		*   Returns         : Void   
		*   Date altered    : 15/08/2022
		*   Contributors    : JG
		*   Notes           :  
		*   See also        :    
		******************************************************/
		private void CheckPath( Vector3 direction )
        {
			if ( m_pathSet )
			{
				// If close to peak pos
				if ( m_agent.remainingDistance <= m_agent.radius + m_stopingDistance )
				{
					// Stop AI 
					m_agent.isStopped = true;

					Debug.Log( "Got to peak position" );

					// Check if AI rotated for final peak position 
					if ( Vector3.Angle( direction, m_agent.transform.forward ) <= m_rotationAngle )
					{
						EndAction( true );
					}
				}
			}
			// Has path been set and is not vaild or task has run for a really long time 
			if ( m_pathSet && m_agent.pathStatus != NavMeshPathStatus.PathComplete || elapsedTime > k_failSafe )
			{
				EndAction( false );
			}
		}
	}
}