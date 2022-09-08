using UnityEngine;
using System.Collections;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: TurretAI
 *
 * Author: JG 
 *
 * Purpose: Turret AI class to control vision & shooting 
 * 
 * Functions:           protected override void Start()
 *                      void Update()
 *                      protected override void DelayDeath()
 *                      private void CheckSight()
 *                      private void CheckState()
 *                      private IEnumerator ChargeAndShoot()
 *                      private void ShootEffects( bool start )
 *                      private void HandleLooseSight()
 *                      private void LerpBeamAndTarget()
 *                      private void OnDisable()                   
 * References:
 * 
 * See Also: AIData
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 02/05/2022    JG          1.00        - Created basic turret 
 * 01/05/2022    JG          2.00        - Started working on fianl version 
 * 03/06/2022    JG          2.01        - Added tracking beam
 * 06/06/2022    JG          2.02        - Removed all logic to redo it. Got it working but messy
 * 10/06/2022    JG          2.03        - Updated FOV paramater 
 * 21/06/2022    JG          2.04        - Replaced ifs with a fsm and cleaning. currently broken 
 * 23/06/2022    JG          2.05        - Restructed and cleanned class 
 * 05/07/2022    JG          2.06        - Get var data from AI data 
 * 11/07/2022    JG          2.07        - Added screen effect
 * 14/07/2022    JG          2.08        - Fixed bug where mutiple turrets would disable each others screen effects
 * 19/07/2022    JG          2.09        - Use FOV from AI data 
 * 23/07/2022    JG          2.10        - Alteration to how shooting is called 
 * 26/07/2022    JG          2.11        - added half sound 
 * 28/07/2022    JG          2.12        - Bug fixes 
 * 02/08/2022    JG          3.00        - completey restructed , now inhererits 
 * 15/08/2022    JG          3.01        - Cleaning
 ****************************************************************************************************/
public class TurretAI : AIData
{
    private BasicGun            m_gun;                              // Create a gun for the turret to use 
    [Header("Turret VFXS")]
    [SerializeField]
    private LineRenderer        m_trackingBeam;                     // Reference to tracking beam 
    [SerializeField]
    private GameObject          m_targetEffect;                     // Reference to target effect
    [SerializeField]
    private GameObject          m_flash;                            // Reference to flash shooting effect
    [SerializeField]
    private GameObject          m_chargeEffect;                     // Reference to the charge effect 
    [SerializeField][Range(1.0f,8.0f)][Tooltip(" Length of time the turret needs to track before shooting")]
    private float               m_trackingTime = 4.0f;              // Time it takes to track target before shooting
    private Coroutine           m_coroutine;
    private float               m_beamLerp = 0.0f;                  // Current lerp of beamd 
    private float               m_beamLerpRate = 2.0f;              // Rate the beam lerps out                    
    private float               m_beamYOffeset = 0.25f;             // Offset of the beam so it shoots higher than the orgrin 
    private float               m_targetLerp;                       // Separate lerp rate for the target
    private float               m_targetLerpOffset = 0.03f;         // Apply a small offset to the target so its in front of beam.
    private enum TurretState
    {
        CheckingFOV,                                                // When the turret is checking FOV
        Shooting,                                                   // The shooting coroutine active 
        BeamEnabled,                                                // Beam is currently tracking
        LostSight                                                   // Turret cannot see target
    }
    private TurretState         m_state = TurretState.CheckingFOV;  // Current turret state 
    private ChangeScreenEffect  m_lockOnEffect;                     // Reference to script that controls screen effects
    private bool                m_lockEffectApplied = false;        // Has screen effect been applied?
    private int                 an_shoot;                           // Aninmator string to hash
    /***************************************************
    *   Function        : Start   
    *   Purpose         : Set up class    
    *   Parameters      : N/A  
    *   Returns         : Void   
    *   Date altered    : 15/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Get attacthed gun
        m_gun = transform.Find( "Gun" ).GetComponent<BasicGun>();

        // Get base damage 
        m_gun.SetBaseDamage( m_defaultDamage );

        // Setup tracking beam
        m_trackingBeam = transform.GetComponentInChildren<LineRenderer>();
        m_trackingBeam.positionCount = 2;
        m_trackingBeam.sortingOrder = 1;

        // Get reference to screen effetc changer 
        m_lockOnEffect = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<ChangeScreenEffect>();

        // Setup string to hash
        an_shoot = Animator.StringToHash( "Shoot" );
    }

    /***************************************************
    *   Function        : Update   
    *   Purpose         : Check AI Vision & update state
    *   Parameters      : N/A   
    *   Returns         : Void  
    *   Date altered    : 12/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    void Update()
    {
        // If dies don't check
        if ( !m_isDead )
        {
            CheckSight();
        }
        
    }
    /***************************************************
    *   Function        : DelayDeath
    *   Purpose         : Dsiable effects appon delayed death
    *   Parameters      : N/A   
    *   Returns         : Void  
    *   Date altered    : 12/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void DelayDeath()
    {
        base.DelayDeath();

        // Disable effects
        ShootEffects( false );
    }
    /***************************************************
    *   Function        : CheckSight
    *   Purpose         : Check if turret has target in sight 
    *   Parameters      : N/A   
    *   Returns         : Void  
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void CheckSight()
    {
        if ( m_fieldOfView.OnCheckVision( transform ) )
        {
            // Make the eye look at target
            m_gun.transform.LookAt( m_fieldOfView.GetTarget() );

            // If current state loose sight update as target now in vision
            if ( m_state == TurretState.LostSight )
            {
                m_state = TurretState.CheckingFOV;
            }
        }
        else
        {
            // Target not in sight change state
            m_state = TurretState.LostSight;
        }
        
        // Check state of vision change 
        CheckState();
    }
    /***************************************************
    *   Function        : CheckState 
    *   Purpose         : Check if turret state needs to be updated 
    *   Parameters      : N/A   
    *   Returns         : Void  
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void CheckState()
    { 
        switch ( m_state )
        {
            case TurretState.CheckingFOV:
                {
                    // Reset corourtine 
                    if ( m_coroutine != null )
                    {
                        // stop current routine 
                        StopCoroutine( m_coroutine );
                    }
                    // Start the shooting 
                    m_coroutine = StartCoroutine( ChargeAndShoot() );
                }
                break;

            case TurretState.Shooting:
                // Do nothing 
                break;
            case TurretState.BeamEnabled:
                {
                    // Setup effects of tracking beam
                    LerpBeamAndTarget();

                }
                break;
            case TurretState.LostSight:
                {
                    // Reset turret
                    HandleLooseSight();
                }
                break;
        }
    }
    /***************************************************
   *   IEnumerator     : ChargeAndShoot
   *   Purpose         : Complete the charge and shoot cycle and apply vfx 
   *   Parameters      : N/A   
   *   Returns         : fireRate,beam,shootDelay, null
   *   Date altered    : 07/08/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    private IEnumerator ChargeAndShoot()
    {
        // Switch state to shooting 
        m_state = TurretState.Shooting;

        // Set current damage or turret to gun
        m_gun.SetBaseDamage( m_baseDamage );

        // Applt firerate 
        WaitForSeconds fireRate = new WaitForSeconds(m_baseAttackRate);
        yield return fireRate;

        // Apply tracking beam
        m_state = TurretState.BeamEnabled;

        // Set positions of beam 
        m_trackingBeam.SetPosition( 0, m_gun.GetFirePos().position );
        m_trackingBeam.SetPosition( 1, m_gun.GetFirePos().position );
        m_trackingBeam.enabled = true;
          

        // Start shoot effects
        ShootEffects( true );


        // Wait for beam to lock
        WaitForSeconds beam = new WaitForSeconds( m_trackingTime );
        yield return beam;

        // Stop shoot effects
        ShootEffects( false );

      

        // Disable tracking 
        m_trackingBeam.enabled = false;

        // Apply a small gap between tracking & shoot so player can dodge 
        WaitForSeconds shootDelay = new WaitForSeconds( m_flash.GetComponentInParent<ParticleSystem>().main.duration );
        m_flash.GetComponentInParent<ParticleSystem>().Play( true );


        // Do shoot anim
        m_animator.SetTrigger( an_shoot );

        yield return shootDelay;

     

        // Fire gun
        m_gun.Shoot( m_gun.GetFirePos() );


        // Play shoot sound
        m_audioPool.PlaySound( m_audioPool.m_attack2 );

        // Reset lerp
        m_beamLerp = 0f;

        // Reset state
        m_state = TurretState.CheckingFOV;
        yield return null;
    }
    /***************************************************
     *   IEnumerator     : ShootEffects
     *   Purpose         : Toggle effects on & off  
     *   Parameters      : bool start
     *   Returns         : Void 
     *   Date altered    : 28/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private void ShootEffects( bool start)
    {
        // Enable effects 
        if (start)
        {
            m_chargeEffect.GetComponent<ParticleSystem>().Play();
            m_targetEffect.GetComponent<ParticleSystem>().Play( true );
            m_trackingBeam.enabled = true;
            m_audioPool.PlaySound( m_audioPool.m_attack1 );
        }
        else
        {
            // Disable effects 
            m_chargeEffect.GetComponent<ParticleSystem>().Stop();
            m_targetEffect.GetComponent<ParticleSystem>().Stop( true );
            m_trackingBeam.enabled = false;

            // Reset screen effect only if applied 
            if ( m_lockEffectApplied )
            {
                m_lockOnEffect.ToggleEffect();
                m_lockEffectApplied = false;
            }

            // Stop the attack sound 
            m_audioPool.StopSound( m_audioPool.m_attack1 );
        }
    }
    /***************************************************
     *   IEnumerator     : HandleLooseSight
     *   Purpose         : When turret loose sight 
     *   Parameters      : N/A
     *   Returns         : Void 
     *   Date altered    : 23/06/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private void HandleLooseSight()
    {
        // Stop shooting if target goes out of site. 
        if (m_coroutine != null)
        {
            StopCoroutine( m_coroutine );
        }

        // Reset Lerp
        m_beamLerp = 0f;

        // Stop gun ripple 
        m_gun.GetComponentInChildren<ParticleSystem>().Stop();

        // Stop other effects
        ShootEffects( false );

        // Reset state 
        m_state = TurretState.CheckingFOV;
    }
    /***************************************************
    *   IEnumerator     : LerpBeamAndTarget 
    *   Purpose         : Lerp beam and target effects
    *   Parameters      : N/A
    *   Returns         : Void 
    *   Date altered    : 23/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
   ******************************************************/
    private void LerpBeamAndTarget()
    {
        // If beam not finished lerp
        if (m_beamLerp < 1)
        {
            // Lerp beam 
            m_beamLerp += m_beamLerpRate * Time.deltaTime;
            m_targetLerp = m_beamLerp;
        }
        else
        {
            // Apply small offset to target lerp
            m_targetLerp = m_beamLerp - m_targetLerpOffset;


            // Apply lock on to screen 
            m_lockOnEffect.ToggleEffect( ChangeScreenEffect.Effect.laser );
            m_lockEffectApplied = true;
        }
        // If charge effect applied but not currently set set effect 
        if( m_lockEffectApplied == true && m_lockOnEffect.GetlastEffect() != ChangeScreenEffect.Effect.laser )
        {
            m_lockOnEffect.ToggleEffect( ChangeScreenEffect.Effect.laser );
        }
            
        
        // Work out where to point beam 
        Vector3 target = new Vector3( m_fieldOfView.GetTarget().position.x, m_fieldOfView.GetTarget().position.y + m_beamYOffeset, m_fieldOfView.GetTarget().position.z );

        // Lerp target
        m_targetEffect.transform.position = Vector3.Lerp( m_gun.GetFirePos().position, target, m_targetLerp );

        // Track Positions for beam
        m_trackingBeam.SetPosition( 0, m_gun.GetFirePos().position );
        m_trackingBeam.SetPosition( 1, Vector3.Lerp(m_gun.GetFirePos().position, target, m_beamLerp ));
    }
    /***************************************************
   *   IEnumerator     : OnDisable
   *   Purpose         : Ensure all effects have stopped
   *   Parameters      : N/A
   *   Returns         : Void 
   *   Date altered    : 11/07/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
  ******************************************************/
    private void OnDisable()
    {
        ShootEffects( false );
    }
}
