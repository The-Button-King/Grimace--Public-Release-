using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Grenade
 *
 * Author: Will Harding
 *
 * Purpose: Throwable child of EnviromentInteractble
 * 
 * Functions:   protected override void Awake()
 *              private IEnumerator ThrowTimer()
 *              public override void OnEnable()
 *              public void Throw( Vector3 dir )
 * 
 * References: 
 * 
 * See Also: EnviromentInteractble
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 19/06/22     WH          1.0         Initial creation
 * 23/06/22     WH          1.1         Added asset pooling
 * 24/06/22     WH          1.2         Added reset and throw functions
 * 03/07/22     WH          1.3         Reset changed to OnEnable
 * 10/08/22     WH          1.4         Changed to awake and cleaning
 * 14/08/22     WH          1.5         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof(Rigidbody))]
public class Grenade : EnviromentInteractble
{
    [SerializeField]
    [Tooltip( "How long until the grenade 'explodes'" )]
    [Range(0.5f, 10f)]
    private float       m_timer         = 3f;       // Time until grenade activates effect and then deactivates

    [SerializeField]
    [Tooltip( "Force to throw grenade with" )]
    [Range( 1f, 50f )]
    private float       m_force         = 10f;      // Force to apply when throwing

    private Rigidbody   m_rigidbody;                // Rigidbody on the grenade

    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Gets rigidbody
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    10/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Awake()
    {
        base.Awake();

        m_rigidbody = GetComponent<Rigidbody>();
    }

    /***************************************************
    *   Function        :    ThrowTimer
    *   Purpose         :    Activates effect and deactivates grenade after timer
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    23/06/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    private IEnumerator ThrowTimer()
    {
        // Wait for timer seconds
        yield return new WaitForSeconds( m_timer );

        TriggerEffect();
        
        // Add grenade back to asset pool
        m_assetPool.ReturnObjectToPool( gameObject );
    }

    /***************************************************
    *   Function        : OnEnable   
    *   Purpose         : Resets object as if it was just spawned in
    *   Parameters      : none   
    *   Returns         : Void   
    *   Date altered    : 10/07/22
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void OnEnable()
    {
        base.OnEnable();

        // Stop the physics
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;

        // Start throw timer so it acts as if it was just throw
        StartCoroutine( ThrowTimer() );
    }

    /***************************************************
    *   Function        :    Throw
    *   Purpose         :    Add force to object
    *   Parameters      :    Vector3 dir
    *   Returns         :    void
    *   Date altered    :    10/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Throw( Vector3 dir )
    {
        // Add force in the specified direction
        m_rigidbody.AddForce( dir * m_force, ForceMode.Impulse );
    }
}
