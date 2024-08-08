﻿using UnityEngine.AI;

public static partial class Utility
{
	public static int? GetNavMeshAgentID( string name )
	{
		for ( int i = 0; i < NavMesh.GetSettingsCount(); i++ )
		{
			NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex( index: i );
			if ( name == NavMesh.GetSettingsNameFromID( agentTypeID: settings.agentTypeID ) )
			{
				return settings.agentTypeID;
			}
		}
		return null;
	}
}
