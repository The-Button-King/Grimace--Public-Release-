using NodeCanvas.BehaviourTrees;
using UnityEngine;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: Bomber
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Child of AI for bomber type 
 * 
 * Functions:           public override void Damage( float dmg )
 *                      public void  Explode()
 *                      
 *                      
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 22/07/2022    JG         1.0         - Created class
 * 02/08/2022    JG         2.0         - Updated inheretaince 
 * 03/08/2022    JG         2.01        - Bugs 
 * 12/08/2022    JG         2.02        - Build Bug fixes 
 * 15/08/2022    JG         2.03        - Clean
 ****************************************************************************************************/
public class Bomber : AgentAI
{
    [SerializeField][Tooltip("Put asset used for bomber explosion")]
    private GameObject m_explosion;         // Rerefence to explosion asset 
    /***************************************************
    *   Function        : Damage 
    *   Purpose         : Alter the HP of the AI   
    *   Parameters      : float dmg 
    *   Returns         : void    
    *   Date altered    : 12/08/2022
    *   Contributors    : JG 
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    public override void Damage( float dmg )
    {
       if(m_isDead == false )
        {
            // Take damage away from health
            m_healthPoints -= dmg;


            // Create a dmg text from pool
            GameObject text = m_enemiesPool.GetObjectFromPool( m_damageText );

            //  Setup text
            text.GetComponent<DamageEffect>().CreateDamageText( dmg, m_critHit, transform.Find( "DamagePos" ).transform.position );

            // Reset crit hit
            m_critHit = false;


            // If player dead
            if ( m_healthPoints <= k_minHealth )
            {

                // Reset Health 
                m_healthPoints = m_startingHealth;


                // If Gameobject active (not exploded on its own acord 
                if ( gameObject.activeSelf == true )
                {
                    // Explode AI
                    Explode();
                }


            }
        }
       
    }
    /***************************************************
    *  void             : Explode AI
    *   Purpose         : Make the AI explode 
    *   Parameters      : N/A
    *   Returns         : void    
    *   Date altered    : 12/08/2022
    *   Contributors    : JG 
    *   Notes           : 
    *   See also        :    
    ******************************************************/
    public void  Explode()
    {
        if(m_isDead == false )
        {
            m_isDead = true;

            // Play explode sound
            m_audioPool.PlaySound( m_audioPool.m_death );


            // Get explosion
            GameObject tempExplosion = m_enemiesPool.GetObjectFromPool( m_explosion, transform.position );

            tempExplosion.GetComponent<Explosion>().SetDamage( m_baseDamage );

            // Toggle effect
            tempExplosion.GetComponent<IToggleable>().ToggleEffect();

            // Disable AI
            GetComponent<BehaviourTreeOwner>().enabled = false;
             
            // Return AI to pool
            m_enemiesPool.ReturnObjectToPool( gameObject );
        }
       
    }
}
