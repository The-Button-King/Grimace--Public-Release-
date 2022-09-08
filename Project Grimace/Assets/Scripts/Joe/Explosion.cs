using CameraShake;
using UnityEngine;
/****************************************************************************************************
* Type: Class
* 
* Name: Explosion
*
* Author: Joseph Gilmore
*
* Purpose: Simulate an explosion within a radius 
* 
* Functions:        public Explosion( float radius)
*                   public Explosion()
*                   private void Awake()
*                   public void ToggleEffect()
*                   private float CalulateDamage(Transform hitObject)
*                   public void SetDamage(float dmg)
*                   protected virtual void ApplyEffect( Collider[] hitColliders, int colliders )
*                   private void OnEnable()
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
* 15/06/2022    JG         1.00        -Created class
* 19/06/2022    JG         1.01        -Altered damage to accept interface
* 20/06/2022    JG         1.02        - Cleaning
* 10/07/2022    JG         1.03        - Tempt fix to get explosions to apply damage once with new hitbox system
* 11/07/2022    JG         1.04       - fixed tempt fixed 
* 22/07/2022    JG         1.05       - Updated screen shake with new method 
* 01/08/2022    JG         2.06       - Added interface, removed call from start 
* 14/08/2022    JG         2.07       - Cleaning
****************************************************************************************************/
public class Explosion : MonoBehaviour , IToggleable
{
                
    private Transform   m_explosionPoint;                       // The point of explosion
    [Header("Explosion stats")]
    [SerializeField][Range(1.0f,25.0f)][Tooltip("Area of effect")]
    private float       m_explosionRadius = 5.0f;               // Area affect of explosion 
    [SerializeField][Tooltip("Layers to take damage ")]
    private LayerMask   m_layerMasks;                            // Layer masks to check what can take damage
    [SerializeField]
    [Range( 1.0f, 100.0f )]
    [Tooltip( "max damage , damage decrease further away from explosion" )]
    private float       m_maxDamage = 80.0f;                    // Max Amount of damage if object at centre of explosion  
    private float       m_facingForwardDot = 0.7f;              // The dot product amount to say object is facing towards exploision 
    private float       m_damageDirectionDamp = 0.9f;           // The amount to reduce damage if facing away from explosion 
    [Header("Camera Shake from explosion")]
    [SerializeField][Range ( 1.0f,25.0f)][Tooltip("Amount to shake camer , higher value more intense")]
    private float       m_cameraShakeValue = 8.0f;              // Amount to shake camera by  
    [SerializeField] [Range( 1.0f, 50.0f )] [Tooltip( "Distance of effect " )]
    private float       m_maxDistanceToShake = 30.0f;           // max amount of distance to apply screen shake
    [SerializeField] [Range( 0.8f, 2.0f )] [Tooltip( "length of shake" )]
    private float       m_cameraShaketime = 0.8f;               // Time to have screen shake for 
    private const float k_defaultDamage = 80.0f;                // Default explosion damage 
    private const int   k_maxColliders = 20;                    // Max amount of colliders can be detected 
    /***************************************************
    *   Function        : Explosion    
    *   Purpose         : overloaded contructor to alter explosion values   
    *   Parameters      : float radius  
    *   Returns         : void  
    *   Date altered    : 15/06/2022
    *   Contributors    : JG
    *   Notes           :    
    *   See also        : ExplosionTask   
    ******************************************************/
    public Explosion( float radius)
    {
       // Set vars
       m_explosionRadius =radius;
    }
      /***************************************************
      *   Function        : Explosion    
      *   Purpose         : Default constructor    
      *   Parameters      : N/A  
      *   Returns         : Void  
      *   Date altered    : 15/06/2022
      *   Contributors    : JG
      *   Notes           :    
      *   See also        :    
      ******************************************************/
    public Explosion()
    {

     
    }
    /***************************************************
    *   Function        :  Awake  
    *   Purpose         :  Setup class  and call it to explode
    *   Parameters      :  N/A 
    *   Returns         :  Void    
    *   Date altered    : 01/08/2022
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/

    private void Awake()
    {
        // Set explosion point
        m_explosionPoint = transform;
    }
  
    /***************************************************
    *   Function        : ToggleEffect   
    *   Purpose         : Create an explosion to damage surronding objects  
    *   Parameters      : N/A   
    *   Returns         : Void   
    *   Date altered    : 01/08/2022
    *   Contributors    : JG
    *   Notes           :  *was named explosion    
    *   See also        :    
    ******************************************************/

    public void ToggleEffect()
    {
        // Create array for colliders 
        Collider[] hitColliders = new Collider[k_maxColliders];

        // Get the amount of colliders detected in sphere in certain layers 
        int colliders = Physics.OverlapSphereNonAlloc( m_explosionPoint.position, m_explosionRadius, hitColliders, m_layerMasks );

        // Apply effects of explsion on to hit objects 
        ApplyEffect( hitColliders,colliders );

      
        // Shake camera
        ScreenShake.Instance.ShakeOverDistanceFromPoint( m_explosionPoint.transform.position, GameObject.FindGameObjectWithTag( "Player" ).transform.position, m_maxDistanceToShake, m_cameraShakeValue, m_cameraShaketime ); 
    }
    /***************************************************
    *   Function        :  CalculateDamage  
    *   Purpose         :  Work out the damage requried  
    *   Parameters      :  Transform hitObject  
    *   Returns         :  float dmg
    *   Date altered    :  UK
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/

    private float CalulateDamage(Transform hitObject)
    { 
        // Calulate distance of hit object from the center of explosion 
        float distance = Vector3.Distance(m_explosionPoint.transform.position, hitObject.position);

        // Work out the percentage of how far along the radius the hit object is
        float damageOverDistance =  1- (distance / m_explosionRadius);
       
        // Check if the player is facing towards explosion using dot product
        if(Vector3.Dot(hitObject.forward, m_explosionPoint.position - hitObject.position) >= m_facingForwardDot)
        {
            // Work out the percentage requried to apply to hit object
            return m_maxDamage * damageOverDistance;

        }
        else
        {
            // Work out the percentage requried to apply to hit object and apply damp as facing away from explosion 
            return (m_maxDamage * damageOverDistance) * m_damageDirectionDamp;
        }

    }
    /***************************************************
   *   Function        :  SetDamage
   *   Purpose         :  Set  damage var
   *   Parameters      :  float dm 
   *   Returns         : void
   *   Date altered    : UK
   *   Contributors    : JG
   *   Notes           :    
   *   See also        :    
   ******************************************************/
    public void SetDamage(float dmg)
    {
        m_maxDamage = dmg;
    }
    /***************************************************
    *   Function        :  ApplyEffect()
    *   Purpose         :  Apply effect of explosion
    *   Parameters      :  Collider[] hitColliders, int colliders
    *   Returns         : void
    *   Date altered    : UK
    *   Contributors    : JG
    *   Notes           :  See children for overrides    
    *   See also        :    
    ******************************************************/
    protected virtual void ApplyEffect( Collider[] hitColliders, int colliders )
    {
        // loop through colliders 
        for ( int i = 0; i < colliders; i++ )
        {
            // If damageble object
            if ( hitColliders[ i ].transform.root.GetComponentInChildren<IDamageable<float>>() != null )
            {
                // Apply damage to hit object via interface 
                hitColliders[ i ].transform.root.GetComponentInChildren<IDamageable<float>>().Damage( CalulateDamage( hitColliders[ i ].transform ) );
            }
        }
    }
    /***************************************************
    *   Function        : OnEnable
    *   Purpose         : Reset explosion
    *   Parameters      : N/A
    *   Returns         : void
    *   Date altered    : 01/08/2022 
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private void OnEnable()
    {
        m_maxDamage = k_defaultDamage;
    }
}
