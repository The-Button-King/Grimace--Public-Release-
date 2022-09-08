using UnityEngine;
using System.Collections;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: FreezeExplosion
 *
 * Author: Joseph Gimore
 *
 * Purpose: FreezeExplosion is to freeze moveable objects 
 * 
 * Functions:   protected override void ApplyEffect( Collider[] hitColliders, int colliders )
 *              private IEnumerator SlowObject(GameObject gameObject )    
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 28/07/2022    JG          1.00        - created a very quick freeze 
 * 03/08/2022    JG          1.01        - Adjusted for new AI structure 
 * 14/08/2022    JG          1.50        - Redid class to use interfaces 
 ****************************************************************************************************/
public class FreezeExplosion : Explosion 
{
    [SerializeField][Tooltip("length of freeze effect")][Range(1.0f,5.0f)]
    private float m_freezeLength = 3.50f;           // Length of effect
    [SerializeField]
    [Tooltip( "how Impactful freeze is (lower the better " )]
    [Range( 0.01f, 0.98f )]
    private float m_freezeMutiplier = 0.2f;         // Mutiplyer 

    /***************************************************
    *   Function        : ApplyEffect 
    *   Purpose         : Apply slow effect to hit gameobjects  
    *   Parameters      : Collider[] hitColliders, int colliders   
    *   Returns         : Void   
    *   Date altered    : 14/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected override void ApplyEffect( Collider[] hitColliders, int colliders )
    {
        // loop through colliders 
        for ( int i = 0; i < colliders; i++ )
        {
            //  If  it can be slowed 
            if ( hitColliders[ i ].transform.root.GetComponentInChildren<ISloweable>() != null )
            {
                // Slow 
                StartCoroutine( SlowObject( hitColliders[ i ].transform.root.GetComponentInChildren<ISloweable>() ) );  
            }
           
        }
        
    }
    /***************************************************
    *   IEnumrator      : SlowObject  
    *   Purpose         : Slow speed for x amount of time    
    *   Parameters      : GameObject gameObject    
    *   Returns         : yield return wait  
    *   Date altered    : 14/08/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator SlowObject( ISloweable objectToSlow )
    {
        // Get default speed ( Have to use componenet in children due to the position of it being on different objects)
        float speed = objectToSlow.GetDefaultSpeed();

        // Slow object using mutiplier 
        objectToSlow.SetSpeed( speed * m_freezeMutiplier );

        WaitForSeconds wait = new WaitForSeconds( m_freezeLength );

        // Wait for the length set 
        yield return wait;

        // Reset speed
        objectToSlow.SetSpeed( speed );
    }
   

}
