using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class HashedString
{
    public string name;
    [ReadOnly] public int hash;

    HashedString() { }

    HashedString( string _name )
    {
        name = _name;
        hash = ( int )Utility.GetHashSHA256( name );
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( HashedString ) )]
public class HashedStringEditorUI : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return EditorGUIUtility.singleLineHeight * 2 + 2;
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        EditorGUI.BeginProperty( position, label, property );

        EditorGUI.indentLevel++;

        var nameRect = new Rect( position.x, position.y, position.width, position.height );
        var hashRect = new Rect( position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height );
        var hashProperty = property.FindPropertyRelative( "hash" );
        var nameProperty = property.FindPropertyRelative( "name" );
        EditorGUI.PropertyField( nameRect, nameProperty );
        hashProperty.intValue = ( int )Utility.GetHashSHA256( nameProperty.stringValue );
        EditorGUI.PropertyField( hashRect, hashProperty );

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}

#endif