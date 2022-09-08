using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: MainMenuAudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Menu audio pool child and has functions to play sounds to make it easier for Unity UI
 * 
 * Functions:   public void PlayButtonHover()
 *              public void PlayMenuChange()
 * 
 * References: 
 * 
 * See Also: AudioPool
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 31/07/22     WH          1.0         Initial creation
 * 14/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class MenuAudioPool : AudioPool
{
    [Header( "Menu Audios" )]

    [Tooltip("Audio for when hovering on a button")]
    public Audio m_buttonHover;

    [Tooltip( "Audio for when changing menus" )]
    public Audio m_menuChange;

    /***************************************************
    *   Function        :    PlayButtonHover
    *   Purpose         :    Plays button hover sound
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    Exists here as is easier to use here than in multiple menu scripts
    *   See also        :    
    ******************************************************/
    public void PlayButtonHover()
    {
        PlaySound( m_buttonHover );
    }

    /***************************************************
    *   Function        :    PlayMenuChange
    *   Purpose         :    Plays menu change sound
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    Exists here as is easier to use here than in multiple menu scripts
    *   See also        :    
    ******************************************************/
    public void PlayMenuChange()
    {
        PlaySound( m_menuChange );
    }
}
