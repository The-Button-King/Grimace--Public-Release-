using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: GameStats
 *
 * Author: Will Harding
 *
 * Purpose: Shows stats in the pause and end screens
 * 
 * Functions:   private void Awake()
 *              public void SetStartTime( float time )
 *              private string FormatTime( float currentTime )
 *              public void DisplayStats( bool saveTime = false )
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 05/08/22     WH          1.0         Initial creation
 * 10/08/22     WH          1.1         Getting to work fully
 * 16/08/22     WH          1.2         Cleaning
 * 17/08/22     WH          1.3         Added time saving
 ****************************************************************************************************/
public class GameStats : MonoBehaviour
{
    [Tooltip( "Text to display current time" )]
    public      TMP_Text    m_time;             // Time text

    [Tooltip( "Text to display current kills" )]
    public      TMP_Text    m_kills;            // Kills text

    [Tooltip( "Text to display current number of purchases" )]
    public      TMP_Text    m_purchases;        // Purchases text

    [Tooltip( "Text to display current number of rooms cleared" )]
    public      TMP_Text    m_roomsCleared;     // Rooms cleare text
        
    private     float       m_startTime;        // Time when the game started

    public      DataHolder  m_dataHolder;       // Data holder


    /***************************************************
    *   Function        :    Awake
    *   Purpose         :    Gets Dataholder
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Awake()
    {
        // Get data holder
        m_dataHolder = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<DataHolder>();
    }


    /***************************************************
    *   Function        :    SetStartTime
    *   Purpose         :    Sets start time
    *   Parameters      :    float time
    *   Returns         :    void
    *   Date altered    :    10/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void SetStartTime( float time )
    {
        m_startTime = time;
    }

    /***************************************************
    *   Function        :    FormatTime
    *   Purpose         :    Formats a time into mintes:seconds
    *   Parameters      :    float currentTime
    *   Returns         :    void
    *   Date altered    :    05/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private string FormatTime( float currentTime )
    {
        // Convert time into a TimeSpan
        TimeSpan newTime = TimeSpan.FromSeconds( currentTime );

        // Format time into mintes:seconds and return it
        return string.Format( "{0:00}:{1:00}", newTime.Minutes, newTime.Seconds );
    }

    /***************************************************
    *   Function        :    DisplayStats
    *   Purpose         :    Updates stats
    *   Parameters      :    bool saveTime = false
    *   Returns         :    void
    *   Date altered    :    17/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public void DisplayStats( bool saveTime = false )
    {
        // Get the amount of time the player has been playing for
        float currentTime = Time.time - m_startTime;

        // Add the previous levels' time
        currentTime += m_dataHolder.GetTotalTime();

        if( saveTime )
        {
            // Save the total time to the dataholder for a cumulative time 
            m_dataHolder.SetTotalTime( currentTime );
        }
        
        // Format the play time and set it 
        m_time.text = FormatTime( currentTime );

        // Set the kills
        m_kills.text = m_dataHolder.GetKills().ToString();

        // Set the purchases
        m_purchases.text = m_dataHolder.GetPurchases().ToString();

        // Set the rooms cleared
        m_roomsCleared.text = m_dataHolder.GetRoomsCleared().ToString();
    }
}
