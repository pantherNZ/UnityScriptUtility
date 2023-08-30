using System;
using System.Linq;
using UnityEngine;

[Flags]
public enum TextInputFilters
{
    Uppercase = 1,
    Lowercase = 2,
    NoWhitespace = 4,
}

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class TextInputFilter : MonoBehaviour
{
    [SerializeField] TextInputFilters filters;

    private void Start()
    {
        var inputField = GetComponent<TMPro.TMP_InputField>();
        inputField.onValueChanged.AddListener( newValue =>
        {
            if( filters.HasFlag( TextInputFilters.NoWhitespace ) )
                newValue = newValue.Replace( " ", "" );

            if( filters.HasFlag( TextInputFilters.Uppercase ) )
                newValue = string.Concat( newValue.Select( x => char.ToUpper( x ) ) );
            else if( filters.HasFlag( TextInputFilters.Lowercase ) )
                newValue = string.Concat( newValue.Select( x => char.ToLower( x ) ) );

            inputField.SetTextWithoutNotify( newValue );
        } );

        //GetComponent<TMPro.TMP_InputField>().onValidateInput += delegate ( string input, int charIndex, char addedChar )
        //{
        //   
        //};
    }
}