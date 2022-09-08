using UnityEngine;
using UnityEngine.AI;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: AIDebug
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Show elements of AI 
 * 
 * Functions: private void Start()
 *            private void OnDrawGizmos()
 * 
 * References:www.youtube.com. (n.d.). Creating your first animated AI Character! [AI #01]. [online] Available at: https://www.youtube.com/watch?v=TpQbqRNCgM0.
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 04/08/2022    JG          1.00         - Class created to debug AI. Please refere to reference 
 ****************************************************************************************************/

public class AIDebug : MonoBehaviour
{
    [Header("Enable/disable visual debugs")]
    [SerializeField]
    private bool            m_velocity;             // Show velocity 
    [SerializeField]
    private bool            m_desiredVeloctiy;      // Show desired velocity 
    [SerializeField]
    private bool            m_path;                 // Show AI Path
    private NavMeshAgent    m_agent;                // Reference to Agent 
    /***************************************************
    *   Function        : Start    
    *   Purpose         : Setup class   
    *   Parameters      : N/A  
    *   Returns         : Void    
    *   Date altered    : 04/08/2022
    *   Contributors    : JG
    *   Notes           :   See header Reference 
    *   See also        :    
    ******************************************************/
    void Start()
    {
        m_agent = GetComponentInChildren<NavMeshAgent>(true);
    }
    /***************************************************
    *   Function        : OnDrawGizmos  
    *   Purpose         : draw debug information 
    *   Parameters      : N/A  
    *   Returns         : Void    
    *   Date altered    : 04/08/2022
    *   Contributors    : JG
    *   Notes           :   See header Reference 
    *   See also        :    
    ******************************************************/
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if ( m_desiredVeloctiy )
        {
            // Draw the desired velocity 
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + m_agent.desiredVelocity);
        }

        if ( m_velocity )
        {
            // Draw velocity 
            Gizmos.color = Color.green;
            Gizmos.DrawLine( transform.position, transform.position + m_agent.velocity );
        }
        if ( m_path )
        {

            Gizmos.color = Color.red;

            // Get current path of AI
            NavMeshPath agentPath = m_agent.path;

            // Use current pos as first corner 
            Vector3 preCorner = transform.position;

            // Get all corners in path 
            foreach(Vector3 corner in agentPath.corners )
            {
                // Draw a line from last corner to current 
                Gizmos.DrawLine(preCorner, corner);

                // Draw sphere on point 
                Gizmos.DrawSphere( corner, 0.1f );
                preCorner = corner;
            }
        }
        #endif
    }
}
