/****************************************************************************************************
* Type: Interface
* 
* Name: IDamageble
*
* Author: Joseph Gilmore
*
* Purpose: Anything that can takes damage 
* 
* Functions: void Damage(T Damage);
* 
* References:
* 
* See Also: AIData,Player Manager, Beacon & more
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 19/06/2022    JG          1.00        - Created Interface
****************************************************************************************************/
public interface IDamageable<T>
{
    
    void Damage( T Damage );
}
