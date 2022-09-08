using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: Shield
 *
 * Author: Will Harding
 *
 * Purpose: Regenerating shield
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
 * 21/07/22     WH          1.1         Comments
 * 03/08/22     WH          1.2         Changed Start to Awake to stop referencing errors on new level
 * 17/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class Shield : RegenStat
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
        
        // Update UI
        m_ui.UpdateShield( m_value, m_maxValue );
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

        // Update UI
        m_ui.UpdateShield( m_value, m_maxValue );
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

        // Update UI
        m_ui.UpdateShield( m_value, m_maxValue );
    }
}
