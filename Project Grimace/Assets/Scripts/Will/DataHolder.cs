using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: DataHolder
 *
 * Author: Will Harding
 *
 * Purpose: Class for holding data between levels, such as level number, money, ammo, etc.
 * 
 * Functions:   private void Start()
 *              public int GetLevel()
 *              public void SetLevel( int level ) 
 *              public float GetTotalTime()
 *              public void SetTotalTime( float time )
 *              public int GetKills()
 *              public void SetKills( int kills )
 *              public int GetPurchases()
 *              public void SetPurchases( int purchases )
 *              public int GetRoomsCleared()
 *              public void SetRoomsCleared( int roomsCleared )
 *              public void HoldData()
 *              private void GetAssaultData()
 *              private void GetChargeData()
 *              private void GetShotgunStats()
 *              public List<float> ApplyDataPlayer()
 *              public (int, int, int, float, float, float, float) ApplyDataAssaultRifle()
 *              public (int, int, int, float, float, float, float) ApplyDataChargeRifle()
 *              public (int, int, int, float, float, int) ApplyDataShotgun()
 *              public List<int> ApplyGrenades()
 *              public int GetAssaultCrosshair()
 *              public void SetAssaultCrosshair( int index )
 *              public int GetChargeCrosshair()
 *              public void SetChargeCrosshair( int index )
 *              public int GetShotgunCrosshair()
 *              public void SetShotgunCrosshair( int index )
 *              public (int, int, int) ApplyCrosshairs()
 *              public void SetScreenShake( bool shake )
 *              public bool ApplyScreenShake()
 * 
 * References:
 * 
 * See Also:    
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 07/05/22     WH          1.0         Created
 * 11/05/22     WH          1.1         Now saves data between levels
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 02/08/22     WH          2.0         Added crosshair and screenshake changing and started updating saving variables
 * 05/08/22     WH          2.1         Added game stats holding
 * 10/08/22     WH          2.2         Added multiple instance destruction
 * 13/08/22     WH          2.3         Added some getters for settings stuff
 * 16/08/22     WH          2.4         Fixed grenade carry over data
 * 17/08/22     WH          2.5         Cleaning
 ****************************************************************************************************/
public class DataHolder : MonoBehaviour
{
    [Tooltip( "The single instance of the dataholder" )]
    public static   DataHolder                                  instance;                   // Store an instance of the manager 

    private         int                                         m_level             = 1;    // Level number

    private         (int, int, int, float, float, float, float) m_assaultStats;             // Assault rifle stats
    private         (int, int, int, float, float, float, float) m_chargeStats;              // Charge rifle stats
    private         (int, int, int, float, float, int)          m_shotgunStats;             // Shotgun stats

    private         List<float>                                 m_playerStats;              // Player stats
    private         List<int>                                   m_grenades;                 // Grenade counts

    private         int                                         m_money;                    // Money the player has

    private         int                                         m_assaultCrosshair  = 0;    // Index of the AR crosshair
    private         int                                         m_chargeCrosshair   = 0;    // Index of the CR crosshair
    private         int                                         m_shotgunCrosshair  = 0;    // Index of the SG crosshair

    private         bool                                        m_screenShake       = true; // Is screenshake enabled?

    private         float                                       m_totalTime;                // Total time playing on the run
    private         int                                         m_kills;                    // Total kills
    private         int                                         m_purchases;                // Total purchases
    private         int                                         m_roomsCleared;             // Total number of rooms cleared

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Keeps GameObject between loads
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    10/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // If there is another dataholder that exists, destory it and keep this one
        if ( instance != null )
        {
            Destroy( instance.gameObject );
        }

        // Create and instance of self
        instance = this;

        // Keeps gameobject alive between loading levels
        DontDestroyOnLoad(this);
    }

    /***************************************************
    *   Function        :    GetLevel
    *   Purpose         :    Level Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/05/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetLevel()
    {
        return m_level;
    }

    /***************************************************
    *   Function        :    SetLevel
    *   Purpose         :    Level Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/05/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetLevel( int level ) 
    {
        m_level = level;
    }


    /***************************************************
    *   Function        :    GetTotalTime
    *   Purpose         :    Total Time Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public float GetTotalTime()
    {
        return m_totalTime;
    }

    /***************************************************
    *   Function        :    SetTotalTime
    *   Purpose         :    Total Time Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetTotalTime( float time )
    {
        m_totalTime = time;
    }

    /***************************************************
    *   Function        :    GetKills
    *   Purpose         :    Kills Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetKills()
    {
        return m_kills;
    }

    /***************************************************
    *   Function        :    SetKills
    *   Purpose         :    Kills Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetKills( int kills )
    {
        m_kills = kills;
    }

    /***************************************************
    *   Function        :    GetPurchases
    *   Purpose         :    Purchases Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetPurchases()
    {
        return m_purchases;
    }

    /***************************************************
    *   Function        :    SetPurchases
    *   Purpose         :    Purchases Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetPurchases( int purchases )
    {
        m_purchases = purchases;
    }

    /***************************************************
    *   Function        :    GetRoomsCleared
    *   Purpose         :    RoomsCleared Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetRoomsCleared()
    {
        return m_roomsCleared;
    }

    /***************************************************
    *   Function        :    SetRoomsCleared
    *   Purpose         :    RoomsCleared Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetRoomsCleared( int roomsCleared )
    {
        m_roomsCleared = roomsCleared;
    }


    /***************************************************
    *   Function        :    HoldData
    *   Purpose         :    Gets data from guns and player and saves it
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void HoldData()
    {
        // Get player
        PlayerManager player = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerManager>();
        
        // Set player stats
        m_playerStats = player.GetPlayerVars();
        m_money = player.GetMoney();

        // Get grenade manager
        GrenadeManager grenadeManager = player.GetComponent<GrenadeManager>();

        // Apply current grenade count
        grenadeManager.ApplyCurrentGrenadeCount();

        // Store the grenade counts
        m_grenades = grenadeManager.GetCounts();

        // Get gun stats
        GetAssaultData();
        GetChargeData();
        GetShotgunStats();
    }

    /***************************************************
    *   Function        :    GetAssaultData
    *   Purpose         :    Gets data from gun 
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void GetAssaultData()
    {
        // Get the AR
        AssaultRifle assaultRifle = GameObject.FindGameObjectWithTag( "Player" ).GetComponentInChildren<AssaultRifle>( true );
        
        // Get the stats
        (int, int, int, float, float) stats = assaultRifle.GetGunStats();
        (float, float) assault = assaultRifle.GetAssaultStats();
        
        // Apply the stats to the stats var
        m_assaultStats.Item1 = stats.Item1;
        m_assaultStats.Item2 = stats.Item2;
        m_assaultStats.Item3 = stats.Item3;
        m_assaultStats.Item4 = stats.Item4;
        m_assaultStats.Item5 = stats.Item5;
        m_assaultStats.Item6 = assault.Item1;
        m_assaultStats.Item7 = assault.Item2;
    }

    /***************************************************
    *   Function        :    GetChargeData
    *   Purpose         :    Gets data from gun 
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void GetChargeData()
    {
        // Get the CR
        ChargeRifle chargeRifle = GameObject.FindGameObjectWithTag( "Player" ).GetComponentInChildren<ChargeRifle>( true );

        // Get the stats
        (int, int, int, float, float) stats = chargeRifle.GetGunStats();
        (float, float) charge = chargeRifle.GetChargeStats();

        // Apply the stats to the stats var
        m_chargeStats.Item1 = stats.Item1;
        m_chargeStats.Item2 = stats.Item2;
        m_chargeStats.Item3 = stats.Item3;
        m_chargeStats.Item4 = stats.Item4;
        m_chargeStats.Item5 = stats.Item5;
        m_chargeStats.Item6 = charge.Item1;
        m_chargeStats.Item7 = charge.Item2;
    }

    /***************************************************
    *   Function        :    GetShotgunStats
    *   Purpose         :    Gets data from gun 
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void GetShotgunStats()
    {
        // Get the SG
        Shotgun shotgun = GameObject.FindGameObjectWithTag( "Player" ).GetComponentInChildren<Shotgun>( true );

        // Get the stats
        (int, int, int, float, float) stats = shotgun.GetGunStats();
        int shotgunStats = shotgun.GetShotgunStats();

        // Apply the stats to the stats var
        m_shotgunStats.Item1 = stats.Item1;
        m_shotgunStats.Item2 = stats.Item2;
        m_shotgunStats.Item3 = stats.Item3;
        m_shotgunStats.Item4 = stats.Item4;
        m_shotgunStats.Item5 = stats.Item5;
        m_shotgunStats.Item6 = shotgunStats;
    }

    /***************************************************
    *   Function        :    ApplyDataPlayer
    *   Purpose         :    Returns player data
    *   Parameters      :    None
    *   Returns         :    List<float> values
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public List<float> ApplyDataPlayer()
    {
        // Add the money to the list and return it
        m_playerStats.Add( m_money );
        return m_playerStats;
    }

    /***************************************************
    *   Function        :    ApplyDataAssaultRifle
    *   Purpose         :    Returns assault rifle data
    *   Parameters      :    None
    *   Returns         :    (int, int, int, float, float, float, float) values
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    Renamed from ApplyDataGun
    *   See also        :    
    ******************************************************/
    public (int, int, int, float, float, float, float) ApplyDataAssaultRifle()
    {
        return m_assaultStats;
    }

    /***************************************************
    *   Function        :    ApplyDataChargeRifle
    *   Purpose         :    Returns charge rifle data
    *   Parameters      :    None
    *   Returns         :    (int, int, int, float, float, float, float) values
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public (int, int, int, float, float, float, float) ApplyDataChargeRifle()
    {
        return m_chargeStats;
    }

    /***************************************************
    *   Function        :    ApplyDataShotgun
    *   Purpose         :    Returns shotgun data
    *   Parameters      :    None
    *   Returns         :    (int, int, int, float, float, int) values
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public (int, int, int, float, float, int) ApplyDataShotgun()
    {
        return m_shotgunStats;
    }

    /***************************************************
    *   Function        :    ApplyGrenades
    *   Purpose         :    Applies grendae counts
    *   Parameters      :    None
    *   Returns         :    List<int>
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public List<int> ApplyGrenades()
    {
        return m_grenades;
    }

    /***************************************************
    *   Function        :    GetAssaultCrosshair
    *   Purpose         :    Gets crosshair
    *   Parameters      :    None
    *   Returns         :    int index
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetAssaultCrosshair()
    {
        return m_assaultCrosshair;
    }

    /***************************************************
    *   Function        :    SetAssaultCrosshair
    *   Purpose         :    Sets crosshair
    *   Parameters      :    int index
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetAssaultCrosshair( int index )
    {
        m_assaultCrosshair = index;
    }


    /***************************************************
    *   Function        :    GetChargeCrosshair
    *   Purpose         :    Gets crosshair
    *   Parameters      :    None
    *   Returns         :    int index
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetChargeCrosshair()
    {
        return m_chargeCrosshair;
    }


    /***************************************************
    *   Function        :    SetChargeCrosshair
    *   Purpose         :    Sets crosshair
    *   Parameters      :    int index
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetChargeCrosshair( int index )
    {
        m_chargeCrosshair = index;
    }


    /***************************************************
    *   Function        :    GetShotgunCrosshair
    *   Purpose         :    Gets crosshair
    *   Parameters      :    None
    *   Returns         :    int index
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetShotgunCrosshair()
    {
        return m_shotgunCrosshair;
    }


    /***************************************************
    *   Function        :    SetShotgunCrosshair
    *   Purpose         :    Sets crosshair
    *   Parameters      :    int index
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetShotgunCrosshair( int index )
    {
        m_shotgunCrosshair = index;
    }

    /***************************************************
    *   Function        :    ApplyCrosshairs
    *   Purpose         :    Returns crosshair indexes
    *   Parameters      :    None
    *   Returns         :    (int, int, int)
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public (int, int, int) ApplyCrosshairs()
    {
        return (m_assaultCrosshair, m_chargeCrosshair, m_shotgunCrosshair);
    }


    /***************************************************
    *   Function        :    SetScreenShake
    *   Purpose         :    Sets screenshake
    *   Parameters      :    bool shake
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetScreenShake( bool shake )
    {
        m_screenShake = shake;
    }

    /***************************************************
    *   Function        :    ApplyScreenShake
    *   Purpose         :    Returns screenshake
    *   Parameters      :    None
    *   Returns         :    bool screenshake
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool ApplyScreenShake()
    {
        return m_screenShake;
    }
}
