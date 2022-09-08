using NodeCanvas.Framework;
using ParadoxNotion.Design;
using LayerMasks;
using UnityEngine;
using UnityEngine.AI;
namespace NodeCanvas.Tasks.Actions
{
	[Category( "Custom Detection task" )]
	[Description(" Find an enviromental hazard near player")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: DetectEnviromentInteractble
	*
	* Author: Joseph Gilmore
	*
	* Purpose: To detect enviromental objects that could effect the player 
	* 
	* Functions:		protected override string OnInit()
	*					protected override void OnExecute()
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
	* 10/06/2022	JG			 1.00		- Created a basic concept of the class
	* 26/06/2022    JG		     1.01		- Updated to work with new target system
	* 19/07/2022    JG			 1.02		- Update refence to fov 
	* 15/08/2022    JG			 1.03		- cleaning 
	****************************************************************************************************/
	public class DetectEnivromentInteractble : ActionTask
	{
		private FieldOfView		m_fieldOfViewEnvironment;  // Detect Enviroment in FOV
		private FieldOfView		m_fieldOfViewPlayer;       // Reference a field of view for AI
		private float			m_playerDistance = 5.0f;   // Distance how close the player needs to be to the hazard 
		private NavMeshAgent	m_agent;
		/***************************************************
		*   Function        : OnInit   
		*   Purpose         : On first call of action set action up   
		*   Parameters      : N/A 
		*   Returns         : null    
		*   Date altered    : 19/07/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get agent reference
			m_agent = agent.GetComponent<NavMeshAgent>();	

			// Setup fov for enviroment 
			m_fieldOfViewEnvironment = new FieldOfView(m_agent.transform, m_agent.GetComponent<AIData>().GetFOVRange(),Layers.EnviromentInteractable);

			// Get fov from AI
			m_fieldOfViewPlayer   = m_agent.GetComponent<AIData>().GetFieldOfView();
			return null;
		}

		/***************************************************
		 *   Function        : OnExecute   
		 *   Purpose         : Run code on task execution. Check fov for player and enviroment. 
		 *   Parameters      : N/A   
		 *   Returns         : Void    
		 *   Date altered    : 26/06/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
			// Start checking FOV for valid targets 
			StartCoroutine(m_fieldOfViewEnvironment.CheckFOV( m_agent.transform ));
			StartCoroutine(m_fieldOfViewPlayer.CheckFOV( m_agent.transform ));

			// If player and a enviroment interactble is in view
			if ( m_fieldOfViewEnvironment.GetTargetInFOV() == true && m_fieldOfViewPlayer.GetTargetInFOV() == true )
			{
				// Check the distance between them to see if in range.
				if (Vector3.Distance( m_fieldOfViewEnvironment.GetTarget().position, m_fieldOfViewPlayer.GetTarget().position) < m_playerDistance )
				{
					// Look at prop 
					m_agent.transform.LookAt( m_fieldOfViewEnvironment.GetTarget().position );

					// Set target to enviroment prop
					blackboard.SetVariableValue( "b_target", m_fieldOfViewEnvironment.GetTarget().gameObject );

					EndAction( true );
				}
			}
			EndAction( false );
		}

	
	}
}