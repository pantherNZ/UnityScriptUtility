using System;
using System.Linq;
using UnityEngine;

public static partial class Utility
{

    public static T ParseEnum<T>( string value, bool ignoreCase = false ) where T : struct
    {
        return ( T )Enum.Parse( typeof( T ), value, ignoreCase );
    }

    public static bool TryParseEnum<T>( string value, out T result, bool ignoreCase = false ) where T : struct
    {
        return Enum.TryParse( value, ignoreCase, out result );
    }

    // Parse a float, return default if failed
    public static float ParseFloat( string text, float defaultValue )
    {
        if( float.TryParse( text, out float f ) )
            return f;
        return defaultValue;
    }

    // Parse a int, return default if failed
    public static int ParseInt( string text, int defaultValue )
    {
        if( int.TryParse( text, out int i ) )
            return i;
        return defaultValue;
    }

    private const string SuperscriptDigits = "\u2070\u00b9\u00b2\u00b3\u2074\u2075\u2076\u2077\u2078\u2079";
    private const string SubscriptDigits = "\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089";

    public static string ToSuperscript( string text )
    {
        return new string( text.Select( x =>
        {
            if( x == '-' ) return '\u207B';
            if( x == '.' ) return '\u2027';
            var num = x - '0';
            return ( num < 0 || num > 9 ) ? x : SuperscriptDigits[num];
        } ).ToArray() );
    }

    public static string ToSubscript( string text )
    {
        //text = text.Replace( ".", "  \u0323" );
        return new string( text.Select( x =>
        {
            if( x == '-' ) return '\u208B';
            var num = x - '0';
            return ( num < 0 || num > 9 ) ? x : SubscriptDigits[num];
        } ).ToArray() );
    }

    public static string ToSuperscriptString<T>( this T t ) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        return ToSuperscript( t.ToString() );
    }

    public static string ToSubscriptString<T>( this T t ) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        return ToSubscript( t.ToString() );
    }

    public static bool StartsWithGet( this string str, string startsWith, out string secondPart )
    {
        var idx = str.IndexOf( startsWith );
        if( idx != -1 )
        {
            secondPart = str.Substring( idx + startsWith.Length );
            return true;
        }

        secondPart = string.Empty;
        return false;
    }

    // String colour utility
    public static string Bold( this string str ) { return $"<b>{str}</b>"; }
    public static string Italic( this string str ) { return $"<i>{str}</i>"; }
    public static string Size( this string str, int size ) { return $"<size={size}>{str}</size>"; }
    public static string Aqua( this string str ) { return Colour( str, Color.cyan ); }
    public static string Black( this string str ) { return Colour( str, Color.black ); }
    public static string Blue( this string str ) { return Colour( str, Color.blue ); }
    public static string Brown( this string str ) { return Colour( str, 0xa52a2aff ); }
    public static string Cyan( this string str ) { return Aqua( str ); }
    public static string DarkBlue( this string str ) { return Colour( str, 0x0000a0ff ); }
    public static string Fuchsia( this string str ) { return Magenta( str ); }
    public static string Green( this string str ) { return Colour( str, Color.green ); }
    public static string Gray( this string str ) { return Colour( str, Color.gray ); }
    public static string Grey( this string str ) { return Grey( str ); }
    public static string LightBlue( this string str ) { return Colour( str, 0xadd8e6ff ); }
    public static string Lime( this string str ) { return Colour( str, 0x00ff00ff ); }
    public static string Magenta( this string str ) { return Colour( str, Color.magenta ); }
    public static string Maroon( this string str ) { return Colour( str, 0x800000ff ); }
    public static string Navy( this string str ) { return Colour( str, 0x000080ff ); }
    public static string Olive( this string str ) { return Colour( str, 0x808000ff ); }
    public static string Orange( this string str ) { return Colour( str, 0xffa500ff ); }
    public static string Purple( this string str ) { return Colour( str, 0x800080ff ); }
    public static string Red( this string str ) { return Colour( str, Color.red ); }
    public static string Silver( this string str ) { return Colour( str, 0xc0c0c0ff ); }
    public static string Teal( this string str ) { return Colour( str, 0x008080ff ); }
    public static string White( this string str ) { return Colour( str, Color.white ); }
    public static string Yellow( this string str ) { return Colour( str, Color.yellow ); }

    public static string Colour( this string str, Color colour )
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA( colour )}>{str}</color>";
    }

    public static string Colour( this string str, uint rgba )
    {
        return Colour( str, ColourFromHex( rgba ) );
    }

    public static string ColourRGB( this string str, uint rgb )
    {
        return Colour( str, ColourFromHexRGB( rgb ) );
    }

    public static string Format( this string str, params object[] args )
    {
        return string.Format( str, args );
    }

    public static void CopyToClipboard( this string str )
    {
        GUIUtility.systemCopyBuffer = str;
    }
}