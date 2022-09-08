using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Elevator
 *
 * Author: Will Harding
 *
 * Purpose: Override of abstact interactable for elevator
 * 
 * Functions:   public override void Interact()
 * 
 * References:
 * 
 * See Also:    Interactable
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 04/05/22     WH          1.0         Created
 * 07/05/22     WH          1.1         Added functionality
 * 11/05/22     WH          1.2         Syntax changing
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 21/06/22     WH          2.0         Changed look at text to use UI
 * 31/07/22     WH          2.1         Interactable text changes
 * 15/08/22     WH          2.2         Cleaning
 ****************************************************************************************************/
public class Elevator : Interactable
{
    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    15/05/22
    *   Contributors    :    WH
    *   Notes           :    Virtual function
    *   See also        :    
    ******************************************************/
    public override void Interact()
    {
        // Gets data holder
        DataHolder data = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        
        // Increses level count
        data.SetLevel( data.GetLevel() + 1 );

        // Gets data from level and holds it
        data.HoldData();

        // Enables menu controls and displays end screen
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().EnableMenuControls();
        GameObject.Find( "EndScreen" ).GetComponent<EndScreen>().Display();
    }
}
