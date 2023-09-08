using UnityEngine;
using UnityEngine.UI;

public class MultiImageButton : Button
{
    private Graphic[] imageGraphics;

    protected override void DoStateTransition( SelectionState state, bool instant )
    {
        base.DoStateTransition( state, instant );

        if( !GetGraphics() )
            return;

        var targetColor =
            state == SelectionState.Disabled ? colors.disabledColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.normalColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;

        foreach( var graphic in imageGraphics )
            graphic.CrossFadeColor( targetColor, instant ? 0 : colors.fadeDuration, true, true );
    }

    private bool GetGraphics()
    {
        var targetGraphics = GetComponent<MultiImageTargetGraphics>();
        if( targetGraphics == null )
            return false;
        imageGraphics = targetGraphics.GetTargetGraphics;
        return imageGraphics.Length > 0;
    }
}