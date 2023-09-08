using Mono.Data.Sqlite;

public static partial class Extensions
{
    public static string GetStringSafe( this SqliteDataReader reader, int colIndex )
    {
        if( !reader.IsDBNull( colIndex ) )
            return reader.GetString( colIndex );
        return string.Empty;
    }

    public static int GetInt32Safe( this SqliteDataReader reader, int colIndex )
    {
        if( !reader.IsDBNull( colIndex ) )
            return reader.GetInt32( colIndex );
        return 0;
    }

}
