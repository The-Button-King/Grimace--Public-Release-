using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
/****************************************************************************************************
 * Type: Class
 * 
 * Name: Cover
 *
 * Author: Joseph Gilmore
 *
 * Purpose: Assigned to cover mesh to allocate accurate peak positions for AI
 * 
 * Functions:		private void Awake()
 *					public void SetPeakPositions( Vector3 searchPos )
 *					private void OrderBestPeakPos( Vector3 searchPos )
 *					public void SetAgent( NavMeshAgent agent )
 *					public Vector3 GetPeakPoint()
 *					public void ReturnPeakPoint( Vector3 point )
 *					public void SetCoverPoints( )
 *					private void  CheckPointsAlongZAxis(Vector3 positionToCheckFrom, float bound, CoverSide side)
 *					private void CheckPointsAlongXAxis( Vector3 positionToCheckFrom, float bound, CoverSide side )
 *					public Vector3 ReturnBestCoverPos( Vector3 hitPoint )
 *					private Vector3 GetVaildPointFromList( List<(Vector3,bool)> list )
 *					public void ReturnCoverPoint( Vector3 point )
 *								
 * 
 * References:
 * 
 * See Also:
 *
 * Change Log:
 * Date          Initials    Version     Comments
 * ----------    --------    -------     ----------------------------------------------
 * 12/04/2022    JG			1.00		- Class created 
 * 14/07/2022    JG			1.01		- Comments 
 * 17/07/2022    JG		    1.02		- More cleaning 
 * 18/07/2022    JG			1.03		- Continued to clean (functions might be able to be compressed in the future but due to time I cannot continue to improve)
 * 15/08/2022    JG			1.04		- Cleaned best as possible with time left 
 ****************************************************************************************************/
public class Cover : MonoBehaviour
{
	private List<(Vector3, bool)>							m_peakPositions = new List<(Vector3, bool)> ();													// A tuple of peak positions and if they are in use
	private NavMeshAgent									m_agent;																						// Reference to the agent 
	private bool											m_positionsSet = false;																			// Have peak positions been set
	private bool											m_coverSet = false;																				// Has cover positions
	private Vector3											m_negativeZSide;																				// Negative z side of cover
	private Vector3											m_negativeXSide;																				// Negative x side of cover	
	private Vector3											m_positiveXSide;																				// Positive x side of cover	
	private Vector3											m_positiveZSide;																				// Positive z side of cover
	private Dictionary<CoverSide, List<(Vector3, bool)>>	m_coverPositions = new Dictionary<CoverSide,List< (Vector3, bool)>>();							// Dictionary that stores points of each side of the cover and wherever if they are in use or not
	private enum CoverSide									{ PositiveX, NegativeX,PositiveZ, NegativeZ};													// Enums of cover sides
	private CoverSide										m_side;                                                                                         // Current Side 
	private float											m_peakBoundOffset = 1.0f;                                                                       // peak position offset
	private float											m_coverPosGapOffset = 0.2f;																		// Used to space cover positions 
	/***************************************************
	 *   Function        : Awake   
	 *   Purpose         : Setup cover     
	 *   Parameters      : N/A   
	 *   Returns         : Void   
	 *   Date altered    : UK
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private void Awake()
    {
		// Populate Dictionary
        m_coverPositions.Add(CoverSide.PositiveX, new List<(Vector3, bool)> ());
		m_coverPositions.Add( CoverSide.NegativeX, new List<(Vector3, bool)>() );
		m_coverPositions.Add( CoverSide.PositiveZ, new List<(Vector3, bool)>() );
		m_coverPositions.Add( CoverSide.NegativeZ, new List<(Vector3, bool)>() );
	}

	/***************************************************
	*   Function        : SetPeakPositions  
	*   Purpose         : Set the posbile peak positions of the AI on this cover   
	*   Parameters      : Vector3 searchPos    
	*   Returns         : Void  
	*   Date altered    : 14/07/2022
	*   Contributors    : JG
	*   Notes           :    
	*   See also        :    
	******************************************************/
	public void SetPeakPositions( Vector3 searchPos )
	{
		if(m_positionsSet == false )
		{
			// Create a new list of peak positions
		 	m_peakPositions = new List<(Vector3, bool)>();

			// Create a local navmesh hit for later 
			NavMeshHit hit;

			// Get bounds of cover 
			Vector3 bounds = transform.GetComponent<Collider>().bounds.size;

			bounds *= 0.5f;
			
			// Work out bounds offsets 
			float x = bounds.x  + m_peakBoundOffset;
			float z = bounds.z  + m_peakBoundOffset;
			
			
			// Set local vector a short hand 
			Vector3 coverPos = transform.position;

			// Add each potential peak position to a list 
			List<Vector3> corners = new List<Vector3>();
			corners.Add( new Vector3( coverPos.x - x, coverPos.y - bounds.y, coverPos.z - z ) );
			corners.Add(  new Vector3( coverPos.x + x, coverPos.y - bounds.y, coverPos.z - z ));
			corners.Add( new Vector3( coverPos.x - x, coverPos.y - bounds.y, coverPos.z + z ) );
			corners.Add( new Vector3( coverPos.x + x, coverPos.y - bounds.y, coverPos.z + z ) );

			// Loop through potential 
			foreach(Vector3 corner in corners )
            {
				// Check if peak position is on a navmesh
                if ( NavMesh.SamplePosition(corner, out hit, 1f, m_agent.areaMask ) )
                {
					// If so add to a list of peak positions 
                    (Vector3, bool) tempTuple = (hit.position, false);
                    m_peakPositions.Add( tempTuple );
                }
            }
			m_positionsSet = true;

		}


		// Order peak pos in the most valuable order
		OrderBestPeakPos( searchPos );
	}
	/***************************************************
	 *   Function        : OrderBestPeakPos 
	 *   Purpose         : Order the peak positions that have most value 
	 *   Parameters      : Vector3 searchPos    
	 *   Returns         : Void  
	 *   Date altered    : 14/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private void OrderBestPeakPos( Vector3 searchPos )
    {
		// If more than one position compare the 
		if ( m_peakPositions.Count > 1 )
        {
			// Order by which is closest to AI 
			m_peakPositions = m_peakPositions.OrderBy(x => Vector3.Distance( m_agent.transform.position, x.Item1 )).ToList();

			// Get the closets two points 
			Vector3 pointOne = m_peakPositions[ 0 ].Item1;
			Vector3 pointTwo = m_peakPositions[ 1 ].Item1;

			// Check the closest two points to the AI and see which is closer to players last positon 
			if ( Vector3.Distance( pointTwo, searchPos ) < Vector3.Distance( pointOne, searchPos ) )
			{
				// Reorder to make the closer point at the top of the list 
				( Vector3, bool ) tempTuple = (pointTwo, false);
				m_peakPositions[ 0 ] = tempTuple;

				tempTuple = (pointOne, false);
				m_peakPositions[ 1 ] = tempTuple;
			}
			// Peak positions are now in order of best
		}



	}
	/***************************************************
	 *   Function        : SetAgent
	 *   Purpose         : Set current agent using cover
	 *   Parameters      : NavMeshAgent agen   
	 *   Returns         : Void  
	 *   Date altered    : 13/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public void SetAgent( NavMeshAgent agent )
    {
		m_agent = agent;
    }
	/***************************************************
	 *   Function        : GetPeakPoint
	 *   Purpose         : return valid peak point 
	 *   Parameters      : N/A
	 *   Returns         : Vector3 peak point
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public Vector3 GetPeakPoint()
    {
		return GetVaildPointFromList( m_peakPositions );
    }

	/***************************************************
	 *   Function        : ReturnPeakPoint
	 *   Purpose         : When AI is no longer using peak pos can now be used 
	 *   Parameters      : Vector3 point
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public void ReturnPeakPoint( Vector3 point )
    {
		// Loop through points 
		for( int i = 0; i < m_peakPositions.Count; i++ )
        {
			// If the peak pos in use and is the same as the paramater 
			if( m_peakPositions[ i ].Item2 == true && m_peakPositions[ i ].Item1 == point )
            {
				// Pos can be used again
				( Vector3,bool ) tuple = ( m_peakPositions[ i ].Item1, false );
				m_peakPositions[i ] = tuple;
				break;
            }
        }
    }
	/***************************************************
	 *   Function        : SetCoverPoints
	 *   Purpose         : Set positions that can be used as cover
	 *   Parameters      : N/A
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public void SetCoverPoints( )
    {
		// Has cover already been set 
		if( m_coverSet == false )
        {
			// Create a	 local nav mesh hit 
			NavMeshHit hit;

			// Get bounds of cover 
			Vector3 bounds = transform.GetComponent<Collider>().bounds.size;

			// Setup bounds offset
			Vector3 boundsOffSet = new Vector3( bounds.x * 0.5f + m_agent.radius, bounds.y * 0.5f, bounds.z * 0.5f + m_agent.radius );
			
			// Create local short hand
			Vector3 coverPos = transform.position;

			// Calculate top right pos 
			m_positiveXSide = new Vector3( coverPos.x + boundsOffSet.x, coverPos.y- boundsOffSet.y, coverPos.z  );

            #region Calulate  a point on each side 
            // Check if its on a navmesh
            if ( NavMesh.SamplePosition( m_positiveXSide, out hit, 1f, m_agent.areaMask ) )
			{
				CheckPointsAlongZAxis( hit.position, bounds.z, CoverSide.PositiveX );
				m_positiveXSide = hit.position;
			}

			// Repeat for each corner 
			m_negativeXSide= new Vector3( coverPos.x - boundsOffSet.x, coverPos.y - boundsOffSet.y, coverPos.z );
			

			// If point on navmesh add to list of peak positions 
			if ( NavMesh.SamplePosition( m_negativeXSide, out hit, 1f, m_agent.areaMask ) )
			{
				// Create points along negative x side 
				CheckPointsAlongZAxis( hit.position, bounds.z, CoverSide.NegativeX );
				m_negativeXSide = hit.position;
			}

			m_negativeXSide= new Vector3( coverPos.x  , coverPos.y - boundsOffSet.y, coverPos.z - boundsOffSet.z );
			
			// If point on navmesh add to list of peak positions
			if ( NavMesh.SamplePosition( m_negativeZSide, out hit, 1f, m_agent.areaMask ) )
			{

				CheckPointsAlongXAxis( hit.position, bounds.x, CoverSide.NegativeZ );
				m_negativeZSide = hit.position;
			}

			m_positiveZSide = new Vector3( coverPos.x  , coverPos.y - boundsOffSet.y, coverPos.z + boundsOffSet.z );
		
			if ( NavMesh.SamplePosition(m_positiveZSide, out hit, 1f, m_agent.areaMask ) )
			{

				CheckPointsAlongXAxis( hit.position, bounds.x, CoverSide.PositiveZ );
				m_positiveZSide= hit.position;
			}
            #endregion
            m_coverSet = true;
        }
	}
	/***************************************************
	 *   Function        : CheckPointsAlongZAxis
	 *   Purpose         : Check points along the z side of the cover 
	 *   Parameters      : Vector3 positionToCheckFrom, float bound, CoverSide side
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private void  CheckPointsAlongZAxis(Vector3 positionToCheckFrom, float bound, CoverSide side)
    {
		NavMeshHit hit;
		
		
		int maxAmountOfPosAlongAxis = ( int )Mathf.Floor( bound / m_agent.radius + m_coverPosGapOffset );
	
		// If its more than one half the amount and take one off so the cover position is not close to the edge( I half it for the next loops as I iterate from the center not either end)
		if ( maxAmountOfPosAlongAxis > 1 )
		{
			maxAmountOfPosAlongAxis = ( int )Mathf.Floor( maxAmountOfPosAlongAxis * 0.5f ) - 1;
		}
		
		
		// From the center cover position reference iterate to the right by looping through half the max amount of cover positions can fit on that cover side 
		for(int i = 0; i < maxAmountOfPosAlongAxis; i++ )
        {
			// Using loop iterater from the center of the side add to the z in set ingrements 
			Vector3 checkVector = new Vector3( positionToCheckFrom.x, positionToCheckFrom.y, positionToCheckFrom.z + (m_agent.radius + m_coverPosGapOffset) * i);

			//  Check point is on navmesh and within the bound length of the cover side 
			if ( NavMesh.SamplePosition( checkVector, out hit, 1f, m_agent.areaMask ) && checkVector.z < transform.position.z + bound - m_agent.radius + m_coverPosGapOffset )
			{
				// Get list of points of that side from the dictornary 
                List<(Vector3, bool)> value;
                m_coverPositions.TryGetValue(side,out value);
				
				// Add point to said list 
				(Vector3,bool) tempTuple = (hit.position,false);
				value.Add(tempTuple);
            }
            else
            {
				break;
            }
			
		}

		// Do the same in the oppersite direction 
		for ( int i = 0; i < maxAmountOfPosAlongAxis; i++ )
		{
			Vector3 checkVector = new Vector3( positionToCheckFrom.x, positionToCheckFrom.y, positionToCheckFrom.z - ( m_agent.radius + m_coverPosGapOffset ) * i );
	
			if ( NavMesh.SamplePosition(checkVector, out hit, 1f, m_agent.areaMask ) && checkVector.z > transform.position.z - bound + m_agent.radius + m_coverPosGapOffset)
			{
				// Get list of points of that side from the dictornary 
				List<(Vector3, bool)> value;
				m_coverPositions.TryGetValue( side, out value );

				// Add point to said list 
				(Vector3, bool) tempTuple = (hit.position, false);
				value.Add( tempTuple );
			}
			else
			{
				break;
			}
		}
	}
	/***************************************************
	 *   Function        : CheckPointsAlongXAxis
	 *   Purpose         : Check points along the x side of the cover 
	 *   Parameters      : Vector3 positionToCheckFrom, float bound, CoverSide side
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private void CheckPointsAlongXAxis( Vector3 positionToCheckFrom, float bound, CoverSide side )
	{
		// Create a local navmesh hit
		NavMeshHit hit;
		
		// Work out the max amounr of points along the axis 
		int maxAmountOfPosAlongAxis = ( int )Mathf.Floor( bound / m_agent.radius + m_coverPosGapOffset );

		if ( maxAmountOfPosAlongAxis > 1 )
		{
			maxAmountOfPosAlongAxis = ( int )Mathf.Floor( maxAmountOfPosAlongAxis * 0.5f ) - 1;
		}
		

		for ( int i = 0; i < maxAmountOfPosAlongAxis; i++ )
		{
			// Using loop iterater from the center of the side add to the x in set ingrements 
			Vector3 checkVector = new Vector3( positionToCheckFrom.x + ( m_agent.radius + m_coverPosGapOffset ) * i, positionToCheckFrom.y, positionToCheckFrom.z  );

			// If within the bounds of the side and on a navmesh
			if ( NavMesh.SamplePosition( checkVector, out hit, 1f, m_agent.areaMask ) && checkVector.x < transform.position.x + bound - m_agent.radius + m_coverPosGapOffset )
			{
				// Add point to the side list within the dictornary 
				List<(Vector3, bool)> value;
				m_coverPositions.TryGetValue( side, out value );
				(Vector3, bool) tempTuple = (hit.position, false);
				value.Add( tempTuple );
			}
			else
			{
				break;
			}

		}
		for ( int i = 0; i < maxAmountOfPosAlongAxis; i++ )
		{
			Vector3 checkVector = new Vector3( positionToCheckFrom.x - ( m_agent.radius + m_coverPosGapOffset ) * i, positionToCheckFrom.y, positionToCheckFrom.z );
			if ( checkVector.x < transform.position.x - bound + m_agent.radius + m_coverPosGapOffset )
			{
				break;
			}
			if ( NavMesh.SamplePosition( checkVector, out hit, 1f, m_agent.areaMask ) )
			{
				List<(Vector3, bool)> value;
				m_coverPositions.TryGetValue( side, out value );
				(Vector3, bool) tempTuple = (hit.position, false);
				value.Add( tempTuple );
			}
			else
			{
				break;
			}
		}
		
	}
	/***************************************************
	 *   Function        : ReturnBestCoverPos
	 *   Purpose         : Return the besy cover position for current AI 
	 *   Parameters      : Vector3 hitpoint 
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public Vector3 ReturnBestCoverPos( Vector3 hitPoint )
	{
		// Add all side position references to a list 
		List<Vector3> list = new List<Vector3>();
		list.Add( m_positiveZSide );
		list.Add(m_positiveXSide );
		list.Add(m_negativeZSide );
		list.Add(m_negativeXSide );

		// Sort list to the shortest distance to the hitpoint 
		list = list.OrderBy( x => Vector3.Distance( hitPoint, x) ).ToList();

		// Set which side the AI is trying to cover behind ( could not use switch )
		if(list[0] == m_positiveZSide )
        {
			m_side = CoverSide.PositiveZ;
        }
		else if ( list[ 0 ] == m_negativeZSide )
		{
			m_side = CoverSide.NegativeZ;
		}
		else if ( list[ 0 ] == m_positiveXSide )
		{
			m_side = CoverSide.PositiveX;
		}
		else if ( list[ 0 ] == m_negativeXSide )
		{
			m_side = CoverSide.NegativeX;
		}

		// Use that side list of points
		List<(Vector3, bool)> value;
		m_coverPositions.TryGetValue( m_side, out value );

		// Get a point from list 
		return GetVaildPointFromList( value);
	}
	/***************************************************
	 *   Function        : GetValidPointFromList
	 *   Purpose         : Used to return the first free point from the list tuple 
	 *   Parameters      : List<(Vector3,bool)> list
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	private Vector3 GetVaildPointFromList( List<(Vector3,bool)> list )
    {
		// Loop through the list ( Using a for loop instead of a foreach as changing value of list)
		for ( int i = 0; i < list.Count; i++ )
		{
			// If index position is not in use
			if ( list[ i ].Item2 == false )
			{
				// Return point and set to now in use and update list 
				(Vector3, bool) tempTuple = (list[ i ].Item1, true);
				list[ i ] = tempTuple;
				
				return list[ i ].Item1;
			

			}
		}
		return Vector3.zero;		
    }
	/***************************************************
	 *   Function        : ReturnCoverPoint
	 *   Purpose         : Used to return cover point to the dictornary 
	 *   Parameters      : Vector3 point
	 *   Returns         : Void  
	 *   Date altered    : 17/07/2022
	 *   Contributors    : JG
	 *   Notes           :    
	 *   See also        :    
	 ******************************************************/
	public void ReturnCoverPoint( Vector3 point )
    {
		// Loop through each key value inside the dictornary 
		foreach( KeyValuePair <CoverSide, List <(Vector3, bool)>> entry in m_coverPositions )
		{
			// Loop through each list stored in that key 
			for(int i = 0; i< entry.Value.Count;i++)
            {
				// Return point if exists in dictonoary 
				if(entry.Value[i].Item1 == point )
                {
					(Vector3,bool) tempTuple = (entry.Value[ i ].Item1, false);
					entry.Value[i] = tempTuple;
					break;
                }
            }
        }

			
		
	}
}
