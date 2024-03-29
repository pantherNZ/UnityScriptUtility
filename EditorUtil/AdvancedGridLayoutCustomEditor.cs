﻿using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor( typeof( AdvancedGridLayout ) )]
public class AdvancedGridLayoutCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AdvancedGridLayout aglg = ( this.target as AdvancedGridLayout );
        aglg.padding = new RectOffset( EditorGUILayout.IntField( "P Left", aglg.padding.left ), aglg.padding.right, aglg.padding.top, aglg.padding.bottom );
        aglg.padding = new RectOffset( aglg.padding.left, EditorGUILayout.IntField( "P Right", aglg.padding.right ), aglg.padding.top, aglg.padding.bottom );
        aglg.padding = new RectOffset( aglg.padding.left, aglg.padding.right, EditorGUILayout.IntField( "P Top", aglg.padding.top ), aglg.padding.bottom );
        aglg.padding = new RectOffset( aglg.padding.left, aglg.padding.right, aglg.padding.top, EditorGUILayout.IntField( "P Bottom", aglg.padding.bottom ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "m_Spacing" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "m_StartCorner" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "m_StartAxis" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "m_ChildAlignment" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "cellsPerLine" ) );
        EditorGUILayout.PropertyField( this.serializedObject.FindProperty( "aspectRatio" ) );
        GUI.enabled = false;
        EditorGUILayout.Vector2Field( "Cell size", aglg.cellSize );
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();

    }
}

#endif