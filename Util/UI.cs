using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace Utils 
{
	public class UI
	{
		static public void EnableClass( bool enabled, VisualElement element, string className ) {
			if ( enabled ) {
				element.AddToClassList( className );
			} else {
				element.RemoveFromClassList( className );
			}
		}
		static public void SwapClass( bool enabled, VisualElement element, string falseClassName, string trueClassName )
		{
			if ( enabled )
			{
				element.RemoveFromClassList( falseClassName );
				element.AddToClassList( trueClassName );
			}
			else
			{
				element.RemoveFromClassList( trueClassName );
				element.AddToClassList( falseClassName );
			}
		}

		static public void FillInText( Label label, List<string> variables )
		{
			for (int i = 0; i < variables.Count; i++ )
			{
				label.text = label.text.Replace( "{" + i + "}", variables[i] );
			}
		}
	}
}
