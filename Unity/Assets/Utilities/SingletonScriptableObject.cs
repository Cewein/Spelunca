using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            
            T[] list = Resources.FindObjectsOfTypeAll<T>();
            if (list.Length == 0)
            {
                //Debug.LogError("There is no " + typeof(T).ToString());
                return null;
            }
            else if (list.Length > 1)
            {
                //Debug.Log("There is more than one " + typeof(T).ToString() + " but it must be a singleton ScriptableObject");
                return null;
            }

            instance = list[0];
            instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return instance;
        }
    }
}
