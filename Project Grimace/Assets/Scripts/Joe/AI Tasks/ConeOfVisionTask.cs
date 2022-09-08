using NodeCanvas.Framework;
using ParadoxNotion.Design;
namespace NodeCanvas.Tasks.Conditions
{
	// Node Canavs inspector details
	[Category( "Custom Detection task" )]
	[Description("Used to spot out player with a cone of vision ")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: ConeOfVisionTask
	*
	* Author: Joseph
	*
	* Purpose:  For the AI To detect a player in a simple cone of visions 
	* 
	* Functions:		protected override string OnInit()
						protected override bool OnCheck()
	* 
	* References: 
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 29/03/2022    JG			1.00		- First iteration of a cone of vision. detecting the player & ignoring obstacles working. Not working debug lines and direction of cone
	* 11/04/2022    JG			1.01		- Changed the acess to vars to private and commented code 
	* 25/04/2022    JG			2.00		- Moved most of code in to a different script so it can be used in several places. 
	* 14/07/2022    JG			2.01		- Updated players last known pos
	* 19/07/2022    JG          2.02		- Use FOV from AI data. 
	* 15/08/2022    JG			2.03		- Cleaning 
	****************************************************************************************************/

	public class ConeOfVisionTask : ConditionTask
	{
		
		private FieldOfView m_fieldOfView; // Reference a field of view for AI


		/***************************************************
		*   Function        : OnInit
		*   Purpose         : Use for initialization. This is called only once in the lifetime of the task.   
		*   Parameters      : N/A
		*   Returns         : Return null if init was successfull. Return an error string otherwise   
		*   Date altered    : 19/07/2022
		*   Contributors    : Joseph Gilmore 
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin. 
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get a FOV from agent 
			m_fieldOfView = agent.GetComponent<AIData>().GetFieldOfView();

			return null;
		}
		/***************************************************
		*   Function        : OnCheck
		*   Purpose         : Called once per frame while the condition is active. check if player is in the vision of the AI
		*   Parameters      : N/A
		*   Returns         : Return whether the condition is success(player in vision) or failure(player not in view).   
		*   See also        : N/A
		*   Date altered    : 14/07/2022
		*   Contributors    : Joseph Gilmore
		*   Notes           : This function is inherited from the node canvas behaviour tree plugin. 
		******************************************************/
		protected override bool OnCheck()
		{
			// Check field of view 
			StartCoroutine( m_fieldOfView.CheckFOV(agent.transform ));

			// If in fov 
            if ( m_fieldOfView.GetTargetInFOV() )
            {
				// Update last known position 
				blackboard.SetVariableValue( "b_searchPosition", m_fieldOfView.GetTarget().position );
			}

			return m_fieldOfView.GetTargetInFOV();
		}
	}
}