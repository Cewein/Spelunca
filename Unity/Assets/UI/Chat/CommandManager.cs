using System;
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
    [SerializeField] private PlayerStats playerStats;
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
            case "invincible":
                return cmdInvincible();
            case "maxresources":
                return cmdMaxResources();
            case "konami":
                return cmdKonami();
            case "help":
                return cmdHelp();
        }
        return commandError("Unknown command. "+help);
    }

    private CommandMessage cmdInvincible()
    {
        if (cmdWords.Length == 2)
        {
            switch (cmdWords[1])
            {
                case "true":
                    playerStats.invincible = true;
                    return commandSuccess("Invincibility activated.");
                case "false":
                    playerStats.invincible = false;
                    return commandSuccess("Invincibility deactivated.");
            }
            return commandError("Unknown '" + cmdWords[1] + "'Expected 'true' or 'false'.");
        }
        return commandError("'/invincible' is expecting 1 parameter.");
    }
    private CommandMessage cmdMaxResources()
    {
        if (cmdWords.Length == 2)
        {
            switch (cmdWords[1])
            {
                case "true":
                    
                    return commandWarning("Will be implemented later.");
                case "false":
                    return commandSuccess("Will be implemented later.");
            }
            return commandError("Unknown '" + cmdWords[1] + "'Expected 'true' or 'false'.");
        }
        return commandError("'/konami' is expecting 1 parameter.");
    }
    private CommandMessage cmdKonami()
    {
        if (cmdWords.Length == 2)
        {
            switch (cmdWords[1])
            {
                case "true":
                    cmdMaxResources();
                    cmdInvincible();
                    return commandSuccess("KONAMI CODE ACTIVATED.");
                case "false":
                    cmdMaxResources();
                    cmdInvincible();
                    return commandSuccess("KONAMI CODE DEACTIVATED.");
            }
            return commandError("Unknown '" + cmdWords[1] + "'Expected 'true' or 'false'.");
        }
        return commandError("'/maxresources' is expecting 1 parameter.");
    }
    private CommandMessage celCommands()
    {
        if (cmdWords.Length > 1) //si on a au moins un parametre en plus que 'chunk'
        {
            switch (cmdWords[1])
            {
                case "true":
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
            return commandError("Unknown '/cel "+cmdWords[1]+"'. "+help);
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
                    string helpGameplay = "---------------- GAMEPLAY ----------------\n\nWhat are you doing in those caves ?\n" +
                                          "You don't know yet but one thing is for sure. There is something strange happening in those caves.\n" +
                                          "It seems like something is calling you from deep inside those caves and it doesn't feel friendly...\n" +
                                          "Equipped with your wonderful St Victoria - an incredible weapon that can shoot multiple types of energy -, your pickaxe and your grappling hook, you'll seek out for that menace while you'll try to survive in those caves...\n\n" +
                                          "As you'll soon figure out, there are enemies hidden in those caves and they are quite... hungry...\n" +
                                          "These enemies seem to have different colors... I bet this is important...\n" +
                                          "And what about those wonderful crystals ? It must be related...\n\n" +
                                          "Well, what are you waiting for ? You're still reading this ?\n" +
                                          "You better grab your pickaxe and your weapon and start your adventure before these caves get the better of you ...\n\n\n" +
                                          "Spelunca's dev team - Tom, Max and Antoine\n";
                    return commandInfo(helpGameplay);
                case "controls":
                    string helpControls = "---------------- CONTROLS ----------------\n\nHere are the controls of Spelunca :\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          " - jump : [" + "x" + "]\n\n" +
                                          "";
                    return commandInfo(helpControls);
            }
        }

        string helpFileContent =
            "---------------- HELP ----------------\n\nHere are all the commands currently available:\n\n" +
            "1)  CHEAT COMMANDS\n\n" +
            " - /godmode [true|false] :\n   (WIP) Prevent the player from loosing HPs.\n\n" +
            " - /fullresources [true|false] :\n   (WIP) Fills up the resource tanks to infinite amount of energy.\n\n" +
            " - /konami [true|false] :\n   (WIP) Well... godmode + fullresources.\n\n" +
            " - /tp [target] [destination] :\n   Teleports [target] to [destination]'s current position'.\n\n" +
            " - /tp [destination] :\n   Teleports the player [destination].\n\n" +
            "3)  CHUNKS COMMANDS\n\n" +
            " - /chunk show border [true|false] :\n   (WIP) Shows the chunk's borders.\n\n" +
            " - /chunk reload :\n   (WIP) Reloads the current chunks.\n\n" +
            "3)  GRAPHISM COMMANDS\n\n" +
            " - /cel debug [true|false] :\n   Activates/deactivates the cel shading debug mode.\n\n" +
            " - /cel debug [true|false] :\n   Activates/deactivates the cel shading debug mode.\n\n" +
            " - /cel [true|false] :\n   Activates/deactivates the cel shading effect.\n\n" +
            "4)  MISCELLANEOUS COMMANDS\n\n" +
            " - /destroy [id] :\n   Destroy a marked object by giving its id.\n\n" +
            " - /setmark player [id] :\n   Creates a marker in game named [id].\n\n" +
            " - /setmark cursor [id] :\n   Creates a marker at cursor's position named [id].\n\n" +
            " - /setmark target [id] :\n   Marks the object pointed by the cursor and names it [id].\n\n";
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
