using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: CameraAndScreen
 *
 * Author: Will Harding
 *
 * Purpose: Makes render texture for screena and links them to a camera
 * 
 * Functions:   private void Start()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 30/06/22     WH          1.0         Initial creation
 * 17/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class CameraAndScreen : MonoBehaviour
{
    private     RenderTexture   m_renderTexture;                // Texture that gets the camera feed

    [Tooltip( "THe camera that the screen will display" )]
    public      Camera          m_camera;                       // The camera that the screen displays

    [Tooltip( "The screen to display the camera feed" )]
    public      GameObject      m_screen;                       // The screen to display the camera feed

    [Tooltip( "The width of the render texture" )]
    public      int             m_textureWidth      = 640;      // Texture width

    [Tooltip( "The height of the render texture" )]
    public      int             m_textureHeight     = 360;      // Texture height


    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Creates and assigns render texture
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    30/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Make new render texture
        m_renderTexture = new RenderTexture( m_textureWidth, m_textureHeight, 8 );
        
        // Set the camera's target texture to the render texture
        m_camera.targetTexture = m_renderTexture;

        // Start the camera rendering
        m_camera.Render();

        // Set the screen's texture to the render texture
        m_screen.GetComponent<Renderer>().material.mainTexture = m_renderTexture;
    }
}
