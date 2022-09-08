using EnemyEnums;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: ArrayGen
 *
 * Author: Will Harding
 *
 * Purpose: Generates the level using an array
 * 
 * Functions:   private void Start()
 *              private void GenerateDungeon()
 *              private int DoorOverlapCheck( Vector3 pos )
 *              private List<GameObject> SpawnDoors( (RoomType, List<Directions>, int) room, Vector2Int pos, RoomArrays array )
 *              private void SpawnLevel()
 *              private void InteriorChoiseSmall( (RoomType, List<Directions>, int) room, Vector2Int pos, RoomArrays array )
 *              private float Modulo( float a, float b )
 *              private Dictionary<EnemyTypes, int> SpawnEnemies( int roomSpawnNum )
 *              private bool NeighbourCheck( Vector2Int pos )
 *              private List<Directions> GetConnectionsBig( Vector2Int pos )
 *              private List<Directions> GetConnections( Vector2Int pos )
 *              private Vector2Int[] NeighbourCheckBigRoom( Vector2Int pos )
 *              private Vector2Int GetRandDirection( Vector2Int pos )
 *              private Vector2Int GetRandDirection( Vector2Int pos, List<Vector2Int> offsetsToCheck )
 * 
 * References:
 * 
 * See Also:    RoomArrays
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 29/03/22     WH          1.0         Initally created 
 * 02/04/22     WH          2.0         Added comments and coding standards. Currently supports
 *                                      big and small room generation using prototype assets
 * 04/04/22     WH          2.1         Reordered how rooms are generated
 * 12/04/22     WH          2.2         Started to make rooms know their connections
 * 14/04/22     WH          2.3         Rooms now know their connections
 * 18/04/22     WH          2.4         Enemies spawn in rooms
 * 19/04/22     WH          2.5         Big rooms spawn again with connections known
 * 03/05/22     WH          2.6         Rooms now only spwan with connecting doors
 * 12/05/22     WH          2.7         Final comment run through before submission
 * 
 * 03/06/22     WH          3.0         Beginnings of new door placement system
 * 04/06/22     WH          3.1         Doors no longer double up
 * 06/06/22     WH          3.2         Cleaning
 * 07/06/22     WH          3.3         Start of interior spawning
 * 08/06/22     WH          3.4         Interior spawing
 * 11/06/22     WH          3.5         Cleaned up new door spawning and added wall toggles
 * 14/06/22     WH          3.6         Added big room interior spawning
 * 16/06/22     WH          3.7         Added bomber spawning
 * 19/06/22     WH          3.8         Added newly shaped rooms
 * 20/06/22     WH          3.9         Debug to make only mages spawn
 * 27/06/22     WH          3.10        Removed debug if on some functions
 * 29/06/22     WH          3.11        Fixed T interoir rotation and adde new end room
 * 04/07/22     WH          3.12        Added beacon
 * 06/07/22     WH          3.13        Added array level scalling, increased level sizes, and some better enemy balancing
 * 08/07/22     WH          3.14        Added start room asset spawning
 * 27/07/22     WH          3.15        Added treasure exterior
 * 03/08/22     WH          3.16        Fixed culling
 * 15/08/22     WH          3.17        In and Exterior generation working
 * 17/08/22     WH          3.18        Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof(RoomArrays))][RequireComponent(typeof(NavMeshSurface))]
public class ArrayGen : MonoBehaviour
{

    [SerializeField]
    [Tooltip( "Number of small rooms that can fit on the X axis of the level" )]
    private     int                                         m_levelSizeX            = 10;                       // Width of the level
    
    [SerializeField]
    [Tooltip( "Number of small rooms that can fit on the Y (Z in world terms) axis of the level" )]
    private     int                                         m_levelSizeY            = 10;                       // Depth of the level

    [SerializeField]
    [Tooltip( "Minimum difficulty a level's rooms have" )]
    private     int                                         m_minLevelDifficulty    = 2;                        // Minimum difficulty to start a level at

    // Components
    private     NavMeshSurface                              m_surface;                                          // Navmesh surface component
    private     RoomArrays                                  m_roomArrays;                                       // Contains arrays of room prefabs

    // Enums
    private     enum                                        RoomType                                            // Enum for setting what room to is in each array index       
    {
        Empty,
        Small,
        Big,
        BigExt,
        Start,
        End,
        Treasure,
    }

    private     enum                                        Directions                                          // Enum for the direction a room connects to another room
    {
        North,
        East,
        South,
        West,
        North2,
        East2,
        South2,
        West2,
    }

    // Generation containers
    private     (RoomType, List<Directions>, int)[,]        m_dungeonArray;                                     // Array that is the layout for the level, consisting of RoomType, connections, and difficulty tuple
    private     List<GameObject>                            m_spawnArray;                                       // Array that contains the level's rooms
    
    // Values
    private     int                                         m_numRooms;                                         // Counter for the total number of rooms spawned
    private     int                                         m_maxRooms;                                         // Number of rooms to spawn into the level
    private     int                                         m_endRoomNum            = 2;                        // Minimum number of rooms with only 1 neighbour
    private     int                                         m_level                 = 1;                        // Current level
    private     int                                         m_spacing               = 15;                       // Spacing between spawn points of rooms

    // Lists of positions of things
    private     List<Vector2Int>                            m_spawnerPositions;                                 // List of the current positions of the random walkers
    private     List<GameObject>                            m_doors;                                            // List of positions to place doors
    private     List<Vector2Int>                            m_endRoomList           = new List<Vector2Int>();   // List of rooms that are dead ends

    private     List<Vector2Int>                            m_offsets               = new List<Vector2Int>      // Spaces around room to check
    {
        new Vector2Int( 0, -1 ),
        new Vector2Int( 1, 0 ),
        new Vector2Int( 0, 1 ),
        new Vector2Int( -1, 0 )
    };

    private     List<Vector2Int>                            m_bigOffsest            = new List<Vector2Int> // Spaces around big room to check
    {
        //NESW Connections
        new Vector2Int( 0, -1 ),
        new Vector2Int( 2, 0 ),
        new Vector2Int( 0, 2 ),
        new Vector2Int( -1, 0 ),

        //NESW 2 Connections
        new Vector2Int( 1, -1 ),
        new Vector2Int( 2, 1 ),
        new Vector2Int( 1, 2 ),
        new Vector2Int( -1, 1 ),
    };

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Sets up variables and generates the dungeon
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    GenerateDungeon
    ******************************************************/
    private void Start()
    {
        // Get the level number from the dataholder
        m_level = GameObject.FindGameObjectWithTag("GameController").GetComponent<DataHolder>().GetLevel();

        // Max number of rooms to spawn in the level. Minimum is 10-12 rooms, and increases by 2-3 each level
        m_maxRooms = 8 + m_level * 2 + UnityEngine.Random.Range( 0, 2 );

        // Scale level array depending on level number
        m_levelSizeX = m_levelSizeY = m_maxRooms - 2;
        
        // Get components and initialise variables
        m_surface = GetComponent<NavMeshSurface>();
        m_roomArrays = gameObject.GetComponent<RoomArrays>();
        m_numRooms = m_maxRooms;
        m_dungeonArray = new (RoomType, List<Directions>, int)[ m_levelSizeX, m_levelSizeY];
        m_spawnArray = new List<GameObject>( m_numRooms );
        m_doors = new List<GameObject>();        

        // Stops an unfortunate shape from generating, we don't want controversy like the first Zelda's had with that one dungeon
        if( m_maxRooms == 13 )
        {
            m_maxRooms = 14;
        }


        // While the number of end rooms in the level is less than the minimum.This ensures a valid dungeon size
        while ( m_endRoomList.Count < m_endRoomNum )
        {
            // Reset dungeon array variable
            m_dungeonArray = new (RoomType, List<Directions>, int)[ m_levelSizeX, m_levelSizeY ];
            for ( int i = 0; i < m_dungeonArray.GetLength( 0 ); i++ )
            {
                for ( int j = 0; j < m_dungeonArray.GetLength( 1 ); j++ )
                {
                    m_dungeonArray[ j, i ] = (RoomType.Empty, new List<Directions>(), 0);
                }
            }

            // Reset num rooms variable
            m_numRooms = m_maxRooms;

            // Reset end room list
            m_endRoomList = new List<Vector2Int>();

            GenerateDungeon();
        }

        // Spawns level after a valid dungeon has been generated
        SpawnLevel();        
    }

    /***************************************************
    *   Function        :    GenerateDungeon
    *   Purpose         :    Random walks in 2+ directions and generates a level
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    04/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SpawnLevel
    ******************************************************/
    private void GenerateDungeon()
    {
        // Diffuculty of each room starts at the level number or the min difficulty, whichever is higher
        int roomDifficulty = m_level <= m_minLevelDifficulty ? m_minLevelDifficulty : m_level;

        // List of rooms in dungeon
        List<Vector2Int> positions = new List<Vector2Int>();

        // Counter to stop infinte loading
        int whileCount = 0;

        // Determines how many random walk points to have, between 2 and 4
        int numSpawnPos = UnityEngine.Random.Range( 2, 5 );

        // Sets initial start room
        m_dungeonArray[ m_levelSizeX / 2, m_levelSizeY / 2 ].Item1 = RoomType.Start;
        m_numRooms--;

        // List of the randomly walking spawners
        m_spawnerPositions = new List<Vector2Int>( numSpawnPos );

        for ( int i = 0; i < numSpawnPos; i++ )
        {
            // Adds random walker positions to list, starting at the center of the level
            m_spawnerPositions.Add(new Vector2Int( m_levelSizeX / 2, m_levelSizeY / 2 ));
        }

        // While there are still rooms to add
        while ( m_numRooms > 0 )
        {
            // Increase while count
            whileCount++;

            // For each random walker in the list
            for ( int i = 0; i < m_spawnerPositions.Count; i++ )
            {
                // Move random walker and set to new temp variable
                Vector2Int temp = GetRandDirection( m_spawnerPositions[ i ] );

                // If temp and current random walker are the same positon, it is out of bounds
                if ( m_spawnerPositions[ i ] == temp )
                {
                    // Remove random walker
                    m_spawnerPositions.RemoveAt( i );
                }

                else
                {
                    // Apply new position to current random walker
                    m_spawnerPositions[ i ] = temp;


                    // If the position walked to is empty and only has 1 neighbour
                    if ( m_dungeonArray[ m_spawnerPositions[ i ].y, m_spawnerPositions[ i ].x ].Item1 == RoomType.Empty &&
                        NeighbourCheck( m_spawnerPositions[ i ] ) )
                    {
                        // Positions of 3 quadrants of big room if one can exist at this position
                        Vector2Int[] bigRoomPos = NeighbourCheckBigRoom( m_spawnerPositions[ i ] );

                        // If 1 in 2 chance to generate big room and there are positions for the extra quadrants
                        if ( UnityEngine.Random.Range( 0, 2 ) == 0 && bigRoomPos.GetLength( 0 ) != 0 && m_numRooms > 3 )
                        {
                            // Set current position in dungeonArray to a big room and set difficulty 
                            m_dungeonArray[ m_spawnerPositions[ i ].y, m_spawnerPositions[ i ].x ].Item1 = RoomType.Big;
                            m_dungeonArray[ m_spawnerPositions[ i ].y, m_spawnerPositions[ i ].x ].Item3 = roomDifficulty + 2;
                            positions.Add( m_spawnerPositions[ i ] );
                            
                            foreach ( var pos in bigRoomPos )
                            {
                                // Set extra quadrants to big room too
                                m_dungeonArray[ pos.y, pos.x ].Item1 = RoomType.Big;
                                m_dungeonArray[ pos.y, pos.x ].Item3 = roomDifficulty + 2;
                                positions.Add( pos );
                            }

                            // Reduce count of rooms left to spawn
                            m_numRooms -= 2;

                            // Moves the spawner to a random position within the big room
                            m_spawnerPositions[ i ] = bigRoomPos[ UnityEngine.Random.Range( 0, 3 ) ];
                        }
                        else
                        {
                            // Set current position to a small room and reduce rooms to spawn count
                            m_dungeonArray[ m_spawnerPositions[ i ].y, m_spawnerPositions[ i ].x ].Item1 = RoomType.Small;
                            m_dungeonArray[ m_spawnerPositions[ i ].y, m_spawnerPositions[ i ].x ].Item3 = roomDifficulty;
                            positions.Add( m_spawnerPositions[ i ] );

                            m_numRooms--;
                        }

                    }
                    // If position walked on is not empty or has more than 1 neighbour
                    else
                    {
                        m_spawnerPositions.RemoveAt( i );
                    }
                }

                // If the number of rooms to add is <= 0
                if ( m_numRooms <= 0 )
                {
                    break;
                }

            }

            // Increase room difficulty every layer deeper the dungeon generates
            roomDifficulty++;

            // If the while count is > 199, something is wrong and will break so it doesn't hang forever
            if( whileCount > 199 )
            {
                Debug.LogWarning( "Generation taking too many loops, will break to avoid never ending load. Max rooms: " + m_maxRooms );
                break;
            }
        }

        // Loop through all rooms placed
        foreach (var room in positions )
        {
            // If the room has only 1 neighbour and is a small room
            if( NeighbourCheck( room ) && m_dungeonArray[ room.y, room.x].Item1 == RoomType.Small )
            {
                // Add to list of end rooms
                m_endRoomList.Add( room );
            }
        }
  
    }

    /***************************************************
    *   Function        :    DoorOverlapCheck
    *   Purpose         :    Checks if a door has already been placed
    *                        at the given position
    *   Parameters      :    Vector3 pos
    *   Returns         :    bool doorAlreadyInList
    *   Date altered    :    04/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SpawnDoors
    ******************************************************/
    private int DoorOverlapCheck( Vector3 pos )
    {
        // Index of preexisting door in array
        int check = -1;

        for( int i = 0; i < m_doors.Count; i++ )
        {
            // If a door exists at the given position
            if ( Vector3.Distance( pos, m_doors[ i ].transform.position ) <= 1f )
            {
                // Save the index
                check = i;
                break;
            }
        }

        // Return the index
        return check;
    }

    /***************************************************
    *   Function        :    SpawnDoors
    *   Purpose         :    Spawns doors to connect rooms
    *   Parameters      :    (RoomType, List<Directions>, int) room
    *                        Vector2Int pos
    *                        RoomArrays array
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SpawnLevel
    ******************************************************/
    private List<GameObject> SpawnDoors( (RoomType, List<Directions>, int) room, Vector2Int pos, RoomArrays array )
    {
        // Half the spacing
        float halfWidth = m_spacing * 0.5f;

        // List of doors a room will have
        List<GameObject> doorsForRoom = new List<GameObject>();

        // Position to place door
        Vector3 doorPlacementPos = Vector3.zero;

        // The rotation to apply to the door
        Quaternion rotation = Quaternion.identity;

        // For each connection in the room
        foreach ( Directions way in room.Item2 )
        {
            switch ( way )
            {
                default:
                case Directions.North:
                {
                    // If the room is not big
                    if ( room.Item1 == RoomType.Small || room.Item1 == RoomType.Start || room.Item1 == RoomType.Treasure || room.Item1 == RoomType.End )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing + halfWidth );
                    }
                    // else if the room is big
                    else if ( room.Item1 == RoomType.Big )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing + ( m_spacing / 2 ) );
                    }

                    // Set rotation
                    rotation = Quaternion.Euler( 0, 90, 0 );
                    break;
                }

                case Directions.East:
                {
                    // If the room is not big
                    if ( room.Item1 == RoomType.Small || room.Item1 == RoomType.Start || room.Item1 == RoomType.Treasure || room.Item1 == RoomType.End )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing + halfWidth, 0, -pos.y * m_spacing );
                    }
                    // else if the room is big
                    else if ( room.Item1 == RoomType.Big )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing + m_spacing + ( m_spacing / 2 ), 0, -pos.y * m_spacing );
                    }

                    // Set rotation
                    rotation = Quaternion.Euler( 0, 180, 0 );
                    break;
                }

                case Directions.South:
                {
                    // If the room is not big
                    if ( room.Item1 == RoomType.Small || room.Item1 == RoomType.Start || room.Item1 == RoomType.Treasure || room.Item1 == RoomType.End )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing - halfWidth );
                    }
                    // else if the room is big
                    else if ( room.Item1 == RoomType.Big )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing - m_spacing - ( m_spacing / 2 ) );
                    }

                    // Set rotation
                    rotation = Quaternion.Euler( 0, -90, 0 );
                    break;
                }

                case Directions.West:
                {
                    // If the room is not big
                    if ( room.Item1 == RoomType.Small || room.Item1 == RoomType.Start || room.Item1 == RoomType.Treasure || room.Item1 == RoomType.End )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing - halfWidth, 0, -pos.y * m_spacing );
                    }
                    // else if the room is big
                    else if ( room.Item1 == RoomType.Big )
                    {
                        // Set position
                        doorPlacementPos = new Vector3( pos.x * m_spacing - halfWidth, 0, -pos.y * m_spacing );
                    }

                    // Set rotation
                    rotation = Quaternion.identity;
                    break;
                }

                case Directions.North2:
                {
                    // Set position
                    doorPlacementPos = new Vector3( pos.x * m_spacing + m_spacing, 0, -pos.y * m_spacing + ( m_spacing / 2 ) );

                    // Set rotation
                    rotation = Quaternion.Euler( 0, 90, 0 );
                    break;
                }

                case Directions.East2:
                {
                    // Set position
                    doorPlacementPos = new Vector3( pos.x * m_spacing + m_spacing + ( m_spacing / 2 ), 0, -pos.y * m_spacing - m_spacing );

                    // Set rotation
                    rotation = Quaternion.Euler( 0, 180, 0 );
                    break;
                }

                case Directions.South2:
                {
                    // Set position
                    doorPlacementPos = new Vector3( pos.x * m_spacing + m_spacing, 0, -pos.y * m_spacing - m_spacing - ( m_spacing / 2 ) );

                    // Set rotation
                    rotation = Quaternion.Euler( 0, -90, 0 );
                    break;
                }

                case Directions.West2:
                {
                    // Set position
                    doorPlacementPos = new Vector3( pos.x * m_spacing - halfWidth, 0, -pos.y * m_spacing - m_spacing );

                    // Set rotation
                    rotation = Quaternion.identity;
                    break;
                }
            }

            // The index of the preexisting door at doorPlacementPos
            int doorIndex = DoorOverlapCheck( doorPlacementPos );

            // If there is no preexisting door
            if ( doorIndex == -1 )
            {
                // Spawn door and add to master and loacal lists
                GameObject door = Instantiate( array.m_door, doorPlacementPos, rotation );
                m_doors.Add( door );
                doorsForRoom.Add( door );
            }
            else
            {
                // Add preexisting door to list
                doorsForRoom.Add( m_doors[ doorIndex ] );
            }
        }

        // Return list of doors for the room
        return doorsForRoom;
    }

    /***************************************************
    *   Function        :    SpawnLevel
    *   Purpose         :    Loops through dungeonArray and spawns rooms
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    GenerateDugeon
    ******************************************************/
    private void SpawnLevel()
    {
        // Index of last room in list of end rooms
        int end = m_endRoomList.Count - 1;

        // Set room at end of the array to be an end room type
        m_dungeonArray[ m_endRoomList[ end ].y, m_endRoomList[ end ].x ] = (RoomType.End, GetConnections( m_endRoomList[ end ] ), 0);

        // Set room 1 away from end of the array to be a treasure room
        m_dungeonArray[ m_endRoomList[ end - 1 ].y, m_endRoomList[ end - 1 ].x ] = (RoomType.Treasure, GetConnections( m_endRoomList[ end - 1 ] ), 0);

        // Makes sure to only place doors after a valid room is placed
        bool validRoom = false;

        // Loop through dungeonArray
        for ( int i = 0; i < m_dungeonArray.GetLength( 0 ); i++ )
        {
            for ( int j = 0; j < m_dungeonArray.GetLength( 1 ); j++ )
            {
                // What type of room is the current room
                RoomType room = m_dungeonArray[ j, i ].Item1;

                // Switch on the room type
                switch ( room )
                {
                    case RoomType.Small:
                    {
                        // Get the directions this room has connections
                        m_dungeonArray[ j, i ].Item2 = GetConnections( new Vector2Int( i, j ) );

                        // Spawn small room
                        InteriorChoiseSmall( m_dungeonArray[ j, i ], new Vector2Int( i, j ), m_roomArrays );

                        // Set enemy dictionary for the newly spawned room room
                        m_spawnArray[ ^1 ].GetComponent<SetUpAI>().SetDictionary( SpawnEnemies( m_dungeonArray[ j, i ].Item3 ) );

                        validRoom = true;

                        break;
                    }

                    case RoomType.Big:
                    {
                        // Get the directions this room has connections
                        m_dungeonArray[ j, i ].Item2 = GetConnectionsBig( new Vector2Int( i, j ) );

                        // Set other 3 quadrants to BigExt so rooms don't spawn there
                        m_dungeonArray[ j + 1, i ].Item1 = RoomType.BigExt;
                        m_dungeonArray[ j, i + 1 ].Item1 = RoomType.BigExt;
                        m_dungeonArray[ j + 1, i + 1 ].Item1 = RoomType.BigExt;

                        // Spawn a big room
                        m_spawnArray.Add( Instantiate( m_roomArrays.m_bigRooms[ UnityEngine.Random.Range( 0, m_roomArrays.m_bigRooms.GetLength( 0 ) ) ], new Vector3( i * m_spacing + ( m_spacing / 2 ) + 0.5f , 0, -j * m_spacing - ( m_spacing / 2 ) - 0.5f ), Quaternion.identity ) );

                        // Set enemy dictionary for the newly spawned room room
                        m_spawnArray[ ^1 ].GetComponent<SetUpAI>().SetDictionary( SpawnEnemies( m_dungeonArray[ j, i ].Item3 ) );

                        // Turn off the walls where doors are
                        Transform walls = m_spawnArray[ ^1 ].transform.Find( "Walls" );
                        foreach ( var connection in m_dungeonArray[ j, i ].Item2 )
                        {
                            walls.GetChild( ( int )connection ).gameObject.SetActive( false );
                        }

                        validRoom = true;

                        break;
                    }

                    case RoomType.Start:
                    {
                        // Get the directions this room has connections
                        m_dungeonArray[ j, i ].Item2 = GetConnections( new Vector2Int( i, j ) );

                        // Get a start room to spawn
                        GameObject startRoom = m_roomArrays.m_startRooms[ UnityEngine.Random.Range( 0, m_roomArrays.m_startRooms.GetLength( 0 ) ) ];
                        
                        // If the first level
                        if( m_level == 1 )
                        {
                            // Set the start room to the level 1 start room
                            startRoom = m_roomArrays.m_startRoomLevel1;
                        }

                        // Spawn start room
                        m_spawnArray.Add( Instantiate( startRoom, new Vector3( i * m_spacing, 0, -j * m_spacing ), Quaternion.identity ) );

                        // Turn off the walls where doors are
                        Transform walls = m_spawnArray[ ^1 ].transform.Find( "Walls" );
                        foreach ( var connection in m_dungeonArray[ j, i ].Item2 )
                        {
                            walls.GetChild( ( int )connection ).gameObject.SetActive( false );
                        }

                        validRoom = true;

                        break;
                    }

                    case RoomType.End:
                    case RoomType.Treasure:
                    {
                        // Get the directions this room has connections
                        m_dungeonArray[ j, i ].Item2 = GetConnections( new Vector2Int( i, j ) );

                        // Spawn room
                        InteriorChoiseSmall( m_dungeonArray[ j, i ], new Vector2Int( i, j ), m_roomArrays );

                        // Cull
                        m_spawnArray[ ^1 ].GetComponent<RoomController>().ToggleCull( true );

                        validRoom = true;

                        break;
                    }

                }

                // If a room was spawned
                if ( validRoom )
                {
                    // Spawn doors
                    List<GameObject> doors = SpawnDoors( m_dungeonArray[ j, i ], new Vector2Int( i, j ), m_roomArrays );

                    for ( int k = 0; k < doors.Count; k++ )
                    {
                        // Add reference to the connected room to each door
                        doors[ k ].GetComponent<DoorAnimator>().GetConnectedRooms().Add( m_spawnArray[ ^1 ].GetComponent<RoomController>() );
                    }

                    // Add door references to room
                    m_spawnArray[ ^1 ].GetComponent<RoomController>().SetDoors( doors );

                    validRoom = false;
                }
            }
        }

        // Build nav mesh on fully spawned dungeon
        m_surface.BuildNavMesh();

        // Loop through all spawned rooms
        foreach ( GameObject room in m_spawnArray )
        {
            // If the room has a SetUpAI component
            if ( room.GetComponent<SetUpAI>() != null )
            {
                // Set up the room now they all have navmeshes
                room.GetComponent<SetUpAI>().SetUpRoom();
            }
        }
    }

    /***************************************************
    *   Function        :    InteriorChoiseSmall
    *   Purpose         :    Spawns a correctly connecting start room
    *   Parameters      :    (RoomType, List<Directions>, int) room
    *                        Vector2Int pos
    *                        RoomArrays array
    *   Returns         :    void
    *   Date altered    :    15/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SpawnLevel
    ******************************************************/
    private void InteriorChoiseSmall( (RoomType, List<Directions>, int) room, Vector2Int pos, RoomArrays array )
    {
        // The new room you're spawning
        GameObject newRoom = null;

        //Switch for how many connections the rooms have
        switch ( room.Item2.Count )
        {
            case 1:
            {
                // Spwan an X room if not special
                GameObject roomToSpawn = array.m_XRoom[ 0 ];

                // If it's a treasure room
                if ( room.Item1 == RoomType.Treasure )
                {
                    roomToSpawn = array.m_treasureRooms[ UnityEngine.Random.Range( 0, array.m_treasureRooms.GetLength( 0 ) ) ];
                }

                // If it's an end room
                else if ( room.Item1 == RoomType.End )
                {
                    roomToSpawn = array.m_endRooms[ UnityEngine.Random.Range( 0, array.m_endRooms.GetLength( 0 ) ) ];
                }

                //Spawn roomToSpawn
                newRoom = Instantiate( roomToSpawn, new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * ( int )room.Item2[ 0 ], 0 ) );
                break;
            }

            case 2:
            {
                // I: NS, EW connecting rooms
                if ( room.Item2[ 0 ] == Directions.North && room.Item2[ 1 ] == Directions.South ||
                        room.Item2[ 0 ] == Directions.East && room.Item2[ 1 ] == Directions.West )
                {
                    //Spawn I room interior
                    newRoom = Instantiate( array.m_IRoom[ UnityEngine.Random.Range( 0, array.m_IRoom.GetLength( 0 ) ) ], new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * ( int )room.Item2[ 0 ], 0 ) );

                }
                // L: NW connecting rooms
                else if ( room.Item2[ 0 ] == Directions.North && room.Item2[ 1 ] == Directions.West )
                {
                    //Spawn L room interior
                    newRoom = Instantiate( array.m_LRoom[ UnityEngine.Random.Range( 0, array.m_LRoom.GetLength( 0 ) ) ], new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * ( int )room.Item2[ 1 ], 0 ) );
                }
                // L: NE, ES, SW connecting rooms
                else
                {
                    //Spawn L room interior, using 90 * Enum.GetValues(room.Item2[0]) + 1 as the rotation
                    newRoom = Instantiate( array.m_LRoom[ UnityEngine.Random.Range( 0, array.m_LRoom.GetLength( 0 ) ) ], new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * ( int )room.Item2[ 0 ], 0 ) );
                }
                break;
            }

            case 3:
            {
                // T: NES, ESW, NSW, NEW connecting rooms

                // The amount of other connections the room has
                int otherConnection = 5;

                // Subtract each connection from the otherConnections int to get the Other Connections the room doesn't have. Use this as your rotation multiplier
                foreach ( var connection in room.Item2 )
                {
                    otherConnection -= ( int )connection;
                }

                //Spawn T room using the working test as rotation
                newRoom = Instantiate( array.m_TRoom[ UnityEngine.Random.Range( 0, array.m_TRoom.GetLength( 0 ) ) ], new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * otherConnection, 0 ) );

                break;
            }

            case 4:
            {
                //Spawn X room with any random rotation I guess for variation
                newRoom = Instantiate( array.m_XRoom[ UnityEngine.Random.Range( 0, array.m_XRoom.GetLength( 0 ) ) ], new Vector3( pos.x * m_spacing, 0, -pos.y * m_spacing ), Quaternion.Euler( 0, 90 * UnityEngine.Random.Range( 0, 4 ), 0 ) );

                break;
            }
        }

        // Add the room to the spawn array
        m_spawnArray.Add( newRoom );

        // If the room is not a special room
        if( room.Item1 != RoomType.Treasure && room.Item1 != RoomType.End)
        {
            // Turn off the walls where doors are
            Transform walls = newRoom.transform.Find( "Walls" );
            foreach ( var connection in room.Item2 )
            {
                // index is the ( connection - rotation / 90) % 4
                // minus by rotation factor and then modulo by 4 gives the correct connection while factoring in rotation
                float index = Modulo( ( ( int )connection - ( newRoom.transform.eulerAngles.y ) / 90 ), 4 );

                // Turn off walls
                walls.GetChild( ( int )(  index ) ).gameObject.SetActive( false );
            }
        }


    }

    /***************************************************
    *   Function        :    Modulo
    *   Purpose         :    Does the correct Modulo with negatives
    *   Parameters      :    float a
    *                        float b
    *   Returns         :    float modulo
    *   Date altered    :    01/08/22
    *   Contributors    :    WH
    *   Notes           :    Shoutout to C# for having a differnt modulo to
    *                        Google so I spend a good 6 hours before I realised
    *                        this is what google uses and what I needed.
    *   See also        :    
    ******************************************************/
    private float Modulo( float a, float b )
    {
        return a - b * Mathf.Floor( a / b );
    }

    /***************************************************
    *   Function        :    SpawnEnemies
    *   Purpose         :    Spawns a correctly connecting start room
    *   Parameters      :    int roomSpawnNum
    *   Returns         :    Dictionary<EnemyTypes, int> enemies
    *   Date altered    :    18/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SetUpAI
    ******************************************************/
    private Dictionary<EnemyTypes, int> SpawnEnemies( int roomSpawnNum )
    {
        // Dictionary of enemies and how many to spawn in room
        Dictionary<EnemyTypes, int> enemies = new Dictionary<EnemyTypes, int>
        {
            { EnemyTypes.Brute, 0 },
            { EnemyTypes.Mage, 0 },
            { EnemyTypes.Turret, 0 },
            { EnemyTypes.Bomber, 0 },
            { EnemyTypes.Beacon, 0 },
        };

        // Bool for if a beacon has spawned
        bool beaconSpawned = false;

        // Loop for how many enemies to spawn
        for ( int i = 0; i < roomSpawnNum; i++ )
        {
            // Enemy choice
            int choice = -1;
            
            // If there can be enough enemies for a beacon
            if( roomSpawnNum > 3 && !beaconSpawned)
            {
                choice = UnityEngine.Random.Range( 0, enemies.Count );
            }
            // If a beacon has been added to the room or there aren't enough enemies for a beacon
            else if( beaconSpawned || roomSpawnNum <= 3 )
            {
                choice = UnityEngine.Random.Range( 0, enemies.Count - 1 );
            }

            
            switch ( choice )
            {
                // Choose brute
                case 0:
                {
                    // Add 1 to brute counter in dictionary
                    enemies[ EnemyTypes.Brute ] += 1;
                    
                    // Reduce spawn number by 1 extra as brutes are tougher enemies
                    roomSpawnNum--;
                    break;
                }

                // Choose mage
                case 1:
                default:
                {
                    // Add 1 to mage counter in dictionary
                    enemies[ EnemyTypes.Mage ] += 1;
                    break;
                }

                // Choose turret
                case 2:
                {
                    // Add 1 to turret counter in dictionary
                    enemies[ EnemyTypes.Turret ] += 1;
                    break;
                }

                case 3:
                {
                    // Add 1 to Bomber counter in dictionary
                    enemies[ EnemyTypes.Bomber ] += 1;
                    break;
                }

                case 4:
                {
                    // Add 1 to Bomber counter in dictionary
                    enemies[ EnemyTypes.Beacon ] += 1;

                    // The beacon has spawned so no more added to a room
                    beaconSpawned = true;
                    break;
                }
            }
        }

        // Return dictionary to pass to SetUpAI
        return enemies;
    }

    /***************************************************
    *   Function        :    NeighbourCheck
    *   Purpose         :    Checks caridnal directions for other existing rooms
    *   Parameters      :    Vector2Int pos
    *   Returns         :    bool
    *   Date altered    :    02/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    GenerateDugeon
    ******************************************************/
    private bool NeighbourCheck( Vector2Int pos )
    {
        // Number of neigbours
        int count = 0;

        // For each cardinal direction
        for ( int i = 0; i < m_offsets.Count; i++ )
        {
            // Check is the postion next to the current pos
            Vector2Int check = pos + m_offsets[ i ];

            // if the check position is within the bounds of the dungeon and the position is empty
            if( ( check.x < m_levelSizeX && check.x >= 0 && check.y < m_levelSizeY && check.y >= 0 ) &&
                  m_dungeonArray[ check.y, check.x ].Item1 != RoomType.Empty )
            {
                count++;
            }
        }

        // return if count == 1
        return (count == 1);
    }

    /***************************************************
    *   Function        :    GetConnectionsBig
    *   Purpose         :    Gets the connections to other rooms from the current one
    *   Parameters      :    Vector2Int pos
    *   Returns         :    List<Directions> connections
    *   Date altered    :    19/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private List<Directions> GetConnectionsBig( Vector2Int pos )
    {
        List<Directions> directions = new List<Directions>();

        // for each cardinal direction in the big rooms
        for ( int i = 0; i < m_bigOffsest.Count; i++ )
        {
            // Check is the postion next to the current pos
            Vector2Int check = pos + m_bigOffsest[ i ];

            // if the check position is within the bounds of the dungeon and the position is empty
            if ( ( check.x < m_levelSizeX && check.x >= 0 && check.y < m_levelSizeY && check.y >= 0 ) &&
                  m_dungeonArray[ check.y, check.x ].Item1 != RoomType.Empty )
            {
                // Add the direction to the list
                directions.Add( ( Directions )(i) );
            }
        }

        return directions;
    }

    /***************************************************
    *   Function        :    GetConnections
    *   Purpose         :    Gets the connections to other rooms from the current one
    *   Parameters      :    Vector2Int pos
    *   Returns         :    void
    *   Date altered    :    12/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private List<Directions> GetConnections( Vector2Int pos )
    {
        List<Directions> directions = new List<Directions>();

        // for each cardinal direction in the small rooms
        for ( int i = 0; i < m_offsets.Count; i++ )
        {
            // Check is the postion next to the current pos
            Vector2Int check = pos + m_offsets[ i ];

            // if the check position is within the bounds of the dungeon and the position is empty
            if ( ( check.x < m_levelSizeX && check.x >= 0 && check.y < m_levelSizeY && check.y >= 0 ) &&
                  m_dungeonArray[ check.y, check.x ].Item1 != RoomType.Empty )
            {
                // Add the direction to the list
                directions.Add( ( Directions )i );
            }
        }

        return directions;
    }


    /***************************************************
    *   Function        :    NeighbourCheckBigRoom
    *   Purpose         :    Checks caridnal directions for other existing rooms and also the diagonals
    *   Parameters      :    Vector2Int pos
    *   Returns         :    Vector2Int[]
    *   Date altered    :    02/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    GenerateDugeon
    ******************************************************/
    private Vector2Int[] NeighbourCheckBigRoom( Vector2Int pos )
    {
        // Arrays of the neighbouring 3 quadrants from the position to make a big room
        Vector2Int[] topRight = new Vector2Int[]
        {
            new Vector2Int( 0, -1 ),
            new Vector2Int( 1, 0 ),
            new Vector2Int( 1, -1 ),
        };

        Vector2Int[] bottomRight = new Vector2Int[]
        {
            new Vector2Int( 0, 1 ),
            new Vector2Int( 1, 0 ),
            new Vector2Int( 1, 1 ),
        };

        Vector2Int[] bottomLeft = new Vector2Int[]
        {
            new Vector2Int( 0, 1 ),
            new Vector2Int( -1, 0 ),
            new Vector2Int( -1, 1 ),
        };

        Vector2Int[] topLeft = new Vector2Int[]
        {
            new Vector2Int( 0, -1 ),
            new Vector2Int( -1, 0 ),
            new Vector2Int( -1, -1 ),
        };

        // array of arrays of potential positions of big rooms
        Vector2Int[][] corners = new Vector2Int[][] { topLeft, bottomLeft, topRight, bottomRight };

        // For each potential way to spawn a big room
        for ( int i = 0; i < corners.GetLength(0); i++ )
        {
            // Get the other 3 quadrants to check
            Vector2Int check1 = pos + corners[ i ][ 0 ];
            Vector2Int check2 = pos + corners[ i ][ 1 ];
            Vector2Int check3 = pos + corners[ i ][ 2 ];

            // If the two cardinal quadrants are within the bounds of the array
            if ( ( check1.x < m_levelSizeX && check1.x >= 0 && check1.y < m_levelSizeY && check1.y >= 0 ) &&
                 ( check2.x < m_levelSizeX && check2.x >= 0 && check2.y < m_levelSizeY && check2.y >= 0 ))
            {
                // if all the quadrants are empty cells
                if( m_dungeonArray[ check1.y, check1.x ].Item1 == RoomType.Empty &&
                    m_dungeonArray[ check2.y, check2.x ].Item1 == RoomType.Empty &&
                    m_dungeonArray[ check3.y, check3.x ].Item1 == RoomType.Empty )
                {
                    // return the positions of the quadrants
                    return new Vector2Int[] { check1, check2, check3 };
                }
            }

        }

        // return an empty array if no big room is viable
        return new Vector2Int[] { };
    }

    /***************************************************
    *   Function        :    GetRandDirection
    *   Purpose         :    Returns a position 1 unit away in a cardinal direction
    *   Parameters      :    Vector2Int pos
    *   Returns         :    Vector2Int
    *   Date altered    :    05/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    GenerateDugeon
    ******************************************************/
    private Vector2Int GetRandDirection( Vector2Int pos )
    {
        // Offset to add to get new position
        List<Vector2Int> offsetsToCheck = new List<Vector2Int>();
        offsetsToCheck.AddRange( m_offsets );

        // Choice of direction
        int choice = UnityEngine.Random.Range( 0, 4 );

        // New position
        Vector2Int check = pos + offsetsToCheck[ choice ];
        
        // if the new position is within the array
        if ( check.x < m_levelSizeX && check.x >= 0 && check.y < m_levelSizeY && check.y >= 0 &&
            m_dungeonArray[ check.y, check.x ].Item1 == RoomType.Empty &&
            NeighbourCheck( check ) )
        {
            return check;
        }
        else
        {
            // Remove this offset
            offsetsToCheck.RemoveAt( choice );
            // Recusrivly try again
            return GetRandDirection(pos, offsetsToCheck );
        }

    }

    /***************************************************
   *   Function        :    GetRandDirection
   *   Purpose         :    Returns a position 1 unit away in a cardinal direction
   *   Parameters      :    Vector2Int pos
   *                        List<Vector2Int> offsetsToCheck
   *   Returns         :    Vector2Int
   *   Date altered    :    05/04/22
   *   Contributors    :    WH
   *   Notes           :    Overload using a given offsetsToCheck list
   *   See also        :    GenerateDugeon
   ******************************************************/
    private Vector2Int GetRandDirection( Vector2Int pos, List<Vector2Int> offsetsToCheck )
    {
        // Choice of direction
        int choice = UnityEngine.Random.Range( 0, offsetsToCheck.Count );

        // New position
        Vector2Int check = pos + offsetsToCheck[ choice ];

        // if the new position is within the array
        if ( check.x < m_levelSizeX && check.x >= 0 && check.y < m_levelSizeY && check.y >= 0 &&
            m_dungeonArray[ check.y, check.x ].Item1 == RoomType.Empty &&
            NeighbourCheck( check ) )
        {
            return check;
        }

        // if there are more offsets to check
        else if( offsetsToCheck.Count - 1 > 0 )
        {
            // Remove this offset
            offsetsToCheck.RemoveAt( choice );
            // Recusrivly try again
            return GetRandDirection( pos, offsetsToCheck );
        }

        else
        {
            // Return the original position
            return pos;
        }

    }
}