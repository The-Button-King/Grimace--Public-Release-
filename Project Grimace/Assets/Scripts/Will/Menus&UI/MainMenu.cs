using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: MainMenu
 *
 * Author: Will Harding
 *
 * Purpose: Controls Main Menu
 * 
 * Functions:   public virtual void Start()
 *              public void Play()
 *              private IEnumerator LoadLevel()
 *              public void Exit()
 *              public void SetVolumeMaster( float volume )
 *              public void SetVolumeMusic( float volume )
 *              public void SetVolumeSFX( float volume )
 *              public void SetQuality( int index )
 *              public void SetResolution( int index )
 *              public void SetFullscreen( bool fullscreen )
 *              public void SetAssaultCrosshair( int index )
 *              public void SetChargeCrosshair( int index )
 *              public void SetShotgunCrosshair( int index )
 *              public void SetScreenShake( bool shake )
 * 
 * References:
 * 
 * See Also:    
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 07/05/22     WH          1.0         Created
 * 10/05/22     WH          1.2         Added cursor back
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 29/07/22     WH          2.0         Adding settings functions
 * 31/07/22     WH          2.1         Adding more settings
 * 02/08/22     WH          2.2         Added crosshair and screenshake changing
 * 13/08/22     WH          2.3         Added loading to play and settings checking for existing data on start
 * 14/08/22     WH          2.4         Loading decimal places
 ****************************************************************************************************/
[RequireComponent(typeof(MenuAudioPool))]
public class MainMenu : MonoBehaviour
{
    [Header( "Dropdowns" )]

    [Tooltip( "Dropdown for quality options" )]
    public      TMP_Dropdown    m_qualityDropdown;      // Quality dropdown

    [Tooltip( "Dropdown for resolution options" )]
    public      TMP_Dropdown    m_resolutionDropdown;   // Resolution dropdown

    [Tooltip( "Dropdown for Assault Rilfe Crosshairs" )]
    public      TMP_Dropdown    m_assaultDropdown;      // Assault crosshairs dropdown

    [Tooltip( "Dropdown for Charge Rilfe Crosshairs" )]
    public      TMP_Dropdown    m_chargeDropdown;       // Charge crosshairs dropdown

    [Tooltip( "Dropdown for Shotgun Crosshairs" )]
    public      TMP_Dropdown    m_shotgunDropdown;      // Shotgun crosshairs dropdown


    [Header( "Sliders" )]

    [Tooltip( "Master Volume Slider" )]
    public      Slider          m_masterVol;            // Master volume slider

    [Tooltip( "Music Volume Slider" )]
    public      Slider          m_musicVol;             // Music volume slider

    [Tooltip( "Sound Effects Slider" )]
    public      Slider          m_sfxVol;               // SFX volume slider


    [Header( "Toggles" )]

    [Tooltip( "FullScreen Toggle" )]
    public      Toggle          m_fullscreenToggle;     // Fullscreen toggle

    [Tooltip( "Screen Shake Toggle" )]
    public      Toggle          m_screenShake;          // Screen Shake toggle


    [Header( "Misc" )]

    [Tooltip( "The Master Audio Mixer" )]
    public      AudioMixer      m_master;               // Audio mixer

    [Tooltip( "The Audio Pool for the Menu" )]
    public      MenuAudioPool   m_audioPool;            // Audio pool

    [Tooltip( "The text of the 'Play' button" )]
    public      TMP_Text        m_buttonText;           // Play button text

    [Tooltip( "Data Holder" )]
    public      DataHolder      m_dataHolder;           // Data Holder

    private     Resolution[]    m_resolutions;          // List of available resolutions generated at runtime

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Sets variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public virtual void Start()
    {
        // Displays cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Get audio pool
        m_audioPool = GetComponent<MenuAudioPool>();

        // Get data holder
        m_dataHolder = GameObject.FindWithTag( "GameController").GetComponent<DataHolder>();

        // Get available resolutions
        m_resolutions = Screen.resolutions;

        // Set dropdown and toggles to their current settings
        m_qualityDropdown.value = QualitySettings.GetQualityLevel();
        m_fullscreenToggle.isOn = Screen.fullScreen;
        m_screenShake.isOn = m_dataHolder.ApplyScreenShake();

        // Set crosshair dropdowns to their current settings
        m_assaultDropdown.value = m_dataHolder.GetAssaultCrosshair();
        m_chargeDropdown.value = m_dataHolder.GetChargeCrosshair();
        m_shotgunDropdown.value = m_dataHolder.GetShotgunCrosshair();

        // Slider value
        float value;

        // Get volume value
        m_master.GetFloat( "Master Volume", out value );
        
        // Apply to slider, converting from a logarithmic value to a linear one
        m_masterVol.value = Mathf.Pow(10, value / 20);

        m_master.GetFloat( "Music Volume", out value );
        m_musicVol.value = Mathf.Pow( 10, value / 20 );

        m_master.GetFloat( "SFX Volume", out value );
        m_sfxVol.value = Mathf.Pow( 10, value / 20 );


        // Index for which resolution to display in dropdown
        int resolutionOption = 0;

        // List of resolutions to add to dropown
        List<string> optionsToAdd = new List<string>();

        // Loop through resolutions
        for (int i = 0; i < m_resolutions.Length; i++)
        {
            // Get resolution in from "width x height"
            string option = m_resolutions[i].width + " x " + m_resolutions[i].height;
            
            // Add to list
            optionsToAdd.Add( option );

            // If the current resolution in the list is the one being displayed
            if ( m_resolutions[i].width == Screen.currentResolution.width && m_resolutions[ i ].height == Screen.currentResolution.height )
            {
                // Get the index of which resolution is the currently displaying
                resolutionOption = i;
            }
        }

        // Clear the existing options
        m_resolutionDropdown.ClearOptions();

        // Add new options
        m_resolutionDropdown.AddOptions( optionsToAdd );

        // Show the current resolution in the dropdown
        m_resolutionDropdown.value = resolutionOption;
    }

    /***************************************************
    *   Function        :    Play
    *   Purpose         :    Loads level
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    12/05/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Play()
    {
        // Loads level
        StartCoroutine( LoadLevel() );
    }


    /***************************************************
    *   Function        :    LoadLevel
    *   Purpose         :    Loads level
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    Coroutine
    *   See also        :    
    ******************************************************/
    private IEnumerator LoadLevel()
    {
        // Loads level
        AsyncOperation loading = SceneManager.LoadSceneAsync( "Level" );

        // While the level is loading
        while ( !loading.isDone )
        {
            // Set button text to loading percentage
            m_buttonText.text = "Loading: " + ( loading.progress * 100 ).ToString( "00" ) + "%";

            yield return null;
        }
    }

    /***************************************************
    *   Function        :    Exit
    *   Purpose         :    Closes game
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    12/05/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Exit()
    {
        // Quit game
        Application.Quit();
    }

    /***************************************************
    *   Function        :    SetVolumeMaster
    *   Purpose         :    Adjusts volume
    *   Parameters      :    float volume
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetVolumeMaster( float volume )
    {
        // Set volume logarithmicly
        m_master.SetFloat( "Master Volume", Mathf.Log10( volume ) * 20 );
    }

    /***************************************************
    *   Function        :    SetVolumeMusic
    *   Purpose         :    Adjusts volume
    *   Parameters      :    float volume
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetVolumeMusic( float volume )
    {
        // Set volume logarithmicly
        m_master.SetFloat( "Music Volume", Mathf.Log10( volume ) * 20 );
    }

    /***************************************************
    *   Function        :    SetVolumeSFX
    *   Purpose         :    Adjusts volume
    *   Parameters      :    float volume
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetVolumeSFX( float volume )
    {
        // Set volume logarithmicly
        m_master.SetFloat( "SFX Volume", Mathf.Log10( volume ) * 20 );
    }

    /***************************************************
    *   Function        :    SetQuality
    *   Purpose         :    Adjusts quality
    *   Parameters      :    int index
    *   Returns         :    void
    *   Date altered    :    29/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetQuality( int index )
    {
        // Changes qualuty level based off of dropdown menu selection
        QualitySettings.SetQualityLevel( index );
    }

    /***************************************************
    *   Function        :    SetResolution
    *   Purpose         :    Adjusts quality
    *   Parameters      :    int index
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetResolution( int index )
    {
        // Changes qualuty level based off of dropdown menu selection
        Resolution resolution = m_resolutions[ index ];
        Screen.SetResolution( resolution.width, resolution.height, Screen.fullScreen );
    }

    /***************************************************
    *   Function        :    SetFullscreen
    *   Purpose         :    Makes game fullscreen
    *   Parameters      :    bool fullscreen
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetFullscreen( bool fullscreen )
    {
        // Make game fullscreen
        Screen.fullScreen = fullscreen;
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
        m_dataHolder.SetAssaultCrosshair( index );
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
        m_dataHolder.SetChargeCrosshair( index );
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
        m_dataHolder.SetShotgunCrosshair( index );
    }

    /***************************************************
    *   Function        :    SetScreenShake
    *   Purpose         :    Toggles screen shake
    *   Parameters      :    bool shake
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetScreenShake( bool shake )
    {
        m_dataHolder.SetScreenShake( shake );
    }
}
