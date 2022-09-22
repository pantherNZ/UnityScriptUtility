using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputPriorityButton : MultiImageButton
{
    public string key;
    public int priority;

    public override void OnPointerClick( PointerEventData eventData )
    {
        SortedList<int, InputPriorityButton> entries = new SortedList<int, InputPriorityButton>();
        var objects = Utility.GetObjectsOverPointer();

        foreach( var obj in objects )
            if( obj.TryGetComponent<InputPriorityButton>( out var button ) )
                if( !entries.ContainsKey( button.priority ) )
                    entries.Add( button.priority, button );

        if( entries.Count == 0 )
            entries.Add( priority, this );

        InputPriority.Instance.Request( () => true, key, priority, () =>
        {
            entries.Values[entries.Values.Count - 1].OnPointerClickBase( eventData );
        } );
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