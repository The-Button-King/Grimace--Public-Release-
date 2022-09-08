using LayerMasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: ShockWave
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Create a radius of effect that expands over time casuing a shock wave 
 * 
 * Functions:			
 * 
 * References:		protected virtual void Start()
 *					protected virtual IEnumerator StartShockWave()
 *					private void DrawAreaOfEffect( float radius )
 *					private Collider[] AreaOfEffect( float radius )
 *					protected virtual void ApplyEffect(Collider[] hitColliders, int collideCount )
 *					public void  SetShockWavePosition(Vector3 pos )
 *					public void SetLineRenderer(LineRenderer line )
 *					protected void SetUpLine()
 *					
 * 
 * See Also: ForceWave
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 21/07/2022    JG			1.00		- Moved code from groundslamtask for better code structure 
 ****************************************************************************************************/
public class ShockWave : MonoBehaviour
{
	[SerializeField][Tooltip("Line renderer  for shockwave")]
	protected LineRenderer		m_shockWaveEffect;							// Reference to Line Renderer 
	protected float				m_shockWaveRadius = 0.0f;					// Current shockwave radius 
	[SerializeField][Range(3.0f,20.0f)][Tooltip("Size of wave")]
	protected float				m_maxRadius = 8.0f;                         // Size of wave 
	[SerializeField][Range( 3.0f, 20.0f )]	[Tooltip( "Wave speed" )]
	private float				m_waveSpeed = 5.0f;							// Speed of wave increasing size
	private int					m_points = 50;								// Amount of points in line renderer 
	private float				m_startWidth;								// Start Width of line renderer 
	protected Vector3			m_shockwavePos;								// Center origin of shockwave 
	protected List<GameObject>	m_hitObjects = new List<GameObject>();		// List of hit objects
	[SerializeField]														
	protected LayerMask			m_layerMasks;                               // Layers to apply shock to 
	private const int			k_maxColliders = 20;						// Max amount of colliders the shockwave can detect 
	private int					m_collidersCount = 0;                       // Store the count
	/***************************************************
	 *   Function        : Start 
	 *   Purpose         : Setup the class
	 *   Parameters      : N/A  
	 *   Returns         :  
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	protected virtual void Start()
	{
		// Set up the line renderer 
		SetUpLine();

		// Set layer mask
		m_layerMasks = LayerMask.GetMask( Layers.EnviromentHazard.ToString() );

		
	}
	/***************************************************
	 *   Function        : StartShockWave   
	 *   Purpose         : Start the shock wave , apply effects and get information
	 *   Parameters      : N/A  
	 *   Returns         : yield return null   
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	protected virtual IEnumerator StartShockWave()
	{
		// Reset the list of hit objects
		m_hitObjects = new List<GameObject>();

		// Reset shockwave radius 
		m_shockWaveRadius = 0.0f;

		// ShockWave started enable effect
		m_shockWaveEffect.enabled = true;

		while ( m_shockWaveRadius < m_maxRadius )
		{
			// Increase radius of wave 
			m_shockWaveRadius += m_waveSpeed * Time.deltaTime;

			// Create shockwave with current radius 
			ApplyEffect(AreaOfEffect( m_shockWaveRadius),m_collidersCount);

			// Draw wave 
			DrawAreaOfEffect( m_shockWaveRadius );

			yield return null;
		}
		// Disable as wave complete 
		m_shockWaveEffect.enabled =  false;
	}
	/***************************************************
	*   Function        :  DrawAreaOfEffect   
	*   Purpose         :  Draw a line renderer in a circle using the radius of the s   
	*   Parameters      :  float radius 
	*   Returns         :  Void  
	*   Date altered    :  21/07/2022
	*   Contributors    :  JG
	*   Notes           :  Function Reference from here : www.youtube.com. (n.d.). BLAST WAVE EXPLOSION | UNITY. [online] Available at: https://www.youtube.com/watch?v=7wmRs3LNtl8 [Accessed 21 Jul. 2022].?
	*   See also        :    
	******************************************************/
	private void DrawAreaOfEffect( float radius )
	{
		// Work out the angle for each line renderer point 
		float anlgeBetweenPoints = 360.0f / m_points;

		// Loop through  each point of the liner renderer 
		for ( int i = 0; i <= m_points; i++ )
		{
			// Get angle for current point by using the point index 
			float angle = i * anlgeBetweenPoints * Mathf.Deg2Rad;
			
			// Work out the direction of the point using the sin & cos of the angle 
			Vector3 direction = new Vector3( Mathf.Sin( angle ), Mathf.Cos( angle ), 0.0f );

			// Calculuate the position by combining curret radius ad direction 
			Vector3 position = direction * radius;

			// Set point to the correct index in the line renderer 
			m_shockWaveEffect.SetPosition( i, position );
		}

		// Lerp the width of the line so it starts small gets big then go back to small
		m_shockWaveEffect.widthMultiplier = Mathf.Lerp( 0, m_startWidth, 1.0f - radius / m_maxRadius );
	}
	/***************************************************
	 *   Function        : AreaOfEffect 
	 *   Purpose         : Work out the effected gameObjects within the shockwave radius 
	 *   Parameters      : Collider[] hitColliders 
	 *   Returns         : Void
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private Collider[] AreaOfEffect( float radius )
	{
		
		// Create array for colliders 
		Collider[] hitColliders = new Collider[ k_maxColliders ];

		
		// Get the amount of colliders detected in sphere in certain layers 
		m_collidersCount =  Physics.OverlapSphereNonAlloc( m_shockwavePos ,radius, hitColliders, m_layerMasks );


		return hitColliders;
	}
	/***************************************************
	 *   Function        : ApplyEffect
	 *   Purpose         : Used to apply effetc in children
	 *   Parameters      :  Collider[] hitColliders, int collideCount
	 *   Returns         : Void
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           : Used for children 
	 *   See also        :    
	 ******************************************************/
	protected virtual void ApplyEffect(Collider[] hitColliders, int collideCount )
    {
		// children
    }
	/***************************************************
	 *   Function        : SetShockWavePosition
	 *   Purpose         : used to set orgin or shockwave
	 *   Parameters      : Vector3 pos
	 *   Returns         : Void
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public void  SetShockWavePosition(Vector3 pos )
    {
		m_shockwavePos = pos;
    }
	/***************************************************
	 *   Function        : SetLineRenderer
	 *   Purpose         : used to set line renderer
	 *   Parameters      : Vector3 pos
	 *   Returns         : Void
	 *   Date altered    : 21/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/

	public void SetLineRenderer(LineRenderer line )
    {
		m_shockWaveEffect = line; 
    }
	/***************************************************
	 *   Function        : SetUpLine
	 *   Purpose         : Setup line renderer 
	 *   Parameters      : N/A
	 *   Returns         : Void
	 *   Date altered    : 24/08/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/

	protected void SetUpLine()
    {
		// Setup shockwave
		m_shockWaveEffect.positionCount = m_points + 1;
		m_startWidth = m_shockWaveEffect.startWidth;
	}
}
