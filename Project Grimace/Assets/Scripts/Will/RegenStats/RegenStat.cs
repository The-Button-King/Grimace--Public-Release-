using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
 * Type: Class
 * 
 * Name: RegenStat
 *
 * Author: Will Harding
 *
 * Purpose: Parent class for stats that regenerate
 * 
 * Functions:   public float GetValue()
 *              public virtual void SetValue( float value )
 *              public float GetMaxValue()
 *              public virtual void SetMaxValue( float value )
 *              public float GetRechargeRate()
 *              public void SetRechargeRate( float value ) 
 *              public float GetTimeStamp()
 *              public void SetTimeStamp( float value )
 *              public bool GetRegen()
 *              public void SetRegen( bool value )
 *              public virtual void Awake()
 *              private void Update()
 * 
 * References: 
 * 
 * See Also: 
 *
 * Change Log:
 * Date         Initials    Version     Comments
 * 19/07/22     WH          1.0         Initial creation
 * 22/07/22     WH          1.1         Added regen bool and comments
 * 03/08/22     WH          1.2         Changed Start to Awake to stop referencing errors on new level
 * 17/08/22     WH          1.3         Cleaning
 ****************************************************************************************************/
public class RegenStat : MonoBehaviour
{
    [SerializeField]
    [Tooltip( "The current value of the stat" )]
    protected float         m_value             = 100f;  // How much of a stat you have
    
    [SerializeField]
    [Tooltip( "The max value of the stat" )]
    protected float         m_maxValue          = 100f;  // The max stat you have
    
    [SerializeField]
    [Tooltip( "How long it takes after using the stat does it wait until it recharges" )]
    protected float         m_rechargeCooldown  = 2.5f;  // How long after timeStamp must pass before recharging stat
    
    [SerializeField]
    [Tooltip( "How fast the stat rechages" )]
    protected float         m_rechargeRate      = 20f;  // How much stat to recharge per second

    protected float         m_timeStamp;                // Time stat was last depleated
    protected bool          m_regen;                    // Is the stat regenerating?

    protected UIController  m_ui;                       // UI


    /***************************************************
    *   Function        : GetValue
    *   Purpose         : Gets value variable
    *   Parameters      : None
    *   Returns         : float value
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetValue()
    {
        return m_value;
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
    public virtual void SetValue( float value )
    {
        m_value = value;
    }

    /***************************************************
    *   Function        : GetMaxValue
    *   Purpose         : Gets max value variable
    *   Parameters      : float value
    *   Returns         : void
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetMaxValue()
    {
        return m_maxValue;
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
    public virtual void SetMaxValue( float value )
    {
        // Set max health and fully heal player
        m_maxValue = value;
        m_value = value;

    }

    /***************************************************
    *   Function        : GetRechargeRate
    *   Purpose         : Gets recharge rate variable
    *   Parameters      : None
    *   Returns         : float recharge rate
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetRechargeRate()
    {
        return m_rechargeRate;
    }

    /***************************************************
    *   Function        : SetRechargeRate
    *   Purpose         : Sets recharge rate variable
    *   Parameters      : float value
    *   Returns         : void
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public void SetRechargeRate( float value ) 
    { 
        m_rechargeRate = value;
    }

    /***************************************************
    *   Function        : GetTimeStamp
    *   Purpose         : Gets timestamp variable
    *   Parameters      : None
    *   Returns         : float timestamp
    *   Date altered    : 19/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetTimeStamp()
    {
        return m_timeStamp;
    }

    /***************************************************
*   Function        : SetTimeStamp
*   Purpose         : Sets timestamp variable
*   Parameters      : float value
*   Returns         : void
*   Date altered    : 19/07/22
*   Contributors    : WH
*   Notes           :  
*   See also        :    
******************************************************/
    public void SetTimeStamp( float value )
    {
        m_timeStamp = value;
    }

    /***************************************************
    *   Function        : GetRegen
    *   Purpose         : Gets regen variable
    *   Parameters      : None
    *   Returns         : bool regen
    *   Date altered    : 22/07/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public bool GetRegen()
    {
        return m_regen;
    }

    /***************************************************
*   Function        : SetRegen
*   Purpose         : Sets regen variable
*   Parameters      : bool regen
*   Returns         : void
*   Date altered    : 22/07/22
*   Contributors    : WH
*   Notes           :  
*   See also        :    
******************************************************/
    public void SetRegen( bool value )
    {
        m_regen = value;
    }


    /***************************************************
    *   Function        : Awake
    *   Purpose         : Sets vars
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 03/08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
******************************************************/
    public virtual void Awake()
    {
        // Get UI
        m_ui = transform.root.GetChild( 0 ).Find( "HUD" ).GetComponent<UIController>();
    }


    /***************************************************
    *   Function        : Update
    *   Purpose         : Regenerates value
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 17//08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    private void Update()
    {
        Regenerate();
    }

    /***************************************************
    *   Function        : UpdaRegeneratete
    *   Purpose         : Regenerates value
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 17//08/22
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    private void Regenerate()
    {
        // if your m_value is less than max and the recharge cooldown has passed
        if ( m_value < m_maxValue && Time.time > m_timeStamp + m_rechargeCooldown )
        {
            // Increse value
            m_value += m_rechargeRate * Time.deltaTime;
            SetValue( m_value );

            // Stat is regenerating
            m_regen = true;
        }
        else
        {
            // Stat is not regenerating
            m_regen = false;
        }
    }
}
