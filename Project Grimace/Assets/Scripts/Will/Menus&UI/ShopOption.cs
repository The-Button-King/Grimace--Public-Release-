using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************************************************************************************
 * Type: Class
 * 
 * Name: ShopOption
 *
 * Author: Will Harding
 *
 * Purpose: Buyable powerup object
 * 
 * Functions:   public string GetDisplayText()
 *              public int GetPrice()
 *              public PowerUpParent GetEffect()
 *              private void Start()
 *              private void OnValidate()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 18/07/22     WH          1.0         Initial creation
 * 16/08/22     WH          1.1         Cleaning
 ****************************************************************************************************/
[RequireComponent(typeof( PowerUpParent ) )]
public class ShopOption : MonoBehaviour
{
    [SerializeField]
    [Tooltip ( "Text to display in option" )]
    private string          m_optionText;   // What the option should say

    [SerializeField]
    [Tooltip( "Price of option" )]
    private int             m_price;        // Price of effect

    [SerializeField]
    [Tooltip( "Full Display of Price + Text. Updates live when you edit the above" )]
    private string          m_displayText;  // What to actually display

    [SerializeField]
    [Tooltip( "Full Display of Price + Text. Updates live when you edit the above" )]
    private PowerUpParent   m_effect;       // Effect to trigger


    /***************************************************
    *   Function        :    GetDisplayText
    *   Purpose         :    Returns display text
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public string GetDisplayText()
    {
        // Format text and return it
        m_displayText = m_price + " " + m_optionText;
        return m_displayText;
    }

    /***************************************************
    *   Function        :    GetPrice
    *   Purpose         :    Returns price
    *   Parameters      :    None
    *   Returns         :    int price
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public int GetPrice()
    {
        return m_price;
    }

    /***************************************************
    *   Function        :    GetEffect
    *   Purpose         :    Returns effect
    *   Parameters      :    None
    *   Returns         :    PowerUpParent effect
    *   Date altered    :    16/08/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public PowerUpParent GetEffect()
    {
        return m_effect;
    }

    /***************************************************
    *   Function        :    Start
    *   Purpose         :    Assigns variables
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void Start()
    {
        m_effect = GetComponent<PowerUpParent>();
    }

    /***************************************************
    *   Function        :    OnValidate
    *   Purpose         :    Refreshes the display text
    *   Parameters      :    None
    *   Returns         :    void
    *   Date altered    :    18/07/22
    *   Contributors    :    WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnValidate()
    {
        GetDisplayText();
    }
}
