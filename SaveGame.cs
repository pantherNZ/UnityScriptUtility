using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SaveGameSystem
{
    public const int currentVersion = 1;
    public const string folderName = "/SavedData/";
    public const string fileExtension = ".dat";

    static string ConvertSaveNameToPath( string name )
    {
        return Application.persistentDataPath + folderName + name + fileExtension;
    }

    static string ConvertPathToSaveName( string path )
    {
        var index = path.LastIndexOf( '/' ) + 1;
        return path.Substring( index, path.Length - index - fileExtension.Length );
    }

    public static bool SaveExists( string name )
    {
        return GetSaveGames().Find( x => x == name ) != null;
    }

    public static List<string> GetSaveGames()
    {
        string folderPath = Application.persistentDataPath + folderName;

        if( !Directory.Exists( folderPath ) )
            return new List<string>();

        var files = Directory.GetFiles( folderPath, "*" + fileExtension );
        files = Array.ConvertAll( files, x => ConvertPathToSaveName( x ) );
        return files.ToList();
    }

    public static bool SaveGame( string name )
    {
        if( name.Length == 0 )
            return false;

        string folderPath = Application.persistentDataPath + folderName;
        string fullPath = ConvertSaveNameToPath( name );

        if( !Directory.Exists( folderPath ) )
            Directory.CreateDirectory( folderPath );

        try
        {
            using( var fileStream = File.Open( fullPath, FileMode.OpenOrCreate ) )
            {
                using( var memoryStream = new MemoryStream() )
                {
                    using( var writer = new BinaryWriter( memoryStream ) )
                    {
                        writer.Write( ( char )( currentVersion ) );
                        foreach( var subscriber in subscribers )
                            subscriber.Serialise( writer );
                    }

                    var content = memoryStream.ToArray();
                    fileStream.Write( content, 0, content.Length );
                }
            }
        }
        catch( Exception e )
        {
            Debug.LogError( "Failed to save game: " + e.ToString() );
            return false;
        }

        return true;
    }

    public static bool LoadGame( string name )
    {
        string fullPath = ConvertSaveNameToPath( name );

        if( !File.Exists( fullPath ) )
            return false;

        try
        {
            using( var fileStream = File.Open( fullPath, FileMode.Open ) )
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read( bytes, 0, bytes.Length );

                using( var memoryStream = new MemoryStream( bytes, writable: false ) )
                {
                    using( var reader = new BinaryReader( memoryStream ) )
                    {
                        var version = reader.ReadByte();
                        foreach( var subscriber in subscribers )
                            subscriber.Deserialise( version, reader );
                    }
                }
            }
        }
        catch( Exception e )
        {
            Debug.LogError( "Failed to load game: " + e.ToString() );
            return false;
        }

        return true;
    }

    public static void AddSaveableComponent( ISavableComponent obj )
    {
        subscribers.Add( obj );
    }

    static readonly List<ISavableComponent> subscribers = new List<ISavableComponent>();
}

// Base savable object interface
public interface ISavableComponent
{
    void Serialise( System.IO.BinaryWriter writer );
    void Deserialise( int saveVersion, System.IO.BinaryReader reader );
}

public static partial class Extensions
{
    public static T Get<T>( this Dictionary<string, object> instance, string name )
    {
        return ( T )instance[name];
    }

    public static Texture2D ToTexture2D( this RenderTexture rTex )
    {
        Texture2D tex = new Texture2D( rTex.width, rTex.height, TextureFormat.ARGB32, false );
        var previous = RenderTexture.active;
        RenderTexture.active = rTex;
        tex.ReadPixels( new Rect( 0, 0, rTex.width, rTex.height ), 0, 0 );
        tex.Apply();
        RenderTexture.active = previous;
        return tex;
    }

    public static RenderTexture ToRenderTexture( this Texture2D tex )
    {
        RenderTexture rTex = new RenderTexture( tex.width, tex.height, 0, RenderTextureFormat.ARGB32 ) { filterMode = FilterMode.Bilinear };
        rTex.Create();
        Graphics.CopyTexture( tex, rTex );
        return rTex;
    }

    // Helper functions
    public static void Write( this BinaryWriter writer, Vector2 vec )
    {
        writer.Write( vec.x );
        writer.Write( vec.y );
    }

    public static Vector2 ReadVector2( this BinaryReader reader )
    {
        return new Vector2
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle()
        };
    }

    public static void Write( this BinaryWriter writer, Vector2Int vec )
    {
        writer.Write( vec.x );
        writer.Write( vec.y );
    }

    public static Vector2Int ReadVector2Int( this BinaryReader reader )
    {
        return new Vector2Int
        {
            x = reader.ReadInt32(),
            y = reader.ReadInt32()
        };
    }

    public static void Write( this BinaryWriter writer, Vector3 vec )
    {
        writer.Write( vec.x );
        writer.Write( vec.y );
        writer.Write( vec.z );
    }

    public static Vector3 ReadVector3( this BinaryReader reader )
    {
        return new Vector3
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle()
        };
    }

    public static void Write( this BinaryWriter writer, Vector3Int vec )
    {
        writer.Write( vec.x );
        writer.Write( vec.y );
        writer.Write( vec.z );
    }

    public static Vector3Int ReadVector3Int( this BinaryReader reader )
    {
        return new Vector3Int
        {
            x = reader.ReadInt32(),
            y = reader.ReadInt32(),
            z = reader.ReadInt32()
        };
    }

    public static void Write( this BinaryWriter writer, Vector4 vec )
    {
        writer.Write( vec.x );
        writer.Write( vec.y );
        writer.Write( vec.z );
        writer.Write( vec.w );
    }

    public static Vector4 ReadVector4( this BinaryReader reader )
    {
        return new Vector4
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle(),
            w = reader.ReadSingle()
        };
    }

    public static void Write( this BinaryWriter writer, Quaternion quat )
    {
        writer.Write( quat.x );
        writer.Write( quat.y );
        writer.Write( quat.z );
        writer.Write( quat.w );
    }

    public static Quaternion ReadQuaternion( this BinaryReader reader )
    {
        return new Quaternion
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle(),
            w = reader.ReadSingle()
        };
    }

    public static void Write( this BinaryWriter writer, Color colour )
    {
        writer.Write( colour.r );
        writer.Write( colour.g );
        writer.Write( colour.b );
        writer.Write( colour.a );
    }

    public static Color ReadColour( this BinaryReader reader )
    {
        return new Color
        {
            r = reader.ReadSingle(),
            g = reader.ReadSingle(),
            b = reader.ReadSingle(),
            a = reader.ReadSingle()
        };
    }

    public static void Write( this BinaryWriter writer, bool value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, byte value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, byte[] value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, char value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, char[] value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, decimal value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, double value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, int value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, long value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, short value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, string value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, ulong value ) { writer.Write( value ); }
    public static void Write( this BinaryWriter writer, ushort value ) { writer.Write( value ); }
    public static bool ReadBoolean( this BinaryReader reader ) { return reader.ReadBoolean(); }
    public static byte ReadByte( this BinaryReader reader ) { return reader.ReadByte(); }
    public static byte[] ReadBytes( this BinaryReader reader ) { return reader.ReadBytes(); }
    public static char ReadChar( this BinaryReader reader ) { return reader.ReadChar(); }
    public static char[] ReadChars( this BinaryReader reader ) { return reader.ReadChars(); }
    public static decimal ReadDecimal( this BinaryReader reader ) { return reader.ReadDecimal(); }
    public static float ReadSingle( this BinaryReader reader ) { return reader.ReadSingle(); }
    public static double ReadDouble( this BinaryReader reader ) { return reader.ReadDouble(); }
    public static string ReadString( this BinaryReader reader ) { return reader.ReadString(); }
    public static Int16 ReadInt16( this BinaryReader reader ) { return reader.ReadInt16(); }
    public static UInt16 ReadUInt16( this BinaryReader reader ) { return reader.ReadUInt16(); }
    public static Int32 ReadInt32( this BinaryReader reader ) { return reader.ReadInt32(); }
    public static UInt32 ReadUInt32( this BinaryReader reader ) { return reader.ReadUInt32(); }
    public static Int64 ReadInt64( this BinaryReader reader ) { return reader.ReadInt64(); }
    public static UInt64 ReadUInt64( this BinaryReader reader ) { return reader.ReadUInt64(); }

    static readonly Dictionary<Type, Func< BinaryReader, object > > lookupTable = new Dictionary<Type, Func< BinaryReader, object >>
    {
        { typeof( bool ), ( reader ) => { return reader.ReadBoolean(); } },
        { typeof( byte ), ( reader ) => { return reader.ReadByte(); } },
        { typeof( byte[] ), ( reader ) => { return reader.ReadBytes(); } },
        { typeof( char ), ( reader ) => { return reader.ReadChar(); } },
        { typeof( char[] ), ( reader ) => { return reader.ReadChars(); } },
        { typeof( decimal ), ( reader ) => { return reader.ReadDecimal(); } },
        { typeof( float ), ( reader ) => { return reader.ReadSingle(); } },
        { typeof( double ), ( reader ) => { return reader.ReadDouble(); } },
        { typeof( string ), ( reader ) => { return reader.ReadString(); } },
        { typeof( Int16 ), ( reader ) => { return reader.ReadInt16(); } },
        { typeof( UInt16 ), ( reader ) => { return reader.ReadUInt16(); } },
        { typeof( Int32 ), ( reader ) => { return reader.ReadInt32(); } },
        { typeof( UInt32 ), ( reader ) => { return reader.ReadUInt32(); } },
        { typeof( Int64 ), ( reader ) => { return reader.ReadInt64(); } },
        { typeof( UInt64 ), ( reader ) => { return reader.ReadUInt64(); } },
        { typeof( Vector2 ), ( reader ) => { return reader.ReadVector2(); } },
        { typeof( Vector2Int ), ( reader ) => { return reader.ReadVector2Int(); } },
        { typeof( Vector3 ), ( reader ) => { return reader.ReadVector3(); } },
        { typeof( Vector3Int ), ( reader ) => { return reader.ReadVector3Int(); } },
        { typeof( Vector4 ), ( reader ) => { return reader.ReadVector4(); } },
        { typeof( Quaternion ), ( reader ) => { return reader.ReadQuaternion(); } },
        { typeof( Color ), ( reader ) => { return reader.ReadColour(); } },
    };
}