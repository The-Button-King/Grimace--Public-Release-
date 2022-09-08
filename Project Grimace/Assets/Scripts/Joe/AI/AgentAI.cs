using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/****************************************************************************************************
* Type: Class
* 
* Name: AIData
*
* Author: Joseph Gilmore
*
* Purpose: Used for AI that requrie a navmesh agent 
* 
* Functions:    
                protected override void Start()
                public List<Vector3> GetPath()
                public void SetPath( List<Vector3> path )
                public void SetSpeed( float speed )
                public float GetSpeed()
                public float GetDefaultSpeed()
                public override void ResetValues()                 
* 
* References: N/A
* 
* See Also: look at behaviour trees ,  AIData , ISloweable
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 02/08/2022    JG          1.00        - created as part of restructuring AI for cleaner inhereteince 
* 14/08/2022    JG          1.01        - Added interface 
* 15/08/2022    JG          1.02        - Clean
****************************************************************************************************/
[RequireComponent( typeof( NavMeshAgent ) )]
public class AgentAI : AIData , ISloweable
{
    private float               m_avoidancePredictionTime = 0.5f;           // Time used to avoid prediction 
    private int                 m_pathfindingIterationsPerFrame = 5000;     // Update rate of path finding
    private List<Vector3>       m_waypoints = new List<Vector3>();          // Randomised patrol points Set by patrol task
    [Header("AI Movement values")]
    [SerializeField]
    [Tooltip( "movement speed of AI " )]
    private float               m_defaultSpeed = 8.0f;                      // Default speed of the AI
    private float               m_baseSpeed;                                // Speed of AI
    protected  NavMeshAgent     m_agent;                                    // Reference to AI 

    /***************************************************
    *   Function        : Start
    *   Purpose         :  Setup AI
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 05/08/2022
    *   Contributors    : JG
    *   Notes           :
    *   See also        :    
    ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Get Agent 
        m_agent = GetComponent<NavMeshAgent>();

        // Set up mesh for better accuracy
        NavMesh.avoidancePredictionTime = m_avoidancePredictionTime;
        NavMesh.pathfindingIterationsPerFrame = m_pathfindingIterationsPerFrame;


        // Set values
        m_baseSpeed = m_defaultSpeed;

        // Apply speed to agent 
        if ( m_agent != null )
        {
           m_agent.speed = m_baseSpeed;

        }


    }
    /***************************************************
   *   Function        : GetPath
   *   Purpose         : return set waypoint path
   *   Parameters      : N/A
   *   Returns         : List<Vector3> m_waypoints
   *   Date altered    : 02/08/2022
   *   Contributors    : JG
   *   Notes           : Moved from AIData part of resutructure    
   *   See also        :    
   ******************************************************/
    public List<Vector3> GetPath()
    {
        return m_waypoints;
    }
    /***************************************************
   *   Function        : SetPath
   *   Purpose         :  set waypoint path
   *   Parameters      : List<Vector3> path
   *   Returns         : Void
   *   Date altered    : 02/08/2022
   *   Contributors    : JG
   *   Notes           : Moved from AIData part of resutructure       
   *   See also        :    
   ******************************************************/
    public void SetPath( List<Vector3> path )
    {
        m_waypoints = path;
    }
    /***************************************************
    *   Function        : SetSpeed 
    *   Purpose         : set speed of AI 
    *   Parameters      : float speed
    *   Returns         : Void
    *   Date altered    : 15/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetSpeed( float speed )
    {
        // Set speed var
        m_baseSpeed = speed;

        // Update navmesh agent speed if has one
        if ( m_agent != null )
        {
           m_agent.speed = m_baseSpeed;

        }
    }
    /***************************************************
    *   Function        : GetSpeed 
    *   Purpose         : Return speed of AI
    *   Parameters      : N/A
    *   Returns         : float m_baseSpeed
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetSpeed()
    {
        return m_baseSpeed;
    }
    /***************************************************
   *   Function        : GetDefautSpeed 
   *   Purpose         : Return  starting speed of AI
   *   Parameters      : N/A
   *   Returns         : float m_defaultSpeed
   *   Date altered    : UK
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    public float GetDefaultSpeed()
    {
        return m_defaultSpeed;
    }
    /***************************************************
  *   Function        : ResetValues
  *   Purpose         : reset values altered by beacon
  *   Parameters      : N/A
  *   Returns         : Void
  *   Date altered    : 02/08/2022
  *   Contributors    : JG
  *   Notes           :    
  *   See also        :    
  ******************************************************/
    public override void ResetValues()
    {
        m_baseSpeed = m_defaultSpeed;
        m_baseDamage = m_defaultDamage;
        m_baseAttackRate = m_defaultAttackRate;
    }
}
