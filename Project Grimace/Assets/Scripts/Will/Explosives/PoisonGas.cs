using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PoisonGas
 *
 * Author: Will Harding
 *
 * Purpose: Poison gas cloud that damages entities
 * 
 * Functions:   private void OnTriggerStay( Collider other )
 *              public override void ToggleEffect()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 15/06/22     WH          1.0         Initial creation
 * 19/06/22     WH          1.1         Now using Interface for damage
 * 20/06/22     WH          1.2         Check for if interface exists on object
 * 23/06/22     WH          1.3         Made a child of Smoke since they were the same and is assetPooled
 * 24/06/22     WH          1.4         Added reset function
 * 03/07/22     WH          1.5         Reset changed to OnEnable
 * 11/07/22     WH          1.6         Added layermask check for colliders
 * 01/08/22     JG          1.7         Changed how activaited 
 * 14/08/22     WH          1.8         Cleaning
 ****************************************************************************************************/
public class PoisonGas : Smoke
{
    [SerializeField]
    [Tooltip( "Damage to apply per frame" )]
    private float       m_damage        = 0.1f; // How much damage the gas deals
    
    [SerializeField]
    private LayerMask   m_layerMask;            // LayerMask of what collider to find to tigger poison damage

    /***************************************************
    *   Function        :    OnTriggerStay
    *   Purpose         :    Damage entities on stay in gas
    *   Parameters      :    Collider other
    *   Returns         :    void
    *   Date altered    :    11/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnTriggerStay( Collider other )
    { 
        // Bit mask the layers to check if the collider is the one for environment interactable
        if( ( ( 1 << other.gameObject.layer ) & m_layerMask.value ) != 0 )
        {
            // Get Damageable interface
            IDamageable<float> damageable = other.transform.root.GetComponentInChildren<IDamageable<float>>();

            // if the collider in the trigger is a damageable thing
            if ( damageable != null )
            {
                // Damage entity
                damageable.Damage( m_damage );
            }
        }
    }

    /***************************************************
    *   Function        : ToggleEffect  
    *   Purpose         : Create Gas
    *   Parameters      : none   
    *   Returns         : Void   
    *   Date altered    : 01/08/22
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void ToggleEffect()
    {
        // Turn on the trigger
        GetComponent<Collider>().enabled = true;

        // Turn on the effect
        base.ToggleEffect();
    }
}
