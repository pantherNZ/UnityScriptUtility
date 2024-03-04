using System;

[Serializable]
public class Interval : Pair<float, float>
{
    public Interval( float min, float max )
        : base( min, max )
    {

    }

    public float Range() { return Second - First; }
    public bool Contains( float value ) { return value >= First && value <= Second; }
    public float Random( Utility.IRandom rng ) { return ( rng ?? Utility.DefaultRng ).Range( First, Second ); }
}
