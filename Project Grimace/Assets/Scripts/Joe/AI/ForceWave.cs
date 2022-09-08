using CameraShake;
using LayerMasks;
using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: ForceWave
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Apply force of shockwave
 * 
 * Functions:           protected override void Start()
 *                      public void StartForceWave()
 *                      protected override void ApplyEffect( Collider[] hitColliders, int collideCount )
 *                      public void SetAll( Vector3 pos , LineRenderer line , float force , float radius , float damage , Layers layer )
 *                      public void SetDamage( float dmg )
 *                      
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 21/07/2022    JG          1.00        - Started class 
 * 22/07/2022    JG          1.02        - Replaced camera shake with new one
 * 02/08/2022    JG          1.03        - Getter 
 * 15/08/2022    JG          1.04        - Cleaning 
 ****************************************************************************************************/
public class ForceWave : ShockWave
{
    // Vars don't get used in an exposed as not needed 
    private float m_upwardsForce = 0.5f;                    // Updwards force amount 
    private float m_minAmountOfForceMutiplyier = 0.2f;      // Min amount of force that can be applied 
    private float m_force = 10.0f;                          // Force Applied to player
    private float m_maxDamage = 80.0f;                      // Damage 
    private float m_cameraForce = 9.0f;                     // Force of cameraShake
    private float m_cameraFadeOut = 0.7f;                   // Camera shake fade out value
    /***************************************************
    *   Function        : Start   
    *   Purpose         : Setup class    
    *   Parameters      : N/A    
    *   Returns         : Void
    *   Date altered    : 21/07/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Set shockwave position 
        m_shockwavePos = transform.position;
    }
    /***************************************************
    *   Function        : StartForceWave 
    *   Purpose         : Start wave of force 
    *   Parameters      : N/A    
    *   Returns         : Void
    *   Date altered    : 22/07/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void StartForceWave()
    {
        // Start shockwave 
        StartCoroutine( StartShockWave() );

        // Shake camera 
        ScreenShake.Instance.ShakeOverDistanceFromPoint( GameObject.FindGameObjectWithTag( "Player" ).transform.position, m_shockwavePos, m_maxRadius, m_cameraForce, m_cameraFadeOut );
    }
   
    /***************************************************
   *   Function        : ApplyEffect
   *   Purpose         : Apply effects of force wave 
   *   Parameters       Collider[] hitColliders, int collideCount 
   *   Returns         : Void
   *   Date altered    : 21/07/2022 
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    protected override void ApplyEffect( Collider[] hitColliders, int collideCount )
    {
        // Loop through hit colliders 
        for ( int i = 0; i < collideCount; i++ )
        {
            // Check if its the player 
            if ( hitColliders[ i ].transform.root.GetComponentInChildren<PlayerManager>() != null )
            {
                // Check if its not already been hit 
                if ( !m_hitObjects.Contains( hitColliders[ i ].transform.gameObject ) )
                {
                    if ( hitColliders[ i ].transform.root.GetComponentInChildren<PlayerManager>().IsGrounded() )
                    {
                        // short hand
                        Vector3 pos = hitColliders[ i ].transform.position;

                        // Work out the direction to appy force to the player
                        Vector3 direction = ( new Vector3( pos.x, pos.y + m_upwardsForce, pos.z ) - m_shockwavePos ).normalized;

                        // Work out the distance to the center of explosion from hit object
                        float distance = Vector3.Distance( hitColliders[ i ].transform.position, m_shockwavePos );

                        // Work out the distance percent 
                        float pernctageAway = 1 - ( distance / m_maxRadius );

                        // Apply damage
                        hitColliders[ i ].transform.root.GetComponentInChildren<PlayerManager>().Damage( m_maxDamage * pernctageAway );

                        // If percent too low cap it at min amount of force 
                        if ( pernctageAway < m_minAmountOfForceMutiplyier )
                        {
                            pernctageAway = m_minAmountOfForceMutiplyier;
                        }
                        // Apply force to direction
                        direction *= m_force * pernctageAway;

                        // Apply direction to the player to fake force being applyied 
                        StartCoroutine( hitColliders[ i ].transform.root.GetComponentInChildren<PlayerManager>().DashMovement( direction ) );

                    }
                    // Add to list of hit objects so it cannot be hit twice by same wave
                    m_hitObjects.Add( hitColliders[ i ].transform.gameObject );
                }



            }
        };
    }
    /***************************************************
      *   Function        : SetAll
      *   Purpose         : Set all needed vars of class
      *   Parameters        Vector3 pos , LineRenderer line , float force , float radius , float damage , Layers layer
      *   Returns         : Void
      *   Date altered    : 21/07/2022 
      *   Contributors    : JG
      *   Notes           :    
      *   See also        :    
      ******************************************************/
    public void SetAll( Vector3 pos , LineRenderer line , float force , float radius , float damage , Layers layer )
    {
        m_shockwavePos = pos;
        m_shockWaveEffect = line;
        m_force = force;
        m_maxRadius = radius;
        m_maxDamage = damage;
        m_layerMasks = LayerMask.GetMask(layer.ToString());

        // Reset line renderer 
        SetUpLine();
    }
    /***************************************************
    *   Function        : SetDamage
    *   Purpose         : Set  damage of shockwave
    *   Parameters      : float dmg 
    *   Returns         : Void
    *   Date altered    : 02/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetDamage( float dmg )
    {
        m_maxDamage = dmg;
    }
}
