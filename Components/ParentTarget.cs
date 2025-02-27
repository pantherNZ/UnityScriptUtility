using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ParentTarget : MonoBehaviour
{
	[SerializeField] GameObject target;
	public GameObject Target { get => target; private set {} }
	public bool matchDestroy;
	public float matchPositionRate;
	public float matchRotationRate;
	public float matchScaleRate;
	public Vector3 positionOffset;
	public Vector3 scaleOffset;
	public Quaternion rotationOffset;

	public Vector3 positionAxisMask;
	public Vector3 scaleAxisMask;

	public ParentTarget SetTarget( GameObject targ, Vector3? positionOffset = null, Vector3? scaleOffset = null, Quaternion? rotationOffset = null )
	{
		target = targ;
		this.positionOffset = positionOffset.GetValueOrDefault( Vector3.zero );
		this.scaleOffset = scaleOffset.GetValueOrDefault( Vector3.zero );
		this.rotationOffset = rotationOffset.GetValueOrDefault( Quaternion.identity );
		return this;
	}
	
	public ParentTarget MatchDestroy( bool matchDestroy = true )
	{
		this.matchDestroy = matchDestroy;
		return this;
	}

	public ParentTarget MatchPosition( float ratePerSec )
	{
		this.matchPositionRate = ratePerSec;
		return this;
	}

	public ParentTarget MatchRotation( float ratePerSec )
	{
		this.matchRotationRate = ratePerSec;
		return this;
	}

	public ParentTarget MatchScale( float ratePerSec )
	{
		this.matchScaleRate = ratePerSec;
		return this;
	}

	public ParentTarget PositionOffset( Vector3 positionOffset )
	{
		this.positionOffset = positionOffset;
		return this;
	}

	public ParentTarget ScaleOffset( Vector3 scaleOffset )
	{
		this.scaleOffset = scaleOffset;
		return this;
	}

	public ParentTarget RotationOffset( quaternion rotationOffset )
	{
		this.rotationOffset = rotationOffset;
		return this;
	}

	public ParentTarget PositionAxisMask( EAxisMask mask )
	{
		positionAxisMask = mask.GetVec3FromAxisMask();
		return this;
	}
	
	public ParentTarget ScaleAxisMask( EAxisMask mask )
	{
		scaleAxisMask = mask.GetVec3FromAxisMask();
		return this;
	}

	private void Update()
	{
		if ( matchDestroy && target == null )
			this.DestroyObject();

		if ( target == null )
			return;

		var direction = target.transform.position - ( transform.position + positionOffset );
		var directionLength = direction.magnitude;
		if( directionLength > 0.0001f )
			transform.position += Mathf.Min( Time.deltaTime * matchPositionRate, directionLength ) * ( direction / directionLength ).ScaleBy( positionAxisMask );

		var scaleDiff = target.transform.localScale - ( transform.localScale + scaleOffset);
		var scaleDiffLength = scaleDiff.magnitude;
		if ( scaleDiffLength > 0.0001f )
			transform.localScale += Mathf.Min( Time.deltaTime * matchScaleRate, scaleDiffLength ) * ( scaleDiff / scaleDiffLength ).ScaleBy( scaleAxisMask );

		transform.rotation = Quaternion.RotateTowards( transform.rotation, rotationOffset * target.transform.rotation, Time.deltaTime * matchRotationRate );
	}
}
