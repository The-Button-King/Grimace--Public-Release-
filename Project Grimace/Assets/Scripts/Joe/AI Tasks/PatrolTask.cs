using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NodeCanvas.Tasks.Actions{

	// Behaviour Tree details
	[Category("Custom Task")]
	[Description("Tell the player to patrol around set points ")]

	/****************************************************************************************************
	* Type: Class
	* 
	* Name: Patrol Task
	*
	* Author: Joseph
	*
	* Purpose: To make an AI move around a set of points
	* 
	* Functions:	
	*					protected override string OnInit()
	*					protected override void OnExecute()
	*					protected override void OnUpdate()
	* 
	* References: N/A
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 28/03/2022    JG			1.00        -created a simple Patrol system. will need to be improved appon when I have a random room to test/ serval AI
	* 1/04/2022		JG			1.01		- created a simple wait timer
	* 05/04/2022    JG			1.02		-temporaly changed to get waypoints using tags.
	* 14/04/2022	JG			1.10		- Removed randomly generating points as its done in SetUpAI  instead
	* 03/08/2022    JG			1.11		- Adjusted for new AI structure 
	****************************************************************************************************/
	public class PatrolTask : ActionTask
	{
		/***************************************************
        *   Enum			: PatrolMethod
        *   Purpose         : Able to change patrol method between random and set
        *   Date altered    : Unkown
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		public enum PatrolMethod
        {
			Setpoints,
			RandomisedRoute,
        }
		
		[SerializeField]
		public List<Vector3>		m_moveTranform = new List<Vector3>();       // Position to move towards
		private NavMeshAgent        m_agent;									// Reference to AI Agent 
		private Vector3				m_currentWaypoint;							// Current position AI is traveling towards 
		private int                 m_waypointIndex;                            // Currrent waypoint index to travel towards
		[SerializeField]
		[Range(0, 3f)]
		public float				m_minimumWaitTime;							// The minimum amount of time AI can wait at point
		[SerializeField]
		[Range(3f, 8f)]
		public float				m_maxWaitTime;								// Max time the AI can wait at point
		private float				m_waypointWaitTime;							// Time to wait at point
		private float               m_timer;									// Timer used to wait at waypoint 
		private bool                m_timerComplete = true;						// If timer complete 
		[SerializeField]
		public  PatrolMethod        m_patrolMode = PatrolMethod.RandomisedRoute;// Set patrol method 
		private float				m_distanceToPoint = 1.5f;					// Value to check when AI is close to patrol point 

		/***************************************************
        *   Function        : OnInit  
        *   Purpose         : Called on the creation of the task.Used to set up patroling   
        *   Parameters      : N/A   
        *   Returns         : null   
        *   Date altered    : 03/08/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override string OnInit()
		{
			// Get the agent 
			m_agent = agent.GetComponent<NavMeshAgent>();

			// Get a random wait time so AI don't all move at the same time
			m_waypointWaitTime = Random.Range( m_minimumWaitTime, m_maxWaitTime);

            // Set the first WP
            switch ( m_patrolMode )
            {
				case PatrolMethod.Setpoints:
					// Find waypoints in the scene with the tag
					foreach(GameObject GO in GameObject.FindGameObjectsWithTag("waypoint"))
                    {
						// Assign to list 
						m_moveTranform.Add(GO.transform.position);
					}
					// Set first waypoint 
					m_currentWaypoint = m_moveTranform[0];
					break;
				case PatrolMethod.RandomisedRoute:
					// Get randomized path from AI data 
					m_moveTranform = m_agent.transform.GetComponent<AgentAI>().GetPath();
					break;
				default:
					// No patrol method seleted action failed
					EndAction( false );
					break;
			}
            
				

            
			return null;
		}

		/***************************************************
        *   Function        : OnExecute 
        *   Purpose         : When task is executed check if a path has been set    
        *   Parameters      : N/A  
        *   Returns         : void  
        *   Date altered    : Unknown
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override void OnExecute()
		{
			if(m_moveTranform == null )
            {
				// If no points been set 
				EndAction( false );
			}
		}
		
		/***************************************************
        *   Function        : OnUpdate  
        *   Purpose         : When task active move AI along path in set intervals   
        *   Parameters      : N/A   
        *   Returns         : void   
        *   Date altered    : 14/04/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override void OnUpdate()
		{
            // Timer for reaching waypoint 
            if (!m_timerComplete)
            {
                // If timer is less than wait time increase timer
                if (m_timer < m_waypointWaitTime)
                {
                    m_timer += Time.deltaTime;

                    // If timer is more than wait time timer complete
                    if (m_timer > m_waypointWaitTime)
                    {
                        m_timerComplete = true;
                    }
                }
            }
            else
            {
                // Set current waypoint to the current index point from the array
				m_currentWaypoint = m_moveTranform[m_waypointIndex];

            

				// If AI is close to point 
				if ( Vector3.Distance( agent.transform.position, m_currentWaypoint ) < m_distanceToPoint )
				{

					// Reached point and update the index.
					m_waypointIndex = (m_waypointIndex + 1) % m_moveTranform.Count;

					
					// Reset timer so AI waits at waypoint
					m_timerComplete = false;
					m_timer = 0f;
				}
				else
				{
					// Continue moving agent to set waypoint
					m_agent.SetDestination(m_currentWaypoint);
					
				}

            }

			// Action is running 
			EndAction( true );
		}

	}
}