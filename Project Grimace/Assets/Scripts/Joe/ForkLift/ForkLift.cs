using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: Forklift
 *
 * Author: Joseph Gilmore
 *
 * Purpose: A forklift the player can drive 
 * 
 * Functions:    
 *              private void Start()
 *              private void FixedUpdate()
 *              private void ApplyLocalPositionToVisuals( WheelCollider collider )
 *              public void ActivateForkLift()
 *              private void Exit()
 *              public bool IsInForkLift()
 *              private void DriveForkLift()
 *              
 * 
 * References:
 * 
 * See Also: ForkLiftAxle
 * Note: The forklift is driveable and works. I did not have time however to get it to work with the AI so it is purely for devs and won't be in the final build
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 24/07/2022    JG          1.02       - Added center of mass offset
 * 31/07/2022    JG          1.03       - Can now exit 
 * 15/08/2022    JG          1.04       - Cleaning 
 ****************************************************************************************************/
public class ForkLift : MonoBehaviour
{
    
    [Tooltip("Axle for forklifts")]
    public List<ForkLiftAxle>   m_axles;                                    // Reference to each axis 
    [Header("ForkLift stats")]
    [SerializeField][Range(500.0f,4000.0f)][Tooltip("Maximum torque of forklifr")]
    private float               m_maxTorque = 1000.0f;                      // Maximum torque that can be applied to wheels
    [SerializeField][Range( 10.0f, 50.0f )] [Tooltip( "Max anlge to steer the forklift Value may casue it to tip" )]
    private float               m_maxSteeringAngle = 30.0f;                 // Maximum steer angle the wheels can have
    private float               m_centerOfMassOffset = -0.1f;               // Reference to center of mass 
    private bool                m_forkLiftOccupied = false;                 // Has player entered forklift
    private PlayerControls      m_playerControls;                           // Reference to input controler 
    private InputAction         m_move;                                     // Moving the forklift input action
    private GameObject          m_player;                                   // Reference to player 
    private InputAction         m_exitAction;                               // Allows the player to leave forklift                                     
    private int                 m_priority = 0;                             // Stores the proirty of the vcams
    /***************************************************
     *   Function        : Start    
     *   Purpose         : Set up class   
     *   Parameters      : N/A   
     *   Returns         : Void    
     *   Date altered    : 24/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void Start()
    {
        // Set center of mass 
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f,m_centerOfMassOffset,0.0f);


        // Store priority for later use
        m_priority = transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority;
        
        // Get reference to player & controls
        m_player = GameObject.FindGameObjectWithTag( "Player" );
        m_playerControls = m_player.GetComponent<PlayerManager>().GetControls();

        // Assign inputs 
        m_move = m_playerControls.ForkLift.Movement;
        m_exitAction = m_playerControls.ForkLift.Interact;

        m_exitAction.Disable();
        m_exitAction.performed += _ => Exit();
    }

    /***************************************************
     *   Function        : FixedUpdate   
     *   Purpose         : Move forklift   
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG 
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void FixedUpdate()
    {
        // Apply forklift inputs 
        DriveForkLift();
    }
    /***************************************************
     *   Function        : ApplyLocalPositionToVisuals    
     *   Purpose         : Make the wheels look like they move   
     *   Parameters      : WheelCollider collider   
     *   Returns         : Void  
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void ApplyLocalPositionToVisuals( WheelCollider collider )
    {
        // If no mesh child 
        if (collider.transform.childCount == 0)
        {
            return;
        }

        // Get the transform of the mesh wheel
        Transform visualWheel = collider.transform.GetChild(0);


        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        // Set position and rotation of wheel to match collider to show wheels turning 
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
    /***************************************************
     *   Function        : ActivateForkLift
     *   Purpose         :  Get player to enter the forklift 
     *   Parameters      : N/A 
     *   Returns         : Void  
     *   Date altered    : 31/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void ActivateForkLift()
    {
        // Disable the player controls
        m_playerControls.Player.Disable();

        // Enable forklift controls 
        m_move.Enable();

        // Change cameras 
        transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority = m_priority + 2;
        transform.GetComponentInChildren<Camera>().enabled = true;

        // Reparent the player to the forklift 
        m_player.transform.root.parent = transform.parent;

        // Disable the player 
        m_player.SetActive( false );

        // Forklift now occupied 
        m_forkLiftOccupied = true;
        m_exitAction.Enable();
       
    }
    /***************************************************
     *   Function        : Exit 
     *   Purpose         : Player exit the forklift 
     *   Parameters      : N/A 
     *   Returns         : Void  
     *   Date altered    : 31/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void Exit()
    {
        // If forklift have player 
        if ( m_forkLiftOccupied )
        {   
            // Disable movement of forklift 
            m_move.Disable();

            // Renable the player controls 
            m_playerControls.Player.Enable();
            
            // forklift no longer occupied 
            m_forkLiftOccupied = false;

            // Reset cameras 
            transform.GetComponentInChildren<Camera>().enabled = false;
            transform.GetComponentInChildren<CinemachineVirtualCamera>().Priority = m_priority;

            // Set player back 
            m_player.SetActive( true );

            // Reposition player 
            m_player.transform.root.parent = null;
            m_player.transform.root.position = transform.position + transform.TransformDirection(Vector3.right);
        }
      
    }
    /***************************************************
     *   Function        : IsInForkLift()
     *   Purpose         : Is the player in the forklift 
     *   Parameters      : N/A 
     *   Returns         : Void  
     *   Date altered    : 31/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public bool IsInForkLift()
    {
        return m_forkLiftOccupied;
    }
    /***************************************************
    *   Function        : DriveForkLift
    *   Purpose         : Apply torque and direction to forklift 
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 15/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void DriveForkLift()
    {
        // If player in forklift 
        if ( m_forkLiftOccupied )
        {
            // Get input and apply torque
            float motor = m_maxTorque * m_move.ReadValue<Vector2>().y;


            // Steer based on input 
            float steering = m_maxSteeringAngle * m_move.ReadValue<Vector2>().x;

            // Loop through each axis 
            foreach ( ForkLiftAxle axleInfo in m_axles )
            {
                // If steering enabled apply 
                if ( axleInfo.m_steering )
                {
                    axleInfo.m_leftWheel.steerAngle = steering;
                    axleInfo.m_rightWheel.steerAngle = steering;
                }
                // If motor enabled apply torque 
                if ( axleInfo.m_motor )
                {
                    axleInfo.m_leftWheel.motorTorque = motor;
                    axleInfo.m_rightWheel.motorTorque = motor;
                }
                // Move wheels visually 
                ApplyLocalPositionToVisuals( axleInfo.m_leftWheel );
                ApplyLocalPositionToVisuals( axleInfo.m_rightWheel );
            }
        }
    }
}



