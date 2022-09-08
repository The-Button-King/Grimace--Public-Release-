/****************************************************************************************************
 * Type: Interface
 * 
 * Name: IToggleable
 * Author: Joseph Gilmore 
 *
 * Purpose: For item effects that need to be toggled to start ( enviroment hazards , grenades & explosions). This was created so they can be pooled as effects were orginaly activated on their start
 *
 * 
 * Functions: void ToggleEffect();
 * 
 * References:
 * 
 * See Also: Explosion , Posion Gas , Smoke 
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 01/08/2022     JG         1.00           - Created 
 ****************************************************************************************************/
public interface IToggleable
{
    void ToggleEffect();
}