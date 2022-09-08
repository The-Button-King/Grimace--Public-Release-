using EnemyEnums;
using LayerMasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
namespace NodeCanvas.Tasks.Conditions{

	[Category( "Custom Detection task" )]
	[Description("If an AI is close to a different AI type")]
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: AICloseToDifferentAIType
	 *
	 * Author: Joseph Gilmore 
	 *
	 * Purpose: Detect if different AI are close 
	 * 
	 * Functions:		protected override string OnInit()
	 *					protected override bool OnCheck()
	 * 
	 * References:
	 * 
	 * See Also:
	 *
	 * Change Log:
	 * Date          Initials    Version     Comments
	 * ----------    --------    -------     ----------------------------------------------
	 * Uk		     JG		     1.00		 - Created 
	 ****************************************************************************************************/
	public class AICloseToDifferentAIType : ConditionTask
	{

		private EnemyTypes	m_targetEnemy;				// Which AI to detect 
		private LayerMask	m_layerToCheck;				// Layer of AI
		private float		m_range = 10f;              // Range of detection
		/***************************************************
		 *   Function        :  OnInit  
		 *   Purpose         :  Setup task   
		 *   Parameters      :  N/A  
		 *   Returns         :  nulll   
		 *   Date altered    :  Uk
		 *   Contributors    :  JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override string OnInit()
		{
			// Get rereferences 
			m_layerToCheck = LayerMask.GetMask( Layers.AI.ToString() );
			m_targetEnemy = EnemyTypes.Turret;
			return null;
		}

		/***************************************************
		 *   Function        :  OnCheck
		 *   Purpose         :  Check the condition of the task   
		 *   Parameters      :  N/A  
		 *   Returns         :  nulll   
		 *   Date altered    :  Uk
		 *   Contributors    :  JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override bool OnCheck()
		{
			// Check AI near by
			Collider[] AI = Physics.OverlapSphere(agent.transform.position, m_range, m_layerToCheck);


			for(int i = 0; i < AI.Length; i++)
            {
				// If the correct ai near by 
				if(AI[i].transform.root.name == m_targetEnemy.ToString())
                {
					// Task sussceful 
					return true;
				}
            }
			return false;
		}
	}
}