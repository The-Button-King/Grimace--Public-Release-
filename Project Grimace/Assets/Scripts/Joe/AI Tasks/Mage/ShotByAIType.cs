using AITypes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{

	[Category( "Custom Detection task" )]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: ShotByAIType
	 *
	 * Author: Joseph Gilmore 
	 *
	 * Purpose: If shot by a certain AI shoot back until that AI is dead
	 * 
	 * Functions:		protected override void OnEnable()
	 *					protected override bool OnCheck()
	 *					
	 * 
	 * References:
	 * 
	 * See Also:
	 *
	 * Change Log:
	 * Date          Initials    Version     Comments
	 * ----------    --------    -------     ----------------------------------------------
	 * 19/07/2022    JG			 1.00		- Created shell
	 * 10/08/2022    JG		     1.01		- Changed enums
	 * 15/08/2022    JG			 1.02		- Clean
	 ****************************************************************************************************/
	public class ShotByAIType : ConditionTask
	{
		
		private EnemyType m_target = EnemyType.Mage; // AI type to check
		private Mage	  m_mage;                    // Reference to Mage type


		/***************************************************
		 *   Function        : OnEnable    
		 *   Purpose         : When task is enabled   
		 *   Parameters      : N/A   
		 *   Returns         : Void  
		 *   Date altered    : 15/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnEnable()
		{
			// Task only works for mages so check if mage 
			if ( agent.transform.GetComponent<Mage>() != null )
            {
				m_mage = agent.transform.GetComponent<Mage>();
            }
            
		}
		/***************************************************
		 *   Function        : OnCheck   
		 *   Purpose         : Check if mage has been hit 
		 *   Parameters      : N/A   
		 *   Returns         : Void  
		 *   Date altered    : 15/08/2022
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override bool OnCheck()
		{
			// If mage has been hit
			if( m_mage.GetHitByEntitie() != null )
			{
				// If mage has been hit by the correct target
				if( m_mage.GetHitByEntitie().name == m_target.ToString() )
				{
					// Set target to the AI who hit current AI
					blackboard.SetVariableValue( "b_target", m_mage.GetHitByEntitie() );
					return true;
				}

             }
			
			return false;
			
		}
	}
}