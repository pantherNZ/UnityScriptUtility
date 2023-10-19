using UnityEngine;
using System;

public static partial class Utility
{
    public abstract class IRandom
    {
        public abstract float value { get; }
        public Vector3 insideUnitSphere => new Vector3( Gaussian( 0.0f, 1.0f ), Gaussian( 0.0f, 1.0f ), Gaussian( 0.0f, 1.0f ) ).normalized;
        public Vector2 insideUnitCircle { get
        {
            var angle = Range( 0, Mathf.PI * 2.0f );
            return new Vector2( Mathf.Sin( angle ), Mathf.Cos( angle ) );
        } }
        public Vector3 onUnitSphere { get
        {
                var theta = 2.0f * Mathf.PI * value;
                var phi = Mathf.Acos( 2.0f * value - 1.0f );
                return new Vector3( Mathf.Sin( phi ) * Mathf.Cos( theta ), Mathf.Sin( phi ) * Mathf.Sin( theta ), Mathf.Cos( phi ) );
        } }
        public Quaternion rotation => Quaternion.Euler( Range( 0.0f, 360.0f ), Range( 0.0f, 360.0f ), Range( 0.0f, 360.0f ) );
        public float Range( float minInclusive, float maxInclusive ) => minInclusive + ( maxInclusive - minInclusive ) * value;
        public int Range( int minInclusive, int maxExclusive ) => minInclusive + Mathf.FloorToInt( ( maxExclusive - minInclusive ) * value );
        public Color ColorHSV() => ColorHSV( 0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f );
        public Color ColorHSV( float hueMin, float hueMax ) => ColorHSV( hueMin, hueMax, 0f, 1f, 0f, 1f, 1f, 1f );
        public Color ColorHSV( float hueMin, float hueMax, float saturationMin, float saturationMax ) => ColorHSV( hueMin, hueMax, saturationMin, saturationMax, 0f, 1f, 1f, 1f );
        public Color ColorHSV( float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax ) => ColorHSV( hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, 1f, 1f );
        public Color ColorHSV( float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax )
        {
            float h = Mathf.Lerp( hueMin, hueMax, value );
            float s = Mathf.Lerp( saturationMin, saturationMax, value );
            float v = Mathf.Lerp( valueMin, valueMax, value );
            Color result = Color.HSVToRGB( h, s, v, hdr: true );
            result.a = Mathf.Lerp( alphaMin, alphaMax, value );
            return result;
        }
        public int Int() => Range( int.MinValue, int.MaxValue );
        public bool Bool() => Range( 0, 100 ) < 50;
        public int Percent() => Range( 0, 100 );
        public bool Roll( int chance ) => chance < Percent();
        public float Gaussian( float mean, float stdDev )
        {
            float u1 = 1.0f - value;
            float u2 = 1.0f - value;
            float randStdNormal = Mathf.Sqrt( -2.0f * Mathf.Log( u1 ) ) * Mathf.Sin( 2.0f * Mathf.PI * u2 ); //random normal(0,1)
            float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
    }

    public class SystemRandom : IRandom
    {
        public SystemRandom( System.Random rng ) { this.rng = rng; }
        public override float value { get => ( float )rng.NextDouble(); }
        public long NextLong( long min, long max ) => rng.NextLong( min, max );
        public long NextLong( long max ) => rng.NextLong( max );
        public long NextLong() => rng.NextLong();
        public void NextBytes( byte[] buffer ) => rng.NextBytes( buffer );
        private System.Random rng;
    }

    public class UnityRandom : IRandom
    {
        public override float value => UnityEngine.Random.value;
    }

    public static UnityRandom DefaultRng = new UnityRandom();
}