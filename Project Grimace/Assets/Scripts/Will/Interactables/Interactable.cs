using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: Interactable
 *
 * Author: Will Harding
 *
 * Purpose: Abstract class for interactable objects
 * 
 * Functions:   protected virtual void Awake()
 *              public virtual void DisplayLookAtText()
 *              public virtual void HideLookAtText()
 *              public virtual void Interact()
 *              public virtual void Update()
 * 
 * References:
 * 
 * See Also:    VendingMachine, Pickup
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 07/04/22     WH          1.0         Created
 * 08/04/22     WH          1.1         Added HideLookAtText
 * 
 * 21/06/22     WH          2.0         Changed look at text to be UI
 * 23/06/22     WH          2.1         Added update to fix controller not being set when awake isn't called
 * 28/07/22     WH          2.2         Removed hover text
 * 31/07/22     WH          2.3         Added text, alt text, and useAltText variables for text display
 * 10/08/22     WH          2.4         Cleaning
 * 15/08/22     WH          2.5         Cleaning
 ****************************************************************************************************/
public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    [Tooltip( "Text to display on screen when interacting with object" )]
    protected   string          m_text;             // Display text

    [SerializeField]
    [Tooltip( "Alternative text to display" )]
    protected   string          m_altText;          // Alt display text

    protected   bool            m_useAltText;       // Bool for if alt text should be used or not

    public      UIController    m_controller;       // UI controller

    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Gets UI controller
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    21/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected virtual void Awake()
    {
        // Get UI controller
        m_controller = GameObject.FindGameObjectWithTag( "Player" ).GetComponentInChildren<UIController>();
    }

    /***************************************************
    *   Function        :    DisplayLookAtText
    *   Purpose         :    Displays hover text for info
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/08/22
    *   Contributors    :    WH
    *   Notes           :    Virtual function
    *   See also        :    
    ******************************************************/
    public virtual void DisplayLookAtText()
    {
        // Show interact text
        m_controller.SetInteractActive( true );

        // Set interact text depending on useAltText bool
        m_controller.UpdateInteract( m_useAltText ? m_altText : m_text );
    }

    /***************************************************
    *   Function        :    HideLookAtText
    *   Purpose         :    Hides hover text for info
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    21/06/22
    *   Contributors    :    WH
    *   Notes           :    Virtual function
    *   See also        :    
    ******************************************************/
    public virtual void HideLookAtText()
    {
        m_controller.SetInteractActive( false );
    }

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    07/04/22
    *   Contributors    :    WH
    *   Notes           :    Virtual function
    *   See also        :    
    ******************************************************/
    public virtual void Interact()
    {
        //Contents added in children
    }
}
