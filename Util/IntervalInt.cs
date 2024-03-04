
using System;

[Serializable]
public class IntervalInt : Pair<int, int>
{
	public IntervalInt( int min, int max )
		: base( min, max )
	{

	}

	public int Range() { return Second - First; }
	public bool Contains( float value ) { return value >= First && value <= Second; }
	public int Random( Utility.IRandom rng ) { return ( rng ?? Utility.DefaultRng ).Range( First, Second ); }
}
