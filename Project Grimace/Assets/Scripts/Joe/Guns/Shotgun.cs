using CameraShake;
using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: Shotgun
 *
 * Author: Joseph Gilmore 
 *
 * Purpose: A child of gunstats , a shotgun firemode.
 * 
 * Functions:           protected override void Start()
 *                      protected override void Update()
 *                      Vector3 GetPelletDirection()
 *                      public override void FireGun()
 *                      public int GetShotgunStats()
 *                      public void SetShotgunStats( int stat )
 *                      public int GetPelletCount()
 *                      public void SetPelletCount( int pellets )
 *                      public void GetCarryOverData( ( int, int, int, float, float, int ) stats )
 *                          
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 01/07/2022    JG          1.00        - Created class 
 * 02/07/2022    JG          1.01        - Added shotgun features 
 * 12/07/2022    WH          1.02        - Added Gunstats function
 * 27/07/2022    JG          1.03        - Sound and animation 
 * 28/07/2022    JG          1.04        - Screen shake 
 * 29/07/2022    JG          1.05        - bug fixes
 * 29/07/2022    WH          1.05        - Added pellet count get/set
 * 31/07/2022    JG          1.06        - Minor structure changes 
 * 02/08/2022    WH          1.09        - Adding carry over data
 * 13/08/2022    JG          1.10        - Bug fixes 
 ****************************************************************************************************/
public class Shotgun : GunStats
{
    [Header("Shotgun Stats")]
    [SerializeField][Range(1,20)][Tooltip( "Amount of pellets shot" )]
    private int     m_pelletCount = 8;     // Pellets in shotgun shell
    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Amount of spread between pellets" )]
    private float   m_spreadRange = 0.1f;  // Amount of spread between pellets    
    /***************************************************
    *   Function        : Start   
    *   Purpose         : Set up class  
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

        // Setup level caryr over data 
        DataHolder data = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        if ( data.GetLevel() > 1 )
        {
            GetCarryOverData( data.ApplyDataShotgun() );
        }

        // Disable muzzle flash 
        m_flashPlay = false;
    }
    /***************************************************
    *   Function        : Update 
    *   Purpose         : Apply firerate
    *   Parameters      : N/A
    *   Returns         : Void   
    *   Date altered    : Uk
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void Update()
    {
        // Call parent 
        base.Update();
        
        // Add to firerate timer 
        m_timer += Time.deltaTime;
    }

    /***************************************************
    *   Function        : GetPellet Direction  
    *   Purpose         : Create a spread for a pellet 
    *   Parameters      : N/A
    *   Returns         : Void   
    *   Date altered    : 04/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    Vector3 GetPelletDirection()
    {
        // Get normal shooting direction
        Vector3 defaultDirection = transform.parent.TransformDirection( Vector3.forward );

        // Apply a small offset on each axis to create a spread
        defaultDirection.x += Random.Range( -m_spreadRange, m_spreadRange );
        defaultDirection.y += Random.Range( -m_spreadRange, m_spreadRange );
        defaultDirection.z += Random.Range( -m_spreadRange, m_spreadRange );

        // Normalize spread direction and return
        return defaultDirection.normalized;
    }
    /***************************************************
    *   Function        : FireGun
    *   Purpose         : Fire all pellets at once 
    *   Parameters      : N/A
    *   Returns         : Void   
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void FireGun()
    {
        // Call parent 
        base.FireGun();

        // Check fireRate 
        if(m_timer > m_fireRate )
        {
            // If in correct state
            if ( m_state == GunState.Fireing )
            {
                // Fire muzzle flash once
                FireMuzzle();

                // Animate gun 
                m_gunAnimator.SetTrigger( an_shoot );

                // Loop through each pellet and get a direction 
                for ( int i = 0; i < m_pelletCount; i++ )
                {
                    if ( m_state != GunState.Fireing )
                    {
                        break;
                    }
                    Vector3 dir = GetPelletDirection();

                    // Fire in the spread direction
                    Shoot( transform.parent, dir );

                }


                // Shake screen with shot
                ScreenShake.Instance.ShakeScreen( m_screenShakeForce, m_screenShakeTime );

                // Play shooting sound 
                m_audioPool.PlaySound( m_audioPool.m_shoot );

                // Update  ammo 
                m_currentAmmoInMag--;


                // Update ammo  UI
                m_ammoUI.UpdateAmmo( m_currentAmmoInMag, m_currentAmmo );

                // Reset timer
                m_timer = 0.0f;

               
            }
        }
     
    }

    /***************************************************
     *   Function        : GetShotgunStats    
     *   Purpose         : Gets gun stats for menu display  
     *   Parameters      : None 
     *   Returns         : int stats    
     *   Date altered    : 12/07/2022
     *   Contributors    : WH
     *   Notes           :  
     *   See also        :    
    ******************************************************/
    public int GetShotgunStats()
    {
        int varsToReturn;

        varsToReturn = m_pelletCount;

        return varsToReturn;
    }
    /***************************************************
    *   Function        : SetShotgunStats    
    *   Purpose         : Sets gun stats  
    *   Parameters      : int stat   
    *   Returns         : void
    *   Date altered    : 02/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetShotgunStats( int stat )
    {
        m_pelletCount = stat;
    }
    /***************************************************
     *   Function        : GetPelletCount    
     *   Purpose         : Gets pellet count
     *   Parameters      : None 
     *   Returns         : int m_pelletCount  
     *   Date altered    : 29/07/2022
     *   Contributors    : WH
     *   Notes           :  
     *   See also        :    
     ******************************************************/
    public int GetPelletCount()
    {
        return m_pelletCount;
    }

    /***************************************************
    *   Function        : SetPelletCount    
    *   Purpose         : Sets pellet count
    *   Parameters      : int pellets   
    *   Returns         : void 
    *   Date altered    : 29/07/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetPelletCount( int pellets )
    {
        m_pelletCount = pellets;
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
    public void GetCarryOverData( ( int, int, int, float, float, int ) stats )
    {
        SetGunStats( (stats.Item1, stats.Item2, stats.Item3, stats.Item4, stats.Item5 ) );

        SetShotgunStats( stats.Item6 );
    }
}
