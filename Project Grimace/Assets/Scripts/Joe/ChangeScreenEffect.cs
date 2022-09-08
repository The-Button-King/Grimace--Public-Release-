using Cyan;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Linq;

/****************************************************************************************************
* Type: Class
* 
* Name: ChangeScreenEffect
*
* Author: JG
*
* Purpose: Change screen effect on screen 
* 
* Functions:    void Start()
*               private void OnDisable()
*               public void ToggleEffect( Effect effect = Effect.blank )
*               public Effect GetlastEffect()
*               public IEnumerator ShieldLerp( bool reverse )
*               
* 
* References:
* 
* See Also:
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 10/07/2022    JG          1.00        - Tested if possible 
* UK            JG          1.01        - Got mutiple effects to play
* 14/07/2022    JG          1.02        - Comments 
* 20/07/2022    WH          1.03        - Added ShieldLerp
* 22/07/22      WH          1.04        - Made ShieldLerp reversible
* 14/08/2022    JG          1.05        - Clean
****************************************************************************************************/
public class ChangeScreenEffect : MonoBehaviour
{
    [SerializeField][Tooltip("Use current forward renderer ")]
    private UniversalRendererData   m_rendererData;                         // Reference to foward renderer 
    [Header("Screen effects")]
    [SerializeField]
    private Material                m_laserLock;                            // Reference to turret lock effect
    [SerializeField]
    private Material                m_lowHealth;                            // Reference to low health effect
    [SerializeField]
    private Material                m_shield;                               // Reference to shield effect
    private Blit                    m_screenEffectRenderFeature;            // Reference to render feature that displays screen effect     
    
    public enum Effect { laser, health , shield , blank}                    // Represents each effect type     
    private Effect                  m_lastEffect;                           // Last set effect

    /***************************************************
     *   Function        : Start    
     *   Purpose         : Setup class by getting the correct references   
     *   Parameters      : N/A   
     *   Returns         : Void  
     *   Date altered    : UK
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    void Start()
    {
       // Get render feature
        m_screenEffectRenderFeature = m_rendererData.rendererFeatures.OfType<Blit>().FirstOrDefault();

        // Set up render feature to start blank 
        if ( m_screenEffectRenderFeature == null ) return;

        // Set to null / blank
        m_screenEffectRenderFeature.settings.blitMaterial = null;

        // Update renderer 
        m_rendererData.SetDirty();

        m_lastEffect = Effect.blank;
    }
    /***************************************************
     *   Function        : OnDisable   
     *   Purpose         : When class disabled remove screen effects   
     *   Parameters      : N/A   
     *   Returns         : Void   
     *   Date altered    : Uk
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    private void OnDisable()
    {
        // If screen renderer is not set do nothing 
        if ( m_screenEffectRenderFeature == null )
        {
            return;

        }
        
        // Set material to blank
       m_screenEffectRenderFeature.settings.blitMaterial = null;

        // Update renderer 
        m_rendererData.SetDirty();
    }
    /***************************************************
     *   Function        : ToggleEffect  
     *   Purpose         : Switch active effect
     *   Parameters      : Effect effect = Effect.blank
     *   Returns         : Void   
     *   Date altered    : Uk
     *   Contributors    : JG
     *   Notes           :    
     *   See also        :    
     ******************************************************/
    public void ToggleEffect( Effect effect = Effect.blank )
    {
        // If paramater effect is not current effect
        if ( m_lastEffect != effect ) 
        {
            // Uodate last effect 
            m_lastEffect = effect;

            switch ( effect )
            {
                // Switch blit material based on enum
                case Effect.laser:
                    {
                        m_screenEffectRenderFeature.settings.blitMaterial = m_laserLock;
                    }
                    break;
                case Effect.health:
                    {
                        m_screenEffectRenderFeature.settings.blitMaterial = m_lowHealth;
                    }
                    break;
                case Effect.shield:
                    {
                        m_screenEffectRenderFeature.settings.blitMaterial = m_shield;
                    }
                    break;
                case Effect.blank:
                    {
                        m_screenEffectRenderFeature.settings.blitMaterial = null;
                    }
                    break;
            }

            // Update renderer 
            m_rendererData.SetDirty();
            
        }
       
    }
    /***************************************************
    *   Function        : GetlastEffect
    *   Purpose         : return last set effect
    *   Parameters      : N/A
    *   Returns         : Effect m_lastEffect  
    *   Date altered    : Uk
    *   Contributors    : JG
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public Effect GetlastEffect()
    {
        return m_lastEffect;
    }

    /***************************************************
    *   Function        : ShieldLerp
    *   Purpose         : Lerp shield away
    *   Parameters      : None
    *   Returns         : void
    *   Date altered    : 22/07/22
    *   Contributors    : WH
    *   Notes           :    
    *   See also        :    
    ******************************************************/
    public IEnumerator ShieldLerp( bool reverse )
    {
        // Lerp float
        float t = 0;

        // Start and end values. These magics are the bounds for the shield effect
        float start = 0.2f;
        float end = 3.3f;

        // Swap the values if reversing
        if ( reverse )
        {
            start = 3.3f;
            end = 0.2f;
        }

        // While lerping. t > 1 so there the effect doesn't instantly end.
        while ( t <= 1.1f )
        {
            // Show shield
            ToggleEffect( Effect.shield );

            // Lerp the progress value to shrink or grow the shield effect
            m_shield.SetFloat( "_Progress", Mathf.Lerp( start, end, t ) );

            // Increase t
            t += 0.4f * Time.deltaTime;

            yield return null;
        }

        // Turn off effect when finished
        ToggleEffect();
    }

   
}
