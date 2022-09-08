using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: RoomArrays
 *
 * Author: Will Harding
 *
 * Purpose: Contains arrays of room prefabs to spawn in level
 * 
 * Functions:   
 * 
 * References: 
 * 
 * See Also: ArrayGen
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 02/04/22     WH          1.0         Initial creation
 * 29/04/22     WH          1.1         Added small room variations
 * 02/04/22     WH          1.2         Added big room variations
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 03/06/22     WH          2.0         Added door variable for testing
 * 08/06/22     WH          2.1         Added interior variables
 * 11/06/22     WH          2.2         Added small and big room variables
 * 14/06/22     WH          3.6         Added big room interiors
 * 19/06/22     WH          3.7         Parent of specific room shape children
 * 29/06/22     WH          3.8         Added end room exterior
 * 08/07/22     WH          3.9         Added start room in and exteriors
 * 27/07/22     WH          3.10        Added treasure room exterior
 * 15/08/22     WH          3.11        Reformatting for how the rooms now generate in and exteriors together
 * 17/08/22     WH          3.12        Cleaning
 ****************************************************************************************************/
public class RoomArrays : MonoBehaviour
{
    [Tooltip( "The door prefab to place between rooms" )]
    public GameObject m_door; // The door prefab to place between rooms

    [Tooltip( "The start room to use for level 1" )]
    public GameObject m_startRoomLevel1; // Start room to be used for level 1


    [Header( "Big & Special Rooms" )]

    [Tooltip( "Big rooms" )]
    public GameObject[] m_bigRooms; // Array of big rooms

    [Tooltip( "Start rooms" )]
    public GameObject[] m_startRooms; // Array of Start rooms to use after level 1

    [Tooltip( "Treasure rooms" )]
    public GameObject[] m_treasureRooms; // array of treasure rooms 

    [Tooltip( "End rooms" )]
    public GameObject[] m_endRooms; // Array of end rooms


    [Header( "Small Room Layouts" )]

    [Tooltip( "2 opposite connection rooms" )]
    public GameObject[] m_IRoom; // Array of small rooms with 2 connections opposite each other

    [Tooltip( "2 adjcent connection rooms" )]
    public GameObject[] m_LRoom; // Array of small rooms with 2 connections next to each other

    [Tooltip( "3 connection rooms" )]
    public GameObject[] m_TRoom; // Array of small rooms with 3 connections

    [Tooltip( "4 connection rooms" )]
    public GameObject[] m_XRoom; // Array of small rooms with 4 connections

}
