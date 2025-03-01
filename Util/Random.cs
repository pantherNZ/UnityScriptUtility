﻿using UnityEngine;
using System;
using Unity.Mathematics;

public static partial class Utility
{
	public static int CombineHashes( int seed, params int[] vars )
	{
		int hash1 = ( seed << 5 + 5381 << 16 ) + 5381;
		int hash2 = hash1;

		int i = 0;
		foreach ( var v in vars )
		{
			if ( i % 2 == 0 )
				hash1 = ( ( hash1 << 5 ) + hash1 + ( hash1 >> 27 ) ) ^ v;
			else
				hash2 = ( ( hash2 << 5 ) + hash2 + ( hash2 >> 27 ) ) ^ v;

			++i;
		}

		return hash1 + ( hash2 * 1566083941 );
	}

	public abstract class IRandom
    {
        public abstract float value { get; }
        public Vector3 insideUnitSphere => new Vector3( Gaussian( 0.0f, 1.0f ), Gaussian( 0.0f, 1.0f ), Gaussian( 0.0f, 1.0f ) ).normalized;
		public Vector2 insideUnitCircle => onUnitCircle * Range( 0, 1.0f );
        public Vector3 onUnitSphere { get
        {
                var theta = 2.0f * Mathf.PI * value;
                var phi = Mathf.Acos( 2.0f * value - 1.0f );
                return new Vector3( Mathf.Sin( phi ) * Mathf.Cos( theta ), Mathf.Sin( phi ) * Mathf.Sin( theta ), Mathf.Cos( phi ) );
        } }
		public Vector2 onUnitCircle { get
        {
            var angle = Range( 0, Mathf.PI * 2.0f );
            return new Vector2( Mathf.Sin( angle ), Mathf.Cos( angle ) );
        } }
		public Vector3 vector3 => new( value, value, value );
		public Quaternion rotation => Quaternion.Euler( angleDegrees, angleDegrees, angleDegrees );
		public Quaternion yRotation => Quaternion.Euler( 0.0f, angleDegrees, 0.0f );
		public float angleDegrees => Range( 0.0f, 360.0f );
		public float Range( float minInclusive, float maxInclusive ) => minInclusive + ( maxInclusive - minInclusive ) * value;
		public float Range( Interval range ) => Range( range.First, range.Second );
        public int Range( int minInclusive, int maxExclusive ) => minInclusive + Mathf.FloorToInt( ( maxExclusive - minInclusive ) * value );
		public int Range( IntervalInt range ) => Range( range.First, range.Second );
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
		public int Percent() => Range( 0, 100 );
		public bool Roll( int chance ) => chance >= 100 || ( chance > 0 && chance >= Percent() );
		public float PercentF() => Range( 0.0f, 1.0f );
		public bool Roll( float chance ) => chance >= 1.0f || ( chance > 0.0f && chance >= PercentF() );
		public bool Bool() => Roll( 50 );
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
        public SystemRandom( int seed ) { rng = new System.Random( seed ); }
        public override float value { get => ( float )rng.NextDouble(); }
        public long NextLong( long min, long max ) => rng.NextLong( min, max );
        public long NextLong( long max ) => rng.NextLong( max );
        public long NextLong() => rng.NextLong();
        public void NextBytes( byte[] buffer ) => rng.NextBytes( buffer );
        private System.Random rng;
    }

    public class UnityRandom : IRandom
    {
        public UnityRandom() : this( ( int )DateTime.Now.Ticks ) { }
        public UnityRandom( int seed ) { rng = new Unity.Mathematics.Random( ( uint )seed ); }
		public override float value => rng.NextFloat();

		Unity.Mathematics.Random rng;
    }

	public class SeededRandom : IRandom
	{
		int seed;

		public SeededRandom( int seed ) { this.seed = seed; }

		public override float value
		{
			get
			{
				seed = seed * 37 + 34222;
				var rng = xxHashSharp.xxHash.CalculateHash( BitConverter.GetBytes( seed ) );
				var result = rng / ( float )( uint.MaxValue - 1 );
				return result;
			}
		}
	}

	public static UnityRandom _DefaultRng;
	public static UnityRandom DefaultRng { get
		{
			_DefaultRng ??= new UnityRandom();
			return _DefaultRng;
		} }
}
