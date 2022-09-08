using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: VendingMachine
 *
 * Author: Will Harding
 *
 * Purpose: Override of abstact interactable for vending machine
 * 
 * Functions:   private void Start()
 *              public override void Interact()
 *              public void Dispense()
 * 
 * References:
 * 
 * See Also:    Interactable
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 07/04/22     WH          1.0         Created
 * 11/04/22     JG          1.01        Updated player reference 
 * 12/04/22     JG          1.02        Updated InputManager reference
 * 12/05/22     WH          1.1         Final comment run through before submission
 * 
 * 21/06/22     WH          2.0         Changed look at text to use UI
 * 22/07/22     WH          2.1         Changed shopUI to call function
 * 28/07/22     WH          2.2         Added sound and animation Dispense function
 * 31/07/22     WH          2.1         Interactable text changes
 * 15/08/22     WH          2.2         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof(AudioSource))][RequireComponent(typeof( Animator ) )]
public class VendingMachine : Interactable
{
    [Tooltip( "The shop menu to display when interacted with" )]
    public      ShopMenu        m_shopUI;       // Shop UI

    private     AudioSource     m_coffee;       // Coffee dispense sound

    private     Animator        m_animator;     // Animator

    private     int             an_dispense;    // Dispense animator ID


    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Gets variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    28/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get components
        m_coffee = GetComponent<AudioSource>();
        m_animator = GetComponent<Animator>();

        an_dispense = Animator.StringToHash( "Dispense" );
    }

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    22/07/22
    *   Contributors    :    WH JG 
    *   Notes           :    Override function
    *   See also        :    
    ******************************************************/
    public override void Interact()
    {
        // Display shop UI and enable shop controls
        m_shopUI.gameObject.SetActive( true );
        m_shopUI.OpenShopMenu();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().EnableMenuControls();
    }

    /***************************************************
    *   Function        :    Dispense
    *   Purpose         :    Play sound and animate
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    26/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Dispense()
    {
        // Play sound and animation
        m_coffee.Play();
        m_animator.SetTrigger( an_dispense );
    }
}
