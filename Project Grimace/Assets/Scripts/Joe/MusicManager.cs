using UnityEngine;
using UnityEngine.Audio;
using System.Collections;


namespace BackGroundMusic
{

    /****************************************************************************************************
     * Type: Class
     * 
     * Name: MusicManager 
     *
     * Author: Joseph Gilmore 
     *
     * Purpose:  Handle the mutiple music sound tracks within the game 
     * 
     * Functions:   private void Awake()
     *              private void Start()
     *              public void ToggleCombatMusic( bool toggle )
     *              private IEnumerator FadeTracks(AudioSource fadeFrom , AudioSource fadeTo)
     * 
     * References: Insipration from www.youtube.com. (n.d.). How to Fade Between Audio Tracks [Unity Tutorial]. [online] Available at: https://www.youtube.com/watch?v=1VXeyeLthdQ&t=250s [Accessed 3 Aug. 2022].

     * 
     * See Also:
     *
     * Change Log:
     * Date          Initials    Version     Comments
     * ----------    --------    -------     ----------------------------------------------
     * 03/08/2022    JG          1.00        - Created
     * 03/08/2022    WH          1.01        - Added mixer output
     * 04/08/2022    JG          1.02        - Fixed audio stop
     * 05/08/2022    JG          1.03        - Audio looping
     * 10/08/2022    WH          1.04        - Added multiple instance destruction
     ****************************************************************************************************/
    public class MusicManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerGroup     m_output;                   // Output mixer

        private AudioSource         m_musicTrackOne;            // Reference to first track 
        private AudioSource         m_musicTrackTwo;            // Reference to secound track 
        [SerializeField]
        private AudioClip           m_nonCombatMusic;           // None combat backing track
        [SerializeField]
        private AudioClip           m_combatMusic;              // Combat backing rack
        public static MusicManager  instance;                   // Store an instance of the manager 
        private float               m_fadeLength = 0.5f;        // Length of time to fade between tracks

        /***************************************************
         *   Function        :  Awake     
         *   Purpose         :  Ensure clas can be utilized properly   
         *   Parameters      :  None
         *   Returns         :  void
         *   Date altered    :  10/08/22
         *   Contributors    :  JG, WH
         *   Notes           :    
         *   See also        :    
         ******************************************************/
        private void Awake()
        {
            // If there is another music that exists, destory it and keep this one
            if( instance != null )
            {
                Destroy( instance.gameObject );
            }
            
            // Create and instance of self
            instance = this;

            // Don't destroy between scenes 
            DontDestroyOnLoad( instance );
        }

        /***************************************************
         *   Function        :  Start  
         *   Purpose         : Setup sources and clips    
         *   Parameters      : N/A   
         *   Returns         : Void   
         *   Date altered    : 05/08/2022
         *   Contributors    : JG
         *   Notes           :    
         *   See also        :    
         ******************************************************/
        private void Start()
        {
            // Add audio sources for each backing track
            m_musicTrackOne = gameObject.AddComponent<AudioSource>();
            m_musicTrackTwo = gameObject.AddComponent<AudioSource>();

            // Set mixer group
            m_musicTrackOne.outputAudioMixerGroup = m_output;
            m_musicTrackTwo.outputAudioMixerGroup = m_output;

            // Loop both tracks 
            m_musicTrackOne.loop = true;
            m_musicTrackTwo.loop = true;

            // Assign sources tracks 
            m_musicTrackOne.clip = m_nonCombatMusic;
            m_musicTrackTwo.clip = m_combatMusic;

            // Start the first track 
            m_musicTrackOne.Play();
        }

        /***************************************************
         *   Function        : ToggleCombatMusic   
         *   Purpose         : Turn combat music on or off   
         *   Parameters      : bool toggle
         *   Returns         : Void   
         *   Date altered    : 03/08/2022 
         *   Contributors    : JG
         *   Notes           :    
         *   See also        :    
         ******************************************************/
        public void ToggleCombatMusic( bool toggle )
        {
            // Stop all pior routines to stop the music repeating weirdly 
            StopAllCoroutines();

            // if combat music toggled 
            if ( toggle )
            {
                StartCoroutine( FadeTracks( m_musicTrackOne, m_musicTrackTwo ) );

            }
            else
            {
                // Turn back to idle music 
                StartCoroutine( FadeTracks( m_musicTrackTwo, m_musicTrackOne ) );
            }
    
        }
        /***************************************************
         *   IEnumrator      : FadeTracks 
         *   Purpose         : fade between two backing tacks    
         *   Parameters      : AudioSource fadeFrom , AudioSource fadeTo   
         *   Returns         : yield return null   
         *   Date altered    : 04/08/2022
         *   Contributors    : JG
         *   Notes           :    
         *   See also        :    
         ******************************************************/
        private IEnumerator FadeTracks( AudioSource fadeFrom , AudioSource fadeTo )
        {
            // Set up a timer 
            float timer = 0;

            if( fadeTo.isPlaying == false )
            {
                // Start playing new sound
                fadeTo.Play();
            }
           

            // While timer no complete fade tracks
            while( timer < m_fadeLength )
            {
                // Increase timer 
                timer += Time.deltaTime;

                // Lower volume of current track 
                fadeFrom.volume = Mathf.Lerp( 1.0f,0.0f , timer / m_fadeLength );

                // Increase volume of new track 
                fadeTo.volume = Mathf.Lerp( 0.0f,1.0f , timer / m_fadeLength );
                yield return null;
            }

          
        }
    }
}
