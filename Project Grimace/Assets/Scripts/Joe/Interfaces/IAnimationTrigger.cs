using UnityEngine.Events;
using UnityEngine;
/****************************************************************************************************
 * Type: Interface
 * 
 * Name: IAnimationTrigger 
 * Author: Joseph Gilmore 
 *
 * Purpose: The blackboard task cannot access animation events. To overcome this AI that requrie animation events use this interface to trigger and pass a reference to events
 *
 * 
 * Functions:  public void AnimationEvent();
 *             public UnityEvent GetAnimationEvent();
 *             public Animator GetAnimator();
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 11/08/2022     JG         1.00           - Created 
 ****************************************************************************************************/
public interface IAnimationTrigger
{
    public void AnimationEvent();
    public UnityEvent GetAnimationEvent();
    public Animator GetAnimator();
}
