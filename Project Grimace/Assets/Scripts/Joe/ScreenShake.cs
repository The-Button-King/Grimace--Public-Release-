using Cinemachine;
using UnityEngine;
using System.Collections;
namespace CameraShake
{
    /****************************************************************************************************
     * Type: Class
     * 
     * Name: ScreenShake
     *
     * Author: Joseph Gilmore
     *
     * Purpose: Shake the camera for an effect
     * 
     * Functions:           private void Awake()
     *                      public void ShakeScreen(float intensity,float time )
     *                      private IEnumerator ShakeForXTime( float intensity,float time)
     *                      public void ShakeOverDistanceFromPoint(Vector3 pointA, Vector3 pointB, float rangeOfEffect, float intensity, float time )
     *                      public void SetToggleScreenShake(bool toggle)
     *                      
     *                      
     * 
     * References:
     * 
     * See Also:
     *
     * Change Log:
     * Date          Initials    Version     Comments
     * ----------    --------    -------     ----------------------------------------------
     * 22/07/2022    JG          1.00       - Created class
     * 02/08/2022    JG          1.01       - Settings toggle
     * 14/08/2022    JG          1.02       - Clean
     ****************************************************************************************************/
    public class ScreenShake : MonoBehaviour
    {
        private float                               m_shakeTimer;           // Timer used for camera shake 
        public static ScreenShake                   Instance;               // Used create a reference to self for easy creation
        private CinemachineBasicMultiChannelPerlin  m_cinemachineShaker;    // Reference to cineMachine component 
        private bool                                m_toggled = true;       // toogle screen shake 
        /***************************************************
        *   Function        : Awake   
        *   Purpose         : Set up class & get references   
        *   Parameters      : N/A   
        *   Returns         : Void
        *   Date altered    : 22/07/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
        private void Awake()
        {
            // Create instance of self 
            Instance = this;
            
            // Setup references for the camera shaker 
            m_cinemachineShaker = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
           
        }
        /***************************************************
         *   Function        : ShakeScreen
         *   Purpose         : Used for a simple screen shake   
         *   Parameters      : float intensity,float time 
         *   Returns         : Void
         *   Date altered    : 22/07/2022
         *   Contributors    : JG
         *   Notes           :    
         *   See also        :    
        ******************************************************/
        public void ShakeScreen(float intensity,float time )
        {
       
            StartCoroutine(ShakeForXTime(intensity,time));
        }
        /***************************************************
         *   Function        : ShakeForXTime
         *   Purpose         : Used to screen shake over time
         *   Parameters      : float intensity,float time 
         *   Returns         : Void
         *   Date altered    : 22/07/2022
         *   Contributors    : JG
         *   Notes           :    
         *   See also        : Menu.cs
        ******************************************************/
        private IEnumerator ShakeForXTime( float intensity,float time)
        {
            if ( m_toggled )
            {
                // Apply  screen shake timer 
                while ( m_shakeTimer < time )
                {
                    // Add to timer
                    m_shakeTimer += Time.deltaTime;

                    // Add to shaker amplitude 
                    m_cinemachineShaker.m_AmplitudeGain = Mathf.Lerp( intensity, 0.0f, ( 1.0f - m_shakeTimer / time ) );

                    yield return null;
                }

                // Reset values 
                m_cinemachineShaker.m_AmplitudeGain = 0;
                m_shakeTimer = 0;
            }
         
        }
        /***************************************************
        *   Function        : ShakeOverDistanceFromPoint
        *   Purpose         : Shake camera taking in a distance and percentage of the max distance of effect
        *   Parameters      : Vector3 pointA, Vector3 pointB, float rangeOfEffect, float intensity, float time
        *   Returns         : Void
        *   Date altered    : 22/07/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
       ******************************************************/
        public void ShakeOverDistanceFromPoint(Vector3 pointA, Vector3 pointB, float rangeOfEffect, float intensity, float time )
        {
            // Work out the distance to the player from explosion
            float distance = Vector3.Distance( pointA, pointB );

            // Calculate shake percent and then invert
            float shakePercent = 1.0f - ( distance / rangeOfEffect );

            // Work out the shake value percent of the max camera shake 
            float shakeValue = intensity * shakePercent;

            StartCoroutine(ShakeForXTime(shakeValue,time));
        }
        /***************************************************
        *   Function        : SetToggleScreenShake()    
        *   Purpose         : Toggle screen shake    
        *   Parameters      : bool toggle  
        *   Returns         : Void    
        *   Date altered    : 02/08/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
        public void SetToggleScreenShake(bool toggle)
        {
            m_toggled = toggle;
        }
    }
    
}
