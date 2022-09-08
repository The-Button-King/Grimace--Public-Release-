using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
namespace NodeCanvas.Tasks.Actions
{

	[Category("Custom Attack Task")]
	/****************************************************************************************************
	* Type: Class
	* 
	* Name: ExplodeTask
	*
	* Author: Joseph Gilmore
	*
	* Purpose: Explode AI 
	* 
	* Functions:			
	*					protected override string OnInit()
	*					protected override void OnExecute()
	*					private IEnumerator Explode()
	*					
	* 
	* References:
	* 
	* See Also:
	*
	* Change Log:
	* Date          Initials    Version     Comments
	* ----------    --------    -------     ----------------------------------------------
	* 14/06/2022    JG			1.00		- 
	* 12/07/2022    JG		    1.01		- Cleaning 
	* 01/08/2022    JG			1.02		- Updated explosion call 
	* 03/08/2022    JG			1.03		- Bug Fixes 
	* 12/08/2022    JG			1.04		- Build bug fixes 
	* 15/08/2022    JG			1.05		- Cleaning 
	****************************************************************************************************/
	public class ExplodeTask : ActionTask
	{
		public  GameObject				m_explosion;			// reference to explosion prefab(serilzed fields don't work in the blackboard)
		private EnemyAudioPool			m_enemyAudioPool;		// Reference to Audio poo 
		private float					m_explodeTime;          // Store explosion time 
		/***************************************************
		*   Function        :  OnInit  
		*   Purpose         :  Setup task  
		*   Parameters      :  N/A 
		*   Returns         :  Void  
		*   Date altered    :  15/08/2022 
		*   Contributors    :  JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override string OnInit()
		{
			// Get reference to asset pool
			m_enemyAudioPool = agent.transform.GetComponent<EnemyAudioPool>();


			// Get length of audio clip 
			m_explodeTime = m_enemyAudioPool.m_attack1.m_clip.length;

			return null;
		}

		/***************************************************
		*   Function        : OnExecute    
		*   Purpose         : Start explosion    
		*   Parameters      : N/A   
		*   Returns         : Void  
		*   Date altered    : 03/08/2022
		*   Contributors    : JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		protected override void OnExecute()
		{
			// If agent active 
			if( agent.GetComponent<NavMeshAgent>().isActiveAndEnabled == true )
            {
				// Start explosion 
				StartCoroutine( Explode() );
            }
            else
            {
				EndAction(false);
            }
			
			
		}
		/***************************************************
		*   IEnumerator     :  Explode  
		*   Purpose         :  Play sounds and effects in the correct order and timings   
		*   Parameters      :  N/A  
		*   Returns         :  Void  
		*   Date altered    :  12/08/2022
		*   Contributors    :  JG
		*   Notes           :    
		*   See also        :    
		******************************************************/
		private IEnumerator Explode()
        {
			// Play beep sound
			m_enemyAudioPool.PlaySound( m_enemyAudioPool.m_attack1 );

			yield return new WaitForSeconds( m_explodeTime );

			// Play explode sound
			m_enemyAudioPool.PlaySound( m_enemyAudioPool.m_death );

			// Explode 
			agent.GetComponent<Bomber>().Explode();
			
			// End task
			EndAction(true);
			
		}
		
	}
}