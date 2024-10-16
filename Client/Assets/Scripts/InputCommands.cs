using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class InputCommands : MonoBehaviour
{
    public Button button { get; set; }
    public TMP_InputField text { get; set; }
    public string inputText;
    public float exp;
    public TMP_Text expScoreUI;
    public GameObject Commands;

    public void changeButton()
    {
        button.interactable = !button.interactable;
    }

    void Start()
    {
        button = GameObject.FindGameObjectWithTag("InputCommandButton").GetComponent<Button>();
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();
        expScoreUI = GameObject.FindGameObjectWithTag("ExpScore").GetComponent<TMP_Text>();
        expScoreUI.text = "exp: 0 %";
        exp = 0;

    }

    public void SendCommand()
    {
        string inputText = text.text.Trim();
        var commandManager = Commands.GetComponent<InteractiveCommands>();

        var selectedCommand = commandManager.currentCommand.SubCommands
            .Find(cmd => string.Equals(cmd.CommandText, inputText, StringComparison.OrdinalIgnoreCase));

        Debug.Log("현재 명령어의 하위 명령어:");
        foreach (var cmd in commandManager.currentCommand.SubCommands)
        {
            Debug.Log(cmd.CommandText);
        }

        if (selectedCommand != null) // 일치하는 명령어가 있으면
        {
            // 현재 명령어를 하위 명령어로 이동
            commandManager.currentCommand = selectedCommand;
            commandManager.UpdateCommandUI();

            exp += 5;
            expScoreUI.text = $"exp: {exp} %";
            Debug.Log($"입력된 명령어: {inputText}");
        }
        else
        {
            Debug.Log("명령어가 일치하지 않습니다.");
        }

        text.text = "";
        Invoke("changeButton", 3);
        button.interactable = !button.interactable;
    }

}
