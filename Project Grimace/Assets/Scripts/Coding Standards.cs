/* --------------------------------------- Coding Standards of Grimace ---------------------------------------------
 * Author - Joseph Gilmore
 * 
 * 
 * 
 *  Variables  camel casing , Example : myVar
 *  Memember Var: m_myVar
 *  
 *  Consts vars: k_constVar;
 *  
 *  Animator String to hash : an_stringToHash;
 *  
 *   blackboard vars: b_searchPosition
 *   
 *  Functions/IEnumrators Pascal case , Example : MyFunction()
 *  
 *  Class/ Inrterface / Script Headers :
 *  
 *    
 * /****************************************************************************************************
 * Type: Class
 * 
 * Name: 
 *
 * Author: 
 *
 * Purpose: 
 * 
 * Functions:
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 ****************************************************************************************************
 *
 *
 * Function Headers:
 * 
 *  /***************************************************
 *   Function        :    
 *   Purpose         :    
 *   Parameters      :    
 *   Returns         :    
 *   Date altered    :
 *   Contributors    :
 *   Notes           :    
 *   See also        :    
 *  *****************************************************
 *
 * BT do not work with serializefield 
 * Unity Features :
 * 
 * Use Requried comoponets within a script if it will break without said component 
 * 
 * Split exposed/serializefield  Variables with [Headers()]
 * 
 * Use [Tooltips()], and [Ranges] on serializefield values. (ensure you set via inspector).
 * 
 * 
 * 
 *  Comments example : " // Captial letter with a space."
 *  
 *  
 *  
 *  Vars at top of class layout:
 * 
 *  private const float     k_potato         // This is a potato
 *  
 *  [Header(" Potato stats")]
 *  [serializefield][Range(5,10)][ToopTip(" Amount of potatos requried to make mash" )]
 *  private int             m_mashAmount     // Amount requried to make a mash   
 *
 *  end.
 *  
 *  Ensure vars are indented and lined up.
 *  
 *  Use Enums or String to hash inplace of strings.
 *   
 *  Function Update Code
 *    Make Using alhpabetical exsample:
        using LayerMasks;
 *       using NodeCanvas.Framework;
        using ParadoxNotion.Design;
        using UnityEngine;
        using UnityEngine.AI;
        using UnityEngine.Events;
        using System.Collections;
 *  have fun <3
 *
 */

