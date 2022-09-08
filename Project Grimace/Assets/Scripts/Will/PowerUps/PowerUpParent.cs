using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PowerUpParent
 *
 * Author: Will Harding
 *
 * Purpose: Parent class for any power ups or effects
 * 
 * Functions:   private void Start()
 *              public virtual void PowerUpEffect()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 18/07/22     WH          1.0         Initial creation
 * 19/07/22     WH          1.1         Fixed gun referencing
 * 25/07/22     WH          1.2         Added status text to base funtion
 * 26/07/22     WH          1.3         UI is now always found from player
 * 04/08/22     WH          1.4         Added grenade manager reference
 * 17/08/22     WH          1.5         Cleaning
 ****************************************************************************************************/
public class PowerUpParent : MonoBehaviour
{
    // Object references
    protected AssaultRifle      m_assaultRifle;                         // Assault rifle
    protected ChargeRifle       m_chargeRifle;                          // Charge rifle
    protected Shotgun           m_shotgun;                              // Shotgun
    protected PlayerManager     m_player;                               // Player
    protected Shield            m_shield;                               // Shield
    protected Stamina           m_stamina;                              // Stamina
    protected GrenadeManager    m_grenadeManager;                       // Grenade manager
    protected UIController      m_ui;                                   // UI

    [SerializeField]
    [Tooltip( "The text to display on the HUD describing what effect the player has just obtained" )]
    protected string            m_statusText        = "Stat increase";  // Text for UI


    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Assigns variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get all the object references
        GameObject player = GameObject.FindGameObjectWithTag( "Player" );
        m_assaultRifle = player.GetComponentInChildren<AssaultRifle>( true );
        m_chargeRifle = player.GetComponentInChildren<ChargeRifle>( true );
        m_shotgun = player.GetComponentInChildren<Shotgun>( true );

        m_player = player.GetComponent<PlayerManager>();
        m_shield = player.GetComponent<Shield>();
        m_stamina = player.GetComponent<Stamina>();
        m_grenadeManager = player.GetComponent<GrenadeManager>();

        m_ui = m_player.transform.root.GetComponentInChildren<UIController>( true );
    }


    /***************************************************
    *   Function        :    PowerUpEffect
    *   Purpose         :    Effects the player or gun in some way
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    WH
    *   Notes           :    Virtual
    *   See also        :    
    ******************************************************/
    public virtual void PowerUpEffect()
    {
        // Display effect on UI
        m_ui.UpdateStatus( m_statusText );

    }
}
