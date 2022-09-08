using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
namespace NodeCanvas.Tasks.Conditions{
	// Node Canavs inspector details
	[Category( "Custom Detection task")]
	[Description("detect player if close ")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: AICloseToPlayerTask
	*
	* Author: Joseph
	*
	* Purpose: To detect the player if its very close to AI
	* 
	* Functions:			protected override string OnInit()
	*						protected override bool OnCheck()
	*
	* 
	* References:
	* 
	* See Also: ConeOfVisionTask
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 01/04/2022    JG          1.00        
	* 11/04/2022	JG			1.01		- Added comments & changed layer masks to private 
	* 15/08/2022    JG			1.02		- Cleaning 
	****************************************************************************************************/
	public class AICloseToPlayerTask : ConditionTask
	{
		
		private NavMeshAgent	m_agent;					// A reference to the AI Agent
		[SerializeField] [Range(0.0f,10.0f)][Tooltip("The distance the AI can detect the player when close")]
		private float			m_distanceRadius  = 5.0f;	// Radius Range around AI to detect the player 
		private LayerMask		m_playerLayer;				// Reference to the player layer mask
		private  LayerMask		m_objectLayer;              // Reference to the object layer mask
		/***************************************************
		*   Function        : OnInit
		*   Purpose         : Use for initialization of task. This is called only once in the lifetime of the task.  
		*   Parameters      : N/A   
		*   Returns         : Null   
		*   Date altered    : 11/04/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get reference to the agent of this tree.
			m_agent = agent.GetComponent<NavMeshAgent>();

			// Set layer Masks
			m_playerLayer = LayerMask.GetMask( Layers.Player.ToString() );
			m_objectLayer = LayerMask.GetMask( Layers.Obstacles.ToString() );
			return null;
		}

		/***************************************************
		*   Function        : OnCheck
		*   Purpose         : Called once per frame while the condition is active. check if player is in range of AI
		*   Parameters      : N/A
		*   Returns         : Return whether the condition is success or failure.   
		*   See also        : N/A
		*   Date altered    : 11/04/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin. 
		******************************************************/
		protected override bool OnCheck()
		{
			// Get a list of colliders hit in a sphere around the agent 
			Collider[] player = Physics.OverlapSphere( m_agent.transform.position,  m_distanceRadius, m_playerLayer );
		
			// If a collider has been found 
			if (player.Length != 0 )
            {
				// Calculate the target direction ( I only do this for the first index as it can only find the player due to the player being only object on that layer)
				Vector3 targetDirection =  (player[0].transform.position - m_agent.transform.position).normalized;

				// Calculate the distance between AI & Target
				float distanceToTarget = Vector3.Distance(m_agent.transform.position,player[0].transform.position );

				// Cast a ray and check it does not hit an object(goes through a wall). 
				if(!Physics.Raycast( m_agent.transform.position, targetDirection, distanceToTarget, m_objectLayer ) )
                {
					// If player close enough to AI and not behind a wall return true and look at player.
					m_agent.transform.LookAt( player[0].transform.position );

					return true;

                }
            }
				// Return false if no target been found
				return false;   

		}
	}
}