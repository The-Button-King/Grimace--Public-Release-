using NodeCanvas.Framework;
using LayerMasks;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category( "Custom Detection task" )]
	[Description( "Look around on the spot for target " )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: LookAroundTask
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: 
	 * 
	 * Functions:			protected override string OnInit()
	 *						protected override void OnExecute()
	 *						protected override void OnUpdate()
	 *						private IEnumerator RotateAgent()
	 *						protected override void OnStop()
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
	 * 14/07/2022   JG		     1.00		- Created class and fiddled with rotating active m_agents 
	 * 15/07/2022   JG			 1.01		- Found a solution to rotating an m_agent 
	 * 19/07/2022   JG			 1.02		- Update reference to fov 
	 * 31/07/2022   JG			 1.03		- Added enviroment hazard option 
	 * 01/08/2022   JG			 1.04		- Reference on dead AI bug
	 * 15/08/2022   JG			 1.05		- Cleaning
	 ****************************************************************************************************/
	public class LookAroundTask : ActionTask
	{
		private FieldOfView					m_fieldOfView;				// Reference to FOV
		private float						m_angularSpeed;				// Store m_agent speed
		private NavMeshAgent				m_agent;					// Reference to agent 
		private Vector3						m_startRotation;			// Rotation of m_agent when task starts
		private bool						m_rotatedRight = false;		// If rotated right 
		private float						m_rotationSpeed = 1.04f;	// Rotation speed
		private Coroutine					m_corutine;                 // Store corutine 
		[SerializeField]
		public float						m_rightRotation = 90.0f;    // Right rotation angle 
		public bool							m_lookForHazard = false;    // Is looking for a hazard 
		public float						m_leftRotation = 90.0f;     // Left rotation angle 
		/***************************************************
		*   Function        : OnInit   
		*   Purpose         : Setup task on first run  
		*   Parameters      : N/A 
		*   Returns         : Void   
		*   Date altered    : 31/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get agent 
			m_agent = agent.GetComponent<NavMeshAgent>();

			// Set up fov depending if  looking for hazard 
            if ( m_lookForHazard )
            {
				m_fieldOfView = new FieldOfView( agent.transform, 360.0f, Layers.EnviromentInteractable );
			}
            else
            {
				// Get a reference to fieldOfView
				m_fieldOfView = agent.GetComponent<AIData>().GetFieldOfView();
			}
			

			// Store angular speed
			m_angularSpeed = m_agent.angularSpeed;
			

			return null;
		}

		/***************************************************
		*   Function        : OnExecute   
		*   Purpose         : Setup task for execution
		*   Parameters      : N/A 
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
			// Remove agent angular speed(This is to remove potential bugs of the agent rotating properly)
			m_agent.angularSpeed = 0;
			
			// Stop AI from moving
			m_agent.isStopped = true;

			// Not rotated right yet
			m_rotatedRight = false;

			// Store AI rotation
			m_startRotation = m_agent.transform.eulerAngles;

			StartCoroutine( m_fieldOfView.CheckFOV( m_agent.transform ) );

			// Start rotating AI and checking fov 
			m_corutine = StartCoroutine( RotateAgent() );

		}

		/***************************************************
		*   Function        : OnUpdate  
		*   Purpose         : Check if Player has walked into fov
		*   Parameters      : N/A 
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnUpdate()
		{
			// End task if player has been spotted
            if ( m_fieldOfView.OnCheckVision( m_agent.transform ) )
            {
				StopCoroutine( m_corutine );
				EndAction( true );
            }
			
		}
		/***************************************************
		*   Function        : RotateAgent   
		*   Purpose         : Rotate the agent right and left in given angles 
		*   Parameters      : N/A 
		*   Returns         : Void   
		*   Date altered    : 15/07/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
	******************************************************/
		private IEnumerator RotateAgent()
        {
            while ( true )
            {
				// If not completed right rotation
				if(m_rotatedRight == false )
                {
					// Rotate AI towards the right (navmesh agents rotation  is funky , need to use look at rotation)
					Quaternion lookRotation = Quaternion.LookRotation( Vector3.RotateTowards( m_agent.transform.forward, m_agent.transform.right, m_rotationSpeed * Time.deltaTime, 0 ), Vector3.up );
					m_agent.transform.rotation = lookRotation;

					float target = m_startRotation.y + m_rightRotation;

					// Keep within 360 degrees
					if ( target > 360.0f )
					{
						target = 359.0f;
					}

					// If AI has completed right rotatation
					if ( m_agent.transform.eulerAngles.y >= target)
					{
						// Now can rotate left 
						m_rotatedRight=true;
					}
                }
                else
                {
					// Rotate AI left
					Quaternion lookRotation = Quaternion.LookRotation( Vector3.RotateTowards( m_agent.transform.forward, -m_agent.transform.right, m_rotationSpeed * Time.deltaTime, 0 ), Vector3.up );
					m_agent.transform.rotation = lookRotation;

					float target = m_startRotation.y -m_leftRotation;

					// Keep within 360 degrees
					if(target < 0.0f )
                    {
						target = 1f;
                    }

					// If AI has rotated leftn from starting rotation
					if ( m_agent.transform.eulerAngles.y <= target )
					{
				
						EndAction(false);
						break;
					}
				}
				
				yield return null;
			}
		
        }
		/***************************************************
		*   Function        :  OnStop  
		*   Purpose         :  Reset values of class when task completed   
		*   Parameters      :  N/A  
		*   Returns         :  Void   
		*   Date altered    :  15/08/2022
		*   Contributors    :  JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnStop()
		{
			// Reset agent 
			if( m_agent.isActiveAndEnabled == true )
            {
				m_agent.angularSpeed = m_angularSpeed;
				m_agent.isStopped = false;

            }

			// Ensure corutine has stopped
			StopCoroutine( m_corutine );

			// Reset var
			m_rotatedRight = false;

		}

	}
}