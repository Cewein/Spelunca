using UnityEngine;

public class InputNameAttribute : PropertyAttribute
{
    public string inputName;

    public InputNameAttribute(string defaultValue)
    {
        inputName = defaultValue;
    }
}
