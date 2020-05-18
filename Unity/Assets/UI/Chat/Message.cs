using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]public class Message
{
    public string text;
    public Text textObject;

    public enum Type
    {
        playerMessage,
        command,
        info,
        warning,
        error
    }
}
