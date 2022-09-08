using LayerMasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/****************************************************************************************************
* Type: Class
* 
* Name: AIData
*
* Author: Joseph Gilmore
*
* Purpose: Store & update AI details
* 
* Functions:   
*               protected  virtual void Start()
*               public  virtual void Damage( float dmg )
*               public void KillFromAnimation()
*               protected virtual void DelayDeath()
*               public float GetStartingHealth()
*               public float GetCurrentHealth()
*               public float GetAttackRate()
*               public float GetBaseDamage()
*               public void SetBaseDamage( float dmg )
*               public void SetAttackRate( float rate )
*               public float GetDefaultDamage()
*               public float GetDefaultAttackRate()
*               public void SetHealth( float health )
*               public float GetHealth()
*               public  virtual void ResetValues()
*               public void SetCrit(bool crit )
*               public FieldOfView GetFieldOfView()
*               public float GetFOVRange()
*               public EnemyAudioPool GetAudioPool()
*               public Animator GetAnimator()
*               private void OnEnable()
*               public bool GetIsDead()
*               private void ChangeMaterial( Material material )
*               private IEnumator DeathShader()
* References:
* 
* See Also: (look at behaviour trees) , AgentAI, Brute, Mage , Turret , Bomber
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 12/04/2022    JG          1.00        - Class made 
* 15/04/2022    JG          1.01        - very basic health
* 31/05/2022    JG          2.00        - changed order of code 
* 03/06/2022    JG          2.01        - Alterd Comments ready for asset bug fixing(waiting on a animation)
* 19/06/2022    JG          2.02        - Removed overload function and replaced with interface
* 04/07/2022    JG          2.03        - Started restructure for beacon 
* 19/07/2022    JG          2.04        - Added FOV here for all AI tasks 
* 26/07/2022    JG          2.05        - SOUND 
* 28/07/2022    JG          2.06        - Cleaning 
* 03/08/2022    JG          3.00        - New children 
* 10/08/2022    JG          3.01        - Clean
* 13/08/2022    JG          3.02        - Death shader fixes 
* 15/08/2022    JG          3.03        - Clean
****************************************************************************************************/
public class AIData : MonoBehaviour, IDamageable<float>
{
   
    protected float             m_healthPoints;                             // Amount of health the AI has
    protected AssetPool         m_enemiesPool;                              // Reference to pooling system
    [Header("Static AI vars")]
    [SerializeField]
    [Tooltip( "Health of AI" )]
    protected float             m_startingHealth = 100.0f;                  // Starting amount of health
    protected const float       k_minHealth = 0.0f;                         // Dead amount of HP
    protected EnemyAudioPool    m_audioPool;                                // Reference to audio pooling 
    [SerializeField]
    [Tooltip("for AI that shoot its the fire rate others just how offten it can attack")]
    protected float             m_defaultAttackRate = 0.1f;                 // Attack rate of AI 
    protected float             m_baseAttackRate;
    [SerializeField]
    [Tooltip( "Field of vision that the AI can detect objects" )]
    private float               m_fov = 135.0f;                             // FOV  
    [SerializeField]
    protected float             m_defaultDamage = 10.0f;                    // Damage of AI 
    protected float             m_baseDamage;                               // Base damage used to change at runtime 
    [Header("Asset References")]
    [SerializeField]
    protected GameObject        m_damageText;                               // Damage text effect
    protected bool              m_critHit = false;                          // Has AI been crit hit 
    protected FieldOfView       m_fieldOfView;                              // Reference to FOV
    public bool                 m_animationsOn = false;                     // Are death Anims on
    protected Animator          m_animator;                                 // Reference to animator 
    protected  bool             m_isDead = false;                           // Is the AI currently dead 
    [SerializeField]
    private Material            m_deathMaterial;                            // Shader for when AI die    
    [SerializeField]
    private Material            m_defaultMaterial;                          // Default AI material reference 
    [SerializeField][Tooltip("Speed of death shader ")][Range(0.1f, 5.0f)]    
    private float               m_dissloveSpeed = 0.1f;                     // Speed of death shader
    private const float         k_startDissloveValue = 0.0f;                   // Used to lerp death shader
    private const float         k_endtDissloveValue = 1.0f;                    // Used to lerp death shader 
    /***************************************************
    *   Function        : Start   
    *   Purpose         : Set up the class   
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected  virtual void Start()
      {
            // Reference animator
            m_animator = GetComponent<Animator>();

            // Set default values 
            ResetValues();

            // Get reference to pool 
            m_enemiesPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();

            // Set health
            m_healthPoints = m_startingHealth;

            // Reference to Audio pool 
            m_audioPool = GetComponent<EnemyAudioPool>();

            // Set up FOV
            m_fieldOfView = new FieldOfView( transform, m_fov, Layers.Player );

            // Set values
            m_baseAttackRate = m_defaultAttackRate;
            m_baseDamage = m_defaultDamage;
      
       }

   
   
    /***************************************************
    *   Function        : Damage 
    *   Purpose         : Alter the HP of the AI   
    *   Parameters      : float dmg 
    *   Returns         : void    
    *   Date altered    : 12/08/2022
    *   Contributors    : JG 
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    public  virtual void Damage( float dmg )
    {
       if( m_isDead == false )
       {
            // Reduce health 
            m_healthPoints -= dmg;


            // Create a dmg text from pool
            GameObject text = m_enemiesPool.GetObjectFromPool( m_damageText );

            // Setup  damage text effect
            text.GetComponent<DamageEffect>().CreateDamageText( dmg, m_critHit, transform.Find( "DamagePos" ).transform.position );



            // If player dead
            if ( m_healthPoints <= k_minHealth )
            {
                // Reset Health 
                m_healthPoints = m_startingHealth;

                // Play Death Sound
                m_audioPool.PlaySound( m_audioPool.m_death );


                // Play animation 
                m_animator.SetTrigger( "Death" );

                // Set character as dead 
                DelayDeath();

            }
            else
            {
                // If crit hit play crit sound 
                if ( m_critHit )
                {
                    m_audioPool.PlaySound( m_audioPool.m_critDamage );
                }
                else
                {
                    // Play normal hit sound 
                    m_audioPool.PlaySound( m_audioPool.m_damaged );

                }
               
            }


            // Reset crit hit
            m_critHit = false;
        }
       
    }
    /***************************************************
    *   Function        : KillFronAnimation
    *   Purpose         : Animation event when death anim over
    *   Parameters      : float dmg 
    *   Returns         : void    
    *   Date altered    : 13/08/2022
    *   Contributors    : JG 
    *   Notes           : Quickest solution with time left  
    *   See also        :    
    ******************************************************/
    public void KillFromAnimation()
    {
        // Start dissvoling AI
        StartCoroutine(DeathShader());   
        
    }
    /***************************************************
    *   Function        : DeathShader
    *   Purpose         : Delay the death and apply a death shader lerp
    *   Parameters      : N/A
    *   Returns         : yield return null    
    *   Date altered    : 13/08/2022
    *   Contributors    : JG 
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    private IEnumerator DeathShader()
    {
        // Reset shader
        m_deathMaterial.SetFloat( "_DissolveOpacity", k_startDissloveValue );

        // Change AI material to death shader 
        ChangeMaterial(m_deathMaterial );

        float DissolveOpacity = k_startDissloveValue;

        // While not dissolved 
        while ( DissolveOpacity < k_endtDissloveValue )
        {
            // Lerp the material to make it dissolve
            m_deathMaterial.SetFloat( "_DissolveOpacity", Mathf.Lerp( k_startDissloveValue, k_endtDissloveValue, DissolveOpacity ) );

            // Add to the lerp value 
            DissolveOpacity +=  m_dissloveSpeed *   Time.deltaTime;
            yield return null;
        }

        // Set to not active 
        m_enemiesPool.ReturnObjectToPool( gameObject );
    }
    /***************************************************
    *   Function        : DelayDeath
    *   Purpose         : Stop the AI acting
    *   Parameters      : N/A
    *   Returns         : void    
    *   Date altered    : 12/08/2022
    *   Contributors    : JG 
    *   Notes           : Quickest solution with time left  
    *   See also        :    
    ******************************************************/
    protected virtual void DelayDeath()
    {
        // Is now dead
        m_isDead = true;
     
        // Stop all actions 
        StopAllCoroutines();

    }
    /***************************************************
    *   Function        : GetStartingHealth   
    *   Purpose         : Return AI starting health   
    *   Parameters      : N/A   
    *   Returns         : float m_startingHealth    
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetStartingHealth()
    {
        return m_startingHealth;
    }
    /***************************************************
    *   Function        : GetCurrentHealth    
    *   Purpose         : return AI current updated health   
    *   Parameters      : N/A  
    *   Returns         : float m_healthPoints   
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetCurrentHealth()
    {
        return m_healthPoints;
    }
    /***************************************************
    *   Function        : GetAttackRate    
    *   Purpose         : return AI current attack rate 
    *   Parameters      : N/A  
    *   Returns         : float m_baseAttackRate
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetAttackRate()
    {
        return m_baseAttackRate;
    }
    /***************************************************
    *   Function        : GetCurrentHealth    
    *   Purpose         : return AI base damage  
    *   Parameters      : N/A  
    *   Returns         : float m_baseDamage 
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetBaseDamage()
    {
        return m_baseDamage;
    }
    /***************************************************
    *   Function        : SetBaseDamage
    *   Purpose         : Set base damage of AI
    *   Parameters      : float dmg
    *   Returns         : Void
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetBaseDamage( float dmg )
    {
        m_baseDamage = dmg;
    }
    /***************************************************
    *   Function        : SetAttackRate
    *   Purpose         : Set how often the AI attacks 
    *   Parameters      : float rate 
    *   Returns         : void
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetAttackRate( float rate )
    {
        m_baseAttackRate = rate;

    }
    /***************************************************
    *   Function        : GetDefaultDamage
    *   Purpose         : Return default dmg
    *   Parameters      : N/A
    *   Returns         : float m_defaultDamage
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetDefaultDamage()
    {
        return m_defaultDamage;
    }
    /***************************************************
    *   Function        : GGetDefaultAttackRate
    *   Purpose         : Return default attack rate
    *   Parameters      : N/A
    *   Returns         : float m_defaultAttackRate
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetDefaultAttackRate()
    {
        return m_defaultAttackRate;
    }
    /***************************************************
    *   Function        : SetHealth
    *   Purpose         : set health of AI
    *   Parameters      : health
    *   Returns         : void
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetHealth( float health ) 
    {
        if( health < m_startingHealth )
        {
            m_healthPoints = health;
           
        }
    }
    /***************************************************
    *   Function        : G=etHealth
    *   Purpose         : Return current health
    *   Parameters      : N/A
    *   Returns         : float m_healhPoints
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetHealth()
    {
        return m_healthPoints;
    }
    /***************************************************
    *   Function        : ResetValues
    *   Purpose         : reset values altered by beacon
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public  virtual void ResetValues()
    {
        
        m_baseDamage = m_defaultDamage;
        m_baseAttackRate=m_defaultAttackRate;
    }
    /***************************************************
    *   Function        : SetCrit
    *   Purpose         : set if AI is been crit hit 
    *   Parameters      : bool crit
    *   Returns         : Void
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetCrit(bool crit )
    {
        m_critHit = crit;
    }
    /***************************************************
    *   Function        : GetFieldOfView
    *   Purpose         : return fov cloass 
    *   Parameters      : N/A
    *   Returns         : FieldOfView m_fieldOfView
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public FieldOfView GetFieldOfView()
    {
        return m_fieldOfView;
    }
    /***************************************************
    *   Function        : GetFOVRange
    *   Purpose         : return fov range
    *   Parameters      : N/A
    *   Returns         : m_float fov
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetFOVRange()
    {
        return m_fov;
    }
    /***************************************************
    *   Function        : GetAudioPool
    *   Purpose         : return audio pool ref
    *   Parameters      : N/A
    *   Returns         : m_audioPool;
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public EnemyAudioPool GetAudioPool()
    {
        return m_audioPool;
    }
    /***************************************************
    *   Function        : GetAnimator
    *   Purpose         : return animator
    *   Parameters      : N/A
    *   Returns         : m_animator
    *   Date altered    : 10/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    
    public Animator GetAnimator()
    {
        return m_animator;
    }
    /***************************************************
    *   Function        : OnEnable
    *   Purpose         : Rest vars on death 
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
       // Reset values 
        m_isDead = false;
        ChangeMaterial( m_defaultMaterial);
       
    }
    /***************************************************
    *   Function        : GetIsDeath
    *   Purpose         : Rest vars on death 
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 12/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool GetIsDead()
    {
        return m_isDead;
    }
    /***************************************************
    *   Function        : ChangeMaterial
    *   Purpose         : Change All renderers attacthed to object
    *   Parameters      : Material material 
    *   Returns         : Void
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void ChangeMaterial( Material material )
    {
        /// Create a list of renderers 
        List<Renderer> meshRenderers = new List<Renderer>();

        // Add the correct render types to the list 
        meshRenderers.AddRange( GetComponentsInChildren<MeshRenderer>() );
        meshRenderers.AddRange( GetComponentsInChildren<SkinnedMeshRenderer>() );
        for ( int i = 0; i < meshRenderers.Count; i++ )
        {
            // Create a tempt array of size two to add outline
            Material[] newMaterials = new Material[ 1 ];

            // Store current material
            newMaterials[ 0 ] = material;

            // Set mesh renderer with updated materials
            meshRenderers[ i ].materials = newMaterials;
        }
    }
   
}
