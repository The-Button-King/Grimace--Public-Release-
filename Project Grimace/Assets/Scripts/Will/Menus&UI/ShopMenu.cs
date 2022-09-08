using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: ShopMenu
 *
 * Author: Will Harding
 *
 * Purpose: Code for shop menu
 * 
 * Functions:   private void Awake()
 *              public void CloseShopMenu()
 *              public void OpenShopMenu()
 *              private bool ReduceMoney( int cost )
 *              public void ChooseOption( int selection )
 *              private void RefreshDropdown()
 *              public void Buy()
 *              private void UpdateAssaultVariables()
 *              private void UpdateChargeVariables()
 *              private void UpdateShotgunVariables()
 *              private void UpdateGrenadeVariables()
 *              private void UpdatePlayerVariables()
 *              private void UpdateVariables()
 *              public void PlayButtonSound()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 08/04/22     WH          1.0         Initally created
 * 27/04/22     WH          1.1         Comment headers
 * 06/05/22     WH          1.2         Added UI connectivity
 * 10/05/22     WH          1.3         Money works
 * 12/05/22     WH          1.4         Final comment run through before submission
 * 
 * 23/06/22     WH          2.0         Added asset pooling for pickups
 * 18/07/22     WH          2.1         Started to add functions for new menu design
 * 19/07/22     WH          2.2         Fixed gun referencing
 * 25/07/22     WH          2.3         Added camera switch
 * 26/07/22     WH          2.4         Added random options and buying and comments
 * 27/07/22     WH          2.5         Bugfixing
 * 28/07/22     WH          2.6         Added sounds
 * 05/08/22     WH          2.7         Added purchase counting
 * 16/08/22     WH          2.8         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof( ShopAudioPool ) )]
public class ShopMenu : MonoBehaviour
{
    private     PlayerManager               m_player;                   // The player
    private     AssaultRifle                m_assaultRifle;             // Assault Rifle
    private     ChargeRifle                 m_chargeRifle;              // Charge Rifle
    private     Shotgun                     m_shotgun;                  // Shotgun
    private     GrenadeManager              m_grenadeManager;           // Grenade manager

    private     DataHolder                  m_dataHolder;               // Data Holder

    private     ShopAudioPool               m_audioPool;                // Audio Pool

    [Header( "Non-Menu Object Reference" )]
    [SerializeField]
    [Tooltip( "The Parent Game Object script" )]
    private     VendingMachine              m_vendingMachine;           // Vending Machine

    [Tooltip( "The camera that shows the menu in game" )]
    public      CinemachineVirtualCamera    m_shopCam;                  // Camera that shows the shop

    private     CinemachineVirtualCamera    m_playerCam;                // Player's camera

    [Tooltip( "GameObject holding free items" )]
    public      GameObject                  m_optionsFreeHolder;        // Holds available free options

    [Tooltip( "GameObject holding paid items" )]
    public      GameObject                  m_optionsHolder;            // Holds available options

    [SerializeField]
    [Tooltip( "GameObject holding paid items" )]
    [Min(1)]
    private     int                         m_numOptions = 5;           // Number of available options in shop
    private     int                         m_selectedOption;           // Index of selected option from dropdown


    [Header( "Menu Object References" )]

    [Tooltip( "The Dropdown list of stock to buy" )]
    public      TMP_Dropdown                m_dropdown;                 // Dropdown list
    private     List<ShopOption>            m_options;                  // List of options for dropdown menu

    [Tooltip( "The money count" )]
    public      TMP_Text                    m_money;                    // Money UI

    [Tooltip( "The box containing the price maths and buy button" )]
    public      GameObject                  m_priceBox;                 // The boc with all the money maths and buy button

    [Tooltip( "Text of Current Money in price box" )]
    public      TMP_Text                    m_priceBoxMoney;            // Your current money displayed in the pricebox

    [Tooltip( "Text of cost of item in price box" )]
    public      TMP_Text                    m_priceBoxCost;             // The item cost displayed in the pricebox

    [Tooltip( "Text of your Discretionary balance in price box" )]
    public      TMP_Text                    m_priceBoxDisc;             // Your discretionary balance after purchase displayed in the pricebox

    [Space( 10 )]
    [Header( "Assault Rifle Variables" )]
    public      TMP_Text                    m_assaultDamage;            // AR Damage text
    public      TMP_Text                    m_assaultROF;               // AR ROF text
    public      TMP_Text                    m_assaultMagSize;           // AR Mag size text
    public      TMP_Text                    m_assaultRecoil;            // AR Recoil text
    public      TMP_Text                    m_assaultReloadSpeed;       // AR Reload speed text
    public      TMP_Text                    m_assaultMaxAmmo;           // AR Max ammo text

    [Space( 10 )]
    [Header( "Charge Rifle Variables" )]
    public      TMP_Text                    m_chargeDamage;             // CR Damage text
    public      TMP_Text                    m_chargeSpeed;              // CR Charge speed text
    public      TMP_Text                    m_chargeMagSize;            // CR Mag size text
    public      TMP_Text                    m_chargeOverheat;           // CR Overheat text
    public      TMP_Text                    m_chargeReloadSpeed;        // CR Reload speed text
    public      TMP_Text                    m_chargeMaxAmmo;            // CR Max ammo text

    [Space( 10 )]
    [Header( "Shotgun Variables" )]
    public      TMP_Text                    m_shotgunDamage;            // SG Damage text
    public      TMP_Text                    m_shotgunPelletCount;       // SG pellet count text
    public      TMP_Text                    m_shotgunMagSize;           // SG Mag size text
    public      TMP_Text                    m_shotgunReloadSpeed;       // SG Reload speed text
    public      TMP_Text                    m_shotgunMaxAmmo;           // SG Max ammo text

    [Space( 20 )]
    [Header( "Grenade Variables" )]
    public      TMP_Text                    m_grenadeExplosive;         // Explosive count text
    public      TMP_Text                    m_grenadePoison;            // Poison count text
    public      TMP_Text                    m_grenadeSmoke;             // Smoke count text
    public      TMP_Text                    m_grenadeIce;               // Ice count text

    [Space( 20 )]
    [Header( "Player Variables" )]
    public      TMP_Text                    m_playerShieldRecharge;     // Shield recharge text
    public      TMP_Text                    m_playerSpeed;              // Speed text
    public      TMP_Text                    m_playerStaminaRecharge;    // Stamina recharge text
    public      TMP_Text                    m_playerDashCost;           // Dash cost text

    [Space( 10 )]
    [Header( "Player Sliders and Text" )]
    public      Slider                      m_playerHealth;             // Health slider
    public      Slider                      m_playerShield;             // Shiled slider
    public      Slider                      m_playerStamina;            // Stamina slider
    public      TMP_Text                    m_playerHealthValue;        // Health text
    public      TMP_Text                    m_playerShieldValue;        // Shield text
    public      TMP_Text                    m_playerStaminaValue;       // Stamina text

    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Sets variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Awake()
    {
        // Get components
        GameObject player = GameObject.FindGameObjectWithTag( "Player" );
        m_dataHolder = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        m_player = player.GetComponent<PlayerManager>();
        m_assaultRifle = player.GetComponentInChildren<AssaultRifle>( true );
        m_chargeRifle = player.GetComponentInChildren<ChargeRifle>( true );
        m_shotgun = player.GetComponentInChildren<Shotgun>( true );
        m_grenadeManager = player.GetComponent<GrenadeManager>();
        m_audioPool = GetComponent<ShopAudioPool>();
        m_playerCam = player.transform.root.GetComponentInChildren<CinemachineVirtualCamera>();

        // Set money text
        m_money.text = ( m_player.GetMoney().ToString() );

        // Make options list
        m_options = new List<ShopOption>( m_numOptions );

        // Get random free item and add to the options list
        ShopOption[] free = m_optionsFreeHolder.GetComponentsInChildren<ShopOption>();
        m_options.Add( free[ Random.Range( 0, free.Length ) ] );

        // Get a list of all the paid options
        List<ShopOption> allOptions = m_optionsHolder.GetComponentsInChildren<ShopOption>().ToList();

        // Index of option
        int index;

        // Get a random selection of upgrades
        for ( int i = 0; i < m_numOptions - 1; i++ )
        {
            // Get a random item
            index = Random.Range( 0, allOptions.Count );

            //Add to options list
            m_options.Add( allOptions[ index ] );

            // Remove option from list to not get duplicates
            allOptions.RemoveAt( index );
        }

        // Update the dropdown and open the menu
        RefreshDropdown();
        OpenShopMenu();
    }

    /***************************************************
    *   Function        :    CloseShopMenu
    *   Purpose         :    Closes menu
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void CloseShopMenu()
    {
        // Deactivate menu
        gameObject.SetActive(false);

        // Disables the mouse and re-enables the player controls
        m_player.DisableMenuControls();

        m_shopCam.Priority = m_playerCam.Priority - 1;

        // Play coffee sound on coffee machine and animate
        m_vendingMachine.Dispense();
    }

    /***************************************************
    *   Function        :    OpenShopMenu
    *   Purpose         :    Updates variables when opening shop
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void OpenShopMenu()
    {
        // Update the variables
        UpdateVariables();

        // Enable menu controls
        m_player.EnableMenuControls();

        // Lerp the cameras
        m_shopCam.Priority = m_playerCam.Priority + 1;
    }

    /***************************************************
    *   Function        :    ReduceMoney
    *   Purpose         :    Reduces the player's money
    *   Parameters      :    int cost
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private bool ReduceMoney( int cost )
    {
        // Get the new money after buying
        int newMoney = m_player.GetMoney() - cost;

        // If you have enough money
        if( newMoney >= 0 )
        {
            // Set's player's new money
            m_player.SetMoney( newMoney );
        
            // Display to UI
            m_money.text = newMoney.ToString();
            m_priceBoxMoney.text = newMoney.ToString();

            // Return true, you have successfully purchased something
            return true;
        }
        else
        {
            // Return false, you could not afford it
            return false;
        }
    }


    /***************************************************
    *   Function        :    ChooseOption
    *   Purpose         :    Updates UI for the selected option in the dropdown
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void ChooseOption( int selection )
    {
        // Dynamic int from dropdown gives array position of the ShopOption selected
        m_selectedOption = selection;

        // Get price
        int price = m_options[ selection ].GetPrice();

        // Display prices on the correct ui texts
        m_priceBox.SetActive( true );
        m_priceBoxCost.text = price.ToString();
        m_priceBoxDisc.text = ( m_player.GetMoney() - price ).ToString();

    }

    /***************************************************
    *   Function        :    RefreshDropdown
    *   Purpose         :    Refreshes options available in the dropdown
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void RefreshDropdown()
    {
        // The list of option names
        List<string> optionsToAdd = new List<string>();

        foreach( var item in m_options )
        {
            // Add each text to the list
            optionsToAdd.Add( item.GetDisplayText() );
        }

        // Clear the existing options
        m_dropdown.ClearOptions();
        
        // Add new options
        m_dropdown.AddOptions( optionsToAdd );
    }

    /***************************************************
    *   Function        :    Buy
    *   Purpose         :    Buys options
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Buy()
    {
        // If you have enough money
        if( ReduceMoney( m_options[ m_selectedOption ].GetPrice() ) )
        {
            // Play sound
            m_audioPool.PlaySound( m_audioPool.m_buy );

            // Do effect
            m_options[ m_selectedOption ].GetEffect().PowerUpEffect();

            // Remove option from the shop
            m_options.RemoveAt( m_selectedOption );

            // Refresh the dropdown menu and update stats
            RefreshDropdown();
            UpdateVariables();

            m_dataHolder.SetPurchases( m_dataHolder.GetPurchases() + 1 );
        }
        else
        {
            // Play sound
            m_audioPool.PlaySound( m_audioPool.m_failBuy );
        }

    }


    /***************************************************
    *   Function        :    UpdateAssaultVariables
    *   Purpose         :    Updates assault rifle stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateAssaultVariables()
    {
        // Get variables
        (int, int, int, float, float) gunStats = m_assaultRifle.GetGunStats();
        (float, float) assaultStats = m_assaultRifle.GetAssaultStats();

        // Set the variables to the correct texts on the ui
        m_assaultDamage.text = gunStats.Item5.ToString();
        m_assaultROF.text = ( 1 / assaultStats.Item1 ).ToString("F1") + "bps";
        m_assaultMagSize.text = gunStats.Item2.ToString();
        m_assaultRecoil.text = "x" + assaultStats.Item2;
        m_assaultReloadSpeed.text = gunStats.Item4 + "s";
        m_assaultMaxAmmo.text = gunStats.Item3.ToString();
    }

    /***************************************************
    *   Function        :    UpdateChargeVariables
    *   Purpose         :    Updates charge rifle stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateChargeVariables()
    {
        // Get variables
        (int, int, int, float, float) gunStats = m_chargeRifle.GetGunStats();
        (float, float) chargeStats = m_chargeRifle.GetChargeStats();

        // Set the variables to the correct texts on the ui
        m_chargeDamage.text = gunStats.Item5.ToString();
        m_chargeSpeed.text = chargeStats.Item1 + "s";
        m_chargeMagSize.text = gunStats.Item2.ToString();
        m_chargeOverheat.text = chargeStats + "s";
        m_chargeReloadSpeed.text = gunStats.Item4 + "s";
        m_chargeMaxAmmo.text = gunStats.Item3.ToString();

    }

    /***************************************************
    *   Function        :    UpdateShotgunVariables
    *   Purpose         :    Updates shotgun stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateShotgunVariables()
    {
        // Get variables
        (int, int, int, float, float) gunStats = m_shotgun.GetGunStats();
        int shotgunStats = m_shotgun.GetShotgunStats();

        // Set the variables to the correct texts on the ui
        m_shotgunDamage.text = gunStats.Item5 + "x" + shotgunStats;
        m_shotgunPelletCount.text = shotgunStats.ToString();
        m_shotgunMagSize.text = gunStats.Item2.ToString();
        m_shotgunReloadSpeed.text = gunStats.Item4 + "s";
        m_shotgunMaxAmmo.text = gunStats.Item3.ToString();
    }

    /***************************************************
    *   Function        :    UpdateGrenadeVariables
    *   Purpose         :    Updates grenade stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateGrenadeVariables()
    {
        List<int> counts = m_grenadeManager.GetCounts();

        // Upgradable stats
        m_grenadeExplosive.text = counts[ 0 ].ToString();
        m_grenadePoison.text = counts[ 1 ].ToString();
        m_grenadeSmoke.text = counts[ 2 ].ToString();
        m_grenadeIce.text = counts[ 3 ].ToString();
    }

    /***************************************************
    *   Function        :    UpdatePlayerVariables
    *   Purpose         :    Updates player stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdatePlayerVariables()
    {
        List<float> vars = m_player.GetPlayerVars();

        // Upgradable stats
        m_playerShieldRecharge.text = vars[0] + "pts/s";
        m_playerSpeed.text = vars[ 1 ] + "m/s";
        m_playerStaminaRecharge.text = vars[ 2 ] + "pts/s";
        m_playerDashCost.text = vars[ 3 ].ToString();

        // Bar values
        m_playerHealth.maxValue = vars[ 5 ];
        m_playerHealth.value = vars[ 4 ];
        m_playerShield.maxValue = vars[ 7 ];
        m_playerShield.value = vars[ 6 ];
        m_playerStamina.maxValue = vars[ 9 ];
        m_playerStamina.value = vars[ 8 ];

        // Bar texts
        m_playerHealthValue.text = vars[ 4 ].ToString("F0") + "/" + vars[ 5 ];
        m_playerShieldValue.text = vars[ 6 ].ToString( "F0" ) + "/" + vars[ 7 ];
        m_playerStaminaValue.text = vars[ 8 ].ToString( "F0" ) + "/" + vars[ 9 ];
    }

    /***************************************************
    *   Function        :    UpdateVariables
    *   Purpose         :    Updates stat variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void UpdateVariables()
    {
        // Update all the stats
        UpdateAssaultVariables();
        UpdateChargeVariables();
        UpdateShotgunVariables();
        UpdateGrenadeVariables();
        UpdatePlayerVariables();

        // Set money text
        string money = m_player.GetMoney().ToString();
        m_money.text = money;
        m_priceBoxMoney.text = money;

        // Display values in price box
        ChooseOption( 0 );
    }

    /***************************************************
    *   Function        :    PlayButtonSound
    *   Purpose         :    Plays sound when clicking button in menu
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void PlayButtonSound()
    {
        // Play sound
        m_audioPool.PlaySound( m_audioPool.m_buttonClick );
    }
}
