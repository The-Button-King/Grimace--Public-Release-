using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: DoorAnimator
 *
 * Author: Will Harding
 *
 * Purpose: Code for interactable doors
 * 
 * Functions:   public List<RoomController> GetConnectedRooms()
 *              public bool GetLocked()
 *              public void SetLocked( bool locked )
 *              public bool GetWasOpen()
 *              public void SetWasOpen( bool wasOpen )
 *              private void Start()
 *              public override void Interact()
 *              public void Open()
 *              public void Close( bool cull = false )
 *              public void Lock()
 *              public void Unlock()
 *              public override void DisplayLookAtText()
 *              
 *              
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 24/04/22     WH          1.0         Initial creation
 * 12/05/22     WH          1.3         Final comment run through before submission
 * 
 * 11/06/22     WH          2.0         New variables for new door system
 * 14/06/22     WH          2.1         Doors now close only if opened and vice versa
 * 21/06/22     WH          2.1         Changed look at text to use UI
 * 28/07/22     WH          2.2         Adding shutters triggers
 * 31/07/22     WH          2.1         Interactable text changes
 * 14/08/22     WH          2.2         Added blocker hitbox to stop door dashing shenanigans
 * 16/08/22     WH          2.2         Fixing culling
 * 17/08/22     WH          2.3         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof( Animator ) )]
public class DoorAnimator : Interactable
{
    private Animator                m_animator;                                     // Animator component

    private bool                    m_wasOpen;                                      // Was the door previously open?

    private List<RoomController>    m_connectedRooms = new List<RoomController>();  // The rooms the door connects to

    [Tooltip( "Object with trigger to block player" )]
    public  GameObject              m_blocker;                                      // The gameobject that has a collider to block the player

    private int                     an_open;                                        // Animator open trigger ID
    private int                     an_close;                                       // Animator close trigger ID
    private int                     an_lock;                                        // Animator lock trigger ID
    private int                     an_unlock;                                      // Animator unlock tigger ID
    private int                     an_isOpen;                                      // Animator isOpen bool ID
    private int                     an_isLocked;                                    // Animator isLocked bool ID

    //public bool open;

    /***************************************************
    *   Function        :    GetConnectedRooms
    *   Purpose         :    Gets connected rooms
    *   Parameters      :    None
    *   Returns         :    List<RoomController> m_connectedRooms
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public List<RoomController> GetConnectedRooms()
    {
        return m_connectedRooms;
    }

    /***************************************************
    *   Function        :    GetLocked
    *   Purpose         :    Locked Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool GetLocked()
    {
        return m_animator.GetBool( an_isLocked );
    }

    /***************************************************
    *   Function        :    SetLocked
    *   Purpose         :    Locked Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    02/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetLocked( bool locked )
    {
        m_animator.SetBool( an_isLocked, locked );
    }

    /***************************************************
    *   Function        :    GetWasOpen
    *   Purpose         :    WasOpen Getter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public bool GetWasOpen()
    {
        return m_wasOpen;
    }

    /***************************************************
    *   Function        :    SetWasOpen
    *   Purpose         :    WasOpen Setter
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/06/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetWasOpen( bool wasOpen )
    {
        m_wasOpen = wasOpen;
    }

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Gets animator
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        // Get animator
        m_animator = gameObject.GetComponent<Animator>();

        // Set all animation vars
        an_open = Animator.StringToHash( "Open" );
        an_close = Animator.StringToHash( "Close" );
        an_lock = Animator.StringToHash( "Lock" );
        an_unlock = Animator.StringToHash( "Unlock" );
        an_isOpen = Animator.StringToHash( "isOpen" );
        an_isLocked = Animator.StringToHash( "isLocked" );

        // Unlock door
        m_animator.SetTrigger( an_unlock );
    }

    //private void OnValidate()
    //{
    //    if ( open )
    //    {
    //        Open();
    //    }
    //    else
    //    {
    //        Close();
    //    }
    //}

    /***************************************************
    *   Function        :    Interact
    *   Purpose         :    Does something when player interacts
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    11/06/22
    *   Contributors    :    WH
    *   Notes           :    Override function
    *   See also        :    
    ******************************************************/
    public override void Interact()
    {
        // If the door is not locked
        if ( !m_animator.GetBool( an_isLocked ) )
        {
            // Close door if it's open
            if ( m_animator.GetBool( an_isOpen ) )
            {
                Close( true );
                m_wasOpen = false;
            }

            //Open door if it's closed
            else
            {
                Open();
                m_wasOpen = true;
            }
        }
    }

    /***************************************************
    *   Function        :    Open
    *   Purpose         :    Opens door
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    03/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Open()
    {
        // Open door if it's closed
        if ( !m_animator.GetBool( an_isOpen ) )
        {
            m_animator.SetTrigger( an_open );
            m_animator.SetBool( an_isOpen, true );

            // For any connected rooms
            foreach ( var room in m_connectedRooms )
            {
                // Stop any culling, uncull the room, and spawn enemies
                room.StopDelayCull();
                room.ToggleCull( false );
                room.SpawnEnemies();
            }
        }
    }

    /***************************************************
    *   Function        :    Close
    *   Purpose         :    Closes door
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Close( bool cull = false )
    {
        // Close door if it's open
        if ( m_animator.GetBool( an_isOpen ) )
        {
            m_animator.SetTrigger( an_close );
            m_animator.SetBool( an_isOpen, false );

            // If you want to cull the room
            if ( cull )
            {
                // For any connected rooms
                foreach ( var room in m_connectedRooms )
                {
                    // Stop any cull calls to stop extra culling happening at the wrong moments
                    room.StopDelayCull();

                    // Cull the room
                    room.StartDelayCull( m_animator.GetCurrentAnimatorStateInfo(0).length );
                }
            }
        }
    }


    /***************************************************
    *   Function        :    Lock
    *   Purpose         :    Locks door
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Lock()
    {
        // Lock door if it isn't locked
        if ( !m_animator.GetBool( an_isLocked ) )
        {
            m_animator.SetTrigger( an_lock );
            m_animator.SetBool( an_isLocked, true );

            // Turn on blocker so the player can't get trapped as easily
            m_blocker.SetActive( true );
        }
    }

    /***************************************************
    *   Function        :    Unlock
    *   Purpose         :    Unlocks door
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    14/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void Unlock()
    {
        // Unlock door if it's locked
        if ( m_animator.GetBool( an_isLocked ) )
        {
            m_animator.SetTrigger( an_unlock );
            m_animator.SetBool( an_isLocked, false );

            // Turn off blocker
            m_blocker.SetActive( false );
        }
    }

    /***************************************************
    *   Function        :    DisplayLookAtText
    *   Purpose         :    Displays hover text for info
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    31/07/22
    *   Contributors    :    WH
    *   Notes           :    Override function
    *   See also        :    
    ******************************************************/
    public override void DisplayLookAtText()
    {
        // If the door is open, use alt text
        m_useAltText = m_animator.GetBool( an_isOpen );

        // Display text as normal
        base.DisplayLookAtText( );
    }
}
