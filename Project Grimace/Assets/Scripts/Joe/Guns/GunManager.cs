using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GunManager : MonoBehaviour
{
    /****************************************************************************************************
    * Type: Class
    * 
    * Name: GunManager
    *
    * Author: Joseph Gilmore
    *
    * Purpose: Manage all gun types and their inputs 
    * 
    * Functions:     void Awake()
    *                private void OnEnable()
    *                private void OnDisable()
    *                public void EnableInputs()
    *                public void DisableInputs()
    *                private void AsssignInputActions()
    *                void Update()
    *                private void ReloadGun(InputAction.CallbackContext context)
    *                private void OnFire()
    *                private void StopFire()
    *                private IEnumerator ShootPerformed()
    *                public float GetHoldTime()
    *                public void SetHoldTime(float time = 0)
    *                private void SwitchFireMode( bool ignoreLastIndex = false)
    *                private void ScrollWeapons(InputAction.CallbackContext context)
    *                private void PressWeaponSwitch(InputAction.CallbackContext context)
    *                public GunStats GetCurrentGun()
    *                public Guns GetCurrentGunState()
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
    * 01/07/2022    JG          1.01        - Added weapon switch for 123 keys  
    * 03/07/2022    JG          1.02        - Fixed inputs to work for all firing modes 
    * 20/07/2022    JG          1.03        - Hud udpate
    * 27/07/2022    JG          1.04        - Reference change added animations 
    * 31/07/2022    JG          1.05        - Minor bug fixes 
    * 02/08/2022    WH          1.06        - Added GetCurrentGunState
    * 03/08/2022    JG          1.07        - Cleaning
    * 13/08/2022    WH          1.08        - Input managing
    * 14/08/2022    JG          1.09        - Cleaned 
    ****************************************************************************************************/
    private InputAction         m_reload;                     // Store the input action of reloading equpied gun
    private InputAction         m_shootClick;                 // Store the input action of shooting 
    private InputAction         m_scrollSwitch;               // Store the input action for srolling to change weapon
    private InputAction         m_shootTap;                   // Store the input action for tap fire   
    private InputAction         m_switchWeaponAction;         // Weapon switch action 
    private InputAction         m_shootHold;                  // Stores the input action for hold 
    private Coroutine           m_shooting;                   // Coroutine of when the player shoots
    private AssaultRifle        m_assultRifle;                // Reference to Assult Rifle 
    private Shotgun             m_shotgun;                    // Reference to shotgun  
    private ChargeRifle         m_chargeRifle;                // Rerfernce to charge rifle   
    private PlayerControls      m_playerControls;             // Reference to input manager 
    private GunStats            m_currentGun;                 // Current Gun type  
    private CrosshairManager    m_crosshairManager;           // Rerefence to crosshair manager   
    private float               m_holdtime = 0f;              // The amount of time the mouse has been held down for
    private int                 an_gunIndex;                  // String to has for gun index on blend tree
    public enum Guns                                          // Different Firemodes
    {
        Assault,
        Charge,
        Shotgun
    }
    private Guns                m_gunState = Guns.Assault;     // Set default gun state 
    private int                 m_weaponIndex = 1;             // Set weapon slot index 
    private const int           m_maxIndex = 3;                // Max weapon slots 
    private int                 m_lastIndex;                   // Last Index stored 
    private Animator            m_gunAnimator;                 // Reference to gun animator
    [Header("Materials for different gun types to show Icon")]
    [SerializeField]
    private Material            m_assultRilfeMaterial;
    [SerializeField]
    private Material            m_chargeRilfeMaterial;
    [SerializeField]
    private Material            m_shotgunMaterial;
    [SerializeField]
    private SkinnedMeshRenderer m_gunRenderer;                  // Gun renderer 
    /***************************************************
    *   Function        : Awake  
    *   Purpose         : Setup the class   
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    void Awake()
    {


        // Get references to fire modes
        m_assultRifle = GetComponentInChildren<AssaultRifle>();
        m_chargeRifle = GetComponentInChildren<ChargeRifle>();
        m_shotgun = GetComponentInChildren<Shotgun>();

        an_gunIndex = Animator.StringToHash( "GunIndex" );

        // Get reference to crosshair manager
        m_crosshairManager = GetComponent<CrosshairManager>(); 

        // Get refernce to inputs
        m_playerControls = transform.root.GetComponentInChildren<PlayerManager>().GetControls();

        // Get animator for all gun types
        m_gunAnimator = GetComponentInChildren<Animator>();

        // Assign inputs an disable them
        AsssignInputActions();
        DisableInputs();
    }

    /***************************************************
    *   Function        : OnEnable
    *   Purpose         : Setup inputs  
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 03/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        AsssignInputActions();

        // Disable all 
        m_shootClick.Disable();
        m_shootHold.Disable();
        m_shootTap.Disable();

        // Does not matter just to stop reference erros
        m_currentGun = m_assultRifle;
        
        // Set up starting mode 
        SwitchFireMode();

        // Set current crosshair
        m_crosshairManager.UpdateGunCrossHair( m_gunState );


    }

    /***************************************************
    *   Function        : OnDisable
    *   Purpose         : Disables inputs  
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 13/08/22
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnDisable()
    {
        DisableInputs();
    }

    /***************************************************
    *   Function        : EnableInputs
    *   Purpose         : Enables inputs  
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 13/08/22
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void EnableInputs()
    {
        m_scrollSwitch.Enable();
        m_switchWeaponAction.Enable();
        m_reload.Enable();

        SwitchFireMode( true );
    }

    /***************************************************
    *   Function        : DisableInputs
    *   Purpose         : Disables inputs  
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 13/08/22
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void DisableInputs()
    {
        m_shootClick.Disable();
        m_shootHold.Disable();
        m_shootTap.Disable();
        m_scrollSwitch.Disable();
        m_switchWeaponAction.Disable();
        m_reload.Disable();
    }

    /***************************************************
    *   Function        : AssignInputActions
    *   Purpose         : 
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 03/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void AsssignInputActions()
    {
        // Assign different shooting types 
        m_shootClick = m_playerControls.Player.ClickShoot;
        m_shootHold = m_playerControls.Player.HoldShoot;
        m_shootTap = m_playerControls.Player.TapShoot;

        // Set up Weapon switching 
        m_switchWeaponAction = m_playerControls.Player.SwitchWep;
        m_scrollSwitch = m_playerControls.Player.ScrollWep;
        m_scrollSwitch.Enable();
        m_switchWeaponAction.Enable();

        // Call SwitchFireMode on action performed 
        m_switchWeaponAction.performed += PressWeaponSwitch;
        m_scrollSwitch.performed += ScrollWeapons;

        // When action started call function
        m_shootClick.performed += _ => OnFire();
        m_shootHold.performed += _ => OnFire();
        m_shootTap.performed += _ => OnFire();

        // When action finished stop shooting
        m_shootClick.canceled += _ => StopFire();
        m_shootHold.canceled += _ => StopFire();
        m_shootTap.canceled += _ => StopFire(); 


        // Set reload input & enable it 
        m_reload = m_playerControls.Player.Reload;
        m_reload.Enable();
        m_reload.performed += ReloadGun;
    }
     /***************************************************
     *   Function        : Update 
     *   Purpose         : Update the manager 
     *   Parameters      : N/A 
     *   Returns         : Void  
     *   Date altered    : 31/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    void Update()
    {
       
        // If shooting started and charge rifle 
        if (m_shootHold.phase == InputActionPhase.Started && m_gunState == Guns.Charge && m_currentGun.GetState() != GunStats.GunState.Reloading)
        {
           // Increase hold time of mouse 
            m_holdtime += Time.deltaTime;
        }

    }
    /***************************************************
    *   Function        : ReloadGun
    *   Purpose         : Start the process of reloading 
    *   Parameters      : InputAction.CallbackContext context
    *   Returns         : Void  
    *   Date altered    : 30/06/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void ReloadGun(InputAction.CallbackContext context)
    {
        // Start reload action
        StartCoroutine(m_currentGun.Reload());
    }
    /***************************************************
     *   Function        : OnFire    
     *   Purpose         : When the gun needs to shoot do it for  set inputs
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 03/07/2022 
     *   Contributors    : JG
     *   Notes           : 
     *   See also        :    
    ******************************************************/
    private void OnFire()
    {
      // If click mode start coroutine for constant firing 
      if ( m_shootClick.enabled )
      {
          m_shooting = StartCoroutine( ShootPerformed() );
      }
      else
      {
          // Shoot current selected gun 
          m_currentGun.FireGun();

          // Reset mouse hold time 
          m_holdtime = 0f;
      }

    }
    /***************************************************
    *   Function        : StopFire   
    *   Purpose         : Stop the gun from autmaticly firing   
    *   Parameters      : N/A  
    *   Returns         : Void   
    *   Date altered    : 03/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
   ******************************************************/
    private void StopFire()
    {
        
        // Stop coroutine if used to fire 
        if (m_shooting != null && m_shootClick.enabled == true)
        {
            // Stop Coroutine & update gun state 
            StopCoroutine(m_shooting);

        }
       if(m_currentGun != null )
        {
            // Stop firing 
            m_currentGun.StopShooting();

            // Rest mouse down time
            m_holdtime = 0;
        }
           

       
        
    }
    /***************************************************
     *   Enumerator      :  ShootPerformed  
     *   Purpose         :  Automaticly shoot gun until coroutine stopped   
     *   Parameters      :  N/A  
     *   Returns         :  yield return null   
     *   Date altered    :  30/06/2022 
     *   Contributors    :  JG
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private IEnumerator ShootPerformed()
    {
        while (true)
        {
           
            // Shoot current selected gun 
            m_currentGun.FireGun();

            // Reset mouse hold time 
            m_holdtime = 0f;
            
            yield return null;

        }
    }
    /***************************************************
     *   Function        :  GetHoldTime
     *   Purpose         :  Return hold time 
     *   Parameters      :  N/A  
     *   Returns         :  Vois  
     *   Date altered    :  30/06/2022 
     *   Contributors    :  JG
     *   Notes           :    
     *   See also        :  GunStats children   
    ******************************************************/
    public float GetHoldTime()
    {
        return m_holdtime;
    }
    /***************************************************
     *   Function        :  SetHoldTime 
     *   Purpose         :  Set HoldTime var   
     *   Parameters      :  float time = 0 
     *   Returns         :  Void  
     *   Date altered    :  30/06/2022 
     *   Contributors    :  JG
     *   Notes           :    
     *   See also        :   GunStats children    
    ******************************************************/
    public void SetHoldTime( float time = 0 )
    {
        m_holdtime = time;
    }
    /***************************************************
    *   Function        :  SwitchFireMode 
    *   Purpose         :  Switch gun is currently fireing 
    *   Parameters      :  bool ignoreLastIndex = false
    *   Returns         :  Void  
    *   Date altered    :  13/08/2022 
    *   Contributors    :  JG
    *   Notes           :   
    *   See also        :  
   ******************************************************/
    private void SwitchFireMode( bool ignoreLastIndex = false )
    {
        if ( m_currentGun != null )
        {

            // Check for a change in weapon
            if ( ignoreLastIndex || m_weaponIndex != m_lastIndex )
            {
                // Stop firing current weapon 
                StopFire();

                switch ( m_weaponIndex )
                {
                    case 1:
                    {
                        // Disable last gun
                        m_currentGun.enabled = false;

                        // Enable AssultRilfe script 
                        m_assultRifle.enabled = true;
                        m_assultRifle.UpdateHUD();

                        // Set to current gun
                        m_currentGun = m_assultRifle;

                        // Disable other input modes
                        m_shootHold.Disable();
                        m_shootTap.Disable();

                        // Enable correct input mode 
                        m_shootClick.Enable();

                        // Update state 
                        m_gunState = Guns.Assault;

                        // Set animator 
                        m_gunAnimator.SetFloat( an_gunIndex, 0.0f );

                        m_gunRenderer.material = m_assultRilfeMaterial;

                    }
                    break;
                    case 2:
                    {
                        // Set State to charge 
                        m_gunState = Guns.Charge;

                        // Disable last gun script 
                        m_currentGun.enabled = false;

                        // Enable Charge rilfe script 
                        m_chargeRifle.enabled = true;
                        m_chargeRifle.UpdateHUD();

                        // Update current gun
                        m_currentGun = m_chargeRifle;

                        // Update input methods 
                        m_shootClick.Disable();
                        m_shootTap.Disable();
                        m_shootHold.Enable();

                        // Set animator 
                        m_gunAnimator.SetFloat( an_gunIndex, 1.0f );

                        // Set material
                        m_gunRenderer.material = m_chargeRilfeMaterial;
                    }
                    break;
                    case 3:
                    {
                        // Set State to charge 
                        m_gunState = Guns.Shotgun;

                        // Disable last gun
                        m_currentGun.enabled = false;

                        m_shotgun.enabled = true;
                        m_shotgun.UpdateHUD();

                        // Update current gun
                        m_currentGun = m_shotgun;

                        // Update input methods 
                        m_shootClick.Disable();
                        m_shootHold.Disable();
                        m_shootTap.Enable();

                        // Set animator 
                        m_gunAnimator.SetFloat( an_gunIndex, 2.0f );

                        m_gunRenderer.material = m_shotgunMaterial;
                    }
                    break;

                }
                // Set last index
                m_lastIndex = m_weaponIndex;

                // Update crosshair to current gun
                m_crosshairManager.UpdateGunCrossHair( m_gunState );
            }

        }
    }


    /***************************************************
    *   Function        :  ScrollWeapons
    *   Purpose         :  Update gun index by scrolling 
    *   Parameters      :  InputAction.CallbackContext context
    *   Returns         :  Void  
    *   Date altered    :  01/07/2022 
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    private void ScrollWeapons( InputAction.CallbackContext context )
    {
        // Get mouse up or down value 
        float scrollValue = context.ReadValue<float>();

        // If scroll up
        if (scrollValue > 0)
        {
           
            if (m_weaponIndex > 1)
            {
                // Go down weapon index
                m_weaponIndex--;
            }
        }
        // If scroll down 
        if(scrollValue < 0)
        {
            if (m_weaponIndex < m_maxIndex)
            {
                // Go up weapon index
                m_weaponIndex++;
            }
        }
        // Update weapons 
        SwitchFireMode();
    }
    /***************************************************
   *   Function        :  ScrollWeapons
   *   Purpose         :  Update gun index by scrolling 
   *   Parameters      :  InputAction.CallbackContext context
   *   Returns         :  Void  
   *   Date altered    :  01/07/2022 
   *   Contributors    :  JG
   *   Notes           :    
   *   See also        :  
   ******************************************************/
    private void PressWeaponSwitch( InputAction.CallbackContext context )
    {
        // Get value of num keys 
        int.TryParse( context.control.name, out int value );

        // If value more than 0 (num pressed)
        if(value > 0)
        {
            // Update index and switch 
            m_weaponIndex = value;
            SwitchFireMode();
        }
       
    }
    /***************************************************
    *   Function        :  GetCurrentGun
    *   Purpose         :  reuturn current selected gun 
    *   Parameters      :  N/A
    *   Returns         :  Void  
    *   Date altered    :  Uk
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    public GunStats GetCurrentGun()
    {
        return m_currentGun;
    }

    /***************************************************
    *   Function        :  GetCurrentGunState
    *   Purpose         :  Gets gunstate
    *   Parameters      :  None
    *   Returns         :  void  
    *   Date altered    :  02/08/2022 
    *   Contributors    :  WH
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    public Guns GetCurrentGunState()
    {
        return m_gunState;
    }
   
}
