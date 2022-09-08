using LayerMasks;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/****************************************************************************************************
* Type: Class
* 
* Name: Beacon
*
* Author: Joseph Gilmore
*
* Purpose: Apply a buff in a radius towards the AI that will last x amount of time
* 
* Functions:            private void OnEnable()
*                       protected override void Start()
*                       private IEnumerator WaveTime()
*                       void Update()
*                       private void CheckHealBuff()
*                       protected override void ApplyEffect( Collider[] hitColliders, int colliderCount )
*                       private void StopEffects( AIData AI )
*                       private IEnumerator ApplyEffect( AIData dataClass )
*                       public void Damage( float Damage )
*                       private void CheckLightColor()
*                       private void SelectEffect()
*                       private void OutlineAI( AIData AI, Material material )
*                       private IEnumerator DelayedDeath()
*                       private void SetAllChildrenState( bool state )
*                       public void SetCrit (bool crit )
*                       private void ApplyLineRendererMaterial()
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
* 04/07/2022    JG           1.00        - Created empty class and made some place holder Vars
* 05/07/2022    JG           1.01        - Added functions   
* 06/07/2022    JG           1.02        - Light change on health 
* 21/07/2022    JG           2.00        -Completey restructured for inheritance
* 22/07/2022    JG           2.01        - Added AI outlines
* 26/07/2022    JG           2.02        - Sound
* 28/07/2022    JG           2.04        - delayed death 
* 29/07/2022    JG           2.05        - Added crit hit and damage text
* 31/07/2022    JG           2.06        - Added line materials for each effect 
* 13/08/2022    JG           2.07        - Fixed effect bug by changing to renderer
* 15/08/2022    JG           2.08        - Cleaned
* 16/08/2022    JG           2.09        - Bug fixs
* 24/08/2022    JG           2.10        - Bug fixs
****************************************************************************************************/
public class Beacon : ShockWave, IDamageable<float>
{

    [SerializeField]
    [Tooltip( "The rate a shock wave is made" )]
    [Range( 2.0f, 10.0f )]
    private float                   m_checkRate = 5.0f;                     // The rate the wave checks for AI
    private float                   m_healthPoints;                         // Health the beacon 
    private const float             k_startingHealth = 100.0f;              // Starting amount health
    private AssetPool               m_assetPool;                            // Reference to asset pool
    [Header("Buffs")]
    [SerializeField]
    [Tooltip( "Times this amount to the base speed of AI effected" )]
    [Range( 1.1f, 2.0f )]
    private float                   m_speedMutiplyer = 1.2f;                // Speed increase buff
    [SerializeField]
    [Tooltip( "Rate of healing buff" )]
    [Range( 1.05f, 2.0f )]
    private float                   m_healRate = 1.05f;                     // Heals the AI at the rate
    [SerializeField]
    [Tooltip( "Increase base damage of AI by mutiplyer" )]
    [Range( 1.1f, 2.0f )]
    private float                   m_damageMutiplyer = 1.5f;               // Increase AI damage 
    [SerializeField]
    [Tooltip( "Increse Attack rate of AI" )]
    [Range( 0.1f, 0.9f )]
    private float                   m_attackRateIncrease = 0.9f;            // Increase attack rate by decreasing value
    private bool                    m_heal = false;                         // Needs to apply healing
    [SerializeField]
    [Tooltip( "Buff apply time" )]
    [Range( 2.0f, 8.0f )]
    private float                   m_effectTime = 5.0f;                     // Length to apply buff 
    private List<AIData>            m_healList;                             // List of effected AI data classes
    private Light                   m_beaconLight;                          // Reference to beacon light
    private int                     m_lightColourCount = 3;                 // Amount of colours the light canchange to
    [Header("AI outlines")]
    [SerializeField]
    private Material                m_outlineEffectSpeed;                   // Enemies outlines references
    [SerializeField]
    private Material                m_outlineEffectHeal;
    [SerializeField]
    private Material                m_outlineEffectDamage;
    [SerializeField]
    private Material                m_outlineEffectAttackRate;
    [SerializeField]
    private GameObject              m_damageText;                           // Reference to damage text effect             
    private enum EffectToApply      {Speed,Heal,Damage,Attack, };           // Enums of effect types
    private EffectToApply           m_effectToApply;                        // Current effect to apply
    private EnemyAudioPool          m_audioPool;                            // Reference to audio pool
    private List<AIData>            m_currentBuffedAI = new List<AIData>(); // List of buffed by AI
    private bool                    m_crit = false;                         // Has the beacon been crit hit 
    [Header( "Wave Line Materials" )]
    [SerializeField]
    private Material                m_speedLineMaterial;                    // Materials for the wave effect 
    [SerializeField]
    private Material                m_damageLineMaterial;
    [SerializeField]
    private Material                m_healLineMaterial;
    [SerializeField]
    private Material                m_attackLineMaterial;
    /***************************************************
    *   Function        : OnEnable()
    *   Purpose         : Enable children    
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 21/07/22
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        SetAllChildrenState( true );
    }
    /***************************************************
    *   Function        : Start
    *   Purpose         : To set up the class and find the right references    
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 24/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Enable effect 
        m_shockWaveEffect.enabled = true;

        // Set shockwave position 
        m_shockwavePos = transform.position;

        // Get references 
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();
        m_audioPool = GetComponent<EnemyAudioPool>();
        m_healList = new List<AIData>();

        // Set health
        m_healthPoints = k_startingHealth;

        // Setup light
        m_beaconLight = GetComponentInChildren<Light>();

        // Set starting colour 
        m_beaconLight.color = Color.blue;

        // Set AI layer (each AI have a single enviroment hazard collider)
        m_layerMasks = LayerMask.GetMask( Layers.EnviromentHazard.ToString() );

        // Select a random effect 
        SelectEffect();

        // Update material 
        ApplyLineRendererMaterial();

        // Start beacon 
        StartCoroutine( WaveTime() );

    }
    /***************************************************
     *   IEnumerator     : WaveTime
     *   Purpose         : Do buff shock waves at set intervals  
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 26/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private IEnumerator WaveTime()
    {
        // Set wait time to check time 
        WaitForSeconds waitTime = new WaitForSeconds( m_checkRate );

        while ( true )
        {

            yield return waitTime;

            // Start Wave
            StartCoroutine( StartShockWave() );

            // Play beacon sound 
            m_audioPool.PlaySound( m_audioPool.m_attack1 );
        }
    }
    /***************************************************
     *   Function        : Update
     *   Purpose         : Update beacon mainly used to apply heal buff
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 01/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    void Update()
    {
        // Check if heal buff active 
        CheckHealBuff();
 
    }
    /***************************************************
     *   Function        : CheckHealBuff()
     *   Purpose         : Check if heal buffed applied 
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 01/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void CheckHealBuff()
    {
        // If heal buff applied 
        if ( m_heal )
        {
            // Loop through AI needing to heal
            foreach ( AIData dataClass in m_healList )
            {
                // Check if it was null 
                if ( dataClass.transform.GetComponent<AIData>() != null )
                {
                    float health = dataClass.GetHealth() * m_healRate;

                    // Apply health buff
                    dataClass.SetHealth( health );
                }

            }
        }

    }
    /***************************************************
     *   Function        : ApplyEffect
     *   Purpose         : Use detected object from parent and apply effect 
     *   Parameters      : Collider[] hitColliders, int colliderCount  
     *   Returns         : Void   
     *   Date altered    : 21/07/2022
     *   Contributors    : JG
     *   Notes           :  Renamed from GetAIInBeacon  
     *   See also        :    
     ******************************************************/
    protected override void ApplyEffect( Collider[] hitColliders, int colliderCount )
    {

        // Loop Through hit colliders 
        for ( int i = 0; i < colliderCount; i++ )
        {
            if ( hitColliders[ i ] != null )
            {
                // Check if its hit AI 
                if ( hitColliders[ i ].transform.root.GetComponentInChildren<AIData>() != null )
                {
                    // If AI not already been hit 
                    if ( !m_hitObjects.Contains( hitColliders[ i ].transform.gameObject ) )
                    {
                        // Apply AI effect
                        StartCoroutine( ApplyEffect( hitColliders[ i ].transform.root.GetComponentInChildren<AIData>() ) );

                        // Add AI to list so it does not get hit twice by same wave
                        m_hitObjects.Add( hitColliders[ i ].transform.gameObject );
                    }
                }
            }

        }

    }




    /***************************************************
     *   Function        : StopEffects
     *   Purpose         : Stop applying buffs to AI
     *   Parameters      : AIData AI
     *   Returns         : Void   
     *   Date altered    : 22/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void StopEffects( AIData AI )
    {
        // If current effect is not heal
        if ( m_effectToApply != EffectToApply.Heal )
        {
            AI.ResetValues();

        }
        else
        {
            // Remove AI from heal list 
            m_healList.Remove( AI );
        }
        // Reset outline
        OutlineAI( AI, null );

        // Remove list 
        m_currentBuffedAI.Remove( AI );


    }
    /***************************************************
     *   IEnumerator     : ApplyEffect
     *   Purpose         : Apply buff to list of AI
     *   Parameters      : AIData dataClass 
     *   Returns         : Void   
     *   Date altered    : 02/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private IEnumerator ApplyEffect( AIData dataClass )
    {
        // Check if not null
        if ( dataClass != null )
        {
            // Add to current ai 
            m_currentBuffedAI.Add( dataClass );

            //Apply effect for for select item to data class 
            switch ( m_effectToApply )
            {
                case EffectToApply.Speed:
                    {
                        // If AI can move 
                        if(dataClass.transform.GetComponent<AgentAI>() != null )
                        {
                            AgentAI agentAI = dataClass.transform.GetComponent<AgentAI>();

                            // Apply buff
                            agentAI.SetSpeed( agentAI.GetDefaultSpeed() * m_speedMutiplyer );

                            // SetOutlineEffect 
                            OutlineAI( dataClass, m_outlineEffectSpeed );

                        }
                    }
                    break;
                case EffectToApply.Damage:
                    {
                        // Apply buff
                        dataClass.SetBaseDamage( dataClass.GetDefaultDamage() * m_damageMutiplyer );

                        // SetOutlineEffect 
                        OutlineAI( dataClass, m_outlineEffectDamage );
                    }
                    break;
                case EffectToApply.Attack:
                    {
                        // Apply buff
                        dataClass.SetAttackRate( dataClass.GetDefaultAttackRate() * m_attackRateIncrease );

                        // SetOutlineEffect 
                        OutlineAI( dataClass, m_outlineEffectAttackRate );
                    }
                    break;
                case EffectToApply.Heal:
                    {
                        // Can heal
                        m_heal = true;
                        m_healList.Add( dataClass );

                        // SetOutlineEffect 
                        OutlineAI( dataClass, m_outlineEffectHeal );
                    }
                    break;

            }


        }

        WaitForSeconds wait = new WaitForSeconds( m_effectTime );

        // After time stop applying buffs 
        yield return wait;

        StopEffects( dataClass );


        yield return null;
    }
    /***************************************************
     *   Function        : Damage  
     *   Purpose         : Damage the beacon   
     *   Parameters      : float damage    
     *   Returns         : Void  
     *   Date altered    : 29/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void Damage( float Damage )
    {
        // Reduece health
        m_healthPoints -= Damage;

        // Update beacon light
        CheckLightColor();


        // Create a dmg text from pool
        GameObject text = m_assetPool.GetObjectFromPool( m_damageText );

        // Setup  damage text effect
        text.GetComponent<DamageEffect>().CreateDamageText( Damage, m_crit, transform.Find( "DamagePos" ).transform.position );

        if ( m_healthPoints < 0 )
        {
            if ( m_healthPoints <= 0 )
            {
                // Reset Health 
                m_healthPoints = k_startingHealth;

                // If AI still being effected 
                if ( m_currentBuffedAI.Count > 0 )
                {
                    // Delay death
                    StartCoroutine( DelayedDeath() );
                }
                else
                {
                    m_assetPool.ReturnObjectToPool( gameObject );
                }

                // Play death sound
                m_audioPool.PlaySound( m_audioPool.m_death );

            }
        }
        // Play hurt sound
        m_audioPool.PlaySound( m_audioPool.m_damaged );
    }
    /***************************************************
        *   Function        : CheckLightColor
        *   Purpose         : Change light colour 
        *   Parameters      : float damage    
        *   Returns         : Void  
        *   Date altered    : 05/07/2022
        *   Contributors    : JG
        *   Notes           :    
        *   See also        :    
        ******************************************************/
    private void CheckLightColor()
    {
        // Split the health into intervals 
        float interval = k_startingHealth / m_lightColourCount;

        // If very low health
        if ( m_healthPoints <= interval )
        {
            // Change light color
            m_beaconLight.color = Color.red;
        }
        // If medium health 
        else if ( m_healthPoints <= interval * 2 )
        {
            m_beaconLight.color = Color.yellow;
        }
        else
        {
            // If above medium health 
            m_beaconLight.color = Color.blue;
        }

    }
    /***************************************************
    *   Function        : SelectEffect
    *   Purpose         : Get a random effect for the beacon 
    *   Parameters      : float damage    
    *   Returns         : Void  
    *   Date altered    : 04/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void SelectEffect()
    {
        // Create an  array of enum effects
        Array values = Enum.GetValues( typeof( EffectToApply ) );

        // Make a new random 
        System.Random random = new System.Random();

        // Assign random effect
        m_effectToApply = ( EffectToApply )values.GetValue( random.Next( values.Length ) );


    }
    /***************************************************
     *   IEnumerator     : ApplyEffect
     *   Purpose         : Apply buff to list of AI
     *   Parameters      : AIData dataClass 
     *   Returns         : Void   
     *   Date altered    : 16/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void OutlineAI( AIData AI, Material material )
    {
        // Check if AI is not in the process of death
        if(AI.GetIsDead() == false )
        {

            // Get all renderers on AI
            List<Renderer> meshRenderers = new List<Renderer>();

            // Add the correct render types to the list 
            meshRenderers.AddRange( AI.GetComponentsInChildren<MeshRenderer>() );
            meshRenderers.AddRange( AI.GetComponentsInChildren<SkinnedMeshRenderer>() );

            for ( int i = 0; i < meshRenderers.Count; i++ )
            {    // ( there is a if null statement due to the build not allowing materials to be null even though there is a base material in the array.)
                if(material == null )
                {
                    // Create a tempt array
                    Material[] newMaterials = new Material[ 1 ];

                    // Store current material
                    newMaterials[ 0 ] = meshRenderers[ i ].materials[ 0 ];

                    // Set mesh renderer with updated materials
                    meshRenderers[ i ].materials = newMaterials;
                }
                else
                {
                    // Create a tempt array of size two to add outline
                    Material[] newMaterials = new Material[ 2 ];

                    // Store current material
                    newMaterials[ 0 ] = meshRenderers[ i ].materials[ 0 ];

                    // Add new outline material
                    newMaterials[ 1 ] = material;

                    // Set mesh renderer with updated materials
                    meshRenderers[ i ].materials = newMaterials;
                }
                
            }
        }
       
      
    }
 
    /***************************************************
    *   IEnumerator     : Delayed death
    *   Purpose         : Delay setting the beacon not active 
    *   Parameters      : N/A
    *   Returns         : Void   
    *   Date altered    : 28/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator DelayedDeath()
    {
        // Set all  children not active 
        SetAllChildrenState( false );

        // While there is still AI using beacon
        while ( m_currentBuffedAI.Count > 0 )
        {
            // do nothing 
            yield return null;
        }

        // Set to not active once all effects are finished 
        m_assetPool.ReturnObjectToPool( gameObject );

    }
    /***************************************************
    *   Function        :  SetAllChildrenState
    *   Purpose         : Set the children to active or not
    *   Parameters      : bool state 
    *   Returns         : Void   
    *   Date altered    : 28/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void SetAllChildrenState( bool state )
    {
        // Loop through all the children 
        for ( int i = 0; i < transform.childCount; i++ )
        {
            // Get child 
            GameObject child = transform.GetChild( i ).gameObject;

            // If child is not null
            if ( child != null )
            {
                // Update state 
                child.SetActive( state );
            }

        }

    }
    /***************************************************
   *   Function        :  SetCrit
   *   Purpose         : Set beacon crit hit
   *   Parameters      : bool crit 
   *   Returns         : Void   
   *   Date altered    : 29/07/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    public void SetCrit (bool crit )
    {
        m_crit = crit;
    }
    /***************************************************
   *   Function        : ApplyLineRendererMaterial
   *   Purpose         : ^
   *   Parameters      : NA
   *   Returns         : Void   
   *   Date altered    : 31/07/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    private void ApplyLineRendererMaterial()
    {
        switch ( m_effectToApply )
        {
            case EffectToApply.Heal:
                {
                        m_shockWaveEffect.material = m_healLineMaterial;
                }
                break;
            case EffectToApply.Attack:
                {
                    m_shockWaveEffect.material = m_attackLineMaterial;
                }
                break;
            case EffectToApply.Damage:
                {
                    m_shockWaveEffect.material = m_damageLineMaterial;
                }
                break;
            case EffectToApply.Speed:
                {
                    m_shockWaveEffect.material = m_speedLineMaterial;
                }
                break;

        }
    }
}
