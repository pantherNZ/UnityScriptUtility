using UnityEditor;
using UnityEngine;

partial class Utility
{
	public static Texture2DArray CreateTexure2DArray( Texture2D[] tex, bool generateMips = true )
	{
		bool mips = generateMips && tex[0].mipmapCount > 1;

		var texArray = new Texture2DArray( tex[0].width, tex[0].height, tex.Length, tex[0].format, mips, false )
		{
			anisoLevel = tex[0].anisoLevel,
			filterMode = tex[0].filterMode,
			wrapMode = tex[0].wrapMode
		};

		// Go over all the textures and add to array
		for ( int texIndex = 0; texIndex < tex.Length; texIndex++ )
		{
			if ( tex[texIndex] != null )
			{
				for ( int mip = 0; mip < ( generateMips ? tex[texIndex].mipmapCount : 1 ); mip++ )
					Graphics.CopyTexture( tex[texIndex], 0, mip, texArray, texIndex, mip );
			}
		}

		return texArray;
	}
}

public class TextureArrayWizard : ScriptableWizard
{
	[MenuItem( "Assets/Create/Texture Array" )]
	static void CreateWizard()
	{
		DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
	}

	void OnWizardCreate()
	{
		if ( textures.Length == 0 )
		{
			return;
		}

		string path = EditorUtility.SaveFilePanelInProject("Save Texture Array", "Texture Array", "asset", "Save Texture Array");

		if ( path.Length == 0 )
		{
			return;
		}

		var texArray = Utility.CreateTexure2DArray( textures );
		AssetDatabase.CreateAsset( texArray, path );
	}

	public Texture2D[] textures;
}
