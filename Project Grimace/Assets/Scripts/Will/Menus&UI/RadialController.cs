using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: RadialController
 *
 * Author: Will Harding
 *
 * Purpose: Radial menu to select different segments
 * 
 * Functions:   public bool GetOpen()
 *              private void Start()
 *              private void Update()
 *              private void GetSelection()
 *              public void OpenMenu()
 *              public void CloseMenu()
 *              public void Click( InputAction.CallbackContext context )
 *              public void SelectSegment()
 *              private IEnumerator Animate( bool reverse )
 *              private IEnumerator CallAnimate( bool reverse )
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 01/07/22     WH          1.0         Initial creation
 * 03/07/22     WH          1.1         Added full functionality
 * 04/07/22     WH          1.2         Added open/close and proper input system start
 * 07/07/22     WH          1.3         Made radial open and close properly with hold button
 * 08/07/22     WH          1.4         Cleaning and comments
 * 11/07/22     WH          1.5         Select on hover and close
 * 28/07/22     WH          1.6         Click input check if radial is open
 * 16/08/22     WH          1.7         Cleaning
 ****************************************************************************************************/
public class RadialController : MonoBehaviour
{
    [Header( "Radial Components" )]

    [Tooltip( "The actual radial parent" )]
    public      GameObject          m_holder;                       // The actual radial

    [Tooltip( "The segemts that make up the radial. Drag them in here to apply them at runtime" )]
    public      GameObject[]        m_segments;                     // Segments in radial
    

    [Header( "Variables" )]

    [SerializeField]
    [Tooltip( "Gap angle between each segment" )]
    [Range(0, 90)]
    private     float               m_gapAngle          = 10f;      // How many degrees of gap to leave between segments

    [SerializeField]
    [Tooltip( "Minimum mouse distance from centre to count as selecting a segment" )]
    [Min(0)]
    private     float               m_minMouseDist      = 65f;      // Minimum mouse distance from centre to count as selecting a segment

    [SerializeField]
    [Tooltip( "The speed the radial lerps in and out" )]
    [Range(0.5f, 10f)]
    private     float               m_speed             = 2.5f;     // Speed to lerp the segments


    private     int                 m_numSegments;                  // Number of segments in radial
    private     float               m_segmentPercentage;            // Percentage of circle that the segment takes up

    private     int                 m_selection         = -1;       // Selected segment
    private     int                 m_prevSelection     = 0;        // Previously selected segment

    private     InputAction         m_click;                        // Click input

    private     bool                m_open;                         // is the radial open?
    private     bool                m_animating         = false;    // is the radial animating?

    private     PlayerManager       m_playerManager;                // Player manager
    private     PlayerControls      m_playerControls;               // Contols


    /***************************************************
    *   Function        :    GetOpen
    *   Purpose         :    Gets open
    *   Parameters      :    None
    *   Returns         :    bool open
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool GetOpen()
    {
        return m_open;
    }

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Aligns the sections
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get player controls
        m_playerManager = GameObject.FindWithTag( "Player" ).GetComponent<PlayerManager>();
        m_playerControls = m_playerManager.GetControls();

        // Set up mouse click detection 
        m_click = m_playerControls.Radial.Click;
        m_click.performed += Click;
        m_click.Disable();

        // Number of segments in radial
        m_numSegments = m_segments.Length;

        // How much of the radial each segemnt takes up
        m_segmentPercentage = 1f / m_numSegments;

        // For each segment
        for( int i = 0; i < m_numSegments; i++)
        {
            // Get the segment script
            RadialSegment segmentScript = m_segments[i].GetComponent<RadialSegment>();
            
            // Get segment percentage with gap
            float fill = m_segmentPercentage - m_gapAngle / 360f;

            // -360 * (index * segment%) gives how far to rotate the segment, -gapAngle / 2 adds the initial gap
            float rot = -360f * ( i * m_segmentPercentage ) - m_gapAngle / 2 ;

            // ( (-360 * segment%) - (gapAngle / 2) ) / 2 gives half of 1 segment rotation, which is the rotation for the icon
            Quaternion iconRot = Quaternion.AngleAxis( ( -360f * ( m_segmentPercentage ) - m_gapAngle / 2 ) / 2, Vector3.forward ) ;
            
            // Align the segment
            segmentScript.Align( fill, rot, iconRot );
        }
    }

    /***************************************************
    *   Function        :    Update
    *   Purpose         :    Checks for selection and hover
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        // If the radial is open
        if ( m_holder.activeSelf )
        {
            // Get radial selection
            GetSelection();

            // if the current selection does not equal the previous selectior or is -1
            if ( m_selection != m_prevSelection && m_selection != -1 )
            {
                // if the previous selection is a segment
                if( m_prevSelection != -1 )
                {
                    // Call exit hover function
                    m_segments[ m_prevSelection ].GetComponent<RadialSegment>().HoverExit();
                }

                // Change what the previous selection was
                m_prevSelection = m_selection;

                // if you can click on a segment
                if( m_click.enabled )
                {
                    // Call enter hover function
                    m_segments[ m_selection ].GetComponent<RadialSegment>().HoverEnter();
                }

            }

            // if you aren't hovering over a selection and the previous selection is a segment
            if( m_selection == -1 && m_prevSelection != -1 )
            {
                // Call exit hover function
                m_segments[ m_prevSelection ].GetComponent<RadialSegment>().HoverExit();

                // Change what the previous selection was
                m_prevSelection = m_selection;
            }
        }
    }

    /***************************************************
    *   Function        :    GetSelection
    *   Purpose         :    Gets mouse position and corrorsponding section
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void GetSelection()
    {
        // Get mouse pos using new input system
        Vector2 mousePos = m_playerControls.Radial.MousePos.ReadValue<Vector2>();
        // Get mouse position from the center of the screen
        Vector2 mouseFromCenter = new Vector2( mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2 );

        // Get angle from the center of the screen and the y axis
        float mouseAngle = Mathf.Atan2( mouseFromCenter.x, mouseFromCenter.y ) * Mathf.Rad2Deg;

        // MouseAngle is between -180 and 180, so +360 to make +ve, and then modulo by 360 to clamp the values to be bwteen 0 and 360
        mouseAngle = ( mouseAngle + 360 ) % 360;

        // if the distance from the center and the mouse is greater than the min mouse distance to select a segment
        if( Vector2.Distance( Vector2.zero, mouseFromCenter) >= m_minMouseDist )
        {
            // Get the section of the radial the mouse is in
            m_selection = ( int )( mouseAngle / ( 360 / m_numSegments ) );
        }
        else
        {
            // Set selection to -1 if no section is hovered on
            m_selection = -1;
        }

    }

    /***************************************************
    *   Function        :    OpenMenu
    *   Purpose         :    Activates radial
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void OpenMenu()
    {
        // Show radial
        m_holder.SetActive( true );

        // Set bool to true
        m_open = true;

        // Enable clicking controls
        m_click.Enable();

        // Grow radial
        StartCoroutine( CallAnimate( false ) );
    }

    /***************************************************
    *   Function        :    CloseMenu
    *   Purpose         :    Deactivates radial
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void CloseMenu()
    {
        // Select segment if hovering over one
        SelectSegment();

        // Disable radial controls and enable player controls
        m_playerManager.DisableRadialControls();
        m_click.Disable();

        // Set open to false
        m_open = false;

        // Shrink menu
        StartCoroutine( CallAnimate( true ) );

        // Exit hovering on the segment you were last on
        m_segments[ ^1 ].GetComponent<RadialSegment>().HoverExit();
    }

    /***************************************************
    *   Function        :    Click
    *   Purpose         :    Selects radial segment on click
    *   Parameters      :    InputAction.CallbackContext context
    *   Returns         :    void
    *   Date altered    :    28/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Click( InputAction.CallbackContext context )
    {
        // If clicked and the radial is open
        if ( context.performed && m_open )
        {
            //Select the clicked on segment
            SelectSegment();

            // Close the radial
            CloseMenu();
        }
    }

    /***************************************************
    *   Function        :    SelectSegment
    *   Purpose         :    Selects radial segment
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SelectSegment()
    {
        // If the selection is a valid index
        if ( m_selection != -1 )
        {
            // Call the selection function on the selected segment
            m_segments[ m_selection ].GetComponent<RadialSegment>().Select();
        }
    }


    /***************************************************
    *   Function        :    DelayDeactivate
    *   Purpose         :    Deactivates gas after duration
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    private IEnumerator Animate( bool reverse )
    {
        // lerp value
        float t = 0;

        // Values to lerp to, initally set to shrink
        float start = 1;
        float end = 0;

        // Make a temp array that is the copy of the segments array
        GameObject[] arrayToLoop = new GameObject[ m_segments.Length ];
        System.Array.Copy( m_segments, arrayToLoop, m_segments.Length );

        // If growing
        if ( !reverse )
        {
            // Make sure radial is visable
            m_holder.SetActive( true );

            // Set lerp points
            start = 0;
            end = 1;
        }
        // If shrinking
        else
        {
            // Reverse segments array so the segements lerp anticlockwise
            System.Array.Reverse( arrayToLoop );
        }

        // Set bool to true to queue up any other animating functions
        m_animating = true;

        // While you are still lerping each segment
        while ( t < 1 + (0.1f * arrayToLoop.Length) )
        {
            for ( int i = 0; i < arrayToLoop.Length; i++ )
            {
                // Lerp the scale. t value for every segment is slightly less to give delay on lerp
                float value = Mathf.SmoothStep( start, end, t + ( i * -0.1f ) );
                arrayToLoop[ i ].transform.localScale = new Vector3( value, value, value );
            }

            // Increase t by speed * deltaTime
            t += m_speed * Time.deltaTime;
            yield return null;
        }

        // Hide radial if you have shrunk the radial
        if( reverse )
        {
            m_holder.SetActive( false );
        }

        // Set bool to false to allow other Animate functions to start
        m_animating = false;
    }

    /***************************************************
    *   Function        :    CallAnimate
    *   Purpose         :    Calls the Animate functions after a
    *                        Animation finishes
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    IEnumerator
    *   See also        :    
    ******************************************************/
    private IEnumerator CallAnimate( bool reverse )
    {
        // Wait until the radial is not animating
        yield return new WaitUntil( () => !m_animating );

        // Call Animate function
        StartCoroutine( Animate( reverse ) );
    }
}
