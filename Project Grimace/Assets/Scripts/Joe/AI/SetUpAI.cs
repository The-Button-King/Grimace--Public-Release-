using EnemyEnums;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
/****************************************************************************************************
* Type: Class
* 
* Name: SetUpAI
*
* Author: Joseph Gilmore
*
* Purpose: Spawn AI at randomized points within a room & set up random patrol points
* 
* Functions:         
*                    void Awake()
*                    private void TurretPoints()
*                    public void SetUpRoom()
*                    private Vector3 SetSpawnPoint()
*                    public List<GameObject> SpawnAI()
*                    private void SetPaths()
*                    private Transform SetTurretSpawn()
*                    private IEnumerator DelayAgent( GameObject tempAI )
*                    private void CreateGridOfPoints()
*                    private List<Vector3> CreatePatrolPaths( Vector3 startPos )
*                    public List<GameObject> GetEnemiesInRoom()
*                    public void SetEnemiesInRoom( List<GameObject> enemies )
*                    public void SetDictionary( Dictionary<EnemyTypes, int> ListOfEnenemies )
*                    
*                   
* 
* References:
* 
* See Also:
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 12/04/2022     JG         1.00        - Created new AI spawning 
* 14/04/2022     JG         1.01        - Altered class so it can be called from elsewhere & fixed agent bugs not getting navmesh
* 18/04/2022     JG         1.10        - Completely changed the structure of the class to use tuples and recieve information from controller
* 19/04/2022     JG         1.11        - Added turret spawning 
* 27/04/2022     JG         1.12        - Altered code to fix points spawning on te navmesh
* 11/05/2022     JG         1.13        - Functioned Turret points 
* 31/05/2022     JG         2.00        - Added getter and altered constructor 
* 04/07/2022     JG         2.01        - Added Beacon
* 01/08/2022     JG         2.02        - pooling turret bug 
* 02/08/2022     JG         2.03        - Adjusted for AI structure 
* 15/08/2022     JG         2.04        - Cleaning 
****************************************************************************************************/
public class SetUpAI : MonoBehaviour
{
    private Collider                    m_collider;                                                              // Reference  to room collider to get bounds               
    private Vector3                     m_leftPoint;                                                             // Top left point of each room
    [SerializeField]
    [Range(1.0F, 10.0F)]
    [Tooltip("The gap between spawn/patrol points. Don't increase too high on small rooms")]
    private float                       m_distanceBewteenPoints;                                                // Gap between grid points
    private bool                        m_gridCreated = false;                                                  // Has the grid been generated yet         
    private List<(Vector3, bool)>       m_gridPoints = new List<(Vector3, bool)>();                             // Turple of points on a grid that can be used as patrol/spawn points. If they been used they are true     
    private List<GameObject>            m_enemiesInRoom = new List<GameObject>();                               // List of AI to spawn
    [Header("AI prefab types")]
    public GameObject                   m_mageAI;                                                               // Rerefence to mageAI prefab
    public GameObject                   m_bruteAI;                                                              // Rerefence to bruteAI prefab
    public GameObject                   m_turretAI;                                                             // Reference to turret A
    public GameObject                   m_bomberAI;                                                             // Reference to bomber   
    public GameObject                   m_beacon;                                                               // Refere to beacon 
    private int                         m_numberOfPointsInPath;                                                 // The amount of waypoints in a path 
    private float                       m_offset;                                                               // Offset of AI so it spawns correctly on the Y axis 
    private float                       m_setUpDelay = 0.1f;                                                    // Used to delay AI setup
    Dictionary<EnemyTypes, int>         m_dictionaryOfEnenemies = new Dictionary<EnemyTypes, int>();            // Dictionary of the different enemyTypes that need to spawn 
    private List<(Transform, bool)>     m_wallPoints = new List<(Transform, bool)>();                           // Transforms for the turrets to spawn on the wall 
    private AssetPool                   m_enemiePool;                                                           // Reference to asset pool   
    private Vector2Int                  m_pathRange = new Vector2Int( 2, 7 );                                   // Range of points in patrol path
    /***************************************************
    *   Function        : Awake 
    *   Purpose         : Set Up class ready for spawning    
    *   Parameters      : N/A 
    *   Returns         : void  
    *   Date altered    : 11/05/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : SetUpRoom 
    ******************************************************/
    void Awake()
    {
        // Get reference to asset pool
        m_enemiePool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();

        // Get refernce to collider on room
        m_collider = this.GetComponent<Collider>();

        // Calculate the most top left point in room
        m_leftPoint = new Vector3(m_collider.bounds.min.x + m_distanceBewteenPoints, transform.position.y, m_collider.bounds.max.z - m_distanceBewteenPoints);

        // Setup Turret Spawn points 
        TurretPoints();

  
            
    }
    /***************************************************
    *   Function        : TurretPoints   
    *   Purpose         : Used to get & assign Turret spawn points    
    *   Parameters      : N/A   
    *   Returns         : Void    
    *   Date altered    : 11/05/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void TurretPoints()
    {
        // Get preset Turret Spawn points 
        Transform[] points = transform.Find( "WallPoints" ).GetComponentsInChildren<Transform>();

        // Loop through each transform & add to tuple 
        foreach (Transform transform in points)
        {
            // Don't add the first element as its the parents
            if (transform != points[0])
            {
                ( Transform, bool ) tempTuple = ( transform, false );
                m_wallPoints.Add( tempTuple );
            }
        }
    }
    /***************************************************
    *   Function        : SetUpRoom
    *   Purpose         : Used to spawn AI and trigger the creation of paths   
    *   Parameters      : N/A 
    *   Returns         : Void  
    *   Date altered    : 26/04/2022
    *   Contributors    : JG
    *   Notes           : Public so the ArreyGen script can spawn AI at the correct moment 
    *   See also        : ArreyGen.cs , RoomControler.cs
    ******************************************************/
    public void SetUpRoom()
    {

        // Get refernce to collider on room
        m_collider = gameObject.GetComponent<Collider>();

        // Create a grid of points within the room that can be used as spawn/patrol points
        CreateGridOfPoints();

       
    }
    /***************************************************
   *   Function        : SetSpawnPoint
   *   Purpose         : Used to set the spawn point of an AI 
   *   Parameters      : N/A
   *   Returns         : Vector3 pos
   *   Date altered    : 18/04/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        : SpawnAI
   ******************************************************/
    private Vector3 SetSpawnPoint()
    {
        // Create a temp spawn pos
        Vector3 spawnPosition = Vector3.zero;

        while (true)
        {
            // Get a random index using the amount of points in the grid
            int index = Random.Range( 0, m_gridPoints.Count );
           
            // If that point has not been used yet
            if ( m_gridPoints[index].Item2 == false )
            {
                // Get a random point on generated grid 
                spawnPosition = m_gridPoints[index].Item1;

                // Update the list by creating a temparay tuple and then setting it to the index we are accessing 
                ( Vector3 , bool )  tempTuple= ( spawnPosition, true );

                m_gridPoints[index] = tempTuple;
                break;
            }

        }
        // Return spawn point
            return spawnPosition;
     }
    /***************************************************
      *   Function        : SpawnAI
      *   Purpose         : Spawn AI at a random grid position    
      *   Parameters      : N/A 
      *   Returns         : List<GameObject> m_enemiesInRoom;
      *   Date altered    : 01/08/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        : SetUpRoom 
      ******************************************************/
    public List<GameObject> SpawnAI()
   {
        // Go through the different enemies needed to spawn in this room 
        foreach (var item in m_dictionaryOfEnenemies)
        {
            switch (item.Key)
            {
                case EnemyTypes.Mage:
                    // Create the amount of mages stored in the dictionary
                    for (int i = 0; i< item.Value; i++)
                    {
                       // Create an instence of the correct AI type
                       GameObject tempMageAI = m_enemiePool.GetObjectFromPool( m_mageAI );

                       // Delay enabling behaviour tree & agent.(this is to fix errors of call order).
                       StartCoroutine(DelayAgent( tempMageAI ));

                      // Add to the list of enemies In the room
                      m_enemiesInRoom.Add( tempMageAI );

                      // Get the offset of the navmeshagent for that prefab
                       m_offset = m_mageAI.GetComponent<NavMeshAgent>().baseOffset;

                      // Get a spawn position
                       Vector3 spawnPos = SetSpawnPoint();

                       // Apply agent offset to spawn point and then warp the AI.(if you set the position instead of warping it can lead to the agent not detecting the nav mesh surface 
                       tempMageAI.GetComponent<NavMeshAgent>().Warp( new Vector3( spawnPos.x, spawnPos.y + m_offset, spawnPos.z ));
                    }
                    break;
                case EnemyTypes.Brute:
                    for ( int i = 0; i < item.Value; i++ )
                    {

                         // Create AI 
                         GameObject tempBruteAI = m_enemiePool.GetObjectFromPool(m_bruteAI);

                        // Delay enabling behaviour tree & agent.(this is to fix errors of call order).
                        StartCoroutine( DelayAgent( tempBruteAI ) );
                        m_enemiesInRoom.Add( tempBruteAI );

                        m_offset = m_bruteAI.GetComponent<NavMeshAgent>().baseOffset;

                        // Get a spawn position
                        Vector3 spawn = SetSpawnPoint();

                        // Apply agent offset to spawn point and then warp the AI.(if you set the position instead of warping it can lead to the agent not detecting the nav mesh surface 
                        tempBruteAI.GetComponent<NavMeshAgent>().Warp(new Vector3(spawn.x, spawn.y + m_offset, spawn.z));
                    }
                    break;
                case EnemyTypes.Bomber:
                    for (int i = 0; i < item.Value; i++)
                    {

                        // Create AI 
                        GameObject tempBomberAI = m_enemiePool.GetObjectFromPool(m_bomberAI);

                        // Delay enabling behaviour tree & agent.(this is to fix errors of call order).
                        StartCoroutine( DelayAgent( tempBomberAI ) );
                        m_enemiesInRoom.Add( tempBomberAI );

                        m_offset = m_bruteAI.GetComponent<NavMeshAgent>().baseOffset;

                        // Get a spawn position
                        Vector3 spawn = SetSpawnPoint();

                        // Apply agent offset to spawn point and then warp the AI.(if you set the position instead of warping it can lead to the agent not detecting the nav mesh surface 
                        tempBomberAI.GetComponent<NavMeshAgent>().Warp(new Vector3( spawn.x, spawn.y + m_offset, spawn.z ));
                    }
                    break;
                case EnemyTypes.Turret:
                    for (int i = 0; i < item.Value; i++)
                    {

                         // Create AI
                         GameObject tempTurretAI = m_enemiePool.GetObjectFromPool( m_turretAI );
                        
                        // Turret now in room
                        m_enemiesInRoom.Add( tempTurretAI );

                        //Get Turret spawn
                        Transform turretTransform = SetTurretSpawn();

                        // Enable Turret 
                        tempTurretAI.GetComponentInChildren<TurretAI>( true ).enabled = true;

                        // Set position 
                        tempTurretAI.transform.position = turretTransform.position;
                        tempTurretAI.transform.rotation = turretTransform.rotation;
                    }
                    break;
                case EnemyTypes.Beacon:
                    for (int i = 0; i < item.Value; i++)
                    {

                        // Create AI 
                        GameObject tempBeacon = m_enemiePool.GetObjectFromPool( m_beacon );

                       
                        
                        m_enemiesInRoom.Add( tempBeacon );

                        m_offset = m_bruteAI.GetComponent<NavMeshAgent>().baseOffset;

                        // Set spawn Pos
                        tempBeacon.transform.position = SetSpawnPoint();

                        
                    }
                    break;

            }
        }
        // Set paths now eneimes are spawned 
        SetPaths();
        return m_enemiesInRoom;
    }
    /***************************************************
      *   Function        : SetPath
      *   Purpose         : Spawn AI at a random grid position    
      *   Parameters      : N/A 
      *   Returns         : Void  
      *   Date altered    : 03/08/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        : SetUpRoom 
      ******************************************************/
    private void SetPaths()
    {
        // Loop through each AI and set their paths
        foreach (GameObject go in m_enemiesInRoom)
        {
            // If Item has a agent (not a turret or Beacon).
            if ( go.GetComponent<NavMeshAgent>() != null )
            {
                go.GetComponent<AgentAI>().SetPath(CreatePatrolPaths( go.transform.position ) );

            }
        }
    }
    private Transform SetTurretSpawn()
    {
        // Create a temp spawn pos
        Transform spawnTransform;

        while (true)
        {
            // Get a random index using the amount of points in the grid
            int index = Random.Range(0, m_wallPoints.Count);

            // If that point has not been used yet
            if (m_wallPoints[index].Item2 == false)
            {
                // Get a random point on generated grid 
                spawnTransform= m_wallPoints[index].Item1;

                // Update the list by creating a temparay tuple and then setting it to the index we are accessing 
                (Transform, bool) tempTuple = (spawnTransform, true);

                m_wallPoints[index] = tempTuple;
                break;
            }

        }
        // Return spawn point
        return spawnTransform;
        
    }
    /***************************************************
    *   IEnumerator     : DelayAgent
    *   Purpose         : Used to prevent AI not finding navmesh and starting its behaviour before fully set up 
    *   Parameters      : GameObject tempAI
    *   Returns         : Void  
    *   Date altered    : 14/04/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : SpawnAI
    ******************************************************/
    private IEnumerator DelayAgent( GameObject tempAI )
    {
        // Prevent warings of "Failed to create agent because it is not close enough to the NavMesh". 
        yield return new WaitForSeconds(m_setUpDelay);
        tempAI.GetComponent<NavMeshAgent>().enabled = true;
        tempAI.GetComponent<BehaviourTreeOwner>().enabled = true;
    }
    /***************************************************
   *   Function        : CreateGridOfPoints
   *   Purpose         : Used to create a grid of points the AI can use for paths/spawn points
   *   Parameters      : GameObject tempAI
   *   Returns         : void  
   *   Date altered    : 18/04/2022
   *   Contributors    : JG
   *   Notes           :    
   *   See also        : SpawnAI
   ******************************************************/
    private void CreateGridOfPoints()
    {
        // Sets the last point to the top left point
        Vector3 lastPoint = m_leftPoint;

        // Create a hit for the navmesh
        NavMeshHit hit;

        // Add top left point to grid
        ( Vector3, bool ) tempTuple = ( m_leftPoint, false);
        m_gridPoints.Add( tempTuple );

        while ( m_gridCreated == false )
        {
            // Create a temp point by adding the gap from the last point 
            Vector3 tempPoint = new Vector3( lastPoint.x + m_distanceBewteenPoints, lastPoint.y, lastPoint.z );

            // Check if its in the bounds of the room - the gap 
            if (tempPoint.x < m_collider.bounds.max.x - m_distanceBewteenPoints)
            {
                bool temp = NavMesh.SamplePosition( tempPoint, out hit, 1f, NavMesh.AllAreas );
                
                // Check if its on the nav mesh floor
                if (NavMesh.SamplePosition(tempPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    
                    // Create a temp tuple of the vector on the navmesh
                    tempTuple = (hit.position, false);

                    // Add point to list as its valid 
                    m_gridPoints.Add(tempTuple);

                    // Set last point to point just made 
                    lastPoint = tempPoint;

                }
                else
                {
                    lastPoint.x += m_distanceBewteenPoints;
                }
            }
            else
            {
                // If not in x bounds new row needed 
                tempPoint = new Vector3(m_leftPoint.x, lastPoint.y, lastPoint.z - m_distanceBewteenPoints);

                // If not in z bounds grid complete 
                if (tempPoint.z < m_collider.bounds.min.z + m_distanceBewteenPoints )
                {
                    m_gridCreated = true;
                    break;
                }
                bool temp = NavMesh.SamplePosition(tempPoint, out hit, 1f, NavMesh.AllAreas);

                // If new point on mesh
                if (NavMesh.SamplePosition(tempPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    // Add point to grid 
                    tempTuple = (hit.position, false);
                    m_gridPoints.Add(tempTuple);

                    // Set new point to the last point 
                    lastPoint = tempPoint;

                }
                else
                {
                    // If not add to the x 
                    lastPoint = tempPoint;
                    lastPoint.x += m_distanceBewteenPoints;
                     
                }
               
            }


        }

    }
    /***************************************************
    *   Function        : CreatePatrolPaths
    *   Purpose         : Used to create a path for the AI to patrol on using the grid 
    *   Parameters      : Vector3 startPos
    *   Returns         : List<Vector3> path
    *   Date altered    : 18/04/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : CreateGridOfPoints
    ******************************************************/
    private List<Vector3> CreatePatrolPaths( Vector3 startPos )
    {
        int loopCount = 0;

        // Generate the amount of points for this path 
        m_numberOfPointsInPath = Random.Range(m_pathRange.x, m_pathRange.y); 

        //  Create a list to store path 
        List<Vector3> path = new List<Vector3>();

        // Add first point to the path 
        path.Add( startPos );

        // Populate path 
        while (path.Count != m_numberOfPointsInPath)
        {
            // Used to prevent stackoverflow 
            loopCount++;

            int index = Random.Range(0, m_gridPoints.Count);

            // Get a random point on the grid 
            Vector3 waypoint = m_gridPoints[index].Item1;

            // Check if point has been assigned 
            if (m_gridPoints[index].Item2 == false)
            {
                // If not add point to path 
                path.Add(waypoint);

                // Set point to being used
               (Vector3, bool)  tempTuple = (waypoint,true );
                m_gridPoints.Add(tempTuple);
               
            }
            if (loopCount > m_gridPoints.Count)
            {
                Debug.Log("ran out of points");
                break;
            }
        }
        return path;

    }
    /***************************************************
     *   Function        : GetEnemiesInRoom   
     *   Purpose         : Return the enemies inside the room   
     *   Parameters      : N/A   
     *   Returns         : List<GameObject> m_enemiesInRoom   
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public List<GameObject> GetEnemiesInRoom()
    {
        return m_enemiesInRoom;
    }
    /***************************************************
     *   Function        : SetEnemiesInRoom   
     *   Purpose         : Set the enemies inside the room   
     *   Parameters      : List<GameObject> enemies
     *   Returns         : Void 
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : GetEnemiesInRoom
     ******************************************************/
    public void SetEnemiesInRoom( List<GameObject> enemies )
    {
        m_enemiesInRoom = enemies;
    }
    /***************************************************
     *   Function        : SetDictionary 
     *   Purpose         : Set the enemies that are meant to spawn in te room
     *   Parameters      : Dictionary<EnemyTypes, int> ListOfEnenemies
     *   Returns         : Void 
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : RoomControler
     ******************************************************/
    public void SetDictionary( Dictionary<EnemyTypes, int> ListOfEnenemies )
    {
        m_dictionaryOfEnenemies = ListOfEnenemies;
    }
}
