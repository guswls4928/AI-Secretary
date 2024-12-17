using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class InputCommands : MonoBehaviour
{
    Button button;
    TMP_InputField text;
    public GameObject Commands;

    private FloatingCommands commandManager;

    public void changeButton()
    {
        button.interactable = !button.interactable;
    }

    void Start()
    {
        button = GameObject.FindGameObjectWithTag("InputCommandButton").GetComponent<Button>();
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();

        commandManager = Commands.GetComponent<FloatingCommands>();
    }

    public void SendCommand()
    {
        string inputText;
        inputText = text.text.Trim();

        commandManager.EnterCommand(inputText);

        text.text = "";
        Invoke("changeButton", 2);
        button.interactable = !button.interactable;
    }
}
