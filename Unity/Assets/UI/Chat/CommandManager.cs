﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;


[Serializable] public class GODictionary : SerializableDictionary<string, GameObject> { }

public class CommandMessage
{
    public Message.Type type;
    public string message;
}

public class CommandManager : MonoBehaviour
{
    
    [SerializeField] private CelShadingMaster celShading;
    [SerializeField] private Camera mainCamera;
    [SerializeField] public GODictionary targetableObjects;
    
    private string[] cmdWords;
    private string help = "See '/help' for more info.";
    private CommandHelpInfo helpInfos;
    
    
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
            case "setmarker":
                return cmdSetMarker();
            case "cel":
                return celCommands();
            case "help":
                return cmdHelp();
        }
        return commandError("Unknown command. "+help);
    }

    private CommandMessage celCommands()
    {
        if (cmdWords.Length > 1) //si on a au moins un parametre en plus que 'chunk'
        {
            switch (cmdWords[1])
            {
                case "on":
                    celShading.intensity = 1f;
                    celShading.updateShader();
                    return commandSuccess("Cel shading activated.");
                case "false":
                    celShading.intensity = 0f;
                    celShading.updateShader();
                    return commandSuccess("Cel shading deactivated.");
                case "debug":
                    return cmdCelDebug();
            }
            return commandError("Unknown '/cel debug "+cmdWords[1]+"'. "+help);
        }
        return commandError("'/cel' is a group of commands. "+help);
    }

    private CommandMessage cmdCelDebug()
    {
        if (cmdWords.Length == 3) //si on a au moins un parametre en plus que 'chunk'
        {
            switch (cmdWords[2])
            {
                case "true":
                    celShading.debugMode = true;
                    celShading.updateShader();
                    return commandSuccess("Cel shading debug mode activated.");
                case "false":
                    celShading.debugMode = false;
                    celShading.updateShader();
                    return commandSuccess("Cel shading debug mode deactivated.");
            }
            return commandError("Unknown '/cel debug "+cmdWords[1]+"'. Expected 'on' or 'false'"+help);
        }
        return commandError("'/cel debug' requires a parameter. "+help);
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

//----------------------------------------------------------------------------------------------------------------

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

        if (cmdWords.Length == 2)
        { 
            string destinationKey= cmdWords[1];
            if(targetableObjects.ContainsKey(destinationKey))
            {
                
                GameObject target = targetableObjects[targetableObjects.Keys.First()]; //ATTENTION TOUJOURS METTRE LE JOUEUR EN PREMIER
                GameObject destination = targetableObjects[destinationKey];
                if (target != null)
                {
                    if (destination != null)
                    {
                        target.transform.position = destination.transform.position;
                        return commandSuccess("'" + targetableObjects.Keys.First() + "' teleported to '" + destinationKey + "'.");
                    }
                    return commandError("'" + destinationKey + "' doesn't exist anymore.");
                }
                return commandError("'" + targetableObjects.Keys.First() + "' doesn't exist anymore.");
            } 
            return commandError("unknown destination : '"+destinationKey+"'.");
        } //mode player to destination
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
        return commandError("'/destroy' command requires 1 parameter. "+help);
    }
    
    
    private CommandMessage cmdSetMarker()
    {
        if (cmdWords.Length == 3) //mode target, destination
        {
            if(targetableObjects.ContainsKey(cmdWords[2]))
                return commandError("Marker name '"+cmdWords[2]+"' already exist. Please chose an other name "+help);
            GameObject marker = new GameObject();
            GameObject go;
            RaycastHit hit;
            switch (cmdWords[1])
            {
                case "player":
                    GameObject player = targetableObjects[targetableObjects.Keys.First()]; 
                    marker = new GameObject();
                    go = Instantiate(marker, player.transform.position, player.transform.rotation);
                    go.name = cmdWords[2];
                    targetableObjects.Add(cmdWords[2],go);
                    return commandSuccess("Marker '"+cmdWords[2]+"' set at player's position :" + go.transform.position + ".");
                case "cursor":
                    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 1000f))
                    {
                        go = Instantiate(marker, hit.point, new Quaternion());
                        go.name = cmdWords[2];
                        targetableObjects.Add(cmdWords[2],go);
                        return commandSuccess("Marker '"+cmdWords[2]+"' set at cursor's position :" + go.transform.position + ".");
                    }
                    return commandError("Nothing is pointed by the cursor.");
                case "target":
                    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 1000f))
                    {
                        targetableObjects.Add(cmdWords[2],hit.transform.gameObject);
                        return commandSuccess("Object '" + hit.transform.gameObject + "' marked as '"+cmdWords[2]+"'.");
                    }
                    return commandError("Nothing is pointed by the cursor.");
                //return commandWarning("Will be implemented later.");
            }
            return commandError("Unknown mode : '"+cmdWords[1]+"'. "+help);
        }
        return commandError("'/setmarker' command requires 2 parameter. "+help);
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

    private CommandMessage cmdHelp()
    {
        if (cmdWords.Length > 1)
        {
            switch (cmdWords[1])
            {
                case "gameplay":
                    return commandInfo("");
                case "controls":
                    return commandInfo("");
            }
        }
        string helpFileContent = "---------------- HELPER ----------------\n\nHere are all the commands currently available:\n\n" +
                                 " - /destroy [id] :\n   Destroy a marked object by giving its id.\n\n" +
                                 " - /setmark player [id] :\n   Creates a marker in game named [id].\n\n" +
                                 " - /setmark cursor [id] :\n   Creates a marker at cursor's position named [id].\n\n" +
                                 " - /setmark target [id] :\n   Marks the object pointed by the cursor and names it [id].\n\n" +
                                 " - /chunk show border [true|false] :\n   (WIP) Shows the chunk's borders.\n\n" +
                                 " - /chunk reload :\n   (WIP) Reloads the current chunks.\n\n" +
                                 " - /tp [target] [destination] :\n   Teleports [target] [destination].\n\n" +
                                 " - /tp [destination] :\n   Teleports the player [destination].\n\n" +
                                 " - /cel [true|false] :\n   Activates/deactivates the cel shading effect.\n\n" +
                                 " - /cel debug [true|false] :\n   Activates/deactivates the cel shading debug mode.\n" +
                                 " - /godmode [true|false] :\n   Activates/deactivates the cel shading debug mode.\n" +
                                 " - /cel debug [true|false] :\n   Activates/deactivates the cel shading debug mode.\n";
        return commandInfo(helpFileContent);
        //Dictionary<string,> = JsonUtility.FromJson(helpFileContent);

    }
/*
    private void updateHelper(string url)
    {
        XmlReader.
        xml.LoadXml(fileData);
        XmlNodeList resources = xml.SelectNodes("data/resource");
        SortedDictionary<string,string> dictionary = new SortedDictionary<string,string>();
        foreach (XmlNode node in resources){
            dictionary.Add(node.Attributes["key"].Value, node.InnerText);
        }
    }*/
    
    private CommandMessage commandSuccess(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage success = new CommandMessage();
        success.message = message;
        success.type = Message.Type.command;
        
        return success;
    }
    private CommandMessage commandError(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage error = new CommandMessage();
        error.type = Message.Type.error;
        error.message = message;
        return error;
    }
    
    private CommandMessage commandWarning(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage warning = new CommandMessage();
        warning.type = Message.Type.warning;
        warning.message = message;
        return warning;
    }
    private CommandMessage commandInfo(string message)
    {
        this.cmdWords = new string[0]; //pour éviter des potentiels résidus on vide la liste de mots
        CommandMessage warning = new CommandMessage();
        warning.type = Message.Type.info;
        warning.message = message;
        return warning;
    }
}