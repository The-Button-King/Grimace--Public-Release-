using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: MenuLight
 *
 * Author: Will Harding
 *
 * Purpose: Moves light with mosue
 * 
 * Functions:   private void Update()
 *              private void FollowMouse()
 * 
 * References: 
 * 
 * See Also:
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 05/08/22     WH          1.0         Initial creation
 * 16/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class MenuLight : MonoBehaviour
{

    /***************************************************
    *   Function        :    Update
    *   Purpose         :    Calls FollowMouse
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        FollowMouse();
    }


    /***************************************************
    *   Function        :    FollowMouse
    *   Purpose         :    Moves light to follow mouse
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void FollowMouse()
    {
        // Get mouse position
        Vector3 pos = Mouse.current.position.ReadValue();

        // Zero out z value
        pos.z = 0;

        // Chaneg light position
        gameObject.transform.position = pos;
    }
}
