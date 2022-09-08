using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: MeshFloorFix
 *
 * Author: Will Harding
 *
 * Purpose: Changes render order on mesh floors in I room to stop rendering issues
 * 
 * Functions:   private void Start()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 01/09/22     WH          1.0         Initial creation
 ****************************************************************************************************/
public class MeshFloorFix : MonoBehaviour
{
    [Tooltip( "The floors of the room, ordered bottom to top" )]
    public GameObject[] m_orderedFloors; // The floors of the room, ordered bottom to top

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Changes render queue of floors
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    01/09/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        int i = 100;
        foreach( GameObject floor in m_orderedFloors )
        {
            floor.GetComponent<MeshRenderer>().material.renderQueue += i;
            i += 100;
        }
    }
}
