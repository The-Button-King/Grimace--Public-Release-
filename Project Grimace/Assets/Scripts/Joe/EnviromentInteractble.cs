using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: EnviromentInteractble
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Used as a parent for different hazards in the enviroment and used by brute for an attack 
 * 
 * Functions:  protected virtual void Awake()
 *             public void Damage( float damage )
 *             public void TriggerEffect()
 *             public virtual void OnEnable()
 *             public void SetThrown( bool thrown )
 *             private void OnCollisionEnter( Collision collision )
 *             public void SetThrownBy( GameObject gameObject )
 *             public bool GetDead()
 *             
 * 
 * References:
 * 
 * See Also:  IDamageble 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * ----------   --------    -------     ----------------------------------------------
 * 10/06/2022   JG          1.00        Initial creation   
 * 15/06/2022   WH          1.10        Added generic spawing of effect
 * 19/06/2022   JG          1.11        Cleaned and added Interface
 * 23/06/2022   WH          1.12        Added starting health
 * 24/06/2022   WH          1.13        Added reset function
 * 02/07/2022   JG          1.14        Removed colour 
 * 03/07/2022   JG          1.15        Added material change 
 * 03/07/2022   WH          1.16        Reset changed to OnEnable
 * 01/08/2022   JG          1.17        Made hazard throwable 
 * 10/08/2022   WH          1.18        Changed start to awake
 * 15/08/2022   WH          1.19        Fixed material setting onenable and added getDead
 * 16/08/2022   JG          1.20        Bug Fix
 * 24/08/2022   JG          1.21        More bug fixes 
 ****************************************************************************************************/
public class EnviromentInteractble : MonoBehaviour, IDamageable<float>
{
    [SerializeField]
    protected  GameObject   m_effectToTrigger;           // Reference to gameobjec
    protected float         m_health;                    // Amount of hit points for prop to destroy
    protected const  float  k_startHealth = 2.0f;        // Always requries two hits 
    protected AssetPool     m_assetPool;                 // Reference to asser pool
    protected const float   k_minHealth = 0.0f;          // Min amount of hit points
    [Header("Material Changes")]
    [SerializeField]
    private Material        m_startMaterial;             // Reference to starting material
    [SerializeField]
    private Material        m_endMaterial;               // Referennce to end material 
    private bool            m_thrown = false;            // Has the hazard been thrown by AI    
    private GameObject      m_thrownBy;                  // Who threw the hazard 
    [Header("Thrown stats")]
    [SerializeField][Range(1.0f,100.0f)][Tooltip("Damage of impact when hits object from being thrown")]
    private float           m_impactDamage = 40.0f;      // The amount of damage delt by the impact when thrown. 
    private bool            m_pickedUp = false;

    /***************************************************
    *   Function        : Start   
    *   Purpose         : Set up class  
    *   Parameters      : N/A 
    *   Returns         : Void   
    *   Date altered    : 10/08/2022
    *   Contributors    : JG
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    protected virtual void Awake()
    {
        // Set starting health
        m_health = k_startHealth;

        // Get reference to pool in scene 
        m_assetPool = GameObject.Find("Pool").GetComponent<AssetPool>();
    }
    /***************************************************
    *   Function        : Damage    
    *   Purpose         : Apply damage to the enviroment prop   
    *   Parameters      : float damage   
    *   Returns         : Void   
    *   Date altered    : 03/07/2022
    *   Contributors    : JG WH
    *   Notes           : Used from inteface , Hazzards always 2 hit to kill thats why damage param not used    
    *   See also        :    
    ******************************************************/
    public void Damage( float damage )
    {
      
        // Apply Damage ( always going to be 2 hit as it feels wrong to use gun damage)
        m_health --;

       // Update material to danger 
       GetComponentInChildren<MeshRenderer>().material = m_endMaterial;
        
        // Kill prop
        if (m_health <= k_minHealth)
        {
            // Do prop effect 
            TriggerEffect();

            // Retun hazard to pool
            m_assetPool.ReturnObjectToPool( gameObject );
        }
   }

    /***************************************************
    *   Function        : TriggerEffect   
    *   Purpose         : Trigger effect of prop 
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 01/08/2022
    *   Contributors    : JG WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void TriggerEffect()
    {
        // Get hazzard from pool and activated it 
       m_assetPool.GetObjectFromPool( m_effectToTrigger, transform.position ).GetComponent<IToggleable>().ToggleEffect();
    }

    /***************************************************
    *   Function        : OnEnable   
    *   Purpose         : Resets object as if it was just spawned in
    *   Parameters      : none   
    *   Returns         : Void   
    *   Date altered    : 15/08/22
    *   Contributors    : WH JG 
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public virtual void OnEnable()
    {
        // Set health
        m_health = k_startHealth;

    }
    /***************************************************
    *   Function        : SetThrown 
    *   Purpose         : Has it been chucked by a brute
    *   Parameters      : bool thrown
    *   Returns         : Void   
    *   Date altered    : 31/07/22
    *   Contributors    :  JG 
    *   Notes           :    
    *   See also        : ThrowHazard   
    ******************************************************/
    public void SetThrown( bool thrown )
    {
        m_thrown = thrown;  
    }
    /***************************************************
    *   Function        : OnCollisionEnter
    *   Purpose         : When the hazard been thrown check what it hits 
    *   Parameters      : bool thrown
    *   Returns         : Void   
    *   Date altered    : 15/08/22
    *   Contributors    : JG 
    *   Notes           :    
    *   See also        : ThrowHazard   
    ******************************************************/
    private void OnCollisionEnter( Collision collision )
    {
        // If the hit object does not equal null
        if( collision.collider.transform.parent != null && collision.transform.root != null )
        {
            // Has the object been thrown and not hit the brue throwing it 
            if ( m_thrown  && collision.transform.root.gameObject != m_thrownBy )
            {
                // If hit a damageble object 
                if( collision.transform.root.GetComponentInChildren<IDamageable<float>>() != null )
                {
                    // Deal impact damage 
                    collision.transform.root.GetComponentInChildren<IDamageable<float>>().Damage( m_impactDamage );
                }

                // Set health to min
                m_health = k_minHealth;

                // Trigger hazard effect 
                TriggerEffect();

                // Return to the pool 
                m_assetPool.ReturnObjectToPool( gameObject );
            }
        }
       
    }
    /***************************************************
    *   Function        : SetThrownBy
    *   Purpose         : Set the game object thats thrown the hazard 
    *   Parameters      : bool thrown
    *   Returns         : Void   
    *   Date altered    : 31/07/22
    *   Contributors    :  JG 
    *   Notes           :    
    *   See also        : ThrowHazard   
    ******************************************************/
    public void SetThrownBy( GameObject gameObject )
    {
        m_thrownBy = gameObject;
    }
    /***************************************************
   *   Function        : SetThrownBy
   *   Purpose         : hhas hazard been picked up
   *   Parameters      : bool thrown
   *   Returns         : Void   
   *   Date altered    :24/08/22
   *   Contributors    :  JG 
   *   Notes           :    
   *   See also        : ThrowHazard   
   ******************************************************/
    public void SetPickedUp( bool pickedUP)
    {
        m_pickedUp = pickedUP;
    }
    /***************************************************
*   Function        : IsPickedUp()
*   Purpose         : hhas hazard been picked up
*   Parameters      : bool thrown
*   Returns         : Void   
*   Date altered    :24/08/22
*   Contributors    :  JG 
*   Notes           :    
*   See also        : ThrowHazard   
******************************************************/
    public bool IsPickedUp()
    {
        return m_pickedUp;
    }
    /***************************************************
    *   Function        : GetDead   
    *   Purpose         : Returns true if the interactable has "died"
    *   Parameters      : N/A 
    *   Returns         : bool dead
    *   Date altered    : 15/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public bool GetDead()
    {
        // If health is <= minHealth, interactable is dead
        return m_health <= k_minHealth;
    }
}
