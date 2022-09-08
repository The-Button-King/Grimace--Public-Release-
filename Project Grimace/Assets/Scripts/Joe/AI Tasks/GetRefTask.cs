using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	/****************************************************************************************************
	 * Type: Class
	 * 
	 * Name: GetRefTask
	 *
	 * Author: Joseph Gilmore
	 *
	 * Purpose: Get requried references to requried bb vars 
	 * 
	 * Functions: protected override void OnExecute()
	 * 
	 * References:
	 * 
	 * See Also:
	 *
	 * Change Log:
	 * Date          Initials    Version     Comments
	 * ----------    --------    -------     ----------------------------------------------
	 * Uk			JG		     1.00		 - Created
	 * 15/08/2022   JG			 1.01		 - Cleaned
	 ****************************************************************************************************/
	public class GetRefTask : ActionTask
	{
		/***************************************************
		 *   Function        : OnExecute()   
		 *   Purpose         : Get the correct references and set the blackboard up   
		 *   Parameters      : N/A    
		 *   Returns         : Void     
		 *   Date altered    : UK
		 *   Contributors    : JG
		 *   Notes           :    
		 *   See also        :    
		 ******************************************************/
		protected override void OnExecute()
		{
			
			blackboard.SetVariableValue( "b_player",GameObject.FindGameObjectWithTag("Player" ).gameObject);
			EndAction( true );
		}

	}
}