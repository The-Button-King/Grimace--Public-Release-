using CameraShake;
using UnityEngine;
/****************************************************************************************************
* Type: Class
* 
* Name: ChargeRifle
*
* Author: Joseph Gilmore 
*
* Purpose: Charge rifle firing mode 
* 
* Functions:   protected override void Start()
*              private void OnEnable()
*              protected override void  Update()
*              private void UpdateLaser()
*              private void LateUpdate()
*              public override void  FireGun()
*              private void ChargeLaser()
*              public override  void StopShooting()
*              private void OnDisable()
*              private void WindUp( bool play = true )
*              public (float, float) GetChargeStats()
*              public void SetChargeStats( (float, float) stats )
*              public void GetCarryOverData( (int, int, int, float, float, float, float) stats )
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
* 29/06/2022    JG          1.00        - Created class 
* 01/07/2022    JG          1.01        - Altered to work with weapon switching 
* 02/07/2022    JG          1.02        - Added on disable function 
* 04/07/2022    JG          1.03        - Now updates UI and sets paramaters 
* 07/07/2022    JG          1.04        - Added updated effects and removed slider
* 12/07/2022    WH          1.05        - Added GetStats and charge vars no longer consts
* 18/07/2022    JG          1.06        - Moved charger laser to late update and fixed synce issues 
* 27/07/2022    JG          1.07        - Sound
* 28/07/2022    JG          1.08        - screen shake 
* 29/07/2022    JG          1.09        - Reload overide bug
* 31/07/2022    JG          1.10        - minor improvements , bug fixes , disabled flash 
* 02/08/2022    WH          1.09        - Adding carry over data
* 13/08/2022    JG          1.10        - Firerate 
* 14/08/2022    JG          1.11        - Cleaning
* 17/08/2022    JG          1.12        - Bug fixs
****************************************************************************************************/
public class ChargeRifle : GunStats
{

    private LineRenderer            m_chargeLaser;              // Reference to vfx 
    private float                   m_holdTime;                 // The time the mouse has been hold down for 
    [Header("Charge Rifle Stats")]

    [SerializeField][Range(0.1f,5.0f)][Tooltip("Length of time it needs to be charged to fire")]
    private float                   m_chargeTime = 2.0f;        // The amount of time the mouse needs to be held down
    [SerializeField][Range( 3.0f, 8.0f )] [Tooltip( "If charge has been held for to long overheat time" )]
    private float                   m_overChargeTime = 3.5f;    // Amount of time when the gun gets overheated 
    private Animator                m_chargeAnimator;           // Referene to charge animator 
    private GameObject              m_chargeEffect;             // Charge particle effect parent
    private bool                    m_windUpPlayed = false;     // Has windup been played 
    private bool                    m_overHeatedPlayed = false;
    [SerializeField][Tooltip( "Position to charge rifle from " )]
    private Transform               m_chargePosition;           // Position to charge rifle from 
    private GunManager              m_gunManager;               // Reference to gun manager 
    private int                     an_enableCharge;            // String to has for charge anim
    /***************************************************
    *   Function        : Start    
    *   Purpose         : Setup class and call up the hierarchy   
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Start()
    {
        // Call parent 
        base.Start();

        // Get data from last level
        DataHolder data = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        if ( data.GetLevel() > 1 )
        {
            GetCarryOverData( data.ApplyDataChargeRifle() );
        }

        // Disable flash 
        m_flashPlay = false;

        // Set up laser position
        m_chargeAnimator = transform.parent.parent.GetComponentInChildren<Animator>();
        

        // Get charge effect
        m_chargeEffect = transform.parent.Find( "WindUp" ).gameObject;


        // Set up charge laser 
        m_chargeLaser = transform.root.GetComponentInChildren<LineRenderer>();

        m_chargeLaser.positionCount = 2;
        m_chargeLaser.sortingOrder = 1;
        m_chargeLaser.useWorldSpace = true;

        // Get Gun manager reference 
        m_gunManager = transform.root.GetComponentInChildren<GunManager>();

        // Set string to hash
        an_enableCharge = Animator.StringToHash( "enable" );
    }

    /***************************************************
   *   Function        : OnEnable  
   *   Purpose         : When scripted is enabled 
   *   Parameters      : N/A  
   *   Returns         : Void   
   *   Date altered    : 20/07/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    private void OnEnable()
    {
        // Ensures effect is synced 
        Application.onBeforeRender += UpdateLaser;
    }
    /***************************************************
    *   Function        : Update  
    *   Purpose         : Update charge and call up the hierachy   
    *   Parameters      : N/A  
    *   Returns         : Void   
    *   Date altered    : 17/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void  Update()
    {
        // Call parent 
        base.Update();

        // Apply firerate 
        m_timer += Time.deltaTime;

       if(m_state != GunState.Reloading   )
        {
            // Get hold time from the gun manager 
            m_holdTime = m_gunManager.GetHoldTime();

            // If holding down and not already fireing update state 
            if ( m_holdTime > 0 && m_state == GunState.Idle && m_currentAmmo > 0 )
            {
                m_state = GunState.Fireing;
            }
        }

       


    }
    /***************************************************
    *   Function        : UpdateLaser  
    *   Purpose         : Update function just for the laser to synce it correctly 
    *   Parameters      : N/A  
    *   Returns         : Void   
    *   Date altered    : 14/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateLaser()
    {
      // if currently fireing 
       if(m_state == GunState.Fireing )
       {
           // Get raycast of gun
           Vector3 target = FireGunRay( transform.parent.parent ).point;

           // Set start & and position of the ray 
           m_chargeLaser.SetPosition( 0, m_chargePosition.position );
           m_chargeLaser.SetPosition( 1, target );
       }
      
                  
          
    }
    /***************************************************
    *   Function        : LateUpdate() 
    *   Purpose         : Apply effects 
    *   Parameters      : N/A  
    *   Returns         : Void   
    *   Date altered    : 14/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void LateUpdate()
    {
        // Check state 
        if ( m_state==GunState.Fireing)
        {
            // Charge laser 
            ChargeLaser();
        }
        
        
    }
    /***************************************************
    *   Function        : FireGun   
    *   Purpose         : Fire the gun in charge rifle mode    
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 17/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void  FireGun()
    {
        // Call parent
        base.FireGun();

        // Get hold time from the gun manager 
        m_holdTime = m_gunManager.GetHoldTime();
        
        // If hold time is longer than charge time can fire 
        if (m_holdTime > m_chargeTime  && m_holdTime < m_overChargeTime && m_currentAmmo > 0)
        {

            // Reset hold time in gun manager 
            m_gunManager.SetHoldTime();

            // Disable the laser 
            m_chargeLaser.enabled = false;
            m_chargeAnimator.SetBool( an_enableCharge, false );

            // Disable windup
            WindUp(false);
            m_windUpPlayed = false;

            // Shoot 
            Shoot(transform.parent.parent);

            // Reset 
            m_timer = 0.0f;

            // Shake screen with shot
            ScreenShake.Instance.ShakeScreen( m_screenShakeForce, m_screenShakeTime );

            // Animate gun 
            m_gunAnimator.SetTrigger(an_shoot );

            // Play  shoot sound 
            m_audioPool.PlaySound( m_audioPool.m_shoot );

            // Update  ammo 
            m_currentAmmoInMag--;


            // Update ammo  UI
            m_ammoUI.UpdateAmmo(m_currentAmmoInMag, m_currentAmmo);

            // Reset state 
            m_state = GunState.Idle;

        }
       
        StopShooting();
    }
    /***************************************************
    *   Function        : ChargeLaser  
    *   Purpose         : Set up laser effect
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 27/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/

    private void ChargeLaser()
    {
       
        // If mouse down and not reloading 
        if ( m_holdTime < m_overChargeTime && m_timer > m_fireRate && m_state != GunState.Reloading)
        {
            // Reset over heat sound 
            m_overHeatedPlayed = false;

            // Id windup not played 
            if (m_windUpPlayed == false )
            {
                // Play sound 
                m_audioPool.PlaySound( m_audioPool.m_charge );
                
                // Do effect 
                WindUp();
                m_windUpPlayed = true;
            }


            // Enable laser 
            m_chargeLaser.enabled = true;
            m_chargeAnimator.SetBool( an_enableCharge, true );

        }
        
       // Gun over heated stop shooting 
       if(m_holdTime > m_overChargeTime)
        {
            // Stop effects 
            StopShooting();

            // Play overheat sound once
            if(m_overHeatedPlayed == false )
            {
                m_audioPool.PlaySound( m_audioPool.m_overHeat );
                m_overHeatedPlayed = true;
            }
        }
          
      
    }
    /***************************************************
    *   Function        : StopShooting 
    *   Purpose         : When the gun stops shooting reset values
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 29/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override  void StopShooting()
    {
        // Rest mouse hold time
        m_holdTime = 0;

        if(m_state != GunState.Reloading )
        {
            // Gun now idle 
            m_state = GunState.Idle;

        }
      
        // Disable charge laser 
        m_chargeLaser.enabled = false;
        m_chargeAnimator.SetBool( an_enableCharge, false );
        m_audioPool.StopSound( m_audioPool.m_charge );

        // Disable windup
        WindUp( false );
        m_windUpPlayed = false;

      
    }
    /***************************************************
   *   Function        : OnDisable
   *   Purpose         : When script disabled s
   *   Parameters      : N/A   
   *   Returns         : Void    
   *   Date altered    : 02/07/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    private void OnDisable()
    {
        StopShooting();
    }
    /***************************************************
     *   Function        : Windup
     *   Purpose         : play windup 
     *   Parameters      : bool play   
     *   Returns         : Void    
     *   Date altered    : 07/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void WindUp( bool play = true )
    {
        // Get all the particle system to play effect
        ParticleSystem[] windups = m_chargeEffect.GetComponentsInChildren<ParticleSystem>();
      
           // Play or stop or effects 
            foreach ( ParticleSystem windup in windups )
            {
                if ( play )
                {

                    windup.Play();
                }
                else
                {
                  windup.Stop();
                }
            }
    }


    /***************************************************
     *   Function        : GetChargeStats    
     *   Purpose         : Gets gun stats for menu display  
     *   Parameters      : None 
     *   Returns         : ( float, float ) stats    
     *   Date altered    : 12/07/2022
     *   Contributors    : WH
     *   Notes           :  
     *   See also        :    
    ******************************************************/
    public (float, float) GetChargeStats()
    {
        (float, float) varsToReturn;

        varsToReturn.Item1 = m_chargeTime;
        varsToReturn.Item2 = m_overChargeTime;

        return varsToReturn;
    }

    /***************************************************
    *   Function        : SetChargeStats    
    *   Purpose         : Sets gun stats  
    *   Parameters      : ( float, float ) stats     
    *   Returns         : void
    *   Date altered    : 02/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetChargeStats( (float, float) stats )
    {
        m_chargeTime = stats.Item1;
        m_overChargeTime = stats.Item2;
    }


    /***************************************************
    *   Function        : GetCarryOverData    
    *   Purpose         : Applies stats from data holder
    *   Parameters      : (int, int, int, float, float, float, float) stats   
    *   Returns         : void
    *   Date altered    : 02/08/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void GetCarryOverData( (int, int, int, float, float, float, float) stats )
    {
        SetGunStats( (stats.Item1, stats.Item2, stats.Item3, stats.Item4, stats.Item5) );

        SetChargeStats( (stats.Item6, stats.Item7) );
    }
}
