using System;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public static partial class Utility
{
#if UNITY_EDITOR
	public static VisualElement FixedHorizontalStyle( this VisualElement elem, int percentage )
	{
		return UI.FixedHorizontalStyle( elem, percentage );
	}
#endif

	public class UI
	{
#if UNITY_EDITOR
		public static PropertyField CreatePropertyField( SerializedProperty property, string name, int percentage )
		{
			var field = new PropertyField( property.FindPropertyRelative( name ), "" );
			FixedHorizontalStyle( field, percentage );
			return field;
		}

		public static VisualElement FixedHorizontalStyle( VisualElement elem, int percentage )
		{
			elem.style.minWidth = new StyleLength( new Length( percentage, LengthUnit.Percent ) );
			elem.style.maxWidth = new StyleLength( new Length( percentage, LengthUnit.Percent ) );
			elem.style.alignSelf = new StyleEnum<Align>( Align.Center );
			return elem;
		}
#endif

		public static void EnableClass( bool enabled, VisualElement element, string className )
		{
			if ( enabled )
			{
				element.AddToClassList( className );
			}
			else
			{
				element.RemoveFromClassList( className );
			}
		}
		public static void SwapClass( bool enabled, VisualElement element, string falseClassName, string trueClassName )
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

		public static void FillInText( Label label, List<string> variables )
		{
			for ( int i = 0; i < variables.Count; i++ )
			{
				label.text = label.text.Replace( "{" + i + "}", variables[i] );
			}
		}

		public static VisualElement Head( VisualElement element )
		{
			var head = element;
			while ( head.parent != null )
			{
				head = head.parent;
			}
			return head;
		}

		public class RepeatTransition
		{
			VisualElement element;
			List<string> ussClasses = new();
			string deactivatedClass;
			bool active = false;
			int index = 0;

			public RepeatTransition( VisualElement element, string ussClass, string deactivatedClass = "" )
			{
				this.element = element;
				this.ussClasses.Add( ussClass );
				this.deactivatedClass = deactivatedClass;
				if ( deactivatedClass != "" )
					element.AddToClassList( deactivatedClass );
			}

			public RepeatTransition( VisualElement element, List<string> ussClasses, string deactivatedClass = "" )
			{
				this.element = element;
				this.ussClasses = ussClasses;
				this.deactivatedClass = deactivatedClass;
				if ( deactivatedClass != "" )
					element.AddToClassList( deactivatedClass );
			}

			public void Activate()
			{
				if ( active )
					return;
				active = true;
				index = 0;
				element.RegisterCallback<TransitionEndEvent>( OnTransitionEnd );
				element.AddToClassList( ussClasses[0] );
				if ( deactivatedClass != "" )
					element.RemoveFromClassList( deactivatedClass );
			}

			public void Deactivate()
			{
				active = false;
			}

			public void OneShot()
			{
				Activate();
				Deactivate();
			}

			void OnTransitionEnd( TransitionEndEvent evt )
			{
				var sameNextClass = ussClasses[index % ussClasses.Count] == ussClasses[(index + 1) % ussClasses.Count];
				if ( sameNextClass || !active )
					element.RemoveFromClassList( ussClasses[index % ussClasses.Count] );
				if ( !active )
				{
					if ( deactivatedClass != "" )
						element.AddToClassList( deactivatedClass );
					element.UnregisterCallback<TransitionEndEvent>( OnTransitionEnd );
					return;
				}

				element.schedule.Execute( () =>
				{
					element.AddToClassList( ussClasses[(index + 1) % ussClasses.Count] );
					if ( !sameNextClass )
						element.RemoveFromClassList( ussClasses[index % ussClasses.Count] );
					++index;
				} );
			}
		}

		public class CounterLabel
		{
			public class ColorThreshold {
				public int threshold;
				public Color color;

				public ColorThreshold( int threshold, Color color )
				{
					this.threshold = threshold;
					this.color = color;
				}
			}

			public event Action finished;

			Label countLabel;
			Label deltaLabel;
			string unit;
			bool showPositive;
			float rootTime;
			float transitionTime;
			float targetTime;
			int rootCount;
			int targetCount;
			IVisualElementScheduledItem task;
			List<ColorThreshold> colorThresholds = new List<ColorThreshold>();

			public CounterLabel( Label label, Label delta, float transitionTime, string unit = "", bool showPositive = true )
			{
				this.countLabel = label;
				this.deltaLabel = delta;
				this.transitionTime = transitionTime;
				this.unit = unit;
				this.showPositive = showPositive;
				task = countLabel.schedule.Execute( UpdateCount );
				task.Every( 20 );
				task.Pause();

				SetBasicColorThresholds();
			}

			public void SetBasicColorThresholds()
			{
				SetColorThresholds( new List<ColorThreshold>() {
					new ( -5, new Color( 1.0f, 0.7f, 0.7f ) ),
					new ( 0, new Color( 0.7f, 0.7f, 0.7f ) ),
					new ( 5, new Color( 0.7f, 1.0f, 0.7f ) )
				} );
			}

			public void SetColorThresholds( List<ColorThreshold> thresholds )
			{
				for ( int i = 0; i < thresholds.Count - 1; i++ )
				{
					if ( thresholds[i].threshold >= thresholds[i + 1].threshold )
						throw new Exception( "Thresholds must be in ascending order" );
				}
				colorThresholds.Clear();
				colorThresholds = thresholds;
			}

			public void SetInstantCount( int count )
			{
				rootCount = count;
				targetCount = count;
				countLabel.text = count.ToString() + unit;
			}

			public void SetCount( int count )
			{
				if ( count == targetCount )
					return;

				if ( rootCount == targetCount )
				{
					rootCount = Int32.Parse( countLabel.text.Substring(0, countLabel.text.Length - unit.Length ) );
					task.Resume();
					rootTime = Time.time;
				}

				targetCount = count;
				targetTime = Time.time + transitionTime;
			}

			void UpdateCount()
			{
				float time = Time.time - rootTime;
				float ratio = time / ( targetTime - rootTime );
				ratio = Mathf.Sqrt( ratio ); //easeOut
				ratio = Mathf.Min( 1.0f, ratio );
				int delta = Mathf.RoundToInt( ( targetCount - rootCount ) * ratio );
				if ( deltaLabel != null )
				{
					deltaLabel.text = ( ( delta > 0 && showPositive ) ? "+" : "" ) + delta.ToString() + unit;
					if ( delta != 0 )
						deltaLabel.RemoveFromClassList( "FadeOut" );
					if ( !colorThresholds.IsEmpty() )
					{
						if ( delta <= colorThresholds.Front().threshold )
						{
							deltaLabel.style.color = colorThresholds.Front().color;
						}
						else if ( delta >= colorThresholds.Back().threshold )
						{
							deltaLabel.style.color = colorThresholds.Back().color;
						}
						else {
							for ( int i = 0; i < colorThresholds.Count - 1; i++ )
							{
								if ( delta >= colorThresholds[i].threshold && delta <= colorThresholds[i + 1].threshold )
								{
									deltaLabel.style.color = Color.Lerp( colorThresholds[i].color, colorThresholds[i + 1].color, ( (float) delta - colorThresholds[i].threshold ) / ( colorThresholds[i + 1].threshold - colorThresholds[i].threshold ) );
									break;
								}
							}
						}
					}
				}

				if ( ratio >= 1.0f )
				{
					deltaLabel?.AddToClassList( "FadeOut" );
					countLabel.text = targetCount.ToString() + unit;
					rootCount = targetCount;
					task.Pause();
					finished?.Invoke();
					return;
				}

				int count = rootCount + delta;
				countLabel.text = count.ToString() + unit;
			}
		}
	}
}
