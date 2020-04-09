using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable] public class GODictionary : SerializableDictionary<string, GameObject> { }

public class CommandMessage
{
    public bool isError;
    public string message;
}

public class CommandManager : MonoBehaviour
{
    
    private string[] cmdWords;
    
    [SerializeField] public GODictionary targetableObjects;
    public CommandMessage parseCommand(string command)
    {
        cmdWords = command.Split(' ');

        switch (cmdWords[0])
        {
            case "tp":
                return cmdTp();
            case "destroy":
                return cmdDestroy();
        }
        return commandError("unknown command.");
    }

    private CommandMessage cmdTp()
    {
        if (cmdWords.Length == 3) //mode target, destination
        {
            string targetKey = cmdWords[1];
            string destinationKey= cmdWords[2];
            if(targetableObjects.ContainsKey(targetKey))
            {
                if(targetableObjects.ContainsKey(destinationKey))
                {
                    GameObject target = targetableObjects[targetKey];
                    GameObject destination = targetableObjects[destinationKey];
                    if (target != null)
                    {
                        if (destination != null)
                        {
                            target.transform.position = destination.transform.position;
                            return commandSuccess("'" + targetKey + "' teleported to '" + destinationKey + "'.");
                        }
                        return commandError("'" + destinationKey + "' doesn't exist anymore.");
                    }
                    return commandError("'" + targetKey + "' doesn't exist anymore.");
                } 
                return commandError("unknown destination : '"+destinationKey+"'.");
            }
            return commandError("unknown target : '"+targetKey+"'.");
            
        }
        else if (cmdWords.Length == 2) //mode player to destination
        {
            
        }
        return commandError("tp command requires at least 1 parameter.");
    }
    
    private CommandMessage cmdDestroy()
    {
        if (cmdWords.Length == 2) //mode target, destination
        {
            string toDestroyKey = cmdWords[1];
            if(targetableObjects.ContainsKey(toDestroyKey))
            {
                GameObject toDestroy = targetableObjects[toDestroyKey];
                if (toDestroy != null)
                {
                    Destroy(toDestroy);
                    //targetableObjects.Remove(toDestroyKey);
                    return commandSuccess("'" + toDestroyKey + "' destroyed.");
                }
                return commandError("'" + toDestroyKey + "' has already been destroyed.");
            }
            return commandError("unknown target : '"+toDestroyKey+"'.");
        }
        return commandError("destroy command requires 1 parameter.");
    }

    private GameObject getTarget(string key)
    {
        if (targetableObjects[key] == null)
        {
            Debug.Log("null");
            return null;
        }
        return targetableObjects[key];
    }
    private CommandMessage commandSuccess(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage success = new CommandMessage();
        success.message = message;
        return success;
    }
    private CommandMessage commandError(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage error = new CommandMessage();
        error.isError = true;
        error.message = message;
        return error;
    }
}
