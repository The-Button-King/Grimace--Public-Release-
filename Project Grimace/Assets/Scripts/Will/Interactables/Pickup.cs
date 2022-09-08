using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: Pickup
 *
 * Author: Will Harding
 *
 * Purpose: Override of abstact interactable for pickups
 * 
 * Functions:   protected override void Awake()
 *              public override void Interact()
 *              public virtual void Effect()
 *              private void OnEnable()
 *              private IEnumerator AnimateAway()
 *              
 *              
 * 
 * References:
 * 
 * See Also:    Interactable
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 07/04/22     WH          1.0         Created
 * 03/05/22     WH          1.1         Added functionality
 * 10/05/22     WH          1.2         Cleaned up code
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 20/06/22     WH          2.0         Moved functions to individual scripts
 * 21/06/22     WH          2.1         Changed look at text to use UI
 * 23/06/22     WH          2.2         Added asset pooling
 * 31/07/22     WH          2.1         Interactable text changes
 * 04/08/22     WH          2.2         Added animations
 * 15/08/22     WH          2.3         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof(PowerUpParent))]
public class Pickup : Interactable
{
    private     AssetPool       m_assetPool;            // Asset pool

    public      Animator        m_animator;             // Animator component

    [SerializeField]
    [Tooltip( "The effect to give to the player" )]
    private     PowerUpParent   m_effect;               // Effect to give to player

    private     bool            m_interactable = true;  // Is the pickup interactable?

    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Sets assetPool
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Awake()
    {
        base.Awake();

        // Assign components
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();
        m_effect = GetComponent<PowerUpParent>();
        m_animator = GetComponent<Animator>(); 
    }

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/08/22
    *   Contributors    :    WH
    *   Notes           :    Override function
    *   See also        :    
    ******************************************************/
    public override void Interact()
    {
        // If interactable
        if ( m_interactable )
        {
            // Make not interactable so you can't spam the effect
            m_interactable = false;

            // Triggers effect
            Effect();

            // Start close animation
            StartCoroutine( AnimateAway() );
        }
    }

    /***************************************************
    *   Function        :    Effect
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    23/06/22
    *   Contributors    :    WH
    *   Notes           :    Override function
    *   See also        :    
    ******************************************************/
    public virtual void Effect()
    {
        // Trigger effect
        m_effect.PowerUpEffect();
    }


    /***************************************************
    *   Function        :    OnEnable
    *   Purpose         :    Calls animation on enable
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        // Play animation
        m_animator.SetTrigger( "Spawn" );
    }


    /***************************************************
    *   Function        :    AnimateAway
    *   Purpose         :    Returns pickup to pool after animating
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator AnimateAway()
    {
        // Calls despawn animation
        m_animator.SetTrigger( "Despawn" );

        // Waits for close animation
        yield return new WaitForSeconds( m_animator.GetCurrentAnimatorStateInfo( 0 ).length );
       
        // Waits for despawn animation which happens after the close animation
        yield return new WaitForSeconds( m_animator.GetCurrentAnimatorStateInfo( 0 ).length );

        // Returns pickup to the pool
        m_assetPool.ReturnObjectToPool( gameObject );
    }

}
