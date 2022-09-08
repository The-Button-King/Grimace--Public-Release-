using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: PickUpGun
 *
 * Author: Joseph Gilmore
 *
 * Purpose:   Allow player to pick up gun
 * 
 * Functions:           private void Start()
 *                      public override void Interact()
 *                      public override void DisplayLookAtText()
 *                      
 *                      
 * 
 * References:
 * 
 * See Also: GunManager
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 10/07/2022    JG          1.00        - Allow player to pick up gun
 * 13/07/2022    JG          1.01        - Enabled animation      
 * 14/07/2022    JG          1.02        - Added text
 * 27/07/2022    JG          1.03        - More animations 
 * 31/07/2022    WH          1.04        - Added text stuff for interactable rework
 * 07/08/2022    JG          1.05       -  Combined wills grenade pickyup into one script
 * 13/08/2022    JG          1.06       - Bug fixes 
 ****************************************************************************************************/
public class PickUpGun : Interactable
{
    private Animator[]      m_animations;            // Animators of gun case
    [SerializeField][Tooltip("Material change for when gun is picked up")]
    private Material        m_gunCaseMat;           // Material for th gun case 
    public GameObject       m_grenades;             // Grenade meshes
    public GameObject       m_gunModel;             // reference to gun model
    private PlayerManager   m_playerManager;        // Reference to player manager 
    /***************************************************
     *   Function        : Start   
     *   Purpose         : Setup Class 
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :
     ******************************************************/
    private void Start()
    {
        // Get gun case animations 
        m_animations = GetComponentsInChildren<Animator>();
        m_playerManager = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerManager>();
    }

    /***************************************************
     *   Function        : Interact    
     *   Purpose         : Enable gun in player and make gun case model go  
     *   Parameters      : N/A    
     *   Returns         : Void   
     *   Date altered    : 07/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :   Interactable,PlayerManager 
     ******************************************************/
    public override void Interact()
    {
        // Enable all aniamtors so theuy play
        foreach ( Animator animator in m_animations )
        {
            animator.enabled = true;
        }

        // Enable guns for player
        m_playerManager.EnableGuns();

        // Disable model in gun case
        m_gunModel.SetActive( false );

        // Set material new material to display gun picked up
        GetComponentInChildren<MeshRenderer>().material = m_gunCaseMat;

        // Enable Grenades 
        m_playerManager.EnableGrenades();

        // Remove grenade mesh
        m_grenades.SetActive( false );

        m_useAltText = true;
    }
    /***************************************************
    *   Function        : DisplayLookAText   
    *   Purpose         : Displays text
    *   Parameters      : N/A    
    *   Returns         : Void   
    *   Date altered    : 31/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : Interactable,PlayerManager 
    ******************************************************/
    public override void DisplayLookAtText()
    {
        base.DisplayLookAtText();
       
    }
}
