using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Stamina
 *
 * Author: Will Harding
 *
 * Purpose: Regenerating stamina
 * 
 * Functions:   public override void Awake()
 *              public override void SetValue( float value )
 *              public override void SetMaxValue( float value )
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 19/07/22     WH          1.0         Initial creation
 * 03/08/22     WH          1.1         Changed Start to Awake to stop referencing errors on new level
 * 17/08/22     WH          1.2         Cleaning
 ****************************************************************************************************/
public class Stamina : RegenStat
{
    /***************************************************
    *   Function        : Awake
    *   Purpose         : Sets values
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 03/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public override void Awake()
    {
        base.Awake();

        // Update UI, function wants % of stamina, not raw value
        m_ui.UpdateStamina( m_value / m_maxValue );
    }

    /***************************************************
    *   Function        : SetValue
    *   Purpose         : Sets value variable
    *   Parameters      : float value
    *   Returns         : void
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public override void SetValue( float value )
    {
        base.SetValue( value );

        // Update UI, function wants % of stamina, not raw value
        m_ui.UpdateStamina( m_value / m_maxValue );
    }

    /***************************************************
    *   Function        : SetMaxValue
    *   Purpose         : Sets max value variable
    *   Parameters      : float value
    *   Returns         : void
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public override void SetMaxValue( float value )
    {
        base.SetMaxValue( value );

        // Update UI, function wants % of stamina, not raw value
        m_ui.UpdateStamina( m_value / m_maxValue );
    }
}
