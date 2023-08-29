using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class TextInputUppercase : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMPro.TMP_InputField>().onValidateInput += delegate ( string input, int charIndex, char addedChar )
        {
            return char.ToUpper( addedChar );
        };
    }
}