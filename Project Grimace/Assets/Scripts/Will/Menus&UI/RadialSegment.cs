using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: RadialSegment
 *
 * Author: Will Harding
 *
 * Purpose: Segment of radial menu
 * 
 * Functions:   private void Awake()
 *              public void Align( float fill, float rotation, Quaternion iconRot )
 *              public virtual void Select()
 *              public void HoverEnter()
 *              public void HoverExit()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 01/07/22     WH          1.0         Initial creation
 * 03/07/22     WH          1.1         Added full functionality
 * 08/07/22     WH          1.2         Set scale to 0 in align
 * 16/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class RadialSegment : MonoBehaviour
{
    [Tooltip( "The ring object of the segment" )]
    public      GameObject  m_ring;                                                     // Ring object
    [Tooltip( "The icon object of the segment" )]
    public      GameObject  m_icon;                                                     // Icon object

    [SerializeField]
    [Tooltip( "The distance from the center of the screen to place the icon" )]
    [Min(0)]
    private     int         m_iconDist;                                                 // Distance from center to icon

    [Tooltip( "The colour of the segment when it's being displayed" )]
    public      Color       m_displayColour     = new Color( 0, 0, 0, 122 );            // Display colour of the segment

    [Tooltip( "The colour of the segment when it's being selected" )]
    public      Color       m_selectedColour    = new Color( 255, 255, 255, 122 );      // Selected colour of the segment
    
    protected   float       m_fillAmount;                                               // Amout of the ring that is filled
    protected   RawImage    m_iconImage;                                                // The image of the icon
    protected   Image       m_ringImage;                                                // The image of the ring


    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Gets components
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    01/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Awake()
    {
        // Get ring and icon images
        m_ringImage = m_ring.GetComponent<Image>();
        m_iconImage = m_icon.GetComponent<RawImage>();
    }

    /***************************************************
    *   Function        :    Align
    *   Purpose         :    Aligns the sections
    *   Parameters      :    float fill
    *                        float rotation
    *                        Quaternion iconRot
    *   Returns         :    void
    *   Date altered    :    08/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Align( float fill, float rotation, Quaternion iconRot )
    {
        // Set the fill amount
        m_fillAmount = fill;

        // Set fill of the ring
        m_ring.GetComponent<Image>().fillAmount = m_fillAmount;

        // Rotate the ring
        transform.localRotation = Quaternion.Euler( 0, 0, rotation );

        // Icon position is the ring's rotated pos + the icon rotation * rotation to rotate from * the distance to the middle
        m_icon.transform.localPosition = m_ring.transform.localPosition + iconRot * Vector3.up * m_iconDist;

        // Rotate the icon to be facing upright
        m_icon.transform.Rotate( 0, 0, -rotation, Space.Self );

        // Set scale to 0 so the segments can animate cleanly
        transform.localScale = Vector3.zero;
    }

    /***************************************************
    *   Function        :    Select
    *   Purpose         :    Does something when segment is selected
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public virtual void Select()
    {
        // Do something in overload
    }

    /***************************************************
    *   Function        :    HoverEnter
    *   Purpose         :    Does something when segment is hovered over
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void HoverEnter()
    {
        // Change colour to selected colour
        m_ringImage.color = m_selectedColour;
    }

    /***************************************************
    *   Function        :    HoverExit
    *   Purpose         :    Does something when segment is hovered off of
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void HoverExit()
    {
        // Change colour to display colour
        m_ringImage.color = m_displayColour;
    }
}
