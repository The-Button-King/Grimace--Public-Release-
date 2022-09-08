using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: EndScreen
 *
 * Author: Will Harding
 *
 * Purpose: Screen for level completion/death
 * 
 * Functions:   private void Start()
 *              public void Continue()
 *              private IEnumerator LoadLevel()
 *              public void MainMenu()
 *              public void Exit()
 *              public void Display( bool death = false, bool gunGrabbed = true )
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 05/08/22     WH          1.0         Initial creation
 * 10/08/22     WH          1.1         Fixed while loop
 * 13/08/22     WH          1.2         Fixed timescale
 * 14/08/22     WH          1.3         Loading decimal places
 * 16/08/22     WH          1.4         Cleaning
 * 17/08/22     WH          1.5         Saves time on level completion
 ****************************************************************************************************/
public class EndScreen : MonoBehaviour
{
    [Tooltip( "The actual menu to display" )]
    public  GameObject      m_screen;       // Menu contents

    [Tooltip( "The title text" )]
    public  TMP_Text        m_Title;        // Title text

    [Tooltip( "The Subtitle text" )]
    public  TMP_Text        m_Subtitle;     // Subtitle text

    [Tooltip( "The 'Next Level' or 'Continue' Button" )]
    public  Button          m_button;       // Continue button

    [Tooltip( "The text component of the 'Next Level' or 'Continue' Button" )]
    public  TMP_Text        m_buttonText;   // Continue button text

    [Tooltip( "The GameStats script attached to the object displaying said stats" )]
    public  GameStats       m_gameStats;    // Game stats

    private AsyncOperation  m_loading;      // The scene loading operation

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Displays stats and loads level
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Sets the time when the game starts
        m_gameStats.SetStartTime( Time.time );
    }

    /***************************************************
    *   Function        :    Continue
    *   Purpose         :    Goes to newly loaded level
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Continue()
    {
        // Make sure game is unpaused
        Time.timeScale = 1;

        // Allow scene to load
        m_loading.allowSceneActivation = true;
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
        m_loading = SceneManager.LoadSceneAsync( "Level" );

        // Stop level from instantly loading
        m_loading.allowSceneActivation = false;

        // While loading the level
        while ( !m_loading.isDone )
        {
            // Set button text to loading percentage
            m_buttonText.text = "Loading: " + ( m_loading.progress * 100 ).ToString( "00" ) + "%";

            // When progress >= 90%, the level is loaded
            if ( m_loading.progress >= 0.9f )
            {
                // Set the button text to contine and make it interactable
                m_buttonText.text = "Continue";
                m_button.interactable = true;
            }

            yield return null;
        }
    }

    /***************************************************
    *   Function        :    MainMenu
    *   Purpose         :    Return to main menu
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    13/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void MainMenu()
    {
        // Make sure game is unpaused
        Time.timeScale = 1;

        // Quit back to main menu
        SceneManager.LoadSceneAsync( "MainMenu" );
    }

    /***************************************************
    *   Function        :    Exit
    *   Purpose         :    Closes game
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
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
    *   Function        :    Display
    *   Purpose         :    Displays screen and changes text depending on death
    *   Parameters      :    bool death = false
    *                        bool gunGrabbed = true
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Display( bool death = false, bool gunGrabbed = true )
    {
        // Display screen
        m_screen.SetActive( true );

        // Display game stats
        m_gameStats.DisplayStats( true );

        // Displays cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game
        Time.timeScale = 0;

        // If the player died
        if ( death )
        {
            // Disable continue button
            m_button.gameObject.SetActive( false );

            // Change text
            m_Title.text = "You Died";

            // If you did not grab your gun
            if ( !gunGrabbed )
            {
                // Change text
                m_Subtitle.text = "Maybe grab a gun next time?";
            }
            else
            {
                // Change text
                m_Subtitle.text = "Better luck on the next run";
            }
        }
        // else if the player completed the level
        else
        {
            // Load next level
            StartCoroutine( LoadLevel() );
        }

    }
}
