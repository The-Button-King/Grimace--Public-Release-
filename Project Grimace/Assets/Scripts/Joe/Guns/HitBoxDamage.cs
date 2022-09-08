using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: HitBoxDamage
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Used as a hitbox and damage the parent with the correct mutiplier 
 * 
 * Functions:   void IDamageable<float>.Damage( float Damage )
 * 
 * References:
 * 
 * See Also: Look at AI prefabs and their colliders 
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 10/07/2022    JG          1.00        - Created Class
 * 11/07/2022    JG          1.01        -- Added crit hit
 * 29/07/2022    JG          1.02       - dirty beacon crit hit
 ****************************************************************************************************/
[RequireComponent(typeof(Collider))] 
public class HitBoxDamage : MonoBehaviour , IDamageable<float>
{
   // These vars are not exposded to inspector as they are meant to be consistent 
    public enum HitType{ Red,Orange,Yellow,Base};           // Enum of damage preset 
    public HitType      m_hitType;                          // Store the hitype
    private float       m_redMutiplyer = 1.5f;              // Mutiplyer for each hit type 
    private float       m_orangeMutiplyer = 1.35f;          //
    private float       m_yellowMutiplyer = 1.20f;
    /***************************************************
    *   Function        : Damage  
    *   Purpose         : work out what damage type it is    
    *   Parameters      : float Damage   
    *   Returns         : void   
    *   Date altered    : 29/07/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    void IDamageable<float>.Damage( float Damage )
    {
        // Is hit a crit 
        bool crit = false;

        switch ( m_hitType )
        {
            case HitType.Red:
                {
                    // Apply mutiplyier 
                    Damage *= m_redMutiplyer;
                    crit = true;
                }
                break;
            case HitType.Orange:
                {
                    Damage *= m_orangeMutiplyer;
                }
                break;
            case HitType.Yellow:
                {
                    Damage *= m_yellowMutiplyer;
                }
                break;
            case HitType.Base:
                {
                    // do nothing;
                }
                break;
        }

        // If AI set crit status 
        if(transform.root.GetComponent<AIData>() != null )
        {
            transform.root.GetComponent<AIData>().SetCrit( crit );
        }

        // If beacon set crit status 
        if ( transform.root.GetComponent<Beacon>() != null )
        {
            transform.root.GetComponent<Beacon>().SetCrit( crit );
        }

        // Deal damage 
        transform.root.GetComponent<IDamageable<float>>().Damage( Damage );
    }
}
