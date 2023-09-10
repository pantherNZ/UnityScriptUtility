using UnityEngine;

public static partial class Utility
{
    public static Color GetRainbowColour( float value )
    {
        float inc = 6.0f;
        float x = value * inc;
        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        if( ( 0 <= x && x <= 1 ) || ( 5 <= x && x <= 6 ) ) r = 1.0f;
        else if( 4 <= x && x <= 5 ) r = x - 4;
        else if( 1 <= x && x <= 2 ) r = 1.0f - ( x - 1 );
        if( 1 <= x && x <= 3 ) g = 1.0f;
        else if( 0 <= x && x <= 1 ) g = x - 0;
        else if( 3 <= x && x <= 4 ) g = 1.0f - ( x - 3 );
        if( 3 <= x && x <= 5 ) b = 1.0f;
        else if( 2 <= x && x <= 3 ) b = x - 2;
        else if( 5 <= x && x <= 6 ) b = 1.0f - ( x - 5 );

        return new Color( r, g, b, 1.0f );
    }

    public static Color InterpolateColour( Color a, Color b, float t, EasingFunction easingFunction = null )
    {
        Color.RGBToHSV( a, out float h1, out float s1, out float v1 );
        Color.RGBToHSV( b, out float h2, out float s2, out float v2 );
        t = easingFunction != null ? easingFunction( t ) : t;
        float h = Lerp( h1, h2, t );
        float s = Lerp( s1, s2, t );
        float v = Lerp( v1, v2, t );
        var colour = Color.HSVToRGB( h, s, v );
        colour.a = Lerp( a.a, b.a, t );
        return colour;
    }

    public static Color ColourFromHex( int r, int g, int b, int a = 255 )
    {
        return new Color(
              r / 255.0f
            , g / 255.0F
            , b / 255.0f
            , a / 255.0f );
    }

    public static Color ColourFromHex( uint rgba )
    {
        return new Color(
              ( ( rgba & 0xff000000 ) >> 0x18 ) / 255.0f
            , ( ( rgba & 0xff0000 ) >> 0x10 ) / 255.0F
            , ( ( rgba & 0xff00 ) >> 0x08 ) / 255.0f
            , ( rgba & 0xff ) / 255.0f );
    }

    public static Color ColourFromHexRGB( uint rgb )
    {
        if( ( rgb & 0xff000000 ) > 0 )
            return ColourFromHex( rgb );

        return new Color(
              ( ( rgb & 0xff0000 ) >> 0x10 ) / 255.0F
            , ( ( rgb & 0xff00 ) >> 0x08 ) / 255.0f
            , ( rgb & 0xff ) / 255.0f
            , 1.0f );
    }

    public static Color SetR( this Color col, float r ) { col.r = r; return col; }
    public static Color SetG( this Color col, float g ) { col.g = g; return col; }
    public static Color SetB( this Color col, float b ) { col.b = b; return col; }
    public static Color SetA( this Color col, float a ) { col.a = a; return col; }

    public static Color ToColour( this uint rgba )
    {
        return ColourFromHex( rgba );
    }
}