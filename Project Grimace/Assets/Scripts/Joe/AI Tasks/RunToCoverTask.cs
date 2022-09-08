using LayerMasks;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;
using ParadoxNotion.Design;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{

	[Category( "Custom Desitnation Task" )]
	[Description( "AI to run to nearist cover " )]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: RunToCoverTask
	*
	* Author: Joseph Gilmore
	*
	* Purpose: Get The AI to find suitable cover and run towards it 
	* 
	* Functions:		protected override string OnInit()
	*					private void HandleGainSight( Transform target )
	*					private void HandleLooseSight()
	*					private IEnumerator Hide( Transform target )
	*					protected override void OnExecute()
	*					protected override void OnUpdate()
	*					protected override void OnStop()
	*					private int ColliderArraySortComparer( Collider a, Collider b )
	*					ResetCoverData()
	*					private void UpdateBlackboardVars( Vector3 coverPosition, Vector3 searchPosition, GameObject cover )
	*					private void SetUpCover( Collider collider )
	*					private bool CoverPositionSetUp( Vector3 desitnation )
	*					
	* 
	* References: Inspiration from Reference : www.youtube.com. (n.d.). Make a NavMeshAgent Hide or Take Cover From Other Objects | AI Series Part 29 | Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=t9e2XBQY4Og [Accessed 25 Apr. 2022].
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 24/04/2022    JG			1.00		- Created the class following a tutorial 
	* 25/04/2022    JG			1.01		- Added Field of view but getting inemurator errors 
	* 24/06/2022    JG			2.00        - Bug fixing
	* 26/06/2022    JG			2.01		- Mental breakdown
	* 12/07/2022    JG			2.02		- Started bug fixing / reworking 
	* 17/07/2022	JG			2.03		- Continued rework with new system needs cleaning 
	* 18/07/2022    JG			2.04		- Cleaned, functioned and added defensive meaures 
	* 13/08/2022    JG			2.05		- Build bugs 
	* 15/08/2022    JG		    2.06		- Cleaning 
	****************************************************************************************************/

	public class RunToCoverTask : ActionTask
	{
		private LayerMask		m_hidableLayers;							// Layers that haev hidable cover 
		private NavMeshAgent	m_agent;									// Rereference to the agetnt
		private float			m_hideSensitivity = 0.0f;					// How senesirive the AI is to finding cover.
		private Coroutine		m_movemebtCorutine;							// Stores corutine of moving thr Ai
		private Coroutine		m_checkFOV;									// Rountine to check fov 
		private Collider[]		m_colliders = new Collider[ 10 ];			// More is worse performance but better accuracy 
		private float			m_minObstacleHeight = 1.25f;				// The minimal height the obstacle can be to be considered hideable 
		public float			m_updateRate = 0.01f;						// The rate it checks if it needs to run to cover.
		private FieldOfView		m_fieldOfView;								// Rereference to the field of vision class 
		private bool			m_targetfound = false;						// Has a target been found 
		private Vector3			m_targetPosition = new Vector3();			// Position of cover point to run towards 
		private float			m_coverRadius = 10.0f;						// Radius distanc where AI can find cover
		private float			m_minTargetDistance = 1.0f;					// The amount of distance you can be to the close to target	
		private float			m_stoppingDistance = 1.1f;					// Stoping distance to target destination
		private Cover			m_cover;                          			// Reference to selected cover 
		private bool			m_pathSet = false;							// Has the path been set 
		private float			m_maxCoverPositionMutipler = 3.0f;			// Used to check navmesh position of cover 
		private float			m_averageCoverThickness = 2.0f;             // Used as a mutipler to check cover size 
		private float			m_coverFOV = 360.0f;                        // Use a larger range of fov for cover
		private float			m_fovRange = 10.0f;                         // Custom fov range for this task  
		private float			m_storeRange;                               // Store FOV range
		private const  float 	k_taskTimeFailSafe = 60.0f;					// If task has gone over a min kill it	
		/***************************************************
        *   Function        : OnInit
        *   Purpose         : Called on creation of the task to set it up   
        *   Parameters      : N/A   
        *   Returns         : null  
        *   Date altered    : 29/07/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override string OnInit()
		{
			// Create a reference to fieldOfView
			m_fieldOfView = agent.GetComponent<AIData>().GetFieldOfView();
			m_fieldOfView.SetFOV(m_coverFOV);

			// Set radius to be shorter 
			m_storeRange = m_fieldOfView.GetRadius();
			m_fieldOfView.SetRadius( m_fovRange ); 

			// Get the navmesg agent via the tree 
			m_agent = agent.GetComponent<NavMeshAgent>();

			// Get correct cover layers 
			m_hidableLayers = LayerMask.GetMask( Layers.Cover.ToString() );

			return null;
		}
		/***************************************************
        *   Function        : HandleGainSight   
        *   Purpose         : Used for when the AI sees the target its hiding fro,
        *   Parameters      : Transform target   
        *   Returns         : Void
        *   Date altered    : 17/07/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		private void HandleGainSight( Transform target )
		{
			// Return the cover point if no longer being used 
			if ( m_targetPosition != Vector3.zero && m_cover != null )
			{
				m_cover.ReturnCoverPoint( m_targetPosition );
			}

			// If player regains sight restart coruntine 
			if ( m_movemebtCorutine != null )
			{
				StopCoroutine( m_movemebtCorutine );
			}

			// Start hiding 
			m_movemebtCorutine = StartCoroutine( Hide( target ) );
		}
		/***************************************************
        *   Function        : HandleLoosesSight  
        *   Purpose         : When the AI looses sight of its target  
        *   Parameters      : N/A  
        *   Returns         : Void 
        *   Date altered    : 24/06/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		private void HandleLooseSight()
		{
			if ( m_movemebtCorutine != null )
			{
				// Player lost sight stop corutine
				StopCoroutine( m_movemebtCorutine );

			}
		}
		/***************************************************
        *   IEnumrator      : Hide   
        *   Purpose         : To locate cover & run towards it  
        *   Parameters      : Transform Target  
        *   Returns         : yield return wait    
        *   Date altered    : 13/08/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		private IEnumerator Hide( Transform target )
		{

			while ( true )
			{
				// Reset data since last run
				ResetCoverData();

				// If AI Dies mid run
				if(m_agent == null )
                {
					yield break;
                }

				// Get the all the colliders in the line of sight radius on the cover layer only 
				int hits = Physics.OverlapSphereNonAlloc( m_agent.transform.position, m_coverRadius, m_colliders, m_hidableLayers );

				// Used to remove the amount of potetianl cover spots 
				int hitReduction = 0;

				for ( int i = 0; i < hits; i++ )
				{
					// If cover is too close to player or the cover height is too low its not suitable so remove it. 
					if ( Vector3.Distance( m_colliders[ i ].transform.position, target.position ) < m_minTargetDistance || m_colliders[ i ].bounds.size.y < m_minObstacleHeight )
					{
						// Set collider to null as not vaild 
						m_colliders[ i ] = null;

						hitReduction++;
					}

				}

				// Correct Hit count index  to reflex suitable AI
				hits -= hitReduction;

				// Arrange the colliders in a better order 
				System.Array.Sort( m_colliders, ColliderArraySortComparer );

				// Loop through the hit colliders 
				for ( int i = 0; i < hits; i++ )
				{
					if ( m_colliders[ i ] != null )
					{
						// Check first side of cover if vaild 
						if ( NavMesh.SamplePosition( m_colliders[ i ].transform.position, out NavMeshHit hit, m_agent.height * m_maxCoverPositionMutipler, m_agent.areaMask ) )
						{
							// Overwrite hit to a closest edge so we can return a normal as part of the doc product.
							if ( !NavMesh.FindClosestEdge( hit.position, out hit, m_agent.areaMask ) )
							{
								// Defensive check 
								Debug.LogError( $"Unable to find edge close to {hit.position}" );
							}

							// If the doc product is less than the hide sensitivity cover is found 
							if ( Vector3.Dot( hit.normal, ( target.position - hit.position ).normalized ) < m_hideSensitivity )
							{

								// Setup cover 
								SetUpCover( m_colliders[ i ] );

								// Return vaild cover point 
								Vector3 desitnation = m_cover.ReturnBestCoverPos( hit.position );

								// Check if cover position is vaild then set it up
								if ( CoverPositionSetUp( desitnation ) )
								{
									UpdateBlackboardVars( hit.position, target.position, m_cover.gameObject );
									break;
								}


							}
							else
							{


								// Since the last spot was not facing away from the player try the oppersite side of cover
								if ( NavMesh.SamplePosition( m_colliders[ i ].transform.position - ( target.position - hit.position ).normalized * m_averageCoverThickness, out NavMeshHit hit2, m_agent.height * m_maxCoverPositionMutipler, m_agent.areaMask ) )
								{
									if ( !NavMesh.FindClosestEdge( hit2.position, out hit2, m_agent.areaMask ) )
									{
										Debug.LogError( $"Unable to find edge close to {hit2.position} secound attempt" );
										EndAction( false );
									}

									// If position is less than hide sensitivity 
									if ( Vector3.Dot( hit2.normal, ( target.position - hit2.position ).normalized ) < m_hideSensitivity )
									{
										// Setup cover 
										SetUpCover( m_colliders[ i ] );
										
										Vector3 desitnation = m_cover.ReturnBestCoverPos( hit2.position );

										// Check if cover position is vaild and set up
                                        if ( CoverPositionSetUp( desitnation ) )
                                        {
											// Update BT Vars
											UpdateBlackboardVars( hit2.position, target.position, m_cover.gameObject );
											break;
										}
										
									}

								}
							}
						}
						else
						{
							// Not found anything 
							Debug.LogError( $"Unable to find navMesh near object {m_colliders[ i ].name} at {m_colliders[ i ].transform.position}" );
							Debug.DrawLine( m_colliders[ i ].transform.position, Vector3.up );

						}

					}
				}
				// If no Cover end task
				if ( m_targetPosition == Vector3.zero )
				{
					EndAction( false );
				}
				yield return null;
			}

		}

		/***************************************************
        *   Function        : OnExecute   
        *   Purpose         : Check Fov to see if player is in view when task is exexuted   
        *   Parameters      : N/A   
        *   Returns         : void   
        *   Date altered    : 25/04/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override void OnExecute()
		{
			// Reset bools
			m_pathSet = false;
			m_targetfound = false;
			m_agent.isStopped = false;

			// If corutine already running 
			if ( m_checkFOV != null )
			{
				// Stop 
				StopCoroutine( m_checkFOV );
			}

			// Start checking agents fov. 
			m_checkFOV = StartCoroutine( m_fieldOfView.CheckFOV( agent.transform ) );

		}
		/***************************************************
        *   Function        : OnUpdate  
        *   Purpose         : While the task is happening check state of task constantly  
        *   Parameters      : N/A   
        *   Returns         : Void   
        *   Date altered    : 04/08/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
		protected override void OnUpdate()
		{
			// If target been found 
			if ( m_fieldOfView.GetTargetInFOV() && m_targetfound == false )
			{
				HandleGainSight( m_fieldOfView.GetTarget() );
				m_targetfound = true;
			}
			// If target been lost 
			if ( m_fieldOfView.GetTargetInFOV() == false && m_targetfound == true )
			{
				HandleLooseSight();
				m_targetfound = false;
			}

            if ( m_pathSet )
            {
				// If close enough to cover position
				if ( m_agent.remainingDistance <= m_stoppingDistance + m_agent.radius )
                {
					
					// Make agent look towards target 
					m_agent.transform.LookAt( m_fieldOfView.GetTarget().transform.position );

					EndAction( true );
				}
            }
			
			
			// If Path has been set and the AI can no longer access it end task (defensive) or task has been running way to long (denfensive)
			if ( m_pathSet && agent.GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathInvalid || elapsedTime > k_taskTimeFailSafe  || agent.GetComponent<NavMeshAgent>() == null )
			{
				EndAction( false );
			}

		}

		/***************************************************
        *   Function        : OnStop 
        *   Purpose         : When task stops stop corutine
        *   Parameters      : N/A
        *   Returns         : Void
        *   Date altered    : 29/07/2022
        *   Contributors    : JG
        *   Notes           :
        *   See also        :    
        ******************************************************/
		protected override void OnStop()
		{

			if ( m_movemebtCorutine != null )
			{
				// Player lost sight stop corutine
				StopCoroutine( m_movemebtCorutine );
			}
			if(m_cover != null )
            {
				// Return cover point to cover as no longer in use 
				m_cover.ReturnCoverPoint( m_targetPosition );

            }


			// Reset FOV
			m_fieldOfView.SetFOV( agent.GetComponent<AIData>().GetFOVRange() );
			m_fieldOfView.SetRadius(m_storeRange);

			// ResEt bools 
			m_targetfound = false;
			m_pathSet = false;
		}

		/***************************************************
        *   Function        : ColliderArraySortComparer   
        *   Purpose         : Sort the Array of colliders 
        *   Parameters      : Collider a, Collider b   
        *   Returns         : N/A  
        *   Date altered    : UK
        *   Contributors    : JG
        *   Notes           : Part of header reference    
        *   See also        :    
        ******************************************************/
		private int ColliderArraySortComparer( Collider a, Collider b )
		{
			if ( a == null && b != null )
			{
				return 1;
			}
			else if ( a != null && b == null )
			{
				return -1;
			}
			else if ( a == null && b == null )
			{
				return 0;
			}
			else
			{
				return Vector3.Distance( m_agent.transform.position, a.transform.position ).CompareTo( Vector3.Distance( m_agent.transform.position, b.transform.position ) );
			}
		}
		/***************************************************
        *   Function        : ResetCoverData()
        *   Purpose         : Reset found colliders and last cover used 
        *   Parameters      : N/A
        *   Returns         : Void
        *   Date altered    : 18/07/2022
        *   Contributors    : JG
        *   Notes           :
        *   See also        :    
        ******************************************************/
		private void ResetCoverData()
		{
			// Return last used point (This is just me being extra defensive)
			if ( m_targetPosition != Vector3.zero && m_cover != null )
			{
				m_cover.ReturnCoverPoint( m_targetPosition );
			}


			// Reset all the colliders since last check 
			for ( int i = 0; i < m_colliders.Length; i++ )
			{
				m_colliders[ i ] = null;
			}
		}
		/***************************************************
        *   Function        :  UpdateBlackboardVars
        *   Purpose         : Update BT vars
        *   Parameters      : Vector3 coverPosition, Vector3 searchPosition, GameObject cover
        *   Returns         : Void
        *   Date altered    : 19/07/2022
        *   Contributors    : JG
        *   Notes           :
        *   See also        :    
        ******************************************************/
		private void UpdateBlackboardVars( Vector3 coverPosition, Vector3 searchPosition, GameObject cover )
		{
			// Set blackboard vars for other tasks
			blackboard.SetVariableValue( "b_coverPosition", coverPosition );
			blackboard.SetVariableValue( "b_cover", cover.gameObject );
			blackboard.SetVariableValue( "b_searchPosition", searchPosition );
		}
		/***************************************************
        *   Function        : SetUpCover
        *   Purpose         : Set up cover to be used by AI 
        *   Parameters      : Collider collider
        *   Returns         : Void
        *   Date altered    : 18/07/2022
        *   Contributors    : JG
        *   Notes           :
        *   See also        :    
        ******************************************************/
		private void SetUpCover( Collider collider )
        {
			// Check if cover has scripted attached 
			if ( collider.gameObject.GetComponent<Cover>() == null )
			{
				collider.gameObject.AddComponent<Cover>();

			}

			// Get script references 
			m_cover = collider.gameObject.GetComponent<Cover>();
			m_cover.SetAgent( m_agent );
			m_cover.SetCoverPoints();
		}
		/***************************************************
        *   Function        :  CoverPositionSetUp
        *   Purpose         : Set up AI to go towards cover position 
        *   Parameters      : Vector3 desitnation
        *   Returns         : Void
        *   Date altered    : 18/07/2022
        *   Contributors    : JG
        *   Notes           :
        *   See also        :    
        ******************************************************/
		private bool CoverPositionSetUp( Vector3 desitnation )
		{
			if ( desitnation != Vector3.zero )
			{
				// Update AI destination
				m_agent.SetDestination( desitnation );

				// Set target pos
				m_targetPosition = desitnation;

				// Path has now been set 
				m_pathSet = true;
				return true;
				
			}
			// If path could not be set up 
			else 
			{ 
				return false; 
			}
		}
	}
}