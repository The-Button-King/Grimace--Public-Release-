using CameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/****************************************************************************************************
* Type: Class
* 
* Name: Player Manager 
*
* Author: JG & WH
*
* Purpose: Manage the player , its data and a range of inputs 
* 
* Functions:                
*                   private void Awake()
*                   private void OnEnable()
*                   void OnDisable()
*                   public void EnableGuns(bool enable = true )
*                   public void EnableGrenades( )
*                   public void GetCarryOverData( List<float> vars )
*                   public int GetMoney()
*                   public void SetMoney( int money )
*                   public bool GetCurrentlyThrowing()
*                   public float GetHealth()
*                   public void SetHealth( float health )
*                   public float GetMaxHealth()
*                   public void SetMaxHealth( float health )
*                   public ( GameObject, int ) GetGrenadeData()
*                   public void SetGrenadeData( GameObject grenade, int count )
*                   public void SetGrenadeData( (GameObject, int) data )
*                   public List<float> GetPlayerVars()
*                   public float GetDashCost()
*                   public void SetDashCost( float cost )
*                   public bool IsGrounded()
*                   public void SetSpeed( float speed )
*                   public float GetDefaultSpeed()
*                   public float GetCurrentSpeed()
*                   public float GetSpeed()
*                   public bool GetForkliftCertification()
*                   public void SetForkliftCertification( bool certified )
*                   private void Movement()
*                   private void ApplyJump()
*                   private void RotateToCamera()
*                   private void Update()
*                   private void GenerateFootSteps()
*                   private void FixedUpdate()
*                   private void Interact()
*                   private bool InteractableRaycast( InteractExecution execution )
*                   public void Damage( float dmg )
*                   public void Jump( InputAction.CallbackContext context )
*                   public void Interact( InputAction.CallbackContext context )
*                   public void Grenade( InputAction.CallbackContext context )
*                   public void ThrowGrenade()
*                   public void Pause( InputAction.CallbackContext context )
*                   public void Dash( InputAction.CallbackContext context )
*                   public void RadialOpen( InputAction.CallbackContext context )
*                   public void RadialClose( InputAction.CallbackContext context )
*                   public IEnumerator DashMovement( Vector3 direction = new Vector3() )
*                   Vector3 GetMoveDirection( bool defaultForward = false )
*                   public void EnableMenuControls()
*                   public void DisableMenuControls()
*                   public PlayerControls GetControls()
*                   public void EnableRadialContols()
*                   public void DisableRadialControls()
*                   private void CheckEffects()
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
* uk            JG          1.00        - Created/setup class 
* 03/05/2022    JG          1.01        - added basic rapid fire 
* 12/05/2022    WH          1.02        - Added comments to my sections and moved control enabling
*                                         functions from InputManager to here
* 31/05/2022    JG          2.00        - Moved dead checking              
* 13/06/2022    WH          2.01        - Added interact audio
* 19/06/2022    JG          2.02        - Added Interface
* 23/06/2022    WH          2.03        - Added grenades
* 24/06/2022    WH          2.04        - Fixed how grenades throw
* 27/06/2022    WH          2.05        - Added pause
* 28/06/2022    WH          2.06        - Added dash and jump that use stamina
* 29/06/2022    JG          2.07        - Removed all gun input and added a getter to PlayerControls
* 30/06/2022    WH          2.08        - Fixed a bug with the interact system
* 04/07/2022    WH          2.09        - Added radial input and rename shop controls functions
* 10/07/2022    JG          2.10        - Gun can now be picked up 
* 11/07/2022    JG          2.11        - Added low health screen effect 
* 12/07/2022    WH          2.12        - Added shield
* 20/07/2022    JG          2.13        - Allow acces to vars for shockwave
* 20/07/2022    WH          2.14        - Added shield effect stuff
* 23/07/2022    JG          2.15        - Fixed screen effect
* 25/07/2022    WH          2.16        - GetVars added and screen effect funtion reworked
* 26/07/2022    WH          2.17        - Adding sound
* 27/07/2022    WH          2.18        - Hiding gun when opening shop
* 28/07/2022    JG          2.19        - first pass of walking anims
* 29/07/2022    WH          2.20        - Added dashcost get set
* 31/07/2022    WH          2.21        - Added forklift certification
* 02/08/2022    WH          2.22        - Added chaning crosshairs and screenshake from pause
* 03/08/2022    WH          2.23        - Fixed low shield damage thing and data holder getting
* 03/08/2022    JG          2.24        - Walking Sound  
* 07/08/2022    JG          2.25        - Grenade throw with animation 
* 10/08/2022    JG          2.26        - Removed warning
* 10/08/2022    WH          2.25        - Added end screen usage on death
* 13/08/2022    JG          2.26        - Second player animations for camera avatar   
* 13/08/2022    WH          2.27        - Disabling inputs and gun inputs
* 14/08/2022    JG          2.28        - Add slow interface for freeze grenades 
* 15/08/2022    WH          2.29        - Fixed radial appearing before grenades
* 15/08/2022    JG          2.30        - Major cleaning 
* 15/08/2022    WH          2.31        - Fixed grenades and sticky gun bugs hopefully, and cleaning
/***************************************************************************************************/
[RequireComponent(typeof(CharacterController))][ RequireComponent(typeof(ChangeScreenEffect))][ RequireComponent(typeof(Stamina))][RequireComponent(typeof(Shield))] [RequireComponent(typeof(GrenadeManager))]
public class PlayerManager : MonoBehaviour, IDamageable<float>, ISloweable
{

    // Player Basics
    private CharacterController              m_characterController;                                                                               // Used to store reference to character controller 
    private PlayerControls                   m_playerControls;                                                                                    // Reference to input script
    private Camera                           m_playerCamera;                                                                                      // Reference to main camera 
    private bool                             m_isAlive;                                                                                           // Is the player alive 
    [SerializeField][Tooltip("Put Gun camera here")]
    private Camera                           m_gunCamera;                                                                                         // Reference to camera that displays the gun  
    private bool                             m_forkliftCertified;                                                                                 // Does the player have access to the forklift   

    private RaycastHit                       m_seenInteractable;                                                                                  // Interactable object last observed

    // InputActions
    private InputAction                      m_move;                                                                                              // Store the input action of moving values 
    private InputAction                      m_interact;                                                                                          // Store the input action of interacting with enviroment
    private InputAction                      m_jump;
    private InputAction                      m_grenade;
    private InputAction                      m_pause;
    private InputAction                      m_dash;
    private InputAction                      m_radial;

    [Header("Player Stats")]
    [SerializeField]
    [Range( 90.0f, 300.0f )]
    [Tooltip("Max amount of health the player can have")]
    private float                           m_maxHealth         = 100.0f;                                                                        // Max amount of health the player can  have
    [SerializeField][Range(1.0f,100.0f)][Tooltip("Amount of health the player starts with")]
    private float                           m_health            = 100.0f;                                                                        // Current player health 
    [SerializeField][Range(10.0f,30.0f)][Tooltip("Amount of health that is considered low to cause a screen effect")]
    private float                           m_lowHealth         = 20.0f;                                                                         // Threshold for low health
    [SerializeField][Range(1.0f,20.0f)][Tooltip("Movement speed of the player")]
    private float                           m_speed;                                                                                             // Movement speed of the player not effected by buffs
    private float                           m_defaultSpeed;
    [SerializeField] [Range( 1.0f, 100.0f )] [Tooltip( "Player starting money" )]
    private int                             m_money             = 0;                                                                             // Money of the player 
    private const float                     k_minHealth         = 0.0f;                                                                          // Min amount of health



    // Pools
    private PlayerAudioPool                 m_audioPool;                                                                                         // Audio pool for playing sounds
    private AssetPool                       m_assetPool;                                                                                         // Asset pool

    [Header("Grenade Information")]
    [SerializeField][Tooltip(" Grenade the player throws")]
    private GameObject                      m_grenadeObject;                                                                                     // Grenade to throw
    [SerializeField][Tooltip("Amount of grenades the player has")]
    private int                             m_grenadeCount;

    [Header("Menus")]
    public  PauseMenu                       m_pauseMenu;                                                                                         // Pause menu to display
    public  EndScreen                       m_endScreen;
    private UIController                    m_ui;                                                                                                // Reference to UI controller 
    public  RadialController                m_radialMenu;                                                                                        // Radial for grenade selection

    [Header("Alternative Movement")]
    [SerializeField][Range(1.1f,5.0f)][Tooltip("Mutipler to increase speed by dashing")]
    private float                           m_dashMultiplier    = 2.0f;                                                                          // How much faster you move from dashing
    [SerializeField][Range(0.1f,1.0f)][Tooltip("The length of a dash")]
    private float                           m_dashDuration      = 0.25f;                                                                         // How long a dash lasts
    [SerializeField][Range(5.0f,50.0f)][Tooltip( "How much Stamina a dash takes up" )]
    private float                           m_dashCost          = 25.0f;                                                                         // How much stamina a dash costs
    [Tooltip("Stamina script controller")]
    public Stamina                          m_stamina;                                                                                           // Stamina manager
    [SerializeField][Range(1.0f,20.0f)][Tooltip("Amount of upwards force for a jumper")]
    private float                           m_jumpForce         = 5f;                                                                            // Force to jump with
    private bool                            m_canJump           = false;                                                                         // Bool for if the player is allowed to jump
    private Vector3                         m_jumpVelocity;                                                                                      // How much veloicty to apply upwards for jumping
    private float                           m_gravity           = -9.81f;                                                                        // How much gravity to apply


    [HideInInspector]
    public  bool                            m_startWithGun      = false;                                                                         // Used for debug play testing
    private bool                            m_startWithGrenades = false;                                                                         // Does player start with grenades
    private DataHolder                      m_dataHolder;

    [Header("Managers")]
    public  Shield                          m_shield;                                                                                            // Shield manager
    private CrosshairManager                m_crosshairManager;                                                                                  // Manages gun crosshairs  
    private ChangeScreenEffect              m_screenEffect;                                                                                      // Reference to Screen effect script
    private GunManager                      m_gunManager;

    [Header("Animators")]
    [SerializeField][Tooltip("Animator for gun and pov")]
    private Animator                        m_gunAnimator;                                                                                       // Reference to gun animatior for movement speed
    [SerializeField][Tooltip( "Animator for scale player on shows on  enviroment cameras" )]                                                     // Reference for scale player animation only shows on enviroment cameras   
    private Animator                        m_scaleManAnims;


    private bool                            m_shieldLerp        = false;                                                                         // Bool for if the shield effect is lerping
  

    

    [Header("FootStep  Sounds")]
    [SerializeField][Range(0.01f,6.0f)][Tooltip(" Mininum player speed to trigger footstep sound")] 
    private float                           m_walkingStepSpeed  = 0.5f;                                                                           // Mininum Speed to trigger footstep sound                                                                                                                                            
    [SerializeField][Range( 0.01f, 1.0f )][Tooltip( "Increase footstep sound when moving faster " )]
    private float                           m_sprintMutiplier   = 0.6f;                                                                           // Used to check if sprinting 
    private AudioSource                     m_stepSource;                                                                                         // Reference to steping audio source   
    [SerializeField][Tooltip("Different step sounds")]
    private AudioClip[]    m_stepClips = default;                                                                                                 // Array of different step sounds   
    private float                           m_footStepTimer     = 0.0f;
    private bool                            m_sprinting         = false;                                                                          // Is currently sprinting   
    private float                           m_footStepOffset    => m_sprinting ? m_walkingStepSpeed * m_sprintMutiplier : m_walkingStepSpeed;     // If player walking set offset to base else set to sprint offset 
    private float                           m_sprintThreshold   = 5.0f;                                                                           // Speed threshold to determind sprinitng or not   

    [SerializeField][Tooltip("Transform for the grenade in animation")]
    private Transform                       m_grenadePosition;                                                                                    // Transform for grenade in animation  
    private GameObject                      m_currentGrenade;                                                                                     // Store current grenade  
    private bool                            m_currentlyThrowing = false;                                                                          // Is the player mid way through grenade throw

    // String to hash
    private int                             an_movement;                                                                                          // Movement string  
    private int                             an_throw;                                                                                             // Throw string to hash 
    
    
    private enum                            InteractExecution                                                                                     // Enum for what action the interactable raycast is doing
    {
        Look,
        Interact,
    }



    /***************************************************
    *   Function        : Awake  
    *   Purpose         : Set up key references for the class    
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 15/08/2022
    *   Contributors    : JG WH 
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    private void Awake()
    {
        // Data holder that contains stats from previous level
        m_dataHolder = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        m_crosshairManager = transform.root.GetComponentInChildren<CrosshairManager>( true );


        // Screen effect reference 
        m_screenEffect = GetComponent<ChangeScreenEffect>();

        // Start with gun or not 
        if ( m_startWithGun )
        {
            EnableGuns();
        }

        // Get step sound source 
        m_stepSource = GetComponent<AudioSource>();

        // Remove cursor from screen 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get reference to atatcthed character controler
        m_characterController = GetComponent<CharacterController>();

        // Get main player camera
        m_playerCamera = transform.Find( "Main Camera" ).GetComponent<Camera>();

        // Create a new input system
        m_playerControls = new PlayerControls();

        // Set default speed 
        m_defaultSpeed = m_speed;



        // Get componenets 
        m_stamina = GetComponent<Stamina>();
        m_shield = GetComponent<Shield>();
        m_ui = transform.root.GetComponentInChildren<UIController>();

        // Enable controls
        m_playerControls.Enable();

        // Update HUD with data 
        m_ui.UpdateMoney( m_money );
        m_ui.UpdateHealth( m_health, m_maxHealth );

        // See if to apply screen shake from settings 
        transform.root.GetComponentInChildren<ScreenShake>().SetToggleScreenShake( m_dataHolder.ApplyScreenShake() );
        

        // Get AudioPool
        m_audioPool = transform.GetComponentInChildren<PlayerAudioPool>();

        // Get Asset Pool
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();

        // Get menus/UI elements 
        m_pauseMenu = GameObject.Find( "PauseMenu" ).GetComponent<PauseMenu>();
        m_endScreen = GameObject.Find( "EndScreen" ).GetComponent<EndScreen>();
        m_radialMenu = GameObject.Find( "GrenadeRadial" ).GetComponent<RadialController>();

        // Get gun manager (before its active)
        m_gunManager = GetComponentInChildren<GunManager>( true );


        // When it's level 1 use default values, otherwise...
        if ( m_dataHolder.GetLevel() > 1 )
        {
            // Set changeable values from previous level
            GetCarryOverData( m_dataHolder.ApplyDataPlayer() );

            // Set grenade counts
            GetComponent<GrenadeManager>().SetCounts( m_dataHolder.ApplyGrenades() );

            // Start with gun and grenades
            m_startWithGun = true;
            m_startWithGrenades = true;
        }

        // Set string hash
        an_movement = Animator.StringToHash( "Movement" );
        an_throw = Animator.StringToHash( "Throw" );
    }


    /***************************************************
    *   Function        : OnEnable  
    *   Purpose         : Enable Input systems  
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 13/08/22
    *   Contributors    : JG WH 
    *   Notes           :
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        // Start with gun or not 
        if ( m_startWithGun )
        {
            EnableGuns();
        }

        // Set movement & enable it
        m_move = m_playerControls.Player.Movement;
        m_move.Enable();

        // Set jump input & enable it 
        m_jump = m_playerControls.Player.Jump;
        m_jump.Enable();
        m_jump.performed += Jump;

        // Set Intereact action & enable it
        m_interact = m_playerControls.Player.Interact;
        m_interact.Enable();
        m_interact.performed += Interact;

        // Set Grenade action and disable it for enabling when you get grenades
        m_grenade = m_playerControls.Player.Grenade;
        m_grenade.Disable();
        m_grenade.performed += Grenade;

        // Set Pause action & enable it
        m_pause = m_playerControls.Player.Pause;
        m_pause.Enable();
        m_pause.performed += Pause;

        // Set Dash action & enable it
        m_dash = m_playerControls.Player.Dash;
        m_dash.Enable();
        m_dash.performed += Dash;

        // Set Radial action and disable it for enabling when you get grenades
        m_radial = m_playerControls.Radial.Toggle;
        m_radial.Disable();
        m_radial.started += RadialOpen;
        m_radial.performed += RadialClose;


        // Start with grenades or not
        if ( m_startWithGrenades )
        {
            EnableGrenades();
        }

        // Make sure the menu contorls are disabled
        DisableMenuControls();
    }

    /***************************************************
    *   Function        : OnDisable  
    *   Purpose         : Disables Input systems  
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 13/08/22
    *   Contributors    : WH 
    *   Notes           :
    *   See also        :    
    ******************************************************/
    void OnDisable()
    {
        // Disable all the player controls

        m_move.Disable();

        m_jump.Disable();

        m_interact.Disable();

        m_grenade.Disable();

        m_pause.Disable();

        m_dash.Disable();

        m_radial.Disable();

    }


    /***************************************************
    *   Function        : EnableGuns 
    *   Purpose         : Let the player pick up a gun
    *   Parameters      : bool enable = true   
    *   Returns         : void    
    *   Date altered    : 15/08/2022
    *   Contributors    : JG
    *   Notes           :  
    *   See also        : PickUpGun
    ******************************************************/
    public void EnableGuns(bool enable = true )
    {
        // Find the gun and set to enable 
        m_gunManager.transform.gameObject.SetActive( enable );

        // Set to true so gun inputs are correctly toggled and you keep the gun on level transitions
        m_startWithGun = true;
    }

    /***************************************************
    *   Function        : EnableGrenades 
    *   Purpose         : Allows usage of grenades
    *   Parameters      : None
    *   Returns         : void    
    *   Date altered    : 15/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        : 
    ******************************************************/
    public void EnableGrenades( )
    {
        // Show UI
        m_ui.ActivateGrenadeUI();

        // Allow controls for grenade stuff
        m_grenade.Enable();
        m_radial.Enable();

        // Set to true so grenade inputs are correctly toggled and you keep the grenades on level transitions
        m_startWithGrenades = true;
    }

    /***************************************************
    *   Function        : GetCarryOverData   
    *   Purpose         : sets upgraded values so they carry over between levels
    *   Parameters      : List<float> vars
    *   Returns         : void    
    *   Date altered    : 15/08/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        : DataHolder
    ******************************************************/
    public void GetCarryOverData( List<float> vars )
    {
        // Set all the variables that can be changed/upgrades
        m_shield.SetRechargeRate( vars[ 0 ] );
        m_speed = vars[ 1 ];
        m_stamina.SetRechargeRate ( vars[ 2 ] );
        m_dashCost = vars[ 3 ];
        m_health = vars[ 4 ];
        m_maxHealth = vars[ 5 ];
        m_shield.SetValue( vars[ 6 ] );
        m_shield.SetMaxValue( vars[ 7 ] );
        m_stamina.SetValue( vars [ 8 ] );
        m_stamina.SetMaxValue( vars[ 9 ] );


        // Update money and UI
        m_ui.UpdateMoney( ( int )vars[ 10 ] );

        // Update UI for health
        m_ui.UpdateHealth( m_health, m_maxHealth );
    }

    /***************************************************
    *   Function        : GetMoney
    *   Purpose         : Gets money variable
    *   Parameters      : None
    *   Returns         : int money
    *   Date altered    : 03/05/2022
    *   Contributors    : WH
    *   Notes           : 
    *   See also        : 
    ******************************************************/
    public int GetMoney()
    {
        return m_money;
    }

    /***************************************************
    *   Function        : SetMoney
    *   Purpose         : Sets money variable
    *   Parameters      : int money
    *   Returns         : Void
    *   Date altered    : 03/05/2022
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetMoney( int money )
    {
        m_money = money;

        // Update UI
        m_ui.UpdateMoney( money );
    }

    /***************************************************
    *   Function        : GetCurrentlyThrowing
    *   Purpose         : is grenade currently being thrown
    *   Parameters      : N/A
    *   Returns         : m_currentlyThrowing 
    *   Date altered    : 13/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool GetCurrentlyThrowing()
    {
        return m_currentlyThrowing;
    }

    /***************************************************
    *   Function        : GetHealth
    *   Purpose         : Gets heath variable
    *   Parameters      : None
    *   Returns         : float health
    *   Date altered    : 03/05/2022
    *   Contributors    : WH
    *   Notes           : 
    *   See also        : 
    ******************************************************/
    public float GetHealth()
    {
        return m_health;
    }

    /***************************************************
    *   Function        : SetHealth
    *   Purpose         : Sets health variable
    *   Parameters      : float health
    *   Returns         : Void
    *   Date altered    : 11/07/2022
    *   Contributors    : WH JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetHealth( float health )
    {
        m_health = health;

        // Update UI
        m_ui.UpdateHealth( m_health, m_maxHealth );

        // Update screen effect
        CheckEffects();
    }

    /***************************************************
     *   Function        : GetMaxHealth
     *   Purpose         : Gets max health variable
     *   Parameters      : None
     *   Returns         : float max health
     *   Date altered    : 03/05/2022
     *   Contributors    : WH
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    /***************************************************
     *   Function        : SetMaxHealth
     *   Purpose         : Sets max health variable
     *   Parameters      : float health
     *   Returns         : void
     *   Date altered    : 03/05/2022
     *   Contributors    : WH JG 
     *   Notes           :  
     *   See also        :    
    ******************************************************/
    public void SetMaxHealth( float health )
    {
        // Set max health and fully heal player
        m_maxHealth = health;
        m_health = health;

        // Update UI
        m_ui.UpdateHealth( m_health, m_maxHealth );

        CheckEffects();
    }


    /***************************************************
    *   Function        : GetGrenadeData
    *   Purpose         : Gets grenade prefab and count
    *   Parameters      : None
    *   Returns         : (GameObject, int) grenadeObject, grenadeCount
    *   Date altered    : 20/07/22
    *   Contributors    : WH 
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public ( GameObject, int ) GetGrenadeData()
    {
        return ( m_grenadeObject, m_grenadeCount );
    }

    /***************************************************
     *   Function        : SetGrenadeData
     *   Purpose         : Sets grenade object and count
     *   Parameters      : GameObject grenade
     *                     int count
     *   Returns         : void
     *   Date altered    : 20/07/22
     *   Contributors    : WH 
     *   Notes           :  
     *   See also        :    
    ******************************************************/
    public void SetGrenadeData( GameObject grenade, int count )
    {
        m_grenadeObject = grenade;
        m_grenadeCount = count;
    }

    /***************************************************
     *   Function        : SetGrenadeData
     *   Purpose         : Sets grenade object and count
     *   Parameters      : (GameObject, int) data
     *   Returns         : void
     *   Date altered    : 20/07/22
     *   Contributors    : WH 
     *   Notes           : Tuple Overload
     *   See also        :    
    ******************************************************/
    public void SetGrenadeData( (GameObject, int) data )
    {
        m_grenadeObject = data.Item1;
        m_grenadeCount = data.Item2;
    }

    /***************************************************
     *   Function        : GetPlayerVars
     *   Purpose         : Gets player data for shop menu
     *   Parameters      : None
     *   Returns         : List<float>
     *   Date altered    : 25/07/22
     *   Contributors    : WH 
     *   Notes           : 
     *   See also        :    
    ******************************************************/
    public List<float> GetPlayerVars()
    {
        List<float> vars = new List<float>();

        vars.Add( m_shield.GetRechargeRate() );
        vars.Add( m_speed );
        vars.Add( m_stamina.GetRechargeRate() );
        vars.Add( m_dashCost );
        vars.Add( m_health );
        vars.Add( m_maxHealth );
        vars.Add( m_shield.GetValue() );
        vars.Add( m_shield.GetMaxValue() );
        vars.Add( m_stamina.GetValue() );
        vars.Add( m_stamina.GetMaxValue() );

        return vars;
    }

    /***************************************************
    *   Function        : GetDashCost
    *   Purpose         : Gets dash cost
    *   Parameters      : None
    *   Returns         : float dash cost
    *   Date altered    : 29/07/22
    *   Contributors    : WH 
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetDashCost()
    {
        return m_dashCost;
    }

    /***************************************************
    *   Function        : SetDashCost
    *   Purpose         : Sets dash cost
    *   Parameters      : float cost
    *   Returns         : Void
    *   Date altered    : 29/07/22
    *   Contributors    : WH 
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetDashCost( float cost )
    {
        m_dashCost = cost;
    }
    /***************************************************
  *   Function        :  IsGrounded
  *   Purpose         :  Is the player cunrrently grounded 
  *   Parameters      :  None
  *   Returns         :  void
  *   Date altered    :  20/07/22
  *   Contributors    :  JG
  *   Notes           : 
  *   See also        :    
  ******************************************************/
    public bool IsGrounded()
    {
        return m_characterController.isGrounded;
    }
    /***************************************************
    *   Function        : SetSpeed
    *   Purpose         : speed
    *   Parameters      : float speed
    *   Returns         : void
    *   Date altered    : 28/07/22
    *   Contributors    : JG
    *   Notes           :   
    *   See also        :    
    ******************************************************/
    public void SetSpeed( float speed )
    {
        m_speed = speed;
    }
    /***************************************************
    *   Function        :   GetDefaultSpeed
    *   Purpose         :   speed
    *   Parameters      :   None
    *   Returns         :   float m_defaultSpeed
    *   Date altered    :   28/07/22
    *   Contributors    :   JG
    *   Notes           :   
    *   See also        :    
    ******************************************************/
    public float GetDefaultSpeed()
    {
        return m_defaultSpeed;
    }
    /***************************************************
   *   Function        : GetCurrentSpeed
   *   Purpose         : Return the  current velocity  of the player
   *   Parameters      : N/A
   *   Returns         : Void 
   *   Date altered    : UK
   *   Contributors    :  JG
   *   Notes           :  
   *   See also        : AssaultRifle
   ******************************************************/
    public float GetCurrentSpeed()
    {
        return m_characterController.velocity.magnitude;
    }

    /***************************************************
    *   Function        : GetSpeed
    *   Purpose         : Return the speed mutiplier 
    *   Parameters      : N/A
    *   Returns         : Void 
    *   Date altered    : UK
    *   Contributors    :  JG
    *   Notes           :  
    *   See also        : 
    ******************************************************/
    public float GetSpeed()
    {
        return m_speed;
    }

    /***************************************************
    *   Function        : GetForkliftCertification
    *   Purpose         : Gets Forklift Certification
    *   Parameters      : None
    *   Returns         : bool forklift certification
    *   Date altered    : 31/07/22
    *   Contributors    : WH 
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public bool GetForkliftCertification()
    {
        return m_forkliftCertified;
    }

    /***************************************************
    *   Function        : SetForkliftCertification
    *   Purpose         : Sets Forklift Certification
    *   Parameters      : bool forklift certification
    *   Returns         : void
    *   Date altered    : 31/07/22
    *   Contributors    : WH 
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetForkliftCertification( bool certified )
    {
        m_forkliftCertified = certified;
    }

    /***************************************************
    *   Function        : Movement
    *   Purpose         : Move the player and update aniamtors 
    *   Parameters      : N/A    
    *   Returns         : Void   
    *   Date altered    : 15/08/2022
    *   Contributors    : JG 
    *   Notes           : Moved from updated  
    *   See also        :    
    ******************************************************/
    private void Movement()
    {
        // Move player
        m_characterController.Move( m_speed * Time.deltaTime * GetMoveDirection() );


        // Check footstep sounds need to be made 
        GenerateFootSteps();

        if ( GetComponentInChildren<GunManager>() != null )
        {
            // Updated gun arms  animator 
            m_gunAnimator.SetFloat( an_movement, m_characterController.velocity.magnitude );
        }

        // Separate animator for full man showing on camera 
        m_scaleManAnims.SetFloat( an_movement, m_characterController.velocity.magnitude );

        // If speed is over walking threshold player is sprinting so increase footstep sound frequncy 
        if ( m_characterController.velocity.magnitude > m_sprintThreshold )
        {
            m_sprinting = true;
        }
        else
        {
            m_sprinting = false;
        }
    }

    /***************************************************
    *   Function        : ApplyJump
    *   Purpose         : Checking player grounded and applying jump if needed 
    *   Parameters      : N/A    
    *   Returns         : Void   
    *   Date altered    : 15/08/2022
    *   Contributors    : WH JG(cleaning)
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void ApplyJump()
    {
        // Have 0 y velocity if gounded
        if ( m_characterController.isGrounded )
        {
            m_jumpVelocity.y = 0;
        }
        else
        {
            // Apply gravity
            m_jumpVelocity.y += m_gravity * Time.deltaTime;
        }

        // If you can jump (if you're grounded properly)
        if ( m_canJump )
        {
            // set jump velocity and reset jump bool
            m_jumpVelocity.y = m_jumpForce;
            m_canJump = false;

            // Set time stamina was used
            m_stamina.SetTimeStamp( Time.time );
        }

        // Apply jump movement
        m_characterController.Move( Time.deltaTime * m_jumpVelocity );
    }

    /***************************************************
     *   Function        : RotateToCamera
     *   Purpose         : Rotate player relative to camera  
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG 
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private void RotateToCamera()
    {
        // Rotate the player to represent camera.
        transform.rotation = new Quaternion( transform.rotation.x, m_playerCamera.transform.rotation.y, transform.rotation.z, transform.rotation.w );
        Camera.main.transform.rotation = new Quaternion( m_playerCamera.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w );

    }
    /***************************************************
     *   Function        : Update  
     *   Purpose         : Upate the player , make them move & update UI   
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : 15/08/2022
     *   Contributors    : JG WH
     *   Notes           :    
     *   See also        :    
    ******************************************************/
    private void Update()
    {
        // Check player movement
        Movement();

        // Check screen effects 
        CheckEffects();

        // Check Player is jumping
        ApplyJump();

        // Update player to camera movement 
        RotateToCamera();
        

    }

    /***************************************************
     *   Function        : GenerateFootSteps    
     *   Purpose         : Make footstep sounds based on the speed     
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : 03/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void GenerateFootSteps()
    {
        
        // If player is not on the ground or moving exit function 
        if ( m_characterController.isGrounded == false || GetMoveDirection() == Vector3.zero )
        {
            return;
        }
        
        // Decrease footstep timer 
        m_footStepTimer -= Time.deltaTime;

        // If timer complete 
        if(m_footStepTimer <= 0 )
        {
            // Play a random footstep sound from array once 
            m_stepSource.PlayOneShot(m_stepClips[ UnityEngine.Random.Range(0,m_stepClips.Length- 1)]);

            // Reset timer using current movement offset 
            m_footStepTimer = m_footStepOffset;
        }
       
    }

    /***************************************************
    *   Function        :    FixedUpdate
    *   Purpose         :    Physics raycast for interactables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    07/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    InteractableRaycast
    ******************************************************/
    private void FixedUpdate()
    {
        // Attempt to show hover text on object
        InteractableRaycast( InteractExecution.Look );
    }

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Fucntion called when interacting with an object
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    InteractableRaycast
    ******************************************************/
    private void Interact()
    {
        // Attempt to interact with object
        if( InteractableRaycast( InteractExecution.Interact ) )
        {
            // Interact sound
            m_audioPool.PlaySound( m_audioPool.m_interactSuccess );
        }
        else
        {
            // Hit nothing interact sound
            m_audioPool.PlaySound( m_audioPool.m_interactFail );
        }
    }

    /***************************************************
    *   Function        :    InteractableRaycast
    *   Purpose         :    Physics raycast for interactables
    *   Parameters      :    InteractExecution execution
    *   Returns         :    bool interacted
    *   Date altered    :    30/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    Interact, FixedUpdate
    ******************************************************/
    private bool InteractableRaycast( InteractExecution execution )
    {
        // Retun bool to say if you successfully interacted
        bool interacted = false;

        // Debug ray
        Debug.DrawRay( m_playerCamera.transform.position, m_playerCamera.transform.TransformDirection( Vector3.forward ) * 2f, Color.blue );
        
        // Object hit via raycast
        RaycastHit hit;

        // If the raycast hits an object
        if ( Physics.Raycast( m_playerCamera.transform.position, m_playerCamera.transform.TransformDirection( Vector3.forward ), out hit, 2f ) )
        {
            // If the hit object is an interactable
            if ( hit.transform.gameObject.GetComponentInParent<Interactable>() )
            {
                // Switch on if looking or interacting
                switch ( execution )
                {
                    case InteractExecution.Look:
                    {
                        // Display hover text on interactable
                        hit.transform.gameObject.GetComponentInParent<Interactable>().DisplayLookAtText();
                        break;
                    }

                    case InteractExecution.Interact:
                    {
                        // Call object's interact function
                        hit.transform.gameObject.GetComponentInParent<Interactable>().Interact();
                        interacted = true;
                        break;
                    }
                }

                // Save a reference to the looked at object
                m_seenInteractable = hit;
            }

            // If the hit object is not an interactable
            else
            {
                // If you previously looked at an interactable
                if ( m_seenInteractable.transform != null )
                {
                    if ( m_seenInteractable.transform.gameObject.activeSelf == true )
                    {
                        // Hide the hover text
                        m_seenInteractable.transform.gameObject.GetComponentInParent<Interactable>().HideLookAtText();
                    }

                    // Reset variable
                    m_seenInteractable = new RaycastHit();
                }


            }
        }
        // If the raycast has not hit anything
        else
        {
            // If you previously looked at an interactable
            if ( m_seenInteractable.transform != null )
            {
                if( m_seenInteractable.transform.gameObject.activeSelf == true )
                {
                    // Hide the hover text
                    m_seenInteractable.transform.gameObject.GetComponentInParent<Interactable>().HideLookAtText();
                }

                // Reset variable
                m_seenInteractable = new RaycastHit();
            }
        }

        return interacted;
    }

    /***************************************************
    *   Function        :  Damage
    *   Purpose         :  Reduce players health
    *   Parameters      :  float dmg
    *   Returns         :  void 
    *   Date altered    :  03/08/2022
    *   Contributors    :  JG, WH
    *   Notes           :    
    *   See also        :  GunStats
    ******************************************************/
    public void Damage( float dmg )
    {
        // If your shield value >= the damage being applied
        if ( m_shield.GetValue() >= dmg )
        {
            // Take shield damage
            m_shield.SetValue( m_shield.GetValue() - dmg );

            // Play shield sound effect
            m_audioPool.PlaySound( m_audioPool.m_shieldHit );
        }
        else
        {
            // If you have some shield but not enough to take all the damage
            if ( m_shield.GetValue() < dmg && m_shield.GetValue() > 0 )
            {
                // Reduce the amount of health damage by the remainder of your shield
                dmg -= m_shield.GetValue();

                // Take shield damage
                m_shield.SetValue( 0 );

                // Play shield sound effect
                m_audioPool.PlaySound( m_audioPool.m_shieldHit );
            }


            // Redeuce health
            m_health -= dmg;

            // If player dies 
            if ( m_health <= k_minHealth )
            {
    
                // Show Game over on screen
                m_endScreen.Display( true, m_startWithGun );

                // Disable player controls
                EnableMenuControls();

                // Play death sound
                m_audioPool.PlaySound( m_audioPool.m_death );

                // No longer alive 
                m_isAlive = false;
            }

            // Update effects based on health
            CheckEffects();

            // Update UI
            m_ui.UpdateHealth( m_health, m_maxHealth );

            // Play damage sound
            m_audioPool.PlaySound( m_audioPool.m_damaged );

        }

        // When damage was last took
        m_shield.SetTimeStamp( Time.time );

    }

   

    /***************************************************
    *   Function        : Jump
    *   Purpose         : When the player jumps 
    *   Parameters      : InputAction.CallbackContext context 
    *   Returns         : Void 
    *   Date altered    : 26/07/22
    *   Contributors    : JG, WH
    *   Notes           :  
    *   See also        : 
    ******************************************************/
    public void Jump( InputAction.CallbackContext context )
    {
      
        // If action peformed 
        if ( context.performed )
        {
           // Check if enough stamina to jump adn on the ground 
            if ( m_stamina.GetValue() >= m_dashCost && m_characterController.isGrounded )
            {
                // Set bool to true so jump movement in update applies jump force
                m_canJump = true;

                // Update stamina 
                m_stamina.SetValue( m_stamina.GetValue() - m_dashCost );

                // Play jump sound effect
                m_audioPool.PlaySound( m_audioPool.m_jump );
            }
        }
    }

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Interact input call
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :    07/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    Interact
    ******************************************************/
    public void Interact( InputAction.CallbackContext context )
    {
        //Debug.Log( context );
        // Inputs have 3 stages 
        // use perform for if you u want the action to happen once
        if ( context.performed )
        {
            //Debug.Log( "Interact" + context.phase );

            Interact();
        }
    }


    /***************************************************
    *   Function        :    Grenade
    *   Purpose         :    Grenade input call
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :   07/08/22
    *   Contributors    :    WH JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Grenade( InputAction.CallbackContext context )
    {
        Debug.Log( context );
        // Inputs have 3 stages 
        // use perform for if you u want the action to happen once
        if ( context.performed )
        {
            // If there are more than 0 grenades, you're not currently throwing, and you're not currently reloading
            if( m_grenadeCount > 0  && m_currentlyThrowing == false && GetComponentInChildren<GunManager>().GetCurrentGun().GetState() != GunStats.GunState.Reloading)
            {
                // Get grenade from pool 
                m_currentGrenade = m_assetPool.GetObjectFromPool( m_grenadeObject, Vector3.zero, transform.rotation );
                
                // Parent the greande to the hand of animation 
                m_currentGrenade.transform.parent = m_grenadePosition;

                // Set local vars
                m_currentGrenade.transform.localPosition = Vector3.zero;
                m_currentGrenade.transform.localEulerAngles = Vector3.zero;


                // Update rigidbody to carried
                m_currentGrenade.GetComponent<Rigidbody>().useGravity = false;
                m_currentGrenade.GetComponent<Rigidbody>().isKinematic = true;

             
                m_currentlyThrowing = true;

                // play anim
                m_gunAnimator.SetTrigger( an_throw );

             
            }
        }
    }

    /***************************************************
     *   Function        : ThrowGrenade 
     *   Purpose         : throw grenade via animation 
     *   Parameters      : N/A
     *   Returns         : void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void ThrowGrenade()
    {
        // Play throw sound
        m_audioPool.PlaySound( m_audioPool.m_throwGrenade );

        // Reparent grenade to nothing
        m_currentGrenade.transform.parent = null;
       
        // Get reference to rb
        Rigidbody body = m_currentGrenade.GetComponent<Rigidbody>();

        // Setup body to be thrown 
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = true;
        body.isKinematic = false;

        // Throw grenade forward 
        m_currentGrenade.GetComponent<Grenade>().Throw( transform.forward );

        // Reduce grenade amount 
        m_grenadeCount--;
        m_ui.UpdateGrenadeCount( m_grenadeCount );

        // No longer throwing 
        m_currentlyThrowing = false;
    }

    /***************************************************
    *   Function        :    Pause
    *   Purpose         :    Pause input call
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Pause( InputAction.CallbackContext context )
    {
        
        // Inputs have 3 stages 
        // use perform for if you u want the action to happen once
        if ( context.performed )
        {
            // If game is not paused
            if( Time.timeScale == 1 )
            {
                // Pause game
                m_pauseMenu.Pause();
            }
            else
            {
                // Resume game
                m_pauseMenu.Resume();
                
                // Change crosshairs
                (int, int, int) crosshairs = m_dataHolder.ApplyCrosshairs();
                m_crosshairManager.ChangeCrossHairStyle( crosshairs.Item1, crosshairs.Item2, crosshairs.Item3 );
            }
        }
    }

    /***************************************************
    *   Function        :    Dash
    *   Purpose         :    Dash input call
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Dash( InputAction.CallbackContext context )
    {
        //Debug.Log( context );
        // Inputs have 3 stages 
        // use perform for if you u want the action to happen once
        if ( context.performed )
        {
            // If you have enough stamina to dash
            if( m_stamina.GetValue() >= m_dashCost )
            {
                // Dash
                StartCoroutine( DashMovement() );

                // Reduce stamina
                m_stamina.SetValue( m_stamina.GetValue() - m_dashCost );

                // Play dash sound
                m_audioPool.PlaySound( m_audioPool.m_dash );
            }
        }
    }


    /***************************************************
    *   Function        :    Radial
    *   Purpose         :    Radial input call
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :    04/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void RadialOpen( InputAction.CallbackContext context )
    {
        // If you press the radialOpen button and the radial is not already open
        if ( context.started && !m_radialMenu.GetOpen() )
        {
            // Open Radial and enable controls
            m_radialMenu.OpenMenu();
            EnableRadialContols();
            
        }
    }

    public void RadialClose( InputAction.CallbackContext context )
    {
        // If you press the radialOpen button and the radial is already open
        if ( context.performed && m_radialMenu.GetOpen() )
        {
            // Close radial and disable controls
            m_radialMenu.CloseMenu();
            DisableRadialControls();
     

        }
    }

    /***************************************************
    *   Function        :    DashMovement
    *   Purpose         :    Dashes player
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    20/07/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    public IEnumerator DashMovement( Vector3 direction = new Vector3() )
    {
        // When the dash started
        float startTime = Time.time;

        // If there was no direction given
        if( direction == Vector3.zero )
        {
            // Get the direction the player is moving, defaulting to forward
            direction = GetMoveDirection( true );
        }

        // While the time passed is < the dash duration
        while ( Time.time < startTime + m_dashDuration )
        {
            // Move the player with a dash
            m_characterController.Move( m_dashMultiplier * m_speed * Time.deltaTime * direction );
            
            // When the stamina was last used
            m_stamina.SetTimeStamp( Time.time );
            yield return null;
        }
    }

    /***************************************************
    *   Function        :    GetMoveDirection
    *   Purpose         :    Gets the direction to move the player in
    *   Parameters      :    bool defaultForward = false
    *   Returns         :    Vector3 directionToMove
    *   Date altered    :    28/06/22
    *   Contributors    :    JG WH
    *   Notes           :    Mostly Joe's code taken from update but made 
    *                        into a function by Will
    *   See also        :    
    ******************************************************/
    Vector3 GetMoveDirection( bool defaultForward = false )
    {
        // Get movement values from input
        Vector2 inputVector = m_move.ReadValue<Vector2>();

        // If no movement
        if ( defaultForward && inputVector == new Vector2( 0.00f, 0.00f ) )
        {
            // Give the forward direction
            inputVector = new Vector2( 0f, 1f );
        }

        // Get the Camera Direction to update the movement direction relvent to where the camera is facing 
        Vector3 forward = m_playerCamera.transform.forward;
        Vector3 right = m_playerCamera.transform.right;

        // Set Y values to 0 as they are not requried 
        forward.y = 0f;
        right.y = 0f;

        // Normalize directions 
        forward.Normalize();
        right.Normalize();


        // Calculate direction of input relativent to camera 
        Vector3 directionToMove = forward * inputVector.y + right * inputVector.x;

        return directionToMove;
    }


    /***************************************************
    *   Function        :    EnableMenuControls
    *   Purpose         :    Disables player controls and enables Menu controls
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void EnableMenuControls()
    {
        // Disable player controls, enable shop controls
        m_playerControls.Player.Disable();
        m_playerControls.Radial.Disable();
        m_playerControls.Menu.Enable();

        // Disable gun inputs if you have a gun
        if( m_startWithGun )
        {
            m_gunManager.DisableInputs();
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Hide UI
        m_ui.gameObject.SetActive( false );
        m_gunCamera.gameObject.SetActive( false );
    }

    /***************************************************
    *   Function        :    DisableMenuControls
    *   Purpose         :    Enables player controls and disables Menu controls
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void DisableMenuControls()
    {
        // Enable player controls, disable shop controls
        m_playerControls.Player.Enable();
        m_playerControls.Menu.Disable();

        // Enable gun inputs if you have a gun
        if ( m_startWithGun )
        {
            m_gunManager.EnableInputs();
        }

        // Enable grenade inputs if you have a grenades
        if ( m_startWithGrenades )
        {
            m_playerControls.Radial.Enable();
        }

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Show UI
        m_ui.gameObject.SetActive( true );
        m_gunCamera.gameObject.SetActive( true );
    }

    /***************************************************
    *   Function        :    GetControls
    *   Purpose         :    Returns Player Controls
    *   Parameters      :    None
    *   Returns         :    PlayerControls playerControls
    *   Date altered    :    N/A
    *   Contributors    :    JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public PlayerControls GetControls()
    {
        return m_playerControls;
    }

    /***************************************************
    *   Function        :    EnableRadialContols
    *   Purpose         :    Disables player controls for using radial
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void EnableRadialContols()
    {
        // Disable grenade and gun inputs
        m_grenade.Disable();
        m_gunManager.DisableInputs();

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Hide UI
        m_ui.gameObject.SetActive( false );
    }

    /***************************************************
    *   Function        :    DisableRadialControls
    *   Purpose         :    Enables player controls for not using radial
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void DisableRadialControls()
    {
        // Enable grenade and gun inputs (if have gun)
        m_grenade.Enable();

        if ( m_startWithGun )
        {
            m_gunManager.EnableInputs();
        }

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Show UI
        m_ui.gameObject.SetActive( true );
    }

    /***************************************************
    *   Function        :    CheckEffects
    *   Purpose         :    If player is on low shield apply an effect 
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    25/07/22
    *   Contributors    :    JG, WH
    *   Notes           :    Combination of Joe's CheckLowHealth funtion
    *                        with new code for shield effect
    *   See also        :    
    ******************************************************/
    private void CheckEffects()
    {
        // If shield value is above 0, the last screen effect was not the laser, and not lerping the shield effect
        if ( m_shield.GetValue() <= 0.1f && m_screenEffect.GetlastEffect() != ChangeScreenEffect.Effect.laser && !m_shieldLerp )
        {
            // Shrink shield
            StartCoroutine( m_screenEffect.ShieldLerp( false ) );

            // Now lerping shield effect
            m_shieldLerp = true;
        }
        // If health is <= lowhealth and the last screen effect was not the laser
        else if ( m_health <= m_lowHealth && m_screenEffect.GetlastEffect() != ChangeScreenEffect.Effect.laser )
        {
            // Set low health effect 
            m_screenEffect.ToggleEffect( ChangeScreenEffect.Effect.health );
        }
        // If the shield is regenerating and the effect is lerping
        else if ( m_shield.GetRegen() && m_shieldLerp )
        {
            // Grow shield
            StartCoroutine( m_screenEffect.ShieldLerp( true ) );

            // Not lerping the shield
            m_shieldLerp = false;
        }
        // If the last screen effect was not the laser or blank
        else if ( m_screenEffect.GetlastEffect() != ChangeScreenEffect.Effect.laser && m_screenEffect.GetlastEffect() != ChangeScreenEffect.Effect.blank )
        {
            // Make screen blank
            m_screenEffect.ToggleEffect();
        }


        // If the shiled is depleated, and not regenerating
        if ( m_shield.GetValue() >= 0.1f && !m_shield.GetRegen() )
        {
            // Allow shield to shrink again
            m_shieldLerp = false;
        }
    }
  
}
