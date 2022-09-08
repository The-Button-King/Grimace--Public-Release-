using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: AudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Parent class for pooling audio clips
 * 
 * Functions:   protected virtual void Start()
 *              protected int GetNextIndex()
 *              public void PlaySound( Audio clip )
 *              public void StopSound( Audio clip )
 * 
 * References: 
 * 
 * See Also:
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 09/06/22     WH          1.0         Initial creation
 * 10/06/22     WH          1.1         Set up pools in start
 * 26/07/22     WH          1.2         Cleaning and comments
 * 27/07/22     JG          1.3         Added stop sound
 * 29/07/22     WH          1.4         Turned off play on awake
 * 31/07/22     WH          1.5         Added spatial blend bool
 * 14/08/22     WH          1.6         Cleaning
 * 15/08/22     JG          1.7         Bugfix
 ****************************************************************************************************/
public abstract class AudioPool : MonoBehaviour
{
    [Header( "Pool Settings" )]

    [Tooltip( "The mixer the audio should output from" )]
    public      AudioMixerGroup     m_output;               // Mixer to output audio
    
    private      AudioSource[]       m_sourcePool;           // Pool of audio sources where the various clips play
    
    [SerializeField]
    [Tooltip( "How many audio sources should be in the pool" )]
    [Min(1)]
    protected   int                 m_poolSize      = 5;    // Size of audio pool
    
    [SerializeField]
    [Tooltip( "Should the audio be directional or be omnipresent? True for 3D, false for 2D audio" )]
    protected   bool                m_spatialBlend  = true; // Spatial blend bool
    
    protected   int                  m_index        = 0;    // next audio source in pool to play audio


    /***************************************************
    *   Function        :  Start
    *   Purpose         :  Makes sources
    *   Parameters      :  None
    *   Returns         :  void
    *   Date altered    :  31/07/2022
    *   Contributors    :  WH
    *   Notes           :  virtual
    *   See also        :  
    ******************************************************/
    protected virtual void Start()
    {
        // Make pool array
        m_sourcePool = new AudioSource[m_poolSize];

        // For the number of audio sources to add to the pool
        for ( int i = 0; i < m_poolSize; i++ )
        {
            // Make audio source
            AudioSource temp = gameObject.AddComponent<AudioSource>();

            // Set the output mixer
            temp.outputAudioMixerGroup = m_output;
            
            // Set if the spacial belend is 3D or 2D 
            temp.spatialBlend = m_spatialBlend ? 1 : 0;

            // Do not play on awake
            temp.playOnAwake = false;

            // Add audio pool to array
            m_sourcePool[ i ] = temp;
        }
    }

    /***************************************************
    *   Function        :  GetNextIndex
    *   Purpose         :  Gets next index of audio source to use
    *   Parameters      :  None
    *   Returns         :  int index 
    *   Date altered    :  26/07/2022
    *   Contributors    :  WH
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    protected int GetNextIndex()
    {
        // If overflowing out of the array range
        if ( m_index + 1 >= m_sourcePool.Length )
        {
            // Roll back to 0
            m_index = 0;
        }
        else
        {
            m_index++;
        }

        return m_index;
    }

    /***************************************************
    *   Function        :  PlaySound
    *   Purpose         :  Plays sound
    *   Parameters      :  Audio clip
    *   Returns         :  void 
    *   Date altered    :  26/07/2022
    *   Contributors    :  WH
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    public void PlaySound( Audio clip )
    {
        // Apply the Audio variables to the source
        clip.SetVars( m_sourcePool[ m_index ] );

        // Play the audio
        m_sourcePool[ m_index ].Play();

        // Get the index of the next source
        GetNextIndex();
    }


    /***************************************************
    *   Function        :  StopSound
    *   Purpose         :  Stops audio from playing
    *   Parameters      :  Audio clip
    *   Returns         :  void 
    *   Date altered    :  15/08/2022
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :  
    ******************************************************/
    public void StopSound( Audio clip )
    {
        if ( m_sourcePool != null )
        {
            // Loop through pool
            foreach ( AudioSource source in m_sourcePool )
            {

                // If the source is playing the specified clip
                if ( source.clip == clip.m_clip )
                {
                    // If the source is actually playing said clip
                    if ( source.isPlaying )
                    {
                        // Stop playing
                        source.Stop();
                        break;
                    }
                }


            }
        }
       
       
    }
}
