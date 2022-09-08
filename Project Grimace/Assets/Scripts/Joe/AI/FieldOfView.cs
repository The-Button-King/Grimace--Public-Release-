using System.Collections;
using UnityEngine;
using LayerMasks;
/****************************************************************************************************
* Type: Class
* 
* Name: FieldOfView
*
* Author: Joseph Gilmore
*
* Purpose: Used to be able to detect a target within a certain angle 
* 
* Functions:          public FieldOfView(Transform agentTransform,float fov,Layers target)
*					  public bool OnCheckVision( Transform agent )
*					  public IEnumerator CheckFOV( Transform agent )
*					  public bool GetTargetInFOV()
*					  public Transform GetTarget()
*					  public void SetFOV( float fov )
*					  ublic void SetRadius ( float radius )
*					  public float GetRadius( )
* References: 
* 
* See Also: ConeOfVisionTask, Turret
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/04/2022    JG          1.00        - Code orginally from Cone of vision task. Move to use it in more than one task. 
* 10/06/2022    JG          1.01		- Altered code to have a general target not just player.
* 29/07/2022    JG			1.02		- Getters/setters
* 13/08/2022    JG			1.03		- Minor bug fixes 
* 15/08/2022    JG			1.04		- Cleaning 
* 16/08/2022    JG			1.05		- Bugs
****************************************************************************************************/
public class FieldOfView 
{
	private Transform				m_agentTransform;				// Get a reference to the agent connected to the behaviour tree
	[Header(" Fov stats")]
	[SerializeField]
	[Range(5.0f, 30.0f)]
	[Tooltip("The sphere radius of the cone detection(Range).")]
	private float					m_sphereRadius = 15.0f;			// The sphere radius of the cone detection
	[SerializeField]
	[Range(0.0f, 360.0f)]
	[Tooltip("The cone of vision angle for the AI to be able to detect player")]
	private float					m_fovAngle = 135.0f;			// The cone of vision angle for the AI to be able to detect player 
	private  Transform				m_target;
	private LayerMask				m_targetLayer;					// A reference to the layer mask that the player is stored on

	private LayerMask				m_objectMask;                   // A Rereence to the layer mask where objects are on 
	[SerializeField][Range(0.0f,2.0f)][Tooltip("The amount of time between checking the field of view (smaller num worse perforamnce")]
	private float					m_timerBetweenChecks = 0.5f;	// Time between check of vision to improve performance 
	private bool					m_targetInFOV = false;          // Is target in the FOV

	/***************************************************
	 *   Function        : FieldOfView(Transform agentTransform)   
	 *   Purpose         : Contruct the class with the transform of the FOV 
	 *   Parameters      : Transform agentTransform,float fov,Layers target
	 *   Returns         : Void   
	 *   Date altered    : 16/08/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
     *****************************************************/
	public FieldOfView( Transform agentTransform,float fov,Layers target )
	{
		// Get the agents transform
		m_agentTransform = agentTransform;
		m_fovAngle = fov;

		// Get Layer References 
		m_targetLayer = LayerMask.GetMask(target.ToString());
		m_objectMask = LayerMask.GetMask( Layers.Wall.ToString(), Layers.Cover.ToString(), Layers.Obstacles.ToString(), Layers.Doors.ToString() ,Layers.SmokeLayer.ToString());
		
	}

	/***************************************************
	*   Function        : OnCheckVision
	*   Purpose         : Used to check the vision of transform
	*   Parameters      : Transform agent
	*   Returns         : Return whether the condition is success(player in vision) or failure(player not in view).   
	*   See also        : N/A
	*   Date altered    : 16/08/2022
	*   Contributors    : Joseph Gilmore
	*   Notes           : 
	******************************************************/
	public bool OnCheckVision( Transform agent )
	{
		m_targetInFOV = false;

		// Get updated agent transform
		m_agentTransform = agent.transform;

		if ( m_agentTransform != null )
        {
			

			// Cast a sphere around the position of the agent and look at the target layer only and return a array of "players"
			Collider[] hitPlayerTargets = Physics.OverlapSphere( m_agentTransform.position, m_sphereRadius, m_targetLayer );

			// Loop through each hit target( Always going to be one as player is only one on that layer)
			foreach ( Collider target in hitPlayerTargets )
			{
				// Get the direction of the target. 
				Vector3 targetDirection = ( target.transform.position - m_agentTransform.position ).normalized;

				// Check to see if the target detected is in the fov angle. 
				if ( Vector3.Angle( m_agentTransform.forward, targetDirection ) < m_fovAngle * 0.5f )
				{
					// Calculate the distance between 
					float distanceToTarget = Vector3.Distance( m_agentTransform.position, target.transform.position );


					// If we don't hit an obstacle (rays go through walls noramally) we have hit target in the cone of vision  
					if ( !Physics.Raycast( m_agentTransform.position, targetDirection, distanceToTarget, m_objectMask ) )
					{
						// Hit target as its in view 
						m_target = target.transform;

						// Target is in vision 
						m_targetInFOV = true;

						return true;
					}
				}
			}
		}
		
		// Player not in vision return false
		m_targetInFOV = false;
		return false;

	}
	/***************************************************
    *   IEnumerator     :  CheckFOV  
    *   Purpose         :  Check field of view at set intervals instead of every update   
    *   Parameters      :  Transform agent  
    *   Returns         :  yield return waitTim  
    *   Date altered    :  UK
    *   Contributors    : JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public IEnumerator CheckFOV( Transform agent )
    {
        WaitForSeconds waitTime = new WaitForSeconds(m_timerBetweenChecks);
        while (!OnCheckVision(agent))
        {
            yield return waitTime;
        }
    }
	/***************************************************
    *   IEnumerator     : GetTargetInFOV
    *   Purpose         : Return if target in vision   
    *   Parameters      : N/A 
    *   Returns         : bool m_targetInFOV
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public bool GetTargetInFOV()
    {
		return m_targetInFOV;
    }
	/***************************************************
    *   Function		:  GetTarget
    *   Purpose         :  Return targert in FOV
    *   Parameters      :  N/A
    *   Returns         :  Transform m_target 
    *   Date altered    :  UK
    *   Contributors    :  JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public Transform GetTarget()
    {
		return m_target;
    }
	/***************************************************
    *	function    :  SetFOV
    *   Purpose         :  set the value of the fov 
    *   Parameters      :  fov
    *   Returns         :  Void
    *   Date altered    :  31/05/2022
    *   Contributors    :  JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public void SetFOV( float fov )
    {
		m_fovAngle = fov;
    }
	/***************************************************
    *	function		:  SetRadius
    *   Purpose         :  set the value of the radius 
    *   Parameters      :  float radius
    *   Returns         :  Void
    *   Date altered    :  29/07/2022
    *   Contributors    :  JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public void SetRadius ( float radius )
    {
		m_sphereRadius = radius;
    }
	/***************************************************
    *	function		:  GetRadius
    *   Purpose         :  Get the value of the radius 
    *   Parameters      :  n/a
    *   Returns         :  float radius
    *   Date altered    :  29/07/2022
    *   Contributors    :  JG
    *   Notes           :    
	*   See also        :    
	******************************************************/
	public float GetRadius( )
	{
		return m_sphereRadius;
	}
	
}
