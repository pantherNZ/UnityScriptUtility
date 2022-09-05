using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputPriorityButton : Button
{
    public string key;
    public int priority;

    public override void OnPointerClick( PointerEventData eventData )
    {
        SortedList<int, InputPriorityButton> entries = new();
        var objects = Utility.GetObjectsOverPointer();

        foreach( var obj in objects )
            if( obj.TryGetComponent<InputPriorityButton>( out var button ) )
                entries.TryAdd( button.priority, button );

        entries.Values[^1].OnPointerClickBase( eventData );

        //Utility.InputPriority.Instance.Request( () => true, key, priority, () =>
        //{
        //    base.OnPointerClick( eventData );
        //} );
    }

    public void OnPointerClickBase( PointerEventData eventData )
    {
        base.OnPointerClick( eventData );
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( InputPriorityButton ) )]
public class InputPriorityButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        var targetMenuButton = ( InputPriorityButton )target;

        targetMenuButton.key = EditorGUILayout.TextField( "Key", targetMenuButton.key );
        targetMenuButton.priority = EditorGUILayout.IntField( "Priority", targetMenuButton.priority );

        base.OnInspectorGUI();
    }
}
#endif