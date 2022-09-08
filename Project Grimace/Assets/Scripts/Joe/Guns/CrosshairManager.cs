using LayerMasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: CrosshairManager
 *
 * Author: Joseph Gilmore
 *
 * Purpose: alter crosshair shown on hud in settings and differemt gun types
 * 
 * Functions:               void Awake()
 *                          void Update()
 *                          public void ChangeCrossHairStyle(int  AR = 0, int CR = 0, int SG = 0)
 *                          public void UpdateGunCrossHair( GunManager.Guns gunType )
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
 * 26/06/2022    JG          1.00        - Made crosshair change colour when hovering over AI
 * 01/07/2022    JG          1.01        - Fixed highlighting through walls
 * 13/07/2022    JG          1.02        - Started adding more crosshairs
 * 14/07/2022    JG          1.03        - Added mutiple crosshairs for different gun types 
 * 03/08/2022    WH          1.04        - Changed start to Awake to update crosshair better on starting level
 * 14/08/2022    JG          1.05        - Cleaning 
 ****************************************************************************************************/
public class CrosshairManager : MonoBehaviour
{

    [SerializeField]
    private Image               m_crosshair;                    // Reference to crosshair 
    private LayerMask           m_layerAI;                      // Reference to AI 
    private Color               m_color;                        // Colour of starting crosshair 
    private LayerMask           m_obstructions;                 // Layers that obstruct the change of crosshair vision
    [Header("Different gun crosshairs options")]
    [SerializeField]
    private List<Sprite>        m_assultRilfeCrosshairs;        // List of assult rilfe crosshairs 

    [SerializeField]
    private List<Sprite>        m_chargeRilfeCrosshairs;        // List of charge rilfe crosshairs
    [SerializeField]
    private List<Sprite>        m_shotGunCrosshairs;            // List of shotgun crosshairs
    private int                 m_assultRilfeIndex = 0;         // Index for selected crosshair type
    private int                 m_chargeRilfeIndex = 0;         // Index for selected crosshair type
    private int                 m_shotGunIndex = 0;             // Index for selected crosshair type

    /***************************************************
    *   Function        : Awake  
    *   Purpose         : Setup crosshair manager    
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 03/08/2022
    *   Contributors    : JG
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    void Awake()
    {
        // Get reference to AI layer 
        m_layerAI = LayerMask.GetMask( Layers.AI.ToString() );

        // Set up obstruction layers 
        m_obstructions = LayerMask.GetMask( Layers.Wall.ToString(), Layers.Cover.ToString(), Layers.Obstacles.ToString() );

        // Set start color 
        m_color = m_crosshair.color;

        // Settings 
        (int, int, int) crosshairs = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>().ApplyCrosshairs();
        ChangeCrossHairStyle( crosshairs.Item1, crosshairs.Item2, crosshairs.Item3 );
    }

    /***************************************************
    *   Function        : Update     
    *   Purpose         : Alter crosshair based on instructions    
    *   Parameters      : N/A   
    *   Returns         : void    
    *   Date altered    : 14/07/2022 
    *   Contributors    : JG 
    *   Notes           :    
    *   See also        :    
    /******************************************************/
    void Update()
    {
        RaycastHit hitObject;

        // Fire Raycast from firfromTransform and in a forward direction 
        if (Physics.Raycast( transform.parent.position, transform.parent.TransformDirection(Vector3.forward), out hitObject, Mathf.Infinity, m_layerAI) )
        {
            // Get distance to hit AI
            float distance = hitObject.distance;    

            // Check nothing is in the way within distance 
            if (!Physics.Raycast( transform.parent.position, transform.parent.TransformDirection(Vector3.forward), out hitObject , distance, m_obstructions) )
            {
                // Set to red 
                m_crosshair.color = Color.red;
            }
        }
        else
        {
            // Reset color 
            m_crosshair.color = m_color;
        }
        
    }

    /***************************************************
    *   Function        : ChangeCrosshairStyle     
    *   Purpose         : Alter each guns crosshair via an index   
    *   Parameters      : int  AR = 0, int CR = 0, int SG = 0 
    *   Returns         : void    
    *   Date altered    : 14/07/2022 
    *   Contributors    : JG 
    *   Notes           : settings   
    *   See also        :    
    /******************************************************/
    public void ChangeCrossHairStyle( int  AR = 0, int CR = 0, int SG = 0 )
    {
        m_assultRilfeIndex = AR;
        m_chargeRilfeIndex = CR;
        m_shotGunIndex = SG;
    }

    /***************************************************
    *   Function        : UpdateGunCrosshair    
    *   Purpose         : Change crosshair to current gun type  
    *   Parameters      : GunManager.Guns gunType
    *   Returns         : void    
    *   Date altered    : 14/07/2022 
    *   Contributors    : JG 
    *   Notes           :    
    *   See also        :    
    /******************************************************/
    public void UpdateGunCrossHair( GunManager.Guns gunType )
    {
        switch ( gunType )
        {
            case GunManager.Guns.Assault:
                {
                    // Set crosshair sprite to the current crosshair selected in the settings for the AR
                    m_crosshair.sprite = m_assultRilfeCrosshairs[m_assultRilfeIndex];
                }
                break;
            case GunManager.Guns.Shotgun:
                {
                    m_crosshair.sprite  = m_shotGunCrosshairs[m_shotGunIndex];
                }
                break;
            case GunManager.Guns.Charge:
                {
                    m_crosshair.sprite = m_chargeRilfeCrosshairs[ m_chargeRilfeIndex ];
                }
                break;
               
        }
    }
}
