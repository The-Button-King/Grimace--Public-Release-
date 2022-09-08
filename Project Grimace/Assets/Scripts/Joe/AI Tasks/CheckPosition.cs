using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
namespace NodeCanvas.Tasks.Actions
{
	[Category( "Custom Desitnation Task" )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: CheckPosition
	 *
	 * Author: Joseph Gilmore 
	 *
	 * Purpose: Check & move AI relative to other AI
	 * 
	 * Functions:   protected override string OnInit()
	 *				protected override void OnExecute()
	 *				protected override void OnUpdate()
	 *				private void CheckSurrondings()
	 *				private void CheckSegments( )
	 *				private void PickMovementPosition()
	 *				protected override void OnStop()
	 *				private void CheckPathStatus()
	 *				
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
	 * 30/07/2022    JG			 1.00		- Created class
	 * 31/07/2022    JG		     1.01		- Minor bug fixes 
	 * 03/08/2022    JG		     1.02		- Minor cleaning 
	 * 04/08/2022    JG			 1.03		- Bug fixes and improvments
	 * 15/08/2033    JG			 1.04	    - Cleaning 
	 ****************************************************************************************************/
	public class CheckPosition : ActionTask
	{
		private float					m_maxCheck = 10.0f;										// Max range to check surrondings
		private LayerMask				m_layerMasks;											// Layer masks used to check surrondings 	
		private List<(Vector3, int )>	m_positions = new List<(Vector3, int)> ();				// Store the amount of AI in each segement 
		[SerializeField]
		private List<GameObject>		m_detectedAI = new List<GameObject>();					// List of AI detected 
		private float					m_segments = 5.0f;                                      // Amount of segments to break up surronding area into ( more will make it more accurate but more expsenive)
		private Vector2Int				m_maxTravelDistance = new Vector2Int( 1 ,12 );			// Range to travel to 
		private NavMeshPath				m_path;													// Store the AI potential path 
		private Vector3					m_targetPosition;										// Store desination 
		private bool					m_pathSet = false;                                      // Has path been set 
		private const float				k_remaningDistance = 0.5f;								// Stopping distance to desitnation 
		private const float				k_minDistance = 2.0f;									// Min amount of move distance 	
		private const float				k_failSafe = 50.0f;										// If task has gone for too long somthing gone wrong 
		private NavMeshAgent			m_agent;                                                // Reference to agent 
		private const int				k_maxCollider = 5;										// Max amount of colliders used for postion checking 
		/***************************************************
		 *   Function        : OnInit    
		 *   Purpose         : Set up position checking    
		 *   Parameters      : N/A  
		 *   Returns         : null   
		 *   Date altered    : 30/07/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override string OnInit()
		{
			// Set layers to everything that can recvive enviromental hazard damage (player & AI)
			m_layerMasks = LayerMask.GetMask( Layers.EnviromentHazard.ToString() );

			// Create a new navmesh path
			m_path = new NavMeshPath();

			// Get agent reference 
			m_agent = agent.GetComponent<NavMeshAgent>();
			return null;
		}

		/***************************************************
		 *   Function        : OnExecute
		 *   Purpose         : Check surrondings on taks executuion  
		 *   Parameters      : N/A  
		 *   Returns         : Void
		 *   Date altered    : 30/07/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnExecute()
		{
			// Create a new navmesh path
			m_path = new NavMeshPath();

			// Ensure var reset 
			m_pathSet = false;

			// Check surronding 
			CheckSurrondings();
		}

		/***************************************************
		 *   Function        : OnUpdate  
		 *   Purpose         : While the task is active check the status of the AI  
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 06/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnUpdate()
		{
			// Check the status of path 
			CheckPathStatus();
		}

		/***************************************************
		 *   Function        : CheckSurrondings
		 *   Purpose         : Check whats near the AI 
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 30/07/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		private void CheckSurrondings()
		{
			
			// Clear last task data 
			m_positions.Clear();
			m_detectedAI.Clear();

			// Create array for colliders 
			Collider[] hitColliders = new Collider[ k_maxCollider ];

			// Get the amount of colliders detected in sphere in certain layers 
			int colliders = Physics.OverlapSphereNonAlloc( agent.transform.position, m_maxCheck, hitColliders, m_layerMasks );

			for ( int i = 0; i < colliders; i++ )
			{
				// Check if AI has been hit and not itself
				if ( hitColliders[ i ].transform.root.GetComponent<AIData>() != null && hitColliders[ i ].transform.root != m_agent.transform.root )
				{
					// Add to list of detected AI 
					m_detectedAI.Add( hitColliders[ i ].transform.root.gameObject );
				}
			
			}


			// Check where the AI is 
			CheckSegments();
		}
		/***************************************************
		 *   Function        : CheckSegments
		 *   Purpose         : cut sphere into segments and see what AI is in which segment 
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 30/07/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		private void CheckSegments( )
		{
			// Get the angle for each segment 
			float angleBetweenPoints = 360.0f / m_segments;

			Vector3 direction = Vector3.zero;

			// Loop through  each segment 
			for ( int i = 0; i < m_segments; i++ )
			{
				// Get angle for current segment by using the point index 
				float angle = i * angleBetweenPoints * Mathf.Deg2Rad;

				// Work out the direction of the that segment  using the sin & cos of the angle 
			    direction = new Vector3( Mathf.Sin( angle ), 0.0f, Mathf.Cos( angle ) );

			
				// Create a local var for the AI in each segement 
				int segmentAI = 0;

                foreach ( GameObject ai in m_detectedAI )
                {
                    // Get the direction of the target. 
                    Vector3 targetDirection = ( ai.transform.position - agent.transform.position ).normalized;

                    // Check to see if the target detected is in the segment angle 
                    if ( Vector3.Angle( direction, targetDirection ) < angleBetweenPoints * 0.5f )
                    {
                        segmentAI++;
                    }
                }

				// Update position list wit the segment direction and how many AI in that segment 
                ( Vector3, int ) tempTuple = new( direction, segmentAI );
                m_positions.Add( tempTuple );
            }
			
			// Pick best segment for AI to move towards 
			PickMovementPosition();


		}
		/***************************************************
		 *   Function        : PickMovementPosition()
		 *   Purpose         : Pick the segement with the least amount of AI to move towards 
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 31/07/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		private void PickMovementPosition()
        {
			// Sort the the segements in order of acsending order to how many AI are in it
			m_positions.Sort( ( a, b ) => a.Item2.CompareTo( b.Item2 ) );
			
			//  Loop through the segments 
			for(int p = 0; p < m_positions.Count; p++ )
            {
				// Get the segement direction 
				Vector3 direction = m_positions[ p ].Item1;

				// If this direction is too close to wall (AI were gravitating towards the walls to much)
				if(Physics.Raycast(agent.transform.position, direction, k_minDistance, LayerMask.GetMask( Layers.Wall.ToString() ) ) )
                {
					break;
                }

				// Get a random range  of travel
				int randomRange = Random.Range( m_maxTravelDistance.x, m_maxTravelDistance.y );

				for ( int i = 0; i < randomRange; i++ )
				{
					// Check the position of the segment direction and distance reqruied to travel.
					Vector3 checkPos = agent.transform.position + direction * i;

					// Check if agent can access that point 
					bool m_pathVaild = NavMesh.CalculatePath( agent.transform.position, checkPos, agent.GetComponent<NavMeshAgent>().areaMask, m_path );


					// If path failed 
					if ( m_path.status != NavMeshPathStatus.PathComplete )
					{
						// if it was first loop break and check next segement as segement direction does not create pat 
						if(i == 0 )
                        {
							break;
                        }

						// Use last checked position for movement 
						m_targetPosition = agent.transform.position + (direction * ( i - 1.0f ));

						// If it moves a min amount 
						if(Vector3.Distance(m_targetPosition,agent.transform.position) > k_minDistance )
                        {
							// Set desitnation of AI 
							m_agent.SetDestination( m_targetPosition );

							// Path now set 
							m_pathSet = true;

							// Exit function as no longer needed 
							return;
						}
						

					}

					// If last point and not broken out 
					if(i == randomRange - 1 )
                    {
                        // Set desitnation to last checked position as vibasble ???
                        m_targetPosition = agent.transform.position + (direction * m_positions.Count);

						// Set desitnation to target 
                        m_agent.SetDestination( m_targetPosition );
                        m_pathSet = true;
                    }
                    Debug.DrawRay( checkPos, Vector3.up, color: Color.blue );
				}

				
			}
			Debug.Log( "failed" );

			// No positions free 
			EndAction(false);
		}

		/***************************************************
		 *   Function        : OnStop
		 *   Purpose         : Reset vars
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 04/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnStop()
		{
			m_path = null;
			m_pathSet=false;
			
		}
		/***************************************************
		 *   Function        : CheckPathStatus
		 *   Purpose         : Check the status of the AI on the path
		 *   Parameters      : N/A  
		 *   Returns         : void   
		 *   Date altered    : 15/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		private void CheckPathStatus()
        {
			// Has the AI path been set
			if ( m_pathSet && m_agent != null )
			{
				// If path complete and has reached destination end task 
				if ( m_agent.remainingDistance < k_remaningDistance )
				{
					EndAction( true );
				}
			}

			// If the path becomes invaild or the task is run for too long end it 
			else if ( m_pathSet && m_agent.pathStatus != NavMeshPathStatus.PathComplete || elapsedTime > k_failSafe )
			{
				EndAction( false );

			}
		}

	}
}
