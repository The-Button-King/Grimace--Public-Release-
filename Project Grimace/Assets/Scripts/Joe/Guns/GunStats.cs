using UnityEngine;
using System.Collections;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: GunStats
 *
 * Author: JG
 *
 * Purpose: To hold stats for different gun child types 
 * 
 * Functions:     protected virtual void Start()
 *                protected override void Awake()
 *                private void CalculateReloadTime()
 *                protected override void Update()
 *                public IEnumerator Reload()
 *                public GunState GetState()
 *                public virtual void FireGun()
 *                public  virtual void StopShooting()
 *                public void GetCarryOverData(int ammo, int magSize)
 *                public void GetCarryOverData((int, int) tuple)
 *                public ( int, int, int, float, float ) GetGunStats()
 *                public void SetGunStats( (int, int, int, float, float) stats )
 *                public void SetCurrentAmmo(int ammo)
 *                public int GetCurrentAmmo()
 *                public int GetMagCap()
 *                public void SetMagCap(int magCap)
 *                public int GetAmmoCap()
 *                public void SetAmmoCap( int cap )
 *                public void UpdateHUD()
 *                public void UpgradeReloadTime()
 *                protected void AutomaticReload()
 *                
 *                
 *                      
 *                      
 *                      
 *                      
 * References:
 * 
 * See Also: ChargeRifle, ShotGun. AssultRilfe & BasicGun
 *
 * Change Log:
 * Date             Initials    Version     Comments
 * ----------       --------    -------     ----------------------------------------------
 * uk               JG          1.00        - Created a list of vars for all guns to use 
 * 10/05/2022       JG          1.01        - Added gun states 
 * 06/06/2022       JG          2.00        - Fixed reload into the minus bug
 * 04/07/2022       JG          2.01        - Moved UI & and carry over data from child into here. Also cleanned
 * 12/07/2022       WH          2.02        - Added GetGunStats function
 * 20/07/2022       JG          2.03        - Added infinate ammo
 * 27/07/2022       WH          2.04        - Added reload time upgrade
 * 27/07/2022       JG          2.05        - Reference updating , sound and new animations 
 * 29/07/2022       JG          2.06        - Fixed reload bug
 * 31/07/2022       JG          2.07        - Fixed another reload bug
 * 03/08/2022       JG          2.08        - Minor restructure when cleaning 
 * 13/08/2022       JG          2.09        - Bug fixes 
 * 16/08/2022       WH          2.10        - Removed reloading text UI as was not used
 ****************************************************************************************************/
public class GunStats : BasicGun
{
    protected int           m_currentAmmo;                                       // Reserve ammo
    protected int           m_currentAmmoInMag = 0;                              // The current remaining ammo in a mag
    [Header("Gun Stats")]
    [SerializeField][Tooltip("Max amount of ammo that can be held")]
    protected int           m_ammoCapacity = 300;                                // The total amount of ammo the gun can hold 
    [SerializeField]
    [Tooltip("Max amount of ammo in a magazine can be used before reloading")]
    protected int           m_magCapacity = 30;                                 // The amount ammo that can be fired before reloading 
    protected float         m_reloadTime = 0.24f;                               // Time it takes to reload 
    [SerializeField]
    [Tooltip( "Smaller the value the faster the rate" )]
    protected float         m_fireRate;                                         // How fast the gun can fire 
    protected  float        m_timer = 0f;                                       // Timer for firerate 
    [SerializeField]
    [Range( 0.1f, 1f )]
    [Tooltip( "Multiplier for reload time that can be changed via upgrades. Smaller is faster." )]
    private float           m_reloadTimeMultiplier = 1.0f;                      // Multiplier for reload time that can be changed via upgrades

    [SerializeField]
    [Range( 0.01f, 0.99f )]
    [Tooltip( "Amount to multiply the reload time multiplier by when upgrading. Smaller is faster" )]
    private float           m_reloadTimeMultiplierUpgrade = 0.9f;               // Amount to multiply the reload time multiplier by when upgrading

    [SerializeField]
    protected UIController  m_ammoUI;                                           // Displays gun related UI
    protected Animator      m_gunAnimator;                                      // Reference to the gun animator 
    public enum GunState                                                        // Different States that the gun can be 
    {
        Idle,
        Reloading,
        Fireing,
    }
    protected GunState      m_state = GunState.Idle;                            // The current State of the gun
    protected bool          m_infiniteAmmo = false;                             // Is infinite mammo 

    [SerializeField][Tooltip("Hud colour")]
    protected Color         m_colourForText;                                    // Colour for text outline
    protected GunAudioPool  m_audioPool;                                        // Reference to Audio pool   
    [Header("Screen shake for shooting")]
    [SerializeField][Tooltip(" Force of screen shake")]
    [Range( 0.01f, 3f )]
    protected float         m_screenShakeForce;                                 // Screen shake force when shooting 
    [SerializeField]
    [Range( 0.01f, 0.2f )][Tooltip("Time the screen shakes for ")]
    protected float         m_screenShakeTime;                                  // How long it shakes for
    protected int           an_shoot;                                           // String to hash for shoot animation
    private const string    m_reloadClipName = "metarig.001_Player_Reload_02";  // Store reload clip name 
    protected PlayerManager m_playerManager;                                    // Reference to player manager 
    /***************************************************
     *   Function        : Start    
     *   Purpose         : Set Up the gun by getting correct references    
     *   Parameters      : N/A   
     *   Returns         : Void    
     *   Date altered    : 14/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    protected virtual void Start()
    {
        
        // Get animator 
        m_gunAnimator = transform.parent.GetComponentInChildren<Animator>();

        // Set string to hash for shooting anim
        an_shoot = Animator.StringToHash( "Shoot" );

        // Get UI controller 
        m_ammoUI  = transform.root.GetComponentInChildren<UIController>();

        // Get Audio pool 
        m_audioPool = GetComponent<GunAudioPool>();

        // Calculate the time it takes to reload 
        CalculateReloadTime();

        // Set timer to firerate so firerate is not applied on first shot
        m_timer = m_fireRate;

        // Get Player manager reference 
        m_playerManager = transform.root.GetComponentInChildren<PlayerManager>();
    }
    /***************************************************
     *   Function        : Awake    
     *   Purpose         : Set ammo up    
     *   Parameters      : N/A   
     *   Returns         : Void    
     *   Date altered    : 02/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    protected override void Awake()
    {
        // Call parent 
        base.Awake();

        // Set starting amount of ammo
        m_currentAmmo = m_ammoCapacity;
        m_currentAmmoInMag = m_magCapacity;

        // Make sure hud displays starting ammo
        UpdateHUD();
    }

    /***************************************************
     *   Function        : CalculateReloadTime  
     *   Purpose         : Get reload time by animation clip length   
     *   Parameters      : N/A   
     *   Returns         : Void    
     *   Date altered    : 27/07/2022
     *   Contributors    : JG
     *   Notes           : Could be done cleaner but unity animator sucks.  
     *   See also        :    
    ******************************************************/
    private void CalculateReloadTime()
    {
        // Get the clips from the animator 
        AnimationClip[] clips = m_gunAnimator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // Get the reload clip
            if (clip.name == m_reloadClipName )
            {
                // Set length of reload to animation time 
                m_reloadTime = clip.length * m_reloadTimeMultiplier;
            }
        }
        
    }
  
    /***************************************************
    *   Function        : Update   
    *   Purpose         : Maintain animation via GunStates  
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 03/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Update()
    {
        // Update Parent
        base.Update();

       // Check autmatic relaod
        AutomaticReload();

    }
    /***************************************************
    *   IEnumerator     : Reload   
    *   Purpose         : To reload the current gun mag   
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 16/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public IEnumerator Reload()
    {
        // If currently throwing a grenade dont reload (animation bug fix)
        if(m_playerManager.GetCurrentlyThrowing() == true )
        {
           yield break;
        }

        // If currently not reloading 
       else if(m_state != GunState.Reloading  )
        {
            // Set reload time
            WaitForSeconds wait = new WaitForSeconds( m_reloadTime );

            // If there is ammo to reload 
            if ( m_currentAmmo != 0 )
            {
                // Update state
                m_state = GunState.Reloading;

                // Play animation 
                m_gunAnimator.SetTrigger( "Reload" );



                // Wait to duiration of animation to update stats
                yield return wait;

                if ( m_infiniteAmmo == false )
                {
                    // Run out of ammo
                    if ( m_currentAmmo <= 0 && m_currentAmmoInMag <= 0 )
                    {
                        m_state = GunState.Idle;
                        yield return null;
                    }

                    // If current ammo is less than mag cap 
                    if ( m_currentAmmo < m_magCapacity && m_currentAmmo > 0 )
                    {
                        // put remaing ammo in the gun
                        m_currentAmmoInMag = m_currentAmmo;
                        m_currentAmmo = 0;

                    }
                    else
                    {
                        // Upate total ammo
                        m_currentAmmo -= m_magCapacity - m_currentAmmoInMag;

                        // If total out of ammo 
                        if ( m_currentAmmo <= 0 && m_currentAmmoInMag <= 0 )
                        {
                            // Set to zero
                            m_currentAmmo = 0;
                            m_currentAmmoInMag = 0;

                        }
                        else
                        {
                            //Refill Mag
                            m_currentAmmoInMag = m_magCapacity;

                        }
                    }
                }
                else
                {
                    //Refill Mag
                    m_currentAmmoInMag = m_magCapacity;
                    m_ammoUI.UpdateAmmo( m_currentAmmoInMag, -1 );

                }
               

            }
            // Play reload sound
            m_audioPool.PlaySound( m_audioPool.m_reload );

            // Reset timer 
            m_timer = m_fireRate;


            // Update UI
            UpdateHUD();

            // Reset state as no longer reloading 
            m_state = GunState.Idle;
       }
        
        
    }
    /***************************************************
    *   Function        : GeState 
    *   Purpose         : Get current gun State 
    *   Parameters      : return the current state of the gun   
    *   Returns         : Void   
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
   ******************************************************/
    public GunState GetState()
    {
        return m_state;
    }
    /***************************************************
    *   Function        : FireGun 
    *   Purpose         : Used to trigger the gun firing process and update ammo 
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :  AssultRifle,Charge Rifle, Shotgun    
    ******************************************************/
    public virtual void FireGun()
    {

        // If currently not reloading or throwing a grenade 
        if (m_state != GunState.Reloading && m_playerManager.GetCurrentlyThrowing() == false )
        {    
           
            // If have ammo
            if (m_currentAmmoInMag > 0 && m_currentAmmo >= 0)
            {
                // Gun is shooting 
                m_state = GunState.Fireing;

            }
            else
            {
                // Play sound to indicate no ammo
                m_audioPool.PlaySound( m_audioPool.m_emptyShoot );
            }

            // Update HUD
            UpdateHUD();
        }
    } 
    /***************************************************
    *   Function        : StopShooting  
    *   Purpose         : Used to stop the shooting process in children
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : AssultRifle,Charge Rifle, Shotgun  
    ******************************************************/
    public  virtual void StopShooting()
    {
        if ( m_state != GunState.Reloading )
        {
            m_state = GunState.Idle;
        }
            
    }

    /***************************************************
    *   Function        : GetCarryOverData   
    *   Purpose         : sets upgraded values so they carry over between levels
    *   Parameters      : int ammo
    *                     int magSize
    *   Returns         : void    
    *   Date altered    : 04/05/2022
    *   Contributors    : WH
    *   Notes           : This has been moved from assult rifle to gunStats (JG)   
    *   See also        : DataHolder
    ******************************************************/
    public void GetCarryOverData(int ammo, int magSize)
    {
        m_currentAmmo = ammo;
        m_magCapacity = magSize;
        m_currentAmmoInMag = m_magCapacity;
    }

    /***************************************************
    *   Function        : GetCarryOverData   
    *   Purpose         : sets upgraded values so they carry over between levels
    *   Parameters      : (int, int) tuple
    *   Returns         : void    
    *   Date altered    : 04/05/2022
    *   Contributors    : WH
    *   Notes           : Tuple overload
    *                     This has been moved from assult rifle to gunStats (JG)   
    *   See also        : DataHolder
    ******************************************************/
    public void GetCarryOverData((int, int) tuple)
    {
        m_currentAmmo = tuple.Item1;
        m_magCapacity = tuple.Item2;
        m_currentAmmoInMag = m_magCapacity;
    }

    /***************************************************
    *   Function        : GetGunStats    
    *   Purpose         : Gets gun stats for menu display  
    *   Parameters      : None 
    *   Returns         : ( int, int, int, float, float ) stats    
    *   Date altered    : 12/07/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public ( int, int, int, float, float ) GetGunStats()
    {
        ( int, int, int, float, float ) varsToReturn;

        varsToReturn.Item1 = m_currentAmmo;
        varsToReturn.Item2 = m_magCapacity;
        varsToReturn.Item3 = m_ammoCapacity;
        varsToReturn.Item4 = m_reloadTime;
        varsToReturn.Item5 = m_baseDamage;

        return varsToReturn;
    }

    /***************************************************
    *   Function        : SetGunStats    
    *   Purpose         : Sets gun stats
    *   Parameters      : (int, int, int, float, float) stats
    *   Returns         : void
    *   Date altered    : 02/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetGunStats( (int, int, int, float, float) stats )
    {
        m_currentAmmo = stats.Item1;
        m_magCapacity = stats.Item2;
        m_ammoCapacity = stats.Item3;
        m_reloadTime = stats.Item4;
        m_baseDamage = stats.Item5;
    }

    /***************************************************
     *   Function        : SetCurrentAmmo    
     *   Purpose         : Set current ammo value    
     *   Parameters      : int ammo  
     *   Returns         : void    
     *   Date altered    : 27/07/2022
     *   Contributors    : JG, WH
     *   Notes           : This has been moved from assult rifle to gunStats (JG)    
     *   See also        :    
   ******************************************************/
    public void SetCurrentAmmo(int ammo)
    {
        if( ammo <= m_ammoCapacity )
        {
            m_currentAmmo = ammo;
        }
    }
    /***************************************************
     *   Function        : GetCurrentAmmo   
     *   Purpose         : Return current ammo   
     *   Parameters      : N/A   
     *   Returns         : int m_currentAmmo   
     *   Date altered    : 04/07/2022
     *   Notes           : This has been moved from assult rifle to gunStats (JG)       
     *   See also        :    
    ******************************************************/
    public int GetCurrentAmmo()
    {
        return m_currentAmmo;
    }
    /***************************************************
     *   Function        : GetMagCap 
     *   Purpose         :  Get the max amount of magazine cap  
     *   Parameters      :  N/A  
     *   Returns         : int m_magCacity   
     *   Date altered    : 04/07/2022
     *   Contributors    : JG WH
     *   Notes           : This has been moved from assult rifle to gunStats (JG)       
     *   See also        :    
     ******************************************************/
    public int GetMagCap()
    {
        return m_magCapacity;
    }
    /***************************************************
    *   Function        : SetMagCap
    *   Purpose         : Set the magazine amount    
    *   Parameters      : int magCap  
    *   Returns         : N/A  
    *   Date altered    : UK
    *   Contributors    : WH JG
    *   Notes           : This has been moved from assult rifle to gunStats (JG)        
    *   See also        :    
    ******************************************************/
    public void SetMagCap(int magCap)
    {
        m_magCapacity = magCap;
        m_currentAmmoInMag = magCap;
    }

    /***************************************************
    *   Function        : GetAmmoCap 
    *   Purpose         : Get the max amount of ammo cap  
    *   Parameters      : None 
    *   Returns         : int ammo cap
    *   Date altered    : 27/07/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetAmmoCap()
    {
        return m_ammoCapacity;
    }

    /***************************************************
    *   Function        : SetAmmoCap
    *   Purpose         : Set the ammo cap   
    *   Parameters      : int cap  
    *   Returns         : None  
    *   Date altered    : 27/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetAmmoCap( int cap )
    {
        m_ammoCapacity = cap;
    }

    /***************************************************
    *   Function        : UpdateHUD    
    *   Purpose         : Updates ammo count on HUD
    *   Parameters      : None   
    *   Returns         : void    
    *   Date altered    : 03/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateHUD()
    {
        if ( m_infiniteAmmo == true )
        {
            m_ammoUI.UpdateAmmo( m_currentAmmoInMag, -1 ,m_colourForText);
        }
        else
        {
            m_ammoUI.UpdateAmmo( m_currentAmmoInMag, m_currentAmmo ,m_colourForText);
        }
    }

    /***************************************************
    *   Function        : UpgradeReloadTime    
    *   Purpose         : Upgrades reload time multiplier
    *   Parameters      : None   
    *   Returns         : void    
    *   Date altered    : 27/07/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpgradeReloadTime()
    {
        m_reloadTimeMultiplier *= m_reloadTimeMultiplierUpgrade;
    }
    /***************************************************
    *   Function        : AutmaticReload
    *   Purpose         : reload gun autmaticly 
    *   Parameters      : None   
    *   Returns         : void    
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected void AutomaticReload()
    {
        // If currently not throwing a grenade 
        if( m_playerManager.GetCurrentlyThrowing() == false   )
        {
            // If mag empty and not already reloading
            if ( m_currentAmmoInMag <= 0 && m_state != GunState.Reloading && m_currentAmmo > 0 )
            {

                // Start autmatic reload 
                StartCoroutine( Reload() );
            }
        }
       
    }
}
