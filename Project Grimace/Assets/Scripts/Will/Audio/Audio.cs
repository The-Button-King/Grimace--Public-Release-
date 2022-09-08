using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: Audio
 *
 * Author: Will Harding
 *
 * Purpose: Class for storing audio clips with settings
 * 
 * Functions:   public void SetVars( AudioSource source )
 * 
 * References: 
 * This class is inspired by the Sound class made in Brackeys: Introduction to AUDIO in Unity,
 * found at https://youtu.be/6OT43pvUyfY. Date accessed: 10/06/22
 * 
 * See Also: AudioPool
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 10/06/22     WH          1.0         Initial creation
 * 26/07/22     WH          1.1         Added maxDistance and function to assign vars
 * 14/08/22     WH          1.2         Cleaning
 ****************************************************************************************************/
[System.Serializable]
public class Audio 
{
    [Tooltip( "Name of the audio" )]
    public string      m_name;                 // name of audio

    [Tooltip( "The audio clip to play" )]
    public     AudioClip   m_clip;                 // Clip to play

    [SerializeField]
    [Tooltip( "Volume of the audio" )]
    [Range(0f, 1f)]
    private     float       m_volume        = 1;    // Volume of the clip
    
    [SerializeField]
    [Tooltip( "Pitch of the audio" )]
    [Range (0.1f, 3f)]
    private     float        m_pitch        = 1;    // Pitch of the clip

    
    [SerializeField]
    [Tooltip( "Max Distance to hear the audio from" )]
    [Range(1f, 40f)]
    private     float       m_maxDistance   = 30f;  // Max distance away that you can hear the audio from

    [SerializeField]
    [Tooltip( "Loop the audio" )]
    private     bool        m_loop;                 // Loop the audio?

    [SerializeField]
    [Tooltip( "Mute the audio" )]
    private     bool        m_mute;                 // Mute the audio?

    /***************************************************
    *   Function        :  SetVars
    *   Purpose         :  Sets variables for audio source
    *   Parameters      :  AudioSource source
    *   Returns         :  void
    *   Date altered    :  26/07/2022
    *   Contributors    :  WH
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    public void SetVars( AudioSource source )
    {
        // Set all the variables onto the source
        source.clip = m_clip;
        source.volume = m_volume;
        source.pitch = m_pitch;
        source.maxDistance = m_maxDistance;
        source.loop = m_loop;
        source.mute = m_mute;
    }
}
