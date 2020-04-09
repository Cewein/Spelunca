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
    private string help = "See '/help' for more info.";
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
            case "chunk":
                return chunkCommands();
        }
        return commandError("Unknown command. "+help);
    }
    
    //CHUNK COMMANDS

    private CommandMessage chunkCommands()
    {
        if (cmdWords.Length > 1) //si on a au moins un parametre en plus que 'chunk'
        {
            switch (cmdWords[1])
            {
                case "show":
                    return chunkShowCommands();
                case "reload":
                    return cmdChunkReload();
            }
            return commandError("Unknown '/chunk "+cmdWords[1]+"'. "+help);
        }
        return commandError("'/chunk' is a group of commands. "+help);
    }

    private CommandMessage chunkShowCommands()
    {
        if (cmdWords.Length > 2) //si on a au moins un parametre en plus que 'chunk show'
        {
            switch (cmdWords[2])
            {
                case "border":
                    return cmdChunkShowBorder();
            }
            return commandError("Unknown '/chunk "+cmdWords[2]+"'. "+help);
        }
        return commandError("'/chunk show' can't be used without parameters. "+help);
    }
    
    private CommandMessage cmdChunkReload()
    {
        return commandSuccess("Chunks reloaded.");
    }
    private CommandMessage cmdChunkShowBorder()
    {
        if (cmdWords.Length < 4)
            return commandError("'/chunk show border' can't be used without parameters. Expected 'true' or 'false'.");
        if (cmdWords.Length > 4)
            return commandError("'/chunk show border' can't have more than 1 parameter. "+help);
        
        switch (cmdWords[3])
        {
            case "true":
                return commandSuccess("Chunk borders are now visible.");
            case "false":
                return commandSuccess("Chunk borders are now hidden.");
        }
        return commandError("Unknown parameter "+cmdWords[3]+"'. Expected 'true' or 'false'.");
        
        return commandError("'/chunk show' can't be used without parameters. "+help);
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
        if (cmdWords.Length == 2){} //mode player to destination
        return commandError("tp command requires at least 1 parameter. "+help);
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
            return commandError("Unknown target : '"+toDestroyKey+"'.");
        }
        return commandError("Destroy command requires 1 parameter. "+help);
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
