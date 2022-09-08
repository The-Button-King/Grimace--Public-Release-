using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
namespace NodeCanvas.Tasks.Conditions
{

	 [Category("Custom Detection")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: ShotByPlayer
	*
	* Author: JG
	*
	* Purpose: When the player shoots in the radius of the AI 
	* 
	* Functions:			protected override string OnInit()
	*						protected override bool OnCheck()
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
	* 06/06/2022    JG			 1.00		 - Created the class 
	* 01/08/2022    JG		     1.01		 - Updated
	* 15/08/2022    JG		     1.02		 - Cleaning 
	****************************************************************************************************/
	public class ShotByPlayer : ConditionTask
	{
		private GameObject  m_player;									// Reference to player 
		private  LayerMask	m_playerLayer;								// Reference to player layer 
		private float		m_playerSoundDectectionRadius = 50.0f;      // Size of player detection radius 	
		private int			m_gunEnum = 2;								// Used to get gun enum	
		/***************************************************
		 *   Function        : OnInit    
		 *   Purpose         : Setup task and get references   
		 *   Parameters      : N/A   
		 *   Returns         : void   
		 *   Date altered    : UK
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override string OnInit()
		{
			// Get Reference to player 
			m_player = GameObject.FindGameObjectWithTag( "Player" );

			// Set up layers 
			m_playerLayer = LayerMask.GetMask( Layers.Player.ToString() );
			return null;
		}
		/***************************************************
		 *   Function        : OnCheck   
		 *   Purpose         : Check the condition of the task    
		 *   Parameters      : N/A   
		 *   Returns         : bool condition 
		 *   Date altered    : 15/08/2022 
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override bool OnCheck()
		{
			// Check if play can shoot 
			if( m_player.GetComponentInChildren<GunStats>() != null )
            {

				// Get current player gun and see if its shooting 
				if ( m_player.GetComponentInChildren<GunManager>().GetCurrentGun().GetState().GetHashCode() == m_gunEnum ) 
				{
					// Creat an array of colliders (only need one as just looking for the player
					Collider[] hitColliders = new Collider[1];

					// Check if the player is within a radius (OverlapSphereNonAlloc is better for perfemace than sphere raycast) 
					int numColliders = Physics.OverlapSphereNonAlloc( agent.transform.position,m_playerSoundDectectionRadius, hitColliders,m_playerLayer );

					if( numColliders != 0 )
					{ 
						// If player in radius 
						if( hitColliders[0].transform.tag == "Player" )
						{
							// Update search position in blackboard vars 
							blackboard.SetVariableValue( "b_searchPosition",hitColliders[0].transform.position );

							return true;
						}
					}
				}
           
            }
            return false;
		}
	}
}