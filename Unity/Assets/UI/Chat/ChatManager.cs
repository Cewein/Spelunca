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
    [SerializeField] private MinerInputHandler playerControls;
    
    [SerializeField] private int maxMessage = 25;

    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatContent;
    [SerializeField] private InputField chatBox;
    [SerializeField] private Scrollbar chatScrollbar;
    [SerializeField] private GameObject chatTextPrefab;
    
    [SerializeField] private float mouseSpeed = 0.1f;
    
    [SerializeField] private Color CaretColor = Color.blue;
    [SerializeField, Range(1,5)] private int CaretSize = 4;
    
    [Header("Message colors")]
    [SerializeField] private Color playerMessageColor = Color.white;
    [SerializeField] private Color commandColor = Color.green;
    [SerializeField] private Color infoColor = Color.cyan;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color errorColor = Color.red;
    
    [SerializeField] private List<Message> messageList = new List<Message>();

    private string previousLine = "";
    private string currentLine = "";

    private bool buttonPressed;
    void Start()
    {
        chat.SetActive(false);
        chatBox.customCaretColor = true;
        chatBox.caretColor = CaretColor;
        chatBox.caretWidth = CaretSize;
    }

    void LateUpdate()
    {
        if (chat.activeInHierarchy)
        {
            
            if (chatBox.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (previousLine != "")
                    {
                        currentLine = chatBox.text;
                        chatBox.text = previousLine;
                        chatBox.caretPosition = previousLine.Length;
                    }
                }if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (currentLine != "")
                    {
                        chatBox.text = currentLine;
                        chatBox.caretPosition = currentLine.Length;
                        currentLine = "";
                    }
                }
            } else if(Input.GetKeyDown(KeyCode.Return)){
                chatBox.ActivateInputField();
            }
            if (chatBox.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    previousLine = chatBox.text;
                    ParseMessage(chatBox.text);
                    chatBox.text = "";
                    chatBox.DeactivateInputField();//ActivateInputField();
                }
            }
        }
        
        if (!chatBox.isFocused)
        {
            if (Input.GetAxis(chatInputName) > 0)
            {
                if (!buttonPressed)
                {
                    chat.SetActive(!chat.activeInHierarchy);
                    buttonPressed = true;
                    chatBox.ActivateInputField();
                    playerControls.disabled = chat.activeInHierarchy;
                    if (chat.activeInHierarchy)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    
                }
                    
            }
            else
            {
                if (buttonPressed)
                    buttonPressed = false;
            }
        }
    }

    private void Update()
    {
        //Debug.Log(chatScrollbar.value);
        float value = chatScrollbar.value + Input.mouseScrollDelta.y*mouseSpeed;
        value = Mathf.Clamp(value, 0f, 1f);
        chatScrollbar.value = value;

    }
/// <summary>
/// 
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
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

            switch (msg.type)
            {
                case Message.Type.command:
                    SendMessageToChat(msg.message,Message.Type.command);
                    break;
                case Message.Type.warning:
                    SendMessageToChat("CommandWarning : "+msg.message,Message.Type.warning);
                    break;
                case Message.Type.error:
                    SendMessageToChat("CommandError : "+msg.message,Message.Type.error);
                    break;
                case Message.Type.info:
                    SendMessageToChat(msg.message,Message.Type.info);
                    break;
            }
        }
        else
        {
            SendMessageToChat(text,Message.Type.playerMessage);
        }
    }
}
