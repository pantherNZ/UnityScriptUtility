using UnityEditor;
using UnityEngine;

public static partial class Utility
{
	public abstract class IShape
	{
		public abstract Vector2 position { get; }
		public abstract bool Contains( Vector2 position );
		public abstract Rect Bounds { get; }
		public RectInt BoundsInt => new( Bounds.min.ToVector2Int( ToIntRounding.Floor ), Bounds.size.ToVector2Int( ToIntRounding.Ceil ) );
		public IShape CopyShape() => ScaledShape( 1.0f );
		public abstract IShape ScaledShape( float modifier );
		public abstract IShape EnlargeShape( float diff );

		public bool Contains( Rect rect ) =>
			Contains( rect.TopLeft() ) &&
			Contains( rect.TopRight() ) &&
			Contains( rect.BottomLeft() ) &&
			Contains( rect.BottomRight() );
		public bool Contains( RectRotated rect ) =>
			Contains( rect.TopLeft() ) &&
			Contains( rect.TopRight() ) &&
			Contains( rect.BottomLeft() ) &&
			Contains( rect.BottomRight() );
		public bool Overlaps( Rect rect ) =>
			Contains( rect.TopLeft() ) ||
			Contains( rect.TopRight() ) ||
			Contains( rect.BottomLeft() ) ||
			Contains( rect.BottomRight() );
		public bool Overlaps( RectRotated rect ) =>
			Contains( rect.TopLeft() ) ||
			Contains( rect.TopRight() ) ||
			Contains( rect.BottomLeft() ) ||
			Contains( rect.BottomRight() );
	}

	public class RectRotated : IShape
	{
		public RectRotated() { }
		public RectRotated( Rect rect, float rotationDegrees ) { this.rect = rect; this.rotationDegrees = rotationDegrees; }
		public RectRotated( Vector2 bottomLeft, Vector2 topRight, float rotationDegrees ) 
		{
			this.rect = new( bottomLeft, topRight - bottomLeft );
			this.rotationDegrees = rotationDegrees;
		}

		private Rect rect;
		public float rotationDegrees;

		public override bool Contains( Vector2 position )
		{
			var newPoint = position - rect.center;
			newPoint = newPoint.Rotate( -rotationDegrees );
			newPoint += rect.center;
			return newPoint.x >= rect.xMin && newPoint.x <= rect.xMax && newPoint.y >= rect.yMin && newPoint.y <= rect.yMax;
		}

		public override Rect Bounds => new( min, max - min );
		public RectRotated Copy() => Scaled( 1.0f );
		public override IShape ScaledShape( float modifier ) => new RectRotated(
				centre + ( rect.BottomLeft() - centre ) * modifier,
				centre + ( rect.TopRight() - centre ) * modifier,
				rotationDegrees );
		public RectRotated Scaled( float modifier ) => ScaledShape( modifier ) as RectRotated;
		public override IShape EnlargeShape( float diff ) => Enlarge( diff );
		public RectRotated Enlarge( float diff ) => new RectRotated(
				centre + new Vector2( -width / 2.0f - diff, -height / 2.0f - diff ),
				centre + new Vector2( width / 2.0f + diff, height / 2.0f + diff ),
				rotationDegrees );

		public override Vector2 position => rect.center;
		public Vector2 centre => rect.center;
		public float width => rect.width;
		public float height => rect.height;
		public Vector2 min => new(
			Mathf.Min( TopLeft().x, TopRight().x, BottomLeft().x, BottomRight().x ),
			Mathf.Min( TopLeft().y, TopRight().y, BottomLeft().y, BottomRight().y )
		);
		public Vector2 max => new(
			Mathf.Max( TopLeft().x, TopRight().x, BottomLeft().x, BottomRight().x ),
			Mathf.Max( TopLeft().y, TopRight().y, BottomLeft().y, BottomRight().y )
		);
		public Vector2 TopLeft() => new Vector2( rect.xMin, rect.yMax ).RotateAround( centre, rotationDegrees );
		public Vector2 TopRight() => new Vector2( rect.xMax, rect.yMax ).RotateAround( centre, rotationDegrees );
		public Vector2 BottomLeft() => new Vector2( rect.xMin, rect.yMin ).RotateAround( centre, rotationDegrees );
		public Vector2 BottomRight() => new Vector2( rect.xMax, rect.yMin ).RotateAround( centre, rotationDegrees );
	};

	public class Circle : IShape
	{
		public Circle() { }
		public Circle( Vector2 centre, float radius ) { this.centre = centre; this.radius = radius; }

		public Vector2 centre;
		public float radius;

		public override Vector2 position => centre;

		public override bool Contains( Vector2 point ) => ( point - centre ).sqrMagnitude <= Mathf.Pow( radius, 2.0f );
		public override Rect Bounds => new( position - radius * Vector2.one, 2.0f * radius * Vector2.one );

		public Circle Copy() => Scaled( 1.0f );
		public override IShape ScaledShape( float modifier ) => new Circle( centre, radius * modifier );
		public Circle Scaled( float modifier ) => ScaledShape( modifier ) as Circle;
		public override IShape EnlargeShape( float diff ) => Enlarge( diff );
		public Circle Enlarge( float diff ) => new Circle( centre, radius + diff );
	};
}
