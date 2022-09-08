using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
/****************************************************************************************************
* Type: Class
* 
* Name: Basic Gun
*  Author: Joseph Gilmore
* Purpose: Used as a basic wepeaon model for gameobjects to shoot such as ai and player 
* 
* Functions:        
*                   protected virtual void Awake()
*                   protected virtual void Update()
*                   public void Shoot( Transform fireFromPosition, Vector3 direction = new Vector3() )
*                   private void DealDamage( RaycastHit hitObject )
*                   private void  CreateShootingEffects( RaycastHit hitObject )
*                   private IEnumerator FadeMesh()
*                   protected RaycastHit FireGunRay( Transform fireFromTransform, Vector3 direction = new Vector3() )
*                   private IEnumerator RenderTracer( RaycastHit hitObject )
*                   private IEnumerator ReturnToPoolAfterPlayed(GameObject obj)
*                   public Transform GetFirePos()
*                   protected void FireMuzzle()
*                   public float GetBaseDamage()
*                   public void SetBaseDamage(float dmg)

* References:
* 
* See Also:
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* uk            JG          1.00         - Class create very basic raycast shooting 
* 3/05/2022     JG          1.01         - completety change the gun to have children.
* 05/05/2022    JG          1.02         - Altered decal fade to work off camera 
* 30/05/2022    JG          2.00         - Added pooling
* 02/06/2022    JG          2.01         - Replaced decals with decal projector to allow decals to bend with the mesh they are projected on.
* 13/06/2022    WH          2.02         - Added shooting sound
* 19/06/2022    JG          2.03         - Changed apply damage to use interface
* 30/06/2022    JG          2.04         - Altered vars 
* 01/07/2022    JG          2.05         - Added dust 
* 05/07/2022    JG          2.06         - Setters & getters for AI data
* 14/07/2022    JG          2.07         - Minor cleaning 
* 28/07/2022    JG          2.08         - Range of blood effects
* 06/08/2022    JG          2.09         - Can't shoot self
* 14/08/2022    JG          2.10         - Clean
****************************************************************************************************/
public class BasicGun : MonoBehaviour
{

    [SerializeField][Tooltip("Ensure the asset prefab is in the scene and drag into this slot")]
    private AssetPool                   m_assetPool;                            // Reference to pool of assets 
    [Header("VFX for guns")]

    [SerializeField][Tooltip("flash of gun played the barrel on the gun firing ")]
    private  GameObject                 m_muzzleFlash;                          // Muzzle flash effect asset
    [SerializeField][Tooltip("Blood splatter played on AI hit")]
    private List<GameObject>            m_blood;                                 // Blood effects
  
   
    [SerializeField]
    [Tooltip("tracer to the bullet")]
    private GameObject                  m_bulletTracer;                         // Bullet tracer prefab asset 

    [SerializeField]
    [Tooltip("when a bullet hits a wall create a decal")]
    private GameObject                  m_bulletHole;                           // Bullet hole decal

    [SerializeField][Tooltip("dust spawned when bullet hits a surface")]
    private GameObject                  m_dustEffect;
    [SerializeField]
    protected Transform                 m_firePosition;                         // Position at the end of the barrel of the gun 

    [SerializeField][Tooltip("Time it takes for the decal to fade once not in view")]
    private float                       m_decalFadeCheck = 6.0f;                  // Time Taken to decal to fade after not in view 

    private List<(GameObject, bool)>    m_activeBulletHoles;                    // List of bullet decals active 

    private bool                        m_checkFade = false;                    // Check if currently dading

    [SerializeField]
    [Tooltip("Damage per one bullet of the gun base")]
    protected float                     m_baseDamage = 10.0f;                   // Base Gun Damage

    [SerializeField]
    protected  bool                     m_flashPlay = true;                     // Should muzzle flash play 
    [Tooltip("Layers th gun should be able to hit")]
    public LayerMask                    m_gunLayers;                            // Layers the gun can hit
   
     /***************************************************
     *   Function        : Awake 
     *   Purpose         : Set up var References   
     *   Parameters      : N/A  
     *   Returns         : Void  
     *   Date altered    : 13/06/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    protected virtual void Awake()
    {

        // Get reference to asset pool
        m_assetPool = GameObject.Find( "Pool" ).GetComponent<AssetPool>();

        // Create the list of bullet holes 
        m_activeBulletHoles = new List<(GameObject,bool)>();
    }

    /***************************************************
     *   Function        : Update  
     *   Purpose         : Check if bullets need fading    
     *   Parameters      : N/A  
     *   Returns         : Void    
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    protected virtual void Update()
    {
        // Check if bullets need fading 
        if(m_checkFade == false)
        {
            // Fade bullets 
            StartCoroutine(FadeMesh());
        }

    }
    /***************************************************
     *   Function        : Shoot 
     *   Purpose         : Set up the correct functions for shooting 
     *   Parameters      : Transform fireFromPosition, Vector3 direction = new Vector3() 
     *   Returns         : Void    
     *   Date altered    :  31/07/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        : All children & AI shoot at target task   
     ******************************************************/
    public void Shoot( Transform fireFromPosition, Vector3 direction = new Vector3() )
    {
       
        // Fire gun 
        RaycastHit hitObject = FireGunRay(fireFromPosition, direction);

        // Create affects for the gun
        StartCoroutine(RenderTracer(hitObject));
        CreateShootingEffects(hitObject);

        // Apply damage to hit object 
        DealDamage( hitObject);
        
     
    }
    /***************************************************
     *   Function        : DealDamage   
     *   Purpose         : To deal damage of hit object from firing  
     *   Parameters      :RaycastHit hitObject   
     *   Returns         : void   
     *   Date altered    : 06/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void DealDamage( RaycastHit hitObject )
    {
        // If hit item is not null and self 
       if(hitObject.transform.GetComponent<IDamageable<float>>() != null && transform.root != hitObject.transform.root)
       {
            // Damage hit object 
            hitObject.transform.GetComponent<IDamageable<float>>().Damage( m_baseDamage );

            // If hit mage give mage information about who hit them
            if ( hitObject.transform.root.GetComponent<Mage>() != null )
            {
                hitObject.transform.root.GetComponent<Mage>().SetHitByEntitie( transform.root.gameObject );
            }
       }
    }
    /***************************************************
     *   Function        : CreateShootingEffects   
     *   Purpose         : Create effects for the gun firing and its impact  
     *   Parameters      : RaycastHit hitObject    
     *   Returns         : Void 
     *   Date altered    : 14/08/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void  CreateShootingEffects( RaycastHit hitObject )
    {
        // If flash requried ( Some guns don't use flash)
        if(m_flashPlay == true)
        {
            FireMuzzle();
        }

        // Check which decals to spawn depending on whats hit 
        if (hitObject.transform.tag != "Player" && hitObject.transform.root.tag != "AI")
        {
            // If it does not hit a creature get a decal from pool and set to hit position
            GameObject tempHit = m_assetPool.GetObjectFromPool(m_bulletHole, hitObject.point, Quaternion.LookRotation( hitObject.normal ));
            GameObject tempDust = m_assetPool.GetObjectFromPool(m_dustEffect, hitObject.point , Quaternion.LookRotation( hitObject.normal ) );

            // Reset decal projector 
            tempHit.GetComponentInChildren<DecalProjector>().fadeFactor = 1;


            // Set the rotataion relative to direction hit 
            tempHit.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitObject.normal);
            tempDust.transform.rotation = Quaternion.FromToRotation(Vector3.up,hitObject.normal);

            // Add decal to list of exsiting decals 
            (GameObject, bool) tempTurple = (tempHit, false);
            m_activeBulletHoles.Add(tempTurple);

            // Return dust to pool afer its finsihed 
            StartCoroutine(ReturnToPoolAfterPlayed(tempDust));
        }
        if( hitObject.transform.root.tag == "AI" )
        {
            // Get  random blood from pool
            GameObject blood = m_assetPool.GetObjectFromPool( m_blood[Random.Range(0,m_blood.Count)], hitObject.point , Quaternion.LookRotation( hitObject.normal ) );

            // Make blood splatte relative to hit
           blood.transform.rotation = Quaternion.FromToRotation( Vector3.up, hitObject.normal );

            // Return to pool after effect
            StartCoroutine(ReturnToPoolAfterPlayed( blood ));
        }
       
    }
    /***************************************************
     *   IEnumerator     : FadeMesh   
     *   Purpose         : Fade decals after not visable on cam  
     *   Parameters      : N/A   
     *   Returns         : yield return null  
     *   Date altered    : 02/06/2022
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private IEnumerator FadeMesh()
    {
        //Fade started
        m_checkFade = true;

        // Loop through & check which ones are in view of the player 
        for (int i = 0; i < m_activeBulletHoles.Count; i++)
        {
            // Check if renderer is in view ( I use a very small material as the decal projector does not allow visible check)
            if ( m_activeBulletHoles[i].Item1.GetComponentInChildren<Renderer>().isVisible == false )
            {
                // Set to can start fading 
                ( GameObject, bool ) tempTuple = (m_activeBulletHoles[i].Item1, true );
                m_activeBulletHoles[i] = tempTuple;
            }
        }
        //Wait to start fading
        WaitForSeconds wait = new WaitForSeconds(m_decalFadeCheck);
        yield return wait;

        // Loop through 
        for (int i = 0; i < m_activeBulletHoles.Count; i++)
        {

            // If tagged for fading start 
            if ( m_activeBulletHoles[i].Item2 == true )
            {
                float fadeTimer = 0.0f;

                // Fade bullet hole 
                while (fadeTimer < 1.0f)
                {
                    // Add to the fade timer 
                    fadeTimer += Time.deltaTime;

                    // Update alpha (Opacity of projector) via a lerp
                    m_activeBulletHoles[i].Item1.GetComponentInChildren<DecalProjector>().fadeFactor = Mathf.Lerp( 1.0f, 0.0f, fadeTimer );
                }


                // Return Item to pool as no longer active 
                m_assetPool.ReturnObjectToPool( m_activeBulletHoles[i].Item1 );

                // Remove from list 
                ( GameObject, bool ) temp = ( m_activeBulletHoles[i].Item1, m_activeBulletHoles[i].Item2 );
                m_activeBulletHoles.Remove( temp );
            }
        }
        // Fade complete 
        m_checkFade = false;
        yield return null;

    }
    /***************************************************
     *   Function        : FireGunRay   
     *   Purpose         : Fire ray cast    
     *   Parameters      : Transform fireFromTransform, Vector3 direction = new Vector3()
     *   Returns         : void     
     *   Date altered    : 04/07/2022
     *   Contributors    : JG 
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    protected RaycastHit FireGunRay( Transform fireFromTransform, Vector3 direction = new Vector3() )
    {
        RaycastHit hitObject;

        // If no direction past through use the forward of the object 
        if( direction == Vector3.zero )
        {
            direction = fireFromTransform.TransformDirection(Vector3.forward);
        }

        // Fire Raycast from firfromTransform and in a forward direction 
        if ( Physics.Raycast(fireFromTransform.position, direction, out hitObject,Mathf.Infinity,m_gunLayers) )
        {
           
            return hitObject;
        }
        else
        {
            // If you shoot the void or an object is missing a collider 
            Debug.LogError("nothing was it");
            return hitObject;
        }
    }
    /***************************************************
    *   IEnumerator     :  RenderTracer 
    *   Purpose         :  To render trails of bullets    
    *   Parameters      :  RaycastHit hitObject  
    *   Returns         :  yield return null
    *   Date altered    :  14/08/2022
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator RenderTracer( RaycastHit hitObject )
    {
        // Create a trail
        GameObject tracer = m_assetPool.GetObjectFromPool(m_bulletTracer, m_firePosition.position , m_firePosition.rotation );


        // Get starting position 
        Vector3 lerpFromPos = tracer.transform.position;

        float tracerTimer = 0.0f;

        // Lerp timer 
        while (tracerTimer < 1.0f)
        {

            tracerTimer += Time.deltaTime / tracer.GetComponentInChildren<TrailRenderer>().time;

            // Lerp tracer to the hit point 
            tracer.transform.position = Vector3.Lerp(lerpFromPos, hitObject.point, tracerTimer);

            yield return null;
        }

        // Set to hit point for absulte position
        tracer.transform.position = hitObject.point;

        // Return tracer to pool
        m_assetPool.ReturnObjectToPool(tracer);

        yield return null;
    }
    /***************************************************
    *   IEnumerator     :  ReturnToPoolAfterPlayed
    *   Purpose         :  Return effect after its finished playing  
    *   Parameters      :  GameObject obj
    *   Returns         :  yield return null
    *   Date altered    :  UK
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    private IEnumerator ReturnToPoolAfterPlayed(GameObject obj)
    {
        // Get the effect length
        WaitForSeconds wait = new WaitForSeconds(obj.GetComponentInChildren<ParticleSystem>().main.duration);

        // Wait
        yield return wait;

        // Return object to pool
        m_assetPool.ReturnObjectToPool(obj);

        yield return null;

    }
    /***************************************************
    *   Function        :  GetFirePos
    *   Purpose         : Get firepost of gun
    *   Parameters      : N/A
    *   Returns         : m_firePosition;
    *   Date altered    :  23/07/2022
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public Transform GetFirePos()
    {
        return m_firePosition;
    }
    /***************************************************
    *   Function        : Fire Muzzle
    *   Purpose         : Fires muzzle flash
    *   Parameters      : N/A
    *   Returns         : Void
    *   Date altered    : 14/08/2022
    *   Contributors    :  JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    protected void FireMuzzle()
    {
        // Get muzzle flash from pool
        GameObject tempMuzzle = m_assetPool.GetObjectFromPool( m_muzzleFlash, m_firePosition.transform.position , m_firePosition.rotation );

        // Start coroutine to remove flash once done
        StartCoroutine(ReturnToPoolAfterPlayed( tempMuzzle ));
    }

    /***************************************************
    *   Function        : GetBaseDamage
    *   Purpose         : Gets the base damage 
    *   Parameters      : None
    *   Returns         : float dmg
    *   Date altered    : 28/07/2022
    *   Contributors    : WH
    *   Notes           :  
    *   See also        :    
    ******************************************************/
    public float GetBaseDamage()
    {
        return m_baseDamage;
    }

   /***************************************************
   *   Function        : SetBaseDamage
   *   Purpose         : set the base damage 
   *   Parameters      : float dmg
   *   Returns         : Void
   *   Date altered    : 05/07/2022
   *   Contributors    :  JG
   *   Notes           : Used for AI data   
   *   See also        :    
   ******************************************************/
    public void SetBaseDamage( float dmg )
    {
        m_baseDamage = dmg;
    }
}
