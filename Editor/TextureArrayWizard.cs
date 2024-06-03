using UnityEditor;
using UnityEngine;

partial class Utility
{
	public static Texture2DArray CreateTexure2DArray( params Texture2D[] tex )
	{
		bool mips = tex[0].mipmapCount > 1;

		var texArray = new Texture2DArray( tex[0].width, tex[0].height, tex.Length, tex[0].format, false, false )
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
				for ( int mip = 0; mip < 1/*tex[texIndex].mipmapCount*/; mip++ )
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
