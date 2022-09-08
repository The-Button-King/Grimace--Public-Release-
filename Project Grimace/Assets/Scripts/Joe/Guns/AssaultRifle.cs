using CameraShake;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: AssultRifle
 *
 * Author: JG
 *
 * Purpose: An autmatic firing weapon for the player 
 * 
 * Functions:   protected override void  Start()
 *              protected override void Awake()
 *              protected override void Update()
 *              private void Recoil()
 *              public override void FireGun()
 *              public ( float, float ) GetAssaultStats()
 *              public void SetAssaultStats( (float, float) stats)
 *              public void UpgradeRecoil()
 *              public void UpgradeFireRate()
 *              public void GetCarryOverData( (int, int, int, float, float, float, float) stats )
 *              
 * 
 * References:
 * 
 * See Also: GunStats.cs & BasicGuns
 *
 * Change Log:
 * Date             Initials    Version     Comments
 * ----------       --------    -------     ----------------------------------------------
 * 03/05/2022       JG          1.00        - Class created 
 * 09/05/2022       JG          1.01        - Created a basic spray pattern 
 * 10/05/2022       JG          1.02        - Added lerping to shooting 
 * 10/05/2022       JG          1.03        - Added gun states 
 * 12/07/2022       WH          1.04        - Added recoilMultiplier vars and added GetStats function
 * 20/07/2022       JG          1.05        - added infinate ammo
 * 27/07/2022       WH          1.06        - Added recoil upgrade
 * 27/07/2022       JG          1.07        - My changes vanished ( fixed aniamtion and shooting 
 * 28/07/2022       Wh          1.08        - Added rate of fire multiplier functionality
 * 28/07/2022       JG          1.08        - Screen shake 
 * 02/08/2022       WH          1.09        - Adding carry over data
 * 05/08/2022       JG          1.10        - No firerate on single shot
 * 14/08/2022       JG          1.11        - Clean
 ****************************************************************************************************/
public class AssaultRifle : GunStats
{
   
    private int                             m_heat = 0;                             // Rcoil amount for continous firing 
    private         List<Vector2>           m_sprayPattern;                         // list of spray points 
    protected       Vector2                 m_sprayOffset = Vector2.zero;           // Current spray offset 
    private float                           m_inAccuracyMutipler;                   // Mutipler to change the accuracy 
    private CinemachineVirtualCamera        m_camera;                               // Rereference to Virtual camera that controls main cam
    private CinemachinePOV                  m_composer;                             // Refereence to change the properties of the VC
    [Header("Recoil Stats")]
    private float                           m_walkingAccuracy;                      // The amount accuracy is effected by movement 
    [SerializeField][Range( 1.0f, 4.0f )][Tooltip("The minnimum amount of speed to effect walking recoil")]
    private float                           m_minimumWalkingSpeed = 2.2f;           // The Min amount the walking 
    [SerializeField][Tooltip("The min Amount of accuracy (smaller more accurate")]
    [Range(1.0f, 3.0f)]
    private   float                         m_minAccuracyRange = 1.0f;              // The min amount of accuracy 
    [SerializeField]
    [Range(1.0f, 10.0f)][Tooltip("The max Amount of accuracy (smaller more accurate")]
    private  float                          m_maxAccuracyRange = 6.4f;              // The max range of  accuracy 
    [SerializeField][Range( 1.0f, 3.0f )][Tooltip("The min amount of shots needed to start recoil")]
    private int                             m_heatThreshold = 1;                    // The min amount heat to start gun recoil 
    [SerializeField][Tooltip("max amount of walking inaccuracy")][Range(5.0f, 30.0f)]
    private float                           m_maxWalkingAccuracy = 3.2f;             // Max amount of walking inaccuracy
    [SerializeField] [Tooltip( "The rate the recoil heat decreasey" )][Range( 1.0f, 3.0f )]
    private int                             m_decreaseHeatRate = 1;                 // The rate the recoil heat decrease
    [SerializeField][Tooltip( "The rate the recoil heat increase" )] [Range( 1.0f, 3.0f )]
    private int                             m_increaseHeatRate = 1;                 // The rate the recoil heat increase
    [Header("Shop upgrades  ")]
    [SerializeField]
    [Range( 0.1f, 1.0f )][Tooltip( "Multiplier for fire rate that can be changed via upgrades. Smaller means faster." )]
    private float                           m_fireRateMultiplier = 1.0f;            // Multiplier for fire rate that can be changed via upgrades
    [SerializeField][Range( 0.01f, 0.99f )][Tooltip( "Amount to multiply the fire rate multiplier by when upgrading. Smaller means faster." )]
    private float                           m_fireRateMultiplierUpgrade = 0.9f;     // Amount to multiply the fire rate multiplier by when upgrading
    [SerializeField][Range( 0.1f, 1.0f )][Tooltip( "Multiplier for recoil that can be changed via upgrades. Smaller means faster." )]
    private float                           m_recoilMultiplier = 1.0f;              // Multiplier for recoil that can be changed via upgrade
    [SerializeField]
    [Range( 0.01f, 0.99f )]
    [Tooltip( "Amount to multiply the recoil multiplier by when upgrading. Smaller means faster." )]
    private float                           m_recoilMultiplierUpgrade = 0.9f;       // Amount to multiply the recoil multiplier by when upgrading
    private int                             m_amountShot = 0;                       // Used to apply no firerate on the first shot

    /***************************************************
    *   Function        : Start  
    *   Purpose         : Setup the class   
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 02/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void  Start()
    {
        // Call Parent
        base.Start();

        // Get data fom last level
        DataHolder data = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        if ( data.GetLevel() > 1 )
        {
            GetCarryOverData( data.ApplyDataAssaultRifle() );
        }

        // Set starting state 
        m_state = GunState.Idle;
        
        // Get camera references 
        m_camera = transform.root.Find( "VCam" ).GetComponent<CinemachineVirtualCamera>();
        m_composer = m_camera.GetCinemachineComponent<CinemachinePOV>();

        // Set current ammo
        m_currentAmmoInMag = m_magCapacity;

        // Assign UI Controller 
        m_ammoUI = transform.root.GetChild(0).Find("HUD").GetComponent<UIController>();

        // Play flash
        m_flashPlay = true;

        // Create Spray Pattern (This is hard coded so I can adpat a spray patter such as csgo, I can then in theroy pass any pattern I wish into the Assault Rilfe if I wanted to change it)
        m_sprayPattern = new List<Vector2> 
        { 
            new Vector2(0.0f, 0.0f),
            new Vector2(0.2f, 0.001f), 
            new Vector2(0.3f, 0.001f), 
            new Vector2(0.25f, 0.001f),
            new Vector2(0.2f, 0.001f), 
            new Vector2(0.35f, 0.001f),
            new Vector2(0.1f,0.001f),  
            new Vector2(0.5f, 0.01f),  
            new Vector2(0.1f, 0.001f), 
            new Vector2(0.1f, 0.001f), 
            new Vector2(0.1f, -0.001f), 
            new Vector2(0.01f, 0.1f),   
            new Vector2(0.02f, 0.12f),  
            new Vector2(-0.01f, 0.135f),
            new Vector2(0.01f, 0.2f),  
            new Vector2(0.01f, 0.22f), 
            new Vector2(0.01f, 0.24f), 
            new Vector2(0.01f, 0.31f), 
            new Vector2(0.1f, 0.31f),  
            new Vector2(-0.01f, 0.25f),
            new Vector2(0.01f, -0.1f), 
            new Vector2(0.02f, -0.12f),
            new Vector2(-0.01f, -0.135f),   
            new Vector2(0.01f, -0.2f),      
            new Vector2(0.01f, -0.22f),     
            new Vector2(0.01f,- 0.24f),     
            new Vector2(0.01f, -0.31f),     
            new Vector2(0.1f, -0.31f),      
            new Vector2(-0.01f,- 0.25f),    
            new Vector2(0.00f, -0.18f)};    
 
    }

    /***************************************************
    *   Function        : Awake
    *   Purpose         : Setup class 
    *   Parameters      : N/A   
    *   Returns         : void    
    *   Date altered    : 03/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/

    protected override void Awake()
    {
        // Assault rifle has infinate ammo
        m_infiniteAmmo = true;

        // Call parent 
        base.Awake();
    }

    /***************************************************
    *   Function        : Update   
    *   Purpose         : Update properties of the gun such as UI
    *   Parameters      : N/A   
    *   Returns         : void    
    *   Date altered    : 05/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Update()
    {
        
        // Call parent update 
        base.Update();

        // Apply firerate 
        m_timer += Time.deltaTime;

        // Update recoil heat
        if (m_state != GunState.Fireing && m_heat > 0)
        {
            // Reduce Heat(lower Recoil Impact)
            m_heat -= m_decreaseHeatRate;
            m_amountShot = 0;
        }

        // Check for walking to decrease accuracy 
        if (m_playerManager.GetCurrentSpeed() >= m_minimumWalkingSpeed)
        {
            // Generate a walking accuracy value using the speed of the player 
            m_walkingAccuracy = Random.Range(m_minimumWalkingSpeed,m_playerManager.GetCurrentSpeed());

            // Clamp value 
            if(m_walkingAccuracy > m_maxWalkingAccuracy)
            {
                m_walkingAccuracy = m_maxWalkingAccuracy;
            }
           
        }
        else
        {
            // Set to default value as not moving 
            m_walkingAccuracy = m_minimumWalkingSpeed;
        }
        if (m_state == GunState.Fireing)
        {
            // Lerp VCPov from current rotation to recoil effected rotation on both axis
            m_composer.m_VerticalAxis.Value = Mathf.Lerp( m_composer.m_VerticalAxis.Value, m_composer.m_VerticalAxis.Value - m_sprayOffset.x, 0.1f );
            m_composer.m_HorizontalAxis.Value = Mathf.Lerp( m_composer.m_HorizontalAxis.Value, m_composer.m_HorizontalAxis.Value - m_sprayOffset.y, 0.1f );
        }
    }
    /***************************************************
    *   Function        : Recoil   
    *   Purpose         : Handle the recoil of the assult rifle    
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 12/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Recoil()
    {
        // Set up a random accuracy value for  new spray 
        if( m_heat <= m_heatThreshold )
        {
           m_inAccuracyMutipler =  Random.Range( m_minAccuracyRange,m_maxAccuracyRange );
        }

        // Get current point in pattern
        m_sprayOffset = m_sprayPattern[m_heat];
      

        // Apply spray offset 
        m_sprayOffset *= m_inAccuracyMutipler * m_walkingAccuracy * m_recoilMultiplier;

    }
    /***************************************************
    *   Function        :  FireGun  
    *   Purpose         :  Fire the gun at a curtain rate  
    *   Parameters      :  N/A
    *   Returns         :  Void   
    *   Date altered    :  13/08/2022
    *   Contributors    :  JG WH(UI)
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void FireGun()
    {
      
        // Call parent to update ammo
        base.FireGun();
       
        if (m_state == GunState.Fireing)
        {
           // Don't apply firerate on a single shot
           if(m_amountShot == 0 )
           {
                m_timer = m_fireRate * m_fireRateMultiplier;
           }

           // Check fireRate 
           if( m_timer >= m_fireRate * m_fireRateMultiplier )
           {
               // Increase the  heat (amount the recoil is effect the gun)
               if(m_heat < m_magCapacity -1) // Indexing 
               {

                   m_heat += m_increaseHeatRate; 
               }

               // Apply animation
               m_gunAnimator.SetTrigger( an_shoot );

               // Shoot Gun
               Shoot( transform.parent.parent );

               // Shake screen with shot
                ScreenShake.Instance.ShakeScreen( m_screenShakeForce,m_screenShakeTime );


               // Play shoot sound 
               m_audioPool.PlaySound( m_audioPool.m_shoot );


               // Apply recoil
               Recoil();

               // Update  ammo 
               m_currentAmmoInMag--;
              

               // Update ammo  UI
               m_ammoUI.UpdateAmmo( m_currentAmmoInMag, -1 );

               // Reset firerate timer 
               m_timer = 0f;

                // Increase amount shot 
               m_amountShot++;
            
           }
        
        }
            
        
       
    }
    /***************************************************
    *   Function        : GetAssaultStats    
    *   Purpose         : Gets gun stats for menu display  
    *   Parameters      : None 
    *   Returns         : ( float, float ) stats    
    *   Date altered    : 12/07/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public ( float, float ) GetAssaultStats()
    {
        ( float, float ) varsToReturn;

        varsToReturn.Item1 = m_fireRate;
        varsToReturn.Item2 = m_recoilMultiplier;

        return varsToReturn;
    }

    /***************************************************
    *   Function        : SetAssaultStats    
    *   Purpose         : Sets gun stats  
    *   Parameters      : ( float, float ) stats     
    *   Returns         : void
    *   Date altered    : 02/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetAssaultStats( (float, float) stats)
    {
        m_fireRate = stats.Item1;
        m_recoilMultiplier = stats.Item2;
    }

    /***************************************************
    *   Function        : UpgradeRecoil    
    *   Purpose         : Upgrades recoil multiplier
    *   Parameters      : None   
    *   Returns         : void
    *   Date altered    : 27/07/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpgradeRecoil()
    {
        m_recoilMultiplier *= m_recoilMultiplierUpgrade;
    }

    /***************************************************
    *   Function        : UpgradeFireRate    
    *   Purpose         : Upgrades fire rate multiplier
    *   Parameters      : None   
    *   Returns         : void
    *   Date altered    : 27/07/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpgradeFireRate()
    {
        m_fireRateMultiplier *= m_fireRateMultiplierUpgrade;
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

        SetAssaultStats( (stats.Item6, stats.Item7) );
    }
}
