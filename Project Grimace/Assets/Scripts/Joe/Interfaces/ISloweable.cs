using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Interface
 * 
 * Name: ISloweable
 * Author: Joseph Gilmore 
 *
 * Purpose: For items that can be slowed by grenades 
 *
 * 
 * Functions:    float   GetDefaultSpeed();
                 void SetSpeed( float speed );
 * 
 * References:
 * 
 * See Also: PlayerManager , AgentAI
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 14/08/2022     JG         1.00           - Created 
 ****************************************************************************************************/
public interface ISloweable
{
    float   GetDefaultSpeed();
    void SetSpeed( float speed );
}
