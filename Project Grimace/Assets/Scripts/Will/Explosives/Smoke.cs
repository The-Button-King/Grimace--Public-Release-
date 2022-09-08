using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: Smoke
 *
 * Author: Will Harding
 *
 * Purpose: Smoke that blocks vision
 * 
 * Functions:   protected virtual void Start()
 *              protected virtual IEnumerator DelayDeactivate()
 *              public virtual void ToggleEffect()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 15/06/22     WH          1.0         Initial creation
 * 23/06/22     WH          1.1         Now parent of PoisonGas and is assetPooled
 * 24/06/22     WH          1.2         Added reset function
 * 03/07/22     WH          1.3         Reset changed to OnEnable
 * 01/08/22     JG          1.4         Added interace and remove activate from start so does not play on pool
 * 10/08/22     WH          1.5         Cleaning
 * 14/08/22     WH          1.6         Cleaning
 ****************************************************************************************************/
public class Smoke : MonoBehaviour , IToggleable
{
    [SerializeField]
    [Tooltip( "Duration of the effect" )]
    private     float       m_duration      = 7f;   // How long the effect lasts

    private     AssetPool   m_assetPool;            // Asset pool reference


    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Gets asset pool
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    10/08/22
    *   Contributors    :    WH 
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected virtual void Start()
    {
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();
    }

    /***************************************************
    *   Function        :    DelayDeactivate
    *   Purpose         :    Deactivates gas after duration
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/06/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    protected virtual IEnumerator DelayDeactivate()
    {
        // Wait for duration seconds
        yield return new WaitForSeconds( m_duration );

        // Return effect to asset pool
        m_assetPool.ReturnObjectToPool( gameObject );
    }

    /***************************************************
    *   Function        : ToggleEffect 
    *   Purpose         : Start effect
    *   Parameters      : none   
    *   Returns         : Void   
    *   Date altered    : 01/08/22
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public virtual void ToggleEffect()
    {
        // Play effect 
        GetComponentInChildren<ParticleSystem>().Play();

        // Turn on the trigger
        GetComponent<Collider>().enabled = true;

        // Deactivated effect 
        StartCoroutine(DelayDeactivate());
    }
}
