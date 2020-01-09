using System.Linq;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if(instance == null) 
                instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            return instance;
        }
    }
}
