﻿// https://gist.github.com/Fonserbc/3d31a25e87fdaa541ddf

using UnityEngine;

public static partial class Utility
{
	/* 
	 * Most functions taken from Tween.js - Licensed under the MIT license
	 * at https://github.com/sole/tween.js
	 * Quadratic.Bezier by @fonserbc - Licensed under WTFPL license
	 */
	public delegate float EasingFunction( float k );

	public class Easing
	{
		public static float Linear( float k )
		{
			return k;
		}

        public static float Inverse( float k )
        {
            return 1.0f - k;
        }

        public class Quadratic
		{
			public static float In( float k )
			{
				return k * k;
			}

			public static float Out( float k )
			{
				return k * ( 2f - k );
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return 0.5f * k * k;
				return -0.5f * ( ( k -= 1f ) * ( k - 2f ) - 1f );
			}

			/* 
			 * Quadratic.Bezier(k,0) behaves like Quadratic.In(k)
			 * Quadratic.Bezier(k,1) behaves like Quadratic.Out(k)
			 *
			 * If you want to learn more check Alan Wolfe's post about it http://www.demofox.org/bezquad1d.html
			 */
			public static float Bezier( float k, float c )
			{
				return c * 2 * k * ( 1 - k ) + k * k;
			}
		};

		public class Cubic
		{
			public static float In( float k )
			{
				return k * k * k;
			}

			public static float Out( float k )
			{
				return 1f + ( ( k -= 1f ) * k * k );
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return 0.5f * k * k * k;
				return 0.5f * ( ( k -= 2f ) * k * k + 2f );
			}
		};

		public class Quartic
		{
			public static float In( float k )
			{
				return k * k * k * k;
			}

			public static float Out( float k )
			{
				return 1f - ( ( k -= 1f ) * k * k * k );
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return 0.5f * k * k * k * k;
				return -0.5f * ( ( k -= 2f ) * k * k * k - 2f );
			}
		};

		public class Quintic
		{
			public static float In( float k )
			{
				return k * k * k * k * k;
			}

			public static float Out( float k )
			{
				return 1f + ( ( k -= 1f ) * k * k * k * k );
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return 0.5f * k * k * k * k * k;
				return 0.5f * ( ( k -= 2f ) * k * k * k * k + 2f );
			}
		};

		public class Sinusoidal
		{
			public static float In( float k )
			{
				return 1f - Mathf.Cos( k * Mathf.PI / 2f );
			}

			public static float Out( float k )
			{
				return Mathf.Sin( k * Mathf.PI / 2f );
			}

			public static float InOut( float k )
			{
				return 0.5f * ( 1f - Mathf.Cos( Mathf.PI * k ) );
			}
		};

		public class Exponential
		{
			public static float In( float k )
			{
				return k <= 0f ? 0f : Mathf.Pow( 1024f, k - 1f );
			}

			public static float Out( float k )
			{
				return k >= 1f ? 1f : 1f - Mathf.Pow( 2f, -10f * k );
			}

			public static float InOut( float k )
			{
				if( k <= 0f ) return 0f;
				if( k >= 1f ) return 1f;
				if( ( k *= 2f ) < 1f ) return 0.5f * Mathf.Pow( 1024f, k - 1f );
				return 0.5f * ( -Mathf.Pow( 2f, -10f * ( k - 1f ) ) + 2f );
			}
		};

		public class Circular
		{
			public static float In( float k )
			{
				return 1f - Mathf.Sqrt( 1f - k * k );
			}

			public static float Out( float k )
			{
				return Mathf.Sqrt( 1f - ( ( k -= 1f ) * k ) );
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return -0.5f * ( Mathf.Sqrt( 1f - k * k ) - 1 );
				return 0.5f * ( Mathf.Sqrt( 1f - ( k -= 2f ) * k ) + 1f );
			}
		};

		public class Elastic
		{
			public static float In( float k )
			{
				if( k <= 0 ) return 0;
				if( k >= 1 ) return 1;
				return -Mathf.Pow( 2f, 10f * ( k -= 1f ) ) * Mathf.Sin( ( k - 0.1f ) * ( 2f * Mathf.PI ) / 0.4f );
			}

			public static float Out( float k )
			{
				if( k <= 0 ) return 0;
				if( k >= 1 ) return 1;
				return Mathf.Pow( 2f, -10f * k ) * Mathf.Sin( ( k - 0.1f ) * ( 2f * Mathf.PI ) / 0.4f ) + 1f;
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return -0.5f * Mathf.Pow( 2f, 10f * ( k -= 1f ) ) * Mathf.Sin( ( k - 0.1f ) * ( 2f * Mathf.PI ) / 0.4f );
				return Mathf.Pow( 2f, -10f * ( k -= 1f ) ) * Mathf.Sin( ( k - 0.1f ) * ( 2f * Mathf.PI ) / 0.4f ) * 0.5f + 1f;
			}
		};

		public class Back
		{
			static readonly float s = 1.70158f;
			static readonly float s2 = 2.5949095f;

			public static float In( float k )
			{
				return k * k * ( ( s + 1f ) * k - s );
			}

			public static float Out( float k )
			{
				return ( k -= 1f ) * k * ( ( s + 1f ) * k + s ) + 1f;
			}

			public static float InOut( float k )
			{
				if( ( k *= 2f ) < 1f ) return 0.5f * ( k * k * ( ( s2 + 1f ) * k - s2 ) );
				return 0.5f * ( ( k -= 2f ) * k * ( ( s2 + 1f ) * k + s2 ) + 2f );
			}
		};

		public class Bounce
		{
			public static float In( float k )
			{
				return 1f - Out( 1f - k );
			}

			public static float Out( float k )
			{
				if( k < ( 1f / 2.75f ) )
				{
					return 7.5625f * k * k;
				}
				else if( k < ( 2f / 2.75f ) )
				{
					return 7.5625f * ( k -= ( 1.5f / 2.75f ) ) * k + 0.75f;
				}
				else if( k < ( 2.5f / 2.75f ) )
				{
					return 7.5625f * ( k -= ( 2.25f / 2.75f ) ) * k + 0.9375f;
				}
				else
				{
					return 7.5625f * ( k -= ( 2.625f / 2.75f ) ) * k + 0.984375f;
				}
			}

			public static float InOut( float k )
			{
				if( k < 0.5f ) return In( k * 2f ) * 0.5f;
				return Out( k * 2f - 1f ) * 0.5f + 0.5f;
			}
		};
	}

    public static float Inverse( float t ) { return 1.0f - t; }
    public static float EaseIn( float t, float exp = 2.0f ) {  return Mathf.Pow( t, exp ); }
    public static float EaseOut( float t, float exp = 2.0f ) { return Inverse( EaseIn( Inverse( t ), exp ) ); }
    public static float EaseInOut( float t, float expIn = 2.0f, float expOut = 2.0f ) { return Lerp( EaseIn( t, expIn ), EaseOut( t, expOut ), t ); }
    public static float SmoothStep( float t ) { return EaseInOut( t ) * ( 3.0f - 2.0f * t ); }
    public static float SmootherStep( float t ) { return EaseInOut( t, 3.0f, 3.0f ) * ( t * ( t * 6.0f - 15.0f ) + 10.0f ); }

    public static float WeightedAverage( float t, float slowDown = 2.0f )
    {
        return ( ( t * ( slowDown - 1.0f ) ) + 1.0f ) / slowDown;
    }

    public static float CatmullRom( float t, float p0 = 0.1f, float p3 = 2.0f )
    {
        float p1 = 0.0f;
        float p2 = 1.0f;
        return 0.5f * (
                      ( 2 * p1 ) +
                      ( -p0 + p2 ) * t +
                      ( 2 * p0 - 5 * p1 + 4 * p2 - p3 ) * t * t +
                      ( -p0 + 3 * p1 - 3 * p2 + p3 ) * t * t * t
                      );
    }

    public static float EaseInOutParametric( float t, float alpha = 2.0f )
    {
        float sqt = t * t;
        return sqt / ( alpha * ( sqt - t ) + 1.0f );
    }

    public static float Spike( float t, float expIn = 2.0f, float expOut = 2.0f )
    {
        if( t <= .5f )
            return EaseIn( t / 0.5f, expIn );
        return EaseIn( Inverse( t ) / 0.5f, expOut );
    }

	public enum EasingFunctionTypes
    {
		Linear,
		Quadratic,
		Cubic,
		Quartic,
		Quintic,
		Elastic,
		Circular,
		Bounce,
		Back,
		Exponential,
		Sinusoidal,
		Spike,
		Parametric,
		CatmullRom,
		SmoothStep,
		SmootherStep,
		Inverse,
		WeightedAverage,
	}

	public enum EasingFunctionMethod
	{
		In,
		Out,
		InOut,
	}

	public static EasingFunction FetchEasingFunction( EasingFunctionTypes type, EasingFunctionMethod method, float? param1 = null, float? param2 = null )
	{
		return type switch
		{
			EasingFunctionTypes.Linear => Easing.Linear,
			EasingFunctionTypes.Quadratic =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Quadratic.InOut,
					EasingFunctionMethod.Out => Easing.Quadratic.InOut,
					EasingFunctionMethod.InOut => Easing.Quadratic.InOut,
					_ => null,
				},
			EasingFunctionTypes.Cubic =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Cubic.InOut,
					EasingFunctionMethod.Out => Easing.Cubic.InOut,
					EasingFunctionMethod.InOut => Easing.Cubic.InOut,
					_ => null,
				},
			EasingFunctionTypes.Quartic =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Quartic.InOut,
					EasingFunctionMethod.Out => Easing.Quartic.InOut,
					EasingFunctionMethod.InOut => Easing.Quartic.InOut,
					_ => null,
				},
			EasingFunctionTypes.Quintic =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Quintic.InOut,
					EasingFunctionMethod.Out => Easing.Quintic.InOut,
					EasingFunctionMethod.InOut => Easing.Quintic.InOut,
					_ => null,
				},
			EasingFunctionTypes.Elastic =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Elastic.InOut,
					EasingFunctionMethod.Out => Easing.Elastic.InOut,
					EasingFunctionMethod.InOut => Easing.Elastic.InOut,
					_ => null,
				},
			EasingFunctionTypes.Circular =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Circular.InOut,
					EasingFunctionMethod.Out => Easing.Circular.InOut,
					EasingFunctionMethod.InOut => Easing.Circular.InOut,
					_ => null,
				},
			EasingFunctionTypes.Bounce =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Bounce.InOut,
					EasingFunctionMethod.Out => Easing.Bounce.InOut,
					EasingFunctionMethod.InOut => Easing.Bounce.InOut,
					_ => null,
				},
			EasingFunctionTypes.Back =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Back.InOut,
					EasingFunctionMethod.Out => Easing.Back.InOut,
					EasingFunctionMethod.InOut => Easing.Back.InOut,
					_ => null,
				},
			EasingFunctionTypes.Exponential =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Exponential.InOut,
					EasingFunctionMethod.Out => Easing.Exponential.InOut,
					EasingFunctionMethod.InOut => Easing.Exponential.InOut,
					_ => null,
				},
			EasingFunctionTypes.Sinusoidal =>
				method switch
				{
					EasingFunctionMethod.In => Easing.Sinusoidal.InOut,
					EasingFunctionMethod.Out => Easing.Sinusoidal.InOut,
					EasingFunctionMethod.InOut => Easing.Sinusoidal.InOut,
					_ => null,
				},
			EasingFunctionTypes.Spike => ( x => param1 != null ? Spike( x, param1.Value, param2 ?? param1.Value ) : Spike( x ) ),
			EasingFunctionTypes.Parametric => ( x => param1 != null ? EaseInOutParametric( x, param1.Value ) : EaseInOutParametric( x ) ),
			EasingFunctionTypes.CatmullRom => ( x => param1 != null ? CatmullRom( x, param1.Value, param2 ?? param1.Value ) : CatmullRom( x ) ),
			EasingFunctionTypes.SmoothStep => SmoothStep,
			EasingFunctionTypes.SmootherStep => SmootherStep,
			EasingFunctionTypes.Inverse => Inverse,
			EasingFunctionTypes.WeightedAverage => ( x => param1 != null ? WeightedAverage( x, param1.Value ) : WeightedAverage( x ) ),
			_ => null,
		};
	}

    public static float EvaluateEasingFunction( float t, EasingFunctionTypes type, EasingFunctionMethod method, float? param1 = null, float? param2 = null )
    {
        return FetchEasingFunction( type, method, param1, param2 )( t );
    }
}
