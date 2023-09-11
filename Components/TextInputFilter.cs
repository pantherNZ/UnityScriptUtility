using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

[Flags]
public enum TextInputFilters
{
    Uppercase = 1 << 0,
    Lowercase = 1 << 1,
    NoWhitespace = 1 << 2,
}

public enum CharacterValidation
{
    None,
    Integer,
    Decimal,
    Digit,
    Alphanumeric,
    Name,
    EmailAddress,
    Regex,
    UnityLobbyCode,
}

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class TextInputFilter : MonoBehaviour
{
    [SerializeField] TextInputFilters filters;
    [SerializeField] CharacterValidation validation = CharacterValidation.None;
    [SerializeField] string regexValue;

    const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";
    private TMPro.TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
        inputField.onValidateInput += Validate;

        if( validation == CharacterValidation.UnityLobbyCode )
            inputField.characterLimit = 6;
    }

    protected char Validate( string text, int pos, char ch )
    {
        // Validation is disabled
        if( filters == 0 || !enabled )
            return ch;

        var lobbyCode = validation == CharacterValidation.UnityLobbyCode;

        if( lobbyCode || filters.HasFlag( TextInputFilters.NoWhitespace ) )
            if( ch == ' ' )
                return ( char )0;

        if( lobbyCode || filters.HasFlag( TextInputFilters.Uppercase ) )
            ch = char.ToUpper( ch );
        else if( filters.HasFlag( TextInputFilters.Lowercase ) )
            ch = char.ToUpper( ch );

        if( lobbyCode )
            if( "ZVUSQOLIB12589".Contains( ch ) )
                return ( char )0;

        if( validation == CharacterValidation.None )
        {
            return ch;
        }
        else if( validation == CharacterValidation.Integer || validation == CharacterValidation.Decimal )
        {
            // Integer and decimal
            bool cursorBeforeDash = ( pos == 0 && text.Length > 0 && text[0] == '-' );
            if( !cursorBeforeDash )
            {
                if( ch >= '0' && ch <= '9' ) 
                    return ch;
                if( ch == '-' && ( pos == 0 || inputField.stringPosition == 0 ) ) 
                    return ch;

                var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if( ch == Convert.ToChar( separator ) && validation == CharacterValidation.Decimal && !text.Contains( separator ) ) 
                    return ch;
            }
        }
        else if( validation == CharacterValidation.Digit )
        {
            if( ch >= '0' && ch <= '9' ) 
                return ch;
        }
        else if( lobbyCode || validation == CharacterValidation.Alphanumeric )
        {
            // All alphanumeric characters
            if( ch >= 'A' && ch <= 'Z' ) return ch;
            if( ch >= 'a' && ch <= 'z' ) return ch;
            if( ch >= '0' && ch <= '9' ) return ch;
        }
        else if( validation == CharacterValidation.Name )
        {
            char prevChar = ( text.Length > 0 ) ? text[Mathf.Clamp( pos - 1, 0, text.Length - 1 )] : ' ';
            char lastChar = ( text.Length > 0 ) ? text[Mathf.Clamp( pos, 0, text.Length - 1 )] : ' ';
            char nextChar = ( text.Length > 0 ) ? text[Mathf.Clamp( pos + 1, 0, text.Length - 1 )] : '\n';

            if( char.IsLetter( ch ) )
            {
                // First letter is always capitalized
                if( char.IsLower( ch ) && pos == 0 )
                    return char.ToUpper( ch );

                // Letter following a space or hyphen is always capitalized
                if( char.IsLower( ch ) && ( prevChar == ' ' || prevChar == '-' ) )
                    return char.ToUpper( ch );

                // Uppercase letters are only allowed after spaces, apostrophes, hyphens or lowercase letter
                if( char.IsUpper( ch ) && pos > 0 && prevChar != ' ' && prevChar != '\'' && prevChar != '-' && !char.IsLower( prevChar ) )
                    return char.ToLower( ch );

                // Do not allow uppercase characters to be inserted before another uppercase character
                if( char.IsUpper( ch ) && char.IsUpper( lastChar ) )
                    return ( char )0;

                // If character was already in correct case, return it as-is.
                // Also, letters that are neither upper nor lower case are always allowed.
                return ch;
            }
            else if( ch == '\'' )
            {
                // Don't allow more than one apostrophe
                if( lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains( "'" ) )
                    return ch;
            }

            // Allow inserting a hyphen after a character
            if( char.IsLetter( prevChar ) && ch == '-' && lastChar != '-' )
            {
                return ch;
            }

            if( ( ch == ' ' || ch == '-' ) && pos != 0 )
            {
                // Don't allow more than one space in a row
                if( prevChar != ' ' && prevChar != '\'' && prevChar != '-' &&
                    lastChar != ' ' && lastChar != '\'' && lastChar != '-' &&
                    nextChar != ' ' && nextChar != '\'' && nextChar != '-' )
                    return ch;
            }
        }
        else if( validation == CharacterValidation.EmailAddress )
        {
            // From StackOverflow about allowed characters in email addresses:
            // Uppercase and lowercase English letters (a-z, A-Z)
            // Digits 0 to 9
            // Characters ! # $ % & ' * + - / = ? ^ _ ` { | } ~
            // Character . (dot, period, full stop) provided that it is not the first or last character,
            // and provided also that it does not appear two or more times consecutively.

            if( ch >= 'A' && ch <= 'Z' ) return ch;
            if( ch >= 'a' && ch <= 'z' ) return ch;
            if( ch >= '0' && ch <= '9' ) return ch;
            if( ch == '@' && text.IndexOf( '@' ) == -1 ) return ch;
            if( kEmailSpecialCharacters.IndexOf( ch ) != -1 ) return ch;
            if( ch == '.' )
            {
                char lastChar = ( text.Length > 0 ) ? text[Mathf.Clamp( pos, 0, text.Length - 1 )] : ' ';
                char nextChar = ( text.Length > 0 ) ? text[Mathf.Clamp( pos + 1, 0, text.Length - 1 )] : '\n';
                if( lastChar != '.' && nextChar != '.' )
                    return ch;
            }
        }
        else if( validation == CharacterValidation.Regex )
        {
            // Regex expression
            if( Regex.IsMatch( ch.ToString(), regexValue ) )
            {
                return ch;
            }
        }

        return ( char )0;
    }
}