using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: ForkLiftAxle
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Store information about vechicle axis
 * 
 * Functions: N/A
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * UK            JG          1.00       - Created class
 * 15/08/2022    JG          1.01       - Cleaning 
 ****************************************************************************************************/
public class ForkLiftAxle : MonoBehaviour
{
        public WheelCollider m_leftWheel;       // Right wheel of axis
        public WheelCollider m_rightWheel;      // Left Wheel of axis 
        public bool          m_motor;           // Is this axis attached to motor?
        public bool          m_steering;        // Does this axis apply steer angle?
}

