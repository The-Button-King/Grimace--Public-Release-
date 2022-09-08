using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: ShopAudioPool
 *
 * Author: Will Harding
 *
 * Purpose: Shop audio pool child
 * 
 * Functions:   
 * 
 * References: 
 * 
 * See Also: AudioPool
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 28/07/22     WH          1.0         Initial creation
 * 14/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
public class ShopAudioPool : AudioPool
{
    [Header( "Shop Audios" )]

    [Tooltip( "Audio for buying an upgrade" )]
    public Audio m_buy;

    [Tooltip( "Audio for unsuccessfully buying" )]
    public Audio m_failBuy;

    [Tooltip( "Audio for clicking a button" )]
    public Audio m_buttonClick;
}
