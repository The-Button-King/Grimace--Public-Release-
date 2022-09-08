using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GreadeEnums;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: GrenadeManager
 *
 * Author: Will Harding
 *
 * Purpose: Manages grenade counts and HUD
 * 
 * Functions:   private void Start()
 *              public void SetGrenade( GrenadeTypes grenade, RawImage icon )
 *              public List<int> GetCounts()
 *              public void SetCounts( List<int> counts )
 *              public void UpdateCurrentGrenade()
 *              public void ApplyCurrentGrenadeCount()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 20/07/22     WH          1.0         Initial creation
 * 21/07/22     WH          1.1         Comments
 * 22/07/22     WH          1.2         Added text update in start
 * 04/08/22     WH          1.3         Added UpdateCurrentGrenade for grenade pickup
 * 15/08/22     WH          1.4         Cleaning
 * 16/08/22     WH          1.5         Fixed carry over data setting
 ****************************************************************************************************/
public class GrenadeManager : MonoBehaviour
{
    private     Dictionary<GrenadeTypes, (GameObject, int, Color)>  m_grenades;                                     // The grenade refereces and counts
    
    [SerializeField]
    [Tooltip( "The different grenades the player can use. Index should align with that of Colour" )]
    private     GameObject[]                                        m_grenadeObjects;                               // Grenade references 

    [SerializeField]
    [Tooltip( "How many grenades to give the player initally" )]
    private     int                                                 m_initialCount;                                 // Initial amount of grenades to have

    [SerializeField]
    [Tooltip( "The differnt colours to change the count text. Index should align with that of Grenade Objects" )]
    private      Color[]                                             m_textColours;                                  // Colours for grenade text

    private     GrenadeTypes                                        m_prevGrenade       = GrenadeTypes.Explosive;   // Previously selected grenade, defaulted to explosive

    private     PlayerManager                                       m_playerManager;                                // Player
    private     UIController                                        m_ui;                                           // UI



    /***************************************************
    *   Function        : Start
    *   Purpose         : Sets values
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 16/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get components
        m_playerManager = GetComponent<PlayerManager>();
        m_ui = transform.root.GetChild( 0 ).Find( "HUD" ).GetComponent<UIController>();
        
        // If there is no grenade dictionary
        if (m_grenades == null )
        {
            // Create dictionary
            m_grenades = new Dictionary<GrenadeTypes, (GameObject, int, Color)>
            {
                { GrenadeTypes.Explosive, ( m_grenadeObjects[ 0 ], m_initialCount, m_textColours[ 0 ] ) },
                { GrenadeTypes.Poison, ( m_grenadeObjects[ 1 ], m_initialCount, m_textColours[ 1 ] ) },
                { GrenadeTypes.Smoke, ( m_grenadeObjects[ 2 ], m_initialCount, m_textColours[ 2 ] ) },
                { GrenadeTypes.Ice, ( m_grenadeObjects[ 3 ], m_initialCount, m_textColours[ 3 ] ) }
            };
        }


        // Set data in player and update UI
        m_playerManager.SetGrenadeData( m_grenades[ m_prevGrenade ].Item1, m_grenades[ m_prevGrenade ].Item2 );
        m_ui.UpdateGrenadeCount( m_grenades[ m_prevGrenade ].Item2, m_grenades[ m_prevGrenade ].Item3 );
    }


    /***************************************************
    *   Function        : SetGrenade
    *   Purpose         : Changes grenade for player and on HUD
    *   Parameters      : GrenadeTypes grenade
    *                     RawImage icon
    *   Returns         : void
    *   Date altered    : 20/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetGrenade( GrenadeTypes grenade, RawImage icon )
    {
        // Get previous grenade data from player
        (GameObject, int) prev = m_playerManager.GetGrenadeData();

        // Update grenade in dictionary
        m_grenades[ m_prevGrenade ] = (prev.Item1, prev.Item2, m_grenades[ m_prevGrenade ].Item3);

        // Change grenade in player
        m_playerManager.SetGrenadeData( m_grenades[ grenade ].Item1, m_grenades[ grenade ].Item2 );

        // Update UI
        m_ui.UpdateGrenadeIcon( icon );
        m_ui.UpdateGrenadeCount( m_grenades[ grenade ].Item2, m_grenades[ grenade ].Item3 );

        // Previous grenade is now the current one
        m_prevGrenade = grenade;
    }

    /***************************************************
    *   Function        : GetCounts
    *   Purpose         : Gets grenade counts for shop
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 20/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public List<int> GetCounts()
    {
        List<int> counts = new List<int>();

        // Add all the grenade counts to a list
        counts.Add( m_grenades[ GrenadeTypes.Explosive ].Item2 );
        counts.Add( m_grenades[ GrenadeTypes.Poison ].Item2 );
        counts.Add( m_grenades[ GrenadeTypes.Smoke ].Item2 );
        counts.Add( m_grenades[ GrenadeTypes.Ice ].Item2 );

        return counts;
    }


    /***************************************************
    *   Function        : SetCounts
    *   Purpose         : Sets grenade counts
    *   Parameters      : List<int> counts
    *   Returns         : void
    *   Date altered    : 15/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetCounts( List<int> counts )
    {
        // Remake the grenade dictionary with the new values
        m_grenades = new Dictionary<GrenadeTypes, (GameObject, int, Color)>
        {
            { GrenadeTypes.Explosive, ( m_grenadeObjects[ 0 ], counts[ 0 ], m_textColours[ 0 ] ) },
            { GrenadeTypes.Poison, ( m_grenadeObjects[ 1 ], counts[ 1 ], m_textColours[ 1 ] ) },
            { GrenadeTypes.Smoke, ( m_grenadeObjects[ 2 ], counts[ 2 ], m_textColours[ 2 ] ) },
            { GrenadeTypes.Ice, ( m_grenadeObjects[ 3 ], counts[ 3 ], m_textColours[ 3 ] ) }
        };
    }


    /***************************************************
    *   Function        : UpdateCurrentGrenade
    *   Purpose         : Updates grenade UI for current grenade
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 04/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void UpdateCurrentGrenade()
    {
        // Update UI
        m_ui.UpdateGrenadeCount( m_grenades[ m_prevGrenade ].Item2 );
    }

    /***************************************************
    *   Function        : ApplyCurrentGrenadeCount
    *   Purpose         : Updates grenade count for current grenade in dictionary
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 16/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void ApplyCurrentGrenadeCount()
    {
        // Get previous grenade data from player
        (GameObject, int) prev = m_playerManager.GetGrenadeData();

        // Update grenade in dictionary
        m_grenades[ m_prevGrenade ] = (prev.Item1, prev.Item2, m_grenades[ m_prevGrenade ].Item3);
    }
}
