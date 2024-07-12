using UnityEngine;

public class VoronoiNoise
{
	private static float Frac( float value )
	{
		return value % 1.0f;
	}

	private static float rand2dTo1d( Vector2 value, Vector2 dotDir )
	{
		Vector2 smallValue = new Vector2( Mathf.Sin( value.x ), Mathf.Sin( value.y ) );
		float random = Vector2.Dot( smallValue, dotDir );
		random = Frac( Mathf.Sin( random ) * 143758.5453f );
		return random;
	}

	private static Vector2 rand2dTo2d( Vector2 value )
	{
		return new Vector2(
			rand2dTo1d( value, new Vector2( 12.989f, 78.233f ) ),
			rand2dTo1d( value, new Vector2( 39.346f, 11.135f ) )
		);
	}

	public static float Noise( float xin, float yin, int seed, out float minEdgeDistanceOut )
	{
		Vector2 value = new Vector2( xin, yin );
		Vector2Int baseCell = new Vector2Int( Mathf.FloorToInt( xin ), Mathf.FloorToInt( yin ) );

		//first pass to find the closest cell
		float minDistToCell = 10;
		Vector2 toClosestCell = Vector2.zero;
		Vector2 closestCell = Vector2.zero;
		for ( int x1 = -1; x1 <= 1; x1++ )
		{
			for( int y1 = -1; y1 <= 1; y1++ )
			{
				Vector2Int cell = baseCell + new Vector2Int( x1, y1 );
				Vector2 cellPosition = cell + rand2dTo2d( cell );
				Vector2 toCell = cellPosition - value;
				float distToCell = toCell.magnitude;
				if( distToCell < minDistToCell )
				{
					minDistToCell = distToCell;
					closestCell = cell;
					toClosestCell = toCell;
				}
			}
		}

		//second pass to find the distance to the closest edge
		minEdgeDistanceOut = 10;
		for( int x2 = -1; x2 <= 1; x2++ )
		{
			for( int y2 = -1; y2 <= 1; y2++ )
			{
				Vector2 cell = baseCell + new Vector2( x2, y2 );
				Vector2 cellPosition = cell + rand2dTo2d( cell );
				Vector2 toCell = cellPosition - value;

				Vector2 diffToClosestCell = new Vector2( Mathf.Abs( closestCell.x - cell.x ), Mathf.Abs( closestCell.y - cell.y ) );
				bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y < 0.1;
				if( !isClosestCell )
				{
					Vector2 toCenter = ( toClosestCell + toCell ) * 0.5f;
					Vector2 cellDifference = ( toCell - toClosestCell ).normalized;
					float edgeDistance = Vector2.Dot( toCenter, cellDifference );
					minEdgeDistanceOut = Mathf.Min( minEdgeDistanceOut, edgeDistance );
				}
			}
		}

		//float random = rand2dTo1d( closestCell );
		return minDistToCell;
	}

	public static float Noise( float xin, float yin, float zin, float seed )
    {
		return 0.0f;
    }
}
