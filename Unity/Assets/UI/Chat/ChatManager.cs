using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField][InputName]
    private string chatInputName;
    [SerializeField] private CommandManager commandManager;
    
    [SerializeField] private int maxMessage = 25;

    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatContent;
    [SerializeField] private GameObject chatTextPrefab;
    [SerializeField] private InputField chatBox;
    
    
    [Header("Message colors")]
    [SerializeField] private Color playerMessageColor = Color.white;
    [SerializeField] private Color commandColor = Color.green;
    [SerializeField] private Color infoColor = Color.cyan;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color errorColor = Color.red;
    
    [SerializeField] private List<Message> messageList = new List<Message>();

    private string previousLine = "";

    private bool buttonPressed;
    void Start()
    {
        chat.SetActive(false);
    }
    void Update()
    {
        if (chat.activeInHierarchy)
        {
            if (chatBox.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    chatBox.text = previousLine;
                }
            }
            if (chatBox.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    previousLine = chatBox.text;
                    ParseMessage(chatBox.text);
                    chatBox.text = "";
                    chatBox.ActivateInputField();
                }
            }
            else if(!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return)){
                chatBox.ActivateInputField();
            }
        }
        
        if (!chatBox.isFocused)
        {
            if (Input.GetAxis(chatInputName) > 0)
            {
                if (!buttonPressed)
                    chat.SetActive(!chat.activeInHierarchy);
                buttonPressed = true;
                chatBox.ActivateInputField();
            }
            else
            {
                if (buttonPressed)
                    buttonPressed = false;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("You pressed the space bar !",Message.Type.info);
            }
        }
        
    }

    private Color MessageTypeColor(Message.Type type)
    {
         Color color = playerMessageColor;

         switch (type)
         {
             case Message.Type.info:
                 color = infoColor;
                 break;
             case Message.Type.playerMessage:
                 color = playerMessageColor;
                 break;
             case Message.Type.command:
                 color = commandColor;
                 break;
             case Message.Type.warning:
                 color = warningColor;
                 break;
             case Message.Type.error:
                 color = errorColor;
                 break;
         }

         return color;
    }

    public void SendMessageToChat(string text,Message.Type type)
    {
        if (messageList.Count >= maxMessage)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(chatTextPrefab, chatContent.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(type);
        messageList.Add(newMessage);
    }

    private void ParseMessage(String text)
    {
        
        if (text[0] == '/')
        {
            CommandMessage msg = commandManager.parseCommand(text.Substring(1));
            
            if (!msg.isError)//si on a pas eu d'erreur
            {
                SendMessageToChat(msg.message,Message.Type.command);
            }
            else
            {
                SendMessageToChat("CommandError : "+msg.message,Message.Type.error);
            }
        }
        else
        {
            SendMessageToChat(text,Message.Type.playerMessage);
        }
    }
}
