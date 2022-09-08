using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackGroundMusic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: RoomController
 *
 * Author: Will Harding
 *
 * Purpose: Spwans enemies in the room and spawns rewards on room clear
 * 
 * Functions:   public void SetDoors( List<GameObject> doors )
 *              private void Start()
 *              private void Update()
 *              private void ClearCheck()
 *              public void SpawnEnemies()
 *              private void EnemyCheck()
 *              private void RoomClear()
 *              private IEnumerator CloseDoors( bool lockDoors = false, bool cull = false )
 *              private void OpenDoors()
 *              private void EnterRoom()
 *              private void OnTriggerEnter( Collider other )
 *              private void OnTriggerExit( Collider other )
 *              private void SpecialRoomUnlock()
 *              public void ToggleCull( bool cull )
 *              private IEnumerator DelayCull( float time = 5f )
 *              public void StartDelayCull( float time = 5f )
 *              public void StopDelayCull()
 * 
 * References:
 * 
 * See Also:    
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 12/04/22     WH          1.0         Created
 * 18/04/22     WH          1.1         Enemies now spawn in rooms
 * 24/04/22     WH          1.2         Checks for killed enemies and spawns pickups and controls doors
 * 26/04/22     WH          1.3         Bugfixing
 * 12/05/22     WH          1.4         Final comment run through before submission
 * 
 * 11/06/22     WH          2.0         Addng new stuff to control new doors
 * 14/06/22     WH          2.1         Doors now work properly in special rooms
 * 01/08/22     JG          2.2         Spawn AI bug fix 
 * 03/08/22     JG          2.3         Music on  door  open & room clearences 
 * 04/08/22     WH          2.4         Fixed culling
 * 04/08/22     WH          2.5         Pooling for pickups
 * 05/08/22     WH          2.6         Added roomclear and kill counting        
 * 14/08/22     WH          2.7         Added door dash & fixed culling maybe
 * 16/08/22     WH          2.8         Fixed the courotine stopping for culling
 * 17/08/22     WH          2.9         Cleaning
 ****************************************************************************************************/
public class RoomController : MonoBehaviour
{
    [SerializeField]
    [Tooltip( "Pool of items to spawn on room completion" )]
    private GameObject[]        m_itemPool;                                     // Available pool of items to spawn

    [SerializeField]
    [Tooltip( "Is the room not a normal room?" )]
    private bool                m_specialRoom       = false;                    // Is the room a special room?

    [SerializeField]
    [Tooltip( "Has the player entered the room? (Alter for start rooms only)" )]
    private bool                m_roomEntered       = false;                    // Has the room been entered?

    [SerializeField]
    [Tooltip( "Has the room been cleared of enemies? (Alter for special rooms only)" )]
    private bool                m_cleared           = false;                    // Have all the enemeis been killed?

    private List<GameObject>    m_enemyList         = new List<GameObject>();   // List of enemies in the room
    private List<GameObject>    m_doors;                                        // The list of doors that are in the room

    private bool                m_enemiesSpawned    = false;                    // Have the enemies spawned in?
    private bool                m_inRoom;                                       // Is the player in the room?

    private Transform[]         m_children;                                     // All of the child gameobjects attached to the object
    private bool                m_culled;                                       // Has the room been culled?
    private IEnumerator         m_delay;                                        // Reference to the culling function

    private AssetPool           m_assetPool;                                    // The asset pool
    private DataHolder          m_dataHolder;                                   // The data holder

    /***************************************************
    *   Function        :    SetDoors
    *   Purpose         :    Sets doors
    *   Parameters      :    List<GameObject> doors
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetDoors( List<GameObject> doors )
    {
        m_doors = doors;
    }

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Get components and open doors
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get components
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();
        m_dataHolder = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
        m_children = GetComponentsInChildren<Transform>();

        // Cull all non-special rooms
        if ( !m_specialRoom )
        {
            ToggleCull( true );
        }

        //SpawnEnemies();
        //// Trun off all the enemies
        //foreach ( GameObject enemy in m_enemyList )
        //{
        //    enemy.SetActive( false );
        //}
    }

    /***************************************************
    *   Function        :    Update
    *   Purpose         :    Checks for enemies and calls RoomClear when they are dead
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        ClearCheck();
    }

    /***************************************************
    *   Function        :    ClearCheck
    *   Purpose         :    Checks for enemies and calls RoomClear when they are dead
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void ClearCheck()
    {
        // If the room has been entered and not all the enemies are dead
        if ( m_roomEntered && !m_cleared )
        {
            // Check if the enemies are alive
            EnemyCheck();

            // If the enemy list is empty and the room has not been cleared
            if ( m_enemyList.Count == 0 || m_enemyList == null && !m_cleared )
            {
                RoomClear();
            }
        }
    }


    /***************************************************
    *   Function        :    SpawnEnemies
    *   Purpose         :    Calls spawn enemies funtion
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    SpawnAI (SetUpAI)
    ******************************************************/
    public void SpawnEnemies()
    {
        if ( !m_specialRoom && !m_enemiesSpawned )
        {
            // Spawn AI from Joe's AI script, which returns the list of enemies for the room
            m_enemyList = gameObject.GetComponent<SetUpAI>().SpawnAI();
            m_enemiesSpawned = true;
        }

    }

    /***************************************************
    *   Function        :    EnemyCheck
    *   Purpose         :    Checks if enemies in the room have died
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void EnemyCheck()
    {
        // Loop through enemies
        for ( int i = 0; i < m_enemyList.Count; i++ )
        {
            // If enemy is inactive or null
            if ( !m_culled && !m_enemyList[ i ].activeSelf || m_enemyList[ i ] == null )
            {
                // Remove enemy from list
                m_enemyList.Remove( m_enemyList[ i ] );

                // Add 1 to the kill count
                m_dataHolder.SetKills( m_dataHolder.GetKills() + 1 );
            }
        }
    }

    /***************************************************
    *   Function        :    RoomClear
    *   Purpose         :    Opens doors and spawns reward
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void RoomClear()
    {
        // For a normal room
        if ( !m_specialRoom )
        {
            // Choose a pickup
            GameObject itemToSpawn = m_itemPool[ Random.Range( 0, m_itemPool.GetLength( 0 ) ) ];

            // Spawn the pickup
            m_assetPool.GetObjectFromPool( itemToSpawn, transform.Find( "ChestSpawnPoint" ).position );

            // Turn off combat music
            MusicManager.instance.ToggleCombatMusic( false );

            // Open all the doors
            OpenDoors();
        }
        else
        {
            SpecialRoomUnlock();
        }

        // Room has been cleared
        m_cleared = true;

        // Add 1 to the clear count
        m_dataHolder.SetRoomsCleared( m_dataHolder.GetRoomsCleared() + 1 );
    }

    /***************************************************
    *   Function        :    CloseDoors
    *   Purpose         :    Close all the doors in the room
    *   Parameters      :    bool lockDoors = false, bool cull = false
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator CloseDoors( bool lockDoors = false, bool cull = false )
    {
        // Close all the doors in the room
        foreach ( var door in m_doors )
        {
            DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();
            doorAnimator.Close( cull );
        }

        // If the room is normal and the doors need to be locked
        if ( !m_specialRoom && lockDoors )
        {
            // Wait a small delay so the player does not get trapped
            yield return new WaitForSeconds( 0.5f );

            // Lock all the doors in the room
            foreach ( var door in m_doors )
            {
                DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();
                doorAnimator.Lock();
            }
        }
    }

    /***************************************************
    *   Function        :    OpenDoors
    *   Purpose         :    Open all the doors in the room
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    24/04/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OpenDoors()
    {
        // For each door in the room
        foreach ( var door in m_doors )
        {
            DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();

            // If the door was open previously, open it
            if ( doorAnimator.GetWasOpen() )
            {
                doorAnimator.Open();
            }

            // Unlock all doors
            doorAnimator.Unlock();
        }

    }

    /***************************************************
    *   Function        :    EnterRoom
    *   Purpose         :    Close doors and spawn enemies
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void EnterRoom()
    {
        // Room has now been entered
        m_roomEntered = true;

        // If it's not a special room
        if ( !m_specialRoom )
        {
            // Play combat music 
            MusicManager.instance.ToggleCombatMusic( true );
        }


        // Close & lock doors
        StartCoroutine( CloseDoors( true ) );
    }

    /***************************************************
    *   Function        :    OnTriggerEnter
    *   Purpose         :    Call EnterRoom when player walks in
    *   Parameters      :    Collider other
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH JG 
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnTriggerEnter( Collider other )
    {
        // Get playermanager from the collider
        PlayerManager player = other.transform.root.GetComponentInChildren<PlayerManager>();

        // If the player entered the trigger
        if ( player != null )
        {
            // The player is now in the room
            m_inRoom = true;

            // Call enter room when you have not already entered the room
            if ( !m_roomEntered )
            {
                EnterRoom();

                // Dash towards the room's centre to avoid getting stuck in doors
                StartCoroutine( player.DashMovement( ( transform.position - player.transform.position ).normalized ) );
            }

            // Stop the DelayCull if one is running
            if ( m_delay != null )
            {
                StopCoroutine( m_delay );
            }
        }

    }


    /***************************************************
    *   Function        :    OnTriggerExit
    *   Purpose         :    Culls room after leaving
    *   Parameters      :    Collider other
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnTriggerExit( Collider other )
    {
        // If the player left the trigger
        if ( other.transform.root.GetComponentInChildren<PlayerManager>() != null )
        {
            // Player no longer in the room
            m_inRoom = false;

            // Set the delay reference and start the culling function
            m_delay = DelayCull();
            StartCoroutine( m_delay );
        }
    }

    /***************************************************
    *   Function        :    SpecialRoomUnlock
    *   Purpose         :    Unlocks doors in special rooms
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void SpecialRoomUnlock()
    {
        // Unlock all the doors
        foreach ( var door in m_doors )
        {
            DoorAnimator doorAnimator = door.GetComponent<DoorAnimator>();
            doorAnimator.Unlock();
        }
    }


    /***************************************************
    *   Function        :    ToggleCull
    *   Purpose         :    Toggles culling rooms to help performance
    *   Parameters      :    bool cull
    *   Returns         :    void
    *   Date altered    :    03/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void ToggleCull( bool cull )
    {
        // Get all the children if you have not already
        if ( m_children == null )
        {
            m_children = GetComponentsInChildren<Transform>();
        }

        // If you're not in the room trying to be culled
        if ( !( m_inRoom && cull ) )
        {
            // Set cull to be if you are culling or not
            m_culled = cull;

            // Loop through all the children
            foreach ( Transform child in m_children )
            {
                // Get if the child is an environmentInteractable
                EnviromentInteractble enviromentInteractble = child.GetComponent<EnviromentInteractble>();

                // If the child is an environmentInteractable
                if ( enviromentInteractble != null )
                {
                    // If it was not destroyed
                    if ( !enviromentInteractble.GetDead() )
                    {
                        // Toggle cull
                        child.gameObject.SetActive( !cull );
                    }
                }
                else
                {
                    // Toggle cull on child
                    child.gameObject.SetActive( !cull );
                }

            }

            // If the enemies have spawned
            if ( m_enemiesSpawned )
            {
                // Trun off all the enemies
                foreach ( GameObject enemy in m_enemyList )
                {
                    enemy.SetActive( !cull );
                }
            }
        }
    }

    /***************************************************
    *   Function        :    DelayCull
    *   Purpose         :    Culls after a timed delay
    *   Parameters      :    float time = 5f
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator DelayCull( float time = 5f )
    {
        // If you're not in the room
        if ( !m_inRoom )
        {
            // Wait for half the time
            yield return new WaitForSeconds( time / 2 );

            // Close doors
            StartCoroutine( CloseDoors() );

            // Wait the other half the time so the doors have time to close
            yield return new WaitForSeconds( time / 2 );

            // Cull room
            ToggleCull( true );
        }
    }

    /***************************************************
    *   Function        :    StartDelayCull
    *   Purpose         :    Starts DelayCull
    *   Parameters      :    float time = 5f
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void StartDelayCull( float time = 5f )
    {
        // Set reference and call DelayCull
        m_delay = DelayCull( time );
        StartCoroutine( m_delay );
    }

    /***************************************************
    *   Function        :    StopDelayCull
    *   Purpose         :    Stops DelayCull
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void StopDelayCull()
    {
        //if( m_delay != null )
        //{
        //    StopCoroutine( m_delay );

        //}

        // Stop coroutines
        StopAllCoroutines();
    }
}
