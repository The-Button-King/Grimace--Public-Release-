using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CameraShake;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: PauseMenu
 *
 * Author: Will Harding
 *
 * Purpose: Controls buttons in pause menu
 * 
 * Functions:   public override void Start()
 *              public void Resume()
 *              public void Pause()
 *              public void Quit()
 *              public void UpdateCrosshairs()
 *              public void UpdateScreenShake()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 27/06/22     WH          1.0         Initial creation
 * 31/07/22     WH          1.1         Added render texture shenanigans for the photo
 * 02/08/22     WH          1.2         Made mainmenu child and crosshair and screenshake changes
 * 03/08/22     WH          1.3         Made photo work
 * 13/08/22     WH          1.4         Fixed timescale on quitting
 * 16/08/22     WH          1.5         Cleaning
 ****************************************************************************************************/
public class PauseMenu : MainMenu
{
    [Tooltip( "The actual menu to display" )]
    public      GameObject      m_screen;                       // The actual menu to display

    [Tooltip( "The object to display the player's screen to when paused" )]
    public      GameObject      m_photo;                        // Image to display player's screen when paused

    [Tooltip( "The GameStats script attached to the object displaying said stats" )]
    public      GameStats       m_gameStats;                    // Gamestats

    private     PlayerManager   m_player;                       // The player
    private     Camera          m_camera;                       // The Player's camera
    private     RenderTexture   m_renderTexture;                // Texture that gets the camera feed


    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Assigns player manager
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public override void Start()
    {
        base.Start();

        // Assign player manager
        m_player = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerManager>();
        m_camera = GameObject.FindGameObjectWithTag( "MainCamera" ).GetComponent<Camera>();

        // Get the size of the photo
        Rect photoRect = m_photo.GetComponent<RectTransform>().rect;

        // Make a render texture of that size
        m_renderTexture = new RenderTexture( ( int )photoRect.width, ( int )photoRect.height, 8 );

        // Set the texture of the photo to the render texture
        m_photo.GetComponent<RawImage>().texture = m_renderTexture;

        // Hides cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the initial time the game started
        m_gameStats.SetStartTime( Time.time );
    }


    /***************************************************
    *   Function        :    Resume
    *   Purpose         :    Resumes the game
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    27/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Resume()
    {
        // Set the camera's target texture to null so it works normally
        m_camera.targetTexture = null;

        // Set timescale back to normal
        Time.timeScale = 1;

        //Disable pause screen
        m_screen.SetActive(false);

        // Re-enable player controls
        m_player.DisableMenuControls();
    }

    /***************************************************
    *   Function        :    Pause
    *   Purpose         :    Pauses the game
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Pause()
    {
        // Set the camera's target texture to the photo texture
        m_camera.targetTexture = m_renderTexture;

        // Render the scene with the camera
        m_camera.Render();

        // Set the camera's target texture to null so it works normally
        m_camera.targetTexture = null;

        // Stop timescale so everything pauses
        Time.timeScale = 0;

        // Activate pasue screen
        m_screen.SetActive(true);

        // Disable Player's controlls and enable menu controls
        m_player.EnableMenuControls();

        // Display the game stats
        m_gameStats.DisplayStats();

    }

    /***************************************************
    *   Function        :    Quit
    *   Purpose         :    Quits to main menu
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Quit()
    {
        // Unpause the game
        Time.timeScale = 1;

        // Load main menu
        SceneManager.LoadSceneAsync( "MainMenu" );
    }

    /***************************************************
    *   Function        :    UpdateCrosshairs
    *   Purpose         :    Updates crosshair
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateCrosshairs()
    {
        // Get the current corsshairs
        (int, int, int) indexes = m_dataHolder.ApplyCrosshairs();
        
        // Get the crosshair manager
        CrosshairManager crosshair = m_player.gameObject.GetComponentInChildren<CrosshairManager>( true );

        // Update which crosshairs to use and update the live crosshair
        crosshair.ChangeCrossHairStyle( indexes.Item1, indexes.Item2, indexes.Item3 );
        crosshair.UpdateGunCrossHair( m_player.gameObject.GetComponentInChildren<GunManager>( true ).GetCurrentGunState() ); 
    }


    /***************************************************
    *   Function        :    UpdateScreenShake
    *   Purpose         :    Updates screenshake
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void UpdateScreenShake()
    {
        // Update if screenshake is on or off
        GameObject.FindGameObjectWithTag( "Player" ).transform.root.GetComponentInChildren<ScreenShake>().SetToggleScreenShake( m_dataHolder.ApplyScreenShake() );
    }
}
