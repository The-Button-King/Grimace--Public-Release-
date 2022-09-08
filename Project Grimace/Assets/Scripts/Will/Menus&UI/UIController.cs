using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: UIController
 *
 * Author: Will Harding
 *
 * Purpose: Controls UI
 * 
 * Functions:   public void UpdateAmmo( int magazine, int maxAmmo )
 *              public void UpdateAmmo( int magazine, int maxAmmo, Color colour )
 *              public void UpdateHealth( float health, float maxHealth )
 *              public void UpdateMoney( int money )
 *              public void UpdateStatus( string text )
 *              public void UpdateInteract( string text )
 *              public void UpdateShield( float value, float maxValue )
 *              public void UpdateStamina( float value )
 *              public void UpdateGrenadeIcon( RawImage icon )
 *              public void UpdateGrenadeCount( int count )
 *              public void UpdateGrenadeCount( int count, Color colour )
 *              public void SetInteractActive( bool active )
 *              public void ActivateGrenadeUI()
 *              private IEnumerator DelayDeactivate()
 *              
 * 
 * References:
 * 
 * See Also:    
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 06/05/22     WH          1.0         Created
 * 09/05/22     JG          1.1         Added reload text
 * 10/05/22     WH          1.2         Added Game Over
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 21/06/22     WH          2.0         Added interactable UI text
 * 19/07/22     WH          2.1         Added and changd functions for new HUD design
 * 20/07/22     WH          2.2         Messing with ammo ui formatting
 * 16/08/22     WH          2.3         Cleaning
 ****************************************************************************************************/
public class UIController : MonoBehaviour
{
    [Header( "Bars and Wheel" )]

    [Tooltip( "Health Slider" )]
    public Slider   m_health;                   // Health UI

    [Tooltip( "Shield Slider" )]
    public Slider   m_shield;                   // Shield slider

    [Tooltip( "Stamina Wheel" )]
    public Image    m_stamina;                  // Stamina wheel
    
    
    [Header( "Counts" )]

    [Tooltip( "Ammo count" )]
    public TMP_Text m_ammo;                     // Ammo UI

    [Tooltip( "Money count" )]
    public TMP_Text m_money;                    // Money UI

    [Tooltip( "Grenade count" )]
    public TMP_Text m_grenadeCount;             // Grenade count

    [Tooltip( "Grenade Icon" )]
    public RawImage m_grenade;                  // Grenade Icon


    [Header( "Text UI" )]

    [Tooltip( "Status Effect Text" )]
    public TMP_Text m_status;                   // Status UI

    [Tooltip( "Interaction Text" )]
    public TMP_Text m_interact;                 // Interaction text UI

    [Space(10)]
    [SerializeField]
    [Tooltip( "Time the status text is displayed for" )]
    [Range(0, 10)]
    private int      m_statusTimer   = 2;        // Length of timer to turn off status

    /***************************************************
    *   Function        :    UpdateAmmo
    *   Purpose         :    Updates Ammo UI
    *   Parameters      :    int magazine
    *                        int maxAmmo
    *   Returns         :    void
    *   Date altered    :    20/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateAmmo( int magazine, int maxAmmo )
    {
        string max = maxAmmo.ToString();

        // If the max ammo is -1 (infinite)
        if( maxAmmo == -1 )
        {
            // Set the text to the infinity symbol
            max = ( ( char )0x221E ).ToString();
        }

        // Display ammo count
        m_ammo.text = ( magazine + "/" + max );
    }

    /***************************************************
    *   Function        :    UpdateAmmo
    *   Purpose         :    Updates Ammo UI
    *   Parameters      :    int magazine
    *                        int maxAmmo
    *                        Color colour
    *   Returns         :    void
    *   Date altered    :    21/07/22
    *   Contributors    :    WH
    *   Notes           :    Overload with colour changing
    *   See also        :    
    ******************************************************/
    public void UpdateAmmo( int magazine, int maxAmmo, Color colour )
    {
        // Update Ammo
        UpdateAmmo( magazine, maxAmmo );

        // Set the colour of the text
        m_ammo.material.SetColor( ShaderUtilities.ID_GlowColor, colour );
    }

    /***************************************************
    *   Function        :    UpdateHealth
    *   Purpose         :    Updates healt UI
    *   Parameters      :    float health
    *                        float maxHealth
    *   Returns         :    void
    *   Date altered    :    19/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateHealth( float health, float maxHealth )
    {
        m_health.maxValue = maxHealth;
        m_health.value = health;
    }

    /***************************************************
    *   Function        :    UpdateMoney
    *   Purpose         :    Updates money UI
    *   Parameters      :    int money
    *   Returns         :    void
    *   Date altered    :    19/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateMoney( int money )
    {
        m_money.text = ( money.ToString() );
    }

    /***************************************************
    *   Function        :    UpdateStatus
    *   Purpose         :    Shows what effect the pickup gave
    *   Parameters      :    string text
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateStatus( string text )
    {
        // Activate and display text
        m_status.gameObject.SetActive( true );
        m_status.text = ( text );

        // Start timer to deactivate if HUD is active
        if( gameObject.activeSelf )
        {
            StartCoroutine( DelayDeactivate() );
        }
    }

    /***************************************************
    *   Function        :    UpdateInteract
    *   Purpose         :    Shows what an interactable is
    *   Parameters      :    string text
    *   Returns         :    void
    *   Date altered    :    21/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateInteract( string text )
    {
        m_interact.text = ( text );
    }

    /***************************************************
    *   Function        :    UpdateShield
    *   Purpose         :    Updates shield bar
    *   Parameters      :    float value
    *                        float maxValue
    *   Returns         :    void
    *   Date altered    :    19/07/22
    *   Contributors    :    WH
    *   Notes           :    Changed from stamina
    *   See also        :    
    ******************************************************/
    public void UpdateShield( float value, float maxValue )
    {
        m_shield.maxValue = maxValue;
        m_shield.value = value;
    }


    /***************************************************
    *   Function        :    UpdateStamina
    *   Purpose         :    Updates stamina wheel
    *   Parameters      :    float value
    *                        float maxValue
    *   Returns         :    void
    *   Date altered    :    19/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateStamina( float value )
    {
        m_stamina.fillAmount = value;
    }

    /***************************************************
    *   Function        :    UpdateGrenadeIcon
    *   Purpose         :    Updates grenade icon
    *   Parameters      :    RawImage icon
    *   Returns         :    void
    *   Date altered    :    21/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateGrenadeIcon( RawImage icon )
    {
        m_grenade.texture = icon.texture;
    }

    /***************************************************
    *   Function        :    UpdateGrenadeCount
    *   Purpose         :    Updates grenade count
    *   Parameters      :    int count
    *   Returns         :    void
    *   Date altered    :    21/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateGrenadeCount( int count )
    {
        m_grenadeCount.text = count.ToString();
    }

    /***************************************************
    *   Function        :    UpdateGrenadeCount
    *   Purpose         :    Updates grenade count
    *   Parameters      :    int count
    *                        Color colour
    *   Returns         :    void
    *   Date altered    :    21/07/22
    *   Contributors    :    WH
    *   Notes           :    Overload with glow colour
    *   See also        :    
    ******************************************************/
    public void UpdateGrenadeCount( int count, Color colour )
    {
        // Update count and set colour of text
        m_grenadeCount.text = count.ToString();
        m_grenadeCount.material.SetColor( ShaderUtilities.ID_GlowColor, colour );
    }

    /***************************************************
    *   Function        :    SetInteractActive
    *   Purpose         :    Toggle for activating the UI element
    *   Parameters      :    bool active
    *   Returns         :    void
    *   Date altered    :    21/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetInteractActive( bool active )
    {
        m_interact.gameObject.SetActive( active );
    }

    /***************************************************
    *   Function        :    ActivateGrenadeUI
    *   Purpose         :    Activates Grenade icon and count
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void ActivateGrenadeUI()
    {
        m_grenade.gameObject.SetActive( true );
        m_grenadeCount.gameObject.SetActive( true );
    }

    /***************************************************
    *   Function        :    DelayDeactivate
    *   Purpose         :    Deactivates statis after timer seconds
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    10/05/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    private IEnumerator DelayDeactivate()
    {
        // Wait for timer seconds
        yield return new WaitForSeconds( m_statusTimer );

        // Deactivate status text
        m_status.gameObject.SetActive( false );
    }
}
