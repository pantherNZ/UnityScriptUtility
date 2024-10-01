using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Save
{
	public abstract class BaseSave
	{
		public BaseSave( string path, int version )
		{
			this.path = path;
			this.version = version;
		}

		[JsonIgnore] public int version { get; protected set; }
		[JsonIgnore] public string path { get; protected set; }
		[JsonIgnore] public abstract string fullPath { get; }
		[JsonIgnore] public string directory => Path.GetDirectoryName( fullPath );
		public abstract void Save();
		public bool ExistsOnDisk() { return File.Exists( fullPath ); }
		public bool DestroyFile() 
		{
			if( ExistsOnDisk() )
			{
				File.Delete( fullPath );
				return true;
			}
			return false;
		}
	}

	public abstract class JSONSave : BaseSave
	{
		[JsonIgnore] public static readonly string extension = ".json";
		[JsonIgnore] public override string fullPath { get { return Application.persistentDataPath + "/" + path + extension; } }


		public JSONSave( string path, int version )
			: base( path, version )
		{
			if ( !File.Exists( fullPath ) )
				return;

			using var reader = File.OpenText( fullPath );
			if ( reader == null )
				return;

			var json = reader.ReadToEnd();
			LoadFromJson( json );
		}


		public JSONSave( TextAsset jsonData, int version )
			: base( jsonData.name, version )
		{
			LoadFromJson( jsonData.text );
		}

		private void LoadFromJson( string json )
		{

			//try
			//{

				JsonConvert.PopulateObject( json, this, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				} );
			//}
			//catch ( Exception e )
			//{
			//	Debug.LogError( $"Failed to load JSON save file: {fullPath}\n{e}" );
			//}
		}

		public override void Save()
		{
			try
			{
				if ( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );

				var json = JsonConvert.SerializeObject( this, Formatting.Indented, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				} );
				using var writer = new StreamWriter( File.Create( fullPath ) );
				writer.Write( json );
			}
			catch ( Exception e )
			{
				Debug.LogError( $"Failed to save json file: {fullPath}\n{e}" );
			}
		}

		public static List<T> LoadAll<T>( string folderPath ) where T : JSONSave
		{
			var files = Directory.GetFiles( folderPath, $"*.{extension}" );
			return files.Select( x => ( T )Activator.CreateInstance( typeof( T ), x ) ).ToList();
		}
	}

	public abstract class BinarySave : BaseSave
	{
		public static readonly string extension = ".dat";
		public override string fullPath { get { return Application.persistentDataPath + "/" + path + extension; } }

		protected abstract void Serialise( System.IO.BinaryWriter writer );
		protected abstract void Deserialise( System.IO.BinaryReader reader );

		public BinarySave( string path, int version )
			: base( path, version )
		{
			if ( !File.Exists( fullPath ) )
				return;

			try
			{
				var bytes = File.ReadAllBytes( fullPath );
				using var memoryStream = new MemoryStream( bytes, writable: false );
				using var reader = new BinaryReader( memoryStream );
				this.version = reader.ReadByte();
				Deserialise( reader );
			}
			catch ( Exception e )
			{
				Debug.LogError( $"Failed to load binary save file: {fullPath}\n{e}" );
			}
		}

		public override void Save()
		{
			try
			{
				if ( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );
			
				using var memoryStream = new MemoryStream();
				using var writer = new BinaryWriter( memoryStream );

				writer.Write( ( byte )version );
				Serialise( writer );

				var content = memoryStream.ToArray();
				File.WriteAllBytes( fullPath, content );
			}
			catch ( Exception e )
			{
				Debug.LogError( "Failed to save game: " + e.ToString() );
			}
		}

		public static List<T> LoadAll<T>( string folderPath ) where T : BinarySave
		{
			var files = Directory.GetFiles( folderPath, $"*.{extension}" );
			return files.Select( x => ( T )Activator.CreateInstance( typeof( T ), x ) ).ToList();
		}
	}

	[JsonArray]
	public class Dictionary<K, V> : System.Collections.Generic.Dictionary<K, V>
	{
	}
}

public static partial class Utility
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

	public static void Write<T>( this BinaryWriter writer, T[] value )
	{
		writer.Write( value.Length );
		foreach ( var obj in value )
			writer.Write( obj );
	}

	public static T[] ReadArray<T>( this BinaryReader reader )
	{
		int count = reader.ReadInt32();
		var data = new T[count];
		for ( int i = 0; i < count; ++i )
			data[i] = ( T )ReadObject( reader, typeof( T ) );
		return data;
	}

	public static void Write<T>( this BinaryWriter writer, T[,] value )
	{
		writer.Write( value.GetLength( 0 ) );
		writer.Write( value.GetLength( 1 ) );
		for ( int x = 0; x < value.GetLength( 0 ); ++x )
			for ( int y = 0; y < value.GetLength( 1 ); ++y )
				writer.Write( value[x, y] );
	}

	public static T[,] ReadArray2D<T>( this BinaryReader reader )
	{
		int xCount = reader.ReadInt32();
		int yCount = reader.ReadInt32();
		var data = new T[xCount, yCount];
		for ( int x = 0; x < xCount; ++x )
			for ( int y = 0; y < yCount; ++y ) 
				data[x, y] = ( T )ReadObject( reader, typeof( T ) );
		return data;
	}

	public static void Write( this BinaryWriter writer, object value )
    {
        if( value == null )
        {
            writer.Write( "{{NULL}}" );
            return;
        }
        var type = value.GetType();
        if( type.IsEnum )
        {
            var underlyingValue = ( dynamic )Convert.ChangeType( value, Enum.GetUnderlyingType( type ) );
			Utility.Write( writer, underlyingValue );
        }
#if UNITY_EDITOR
		else if ( type == typeof( Material ) )
        {
			Utility.Write( writer, Utility.GetResourcePath( value as Material ) );
        }
        else if( type == typeof( Mesh ) )
        {
            var path = Utility.GetResourcePath( value as Mesh );
			Utility.Write( writer, path );
        }
        else if( type == typeof( PhysicMaterial ) )
        {
			Utility.Write( writer, Utility.GetResourcePath( value as PhysicMaterial ) );
        }
#endif
		else if( type == typeof( string ) )
        {
			Utility.Write( writer, value as string );
        }
        else
        {
            var dynamicValue = ( dynamic )value;
			Utility.Write( writer, dynamicValue );
        }
    }

	public static object ReadObject( this BinaryReader reader, Type type )
    {
        if( type == typeof( string ) )
        {
            var str = reader.ReadString();
            return str == "{{NULL}}" ? null : str;
        }
        if( type.IsEnum )
        {
            return SaveDetail.lookupTable[Enum.GetUnderlyingType( type )].Invoke( reader );
        }
        else if( type == typeof( Material ) )
        {
            var path = reader.ReadString();
            return Resources.Load<Material>( path );
        }
        else if( type == typeof( Mesh ) )
        {
            var path = reader.ReadString();
            return Resources.Load<Mesh>( path );
        }
        else if( type == typeof( PhysicMaterial ) )
        {
            var path = reader.ReadString();
            return Resources.Load<PhysicMaterial>( path );
        }
        else if( !SaveDetail.lookupTable.ContainsKey( type ) )
            Debug.LogError( "ReadObject Error: Failed to read object of type: " + type.ToString() );
        return SaveDetail.lookupTable[type].Invoke( reader );
    }

    public static class SaveDetail
    {
        public static readonly Dictionary<Type, Func<BinaryReader, object>> lookupTable = new Dictionary<Type, Func<BinaryReader, object>>
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
}
