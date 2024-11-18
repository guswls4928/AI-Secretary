using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class InputCommands : MonoBehaviour
{
    public Button button;
    public TMP_InputField text;
    public GameObject Commands;

    public void changeButton()
    {
        button.interactable = !button.interactable;
    }

    void Start()
    {
        button = GameObject.FindGameObjectWithTag("InputCommandButton").GetComponent<Button>();
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();

    }

    public void SendCommand()
    {
        string inputText = text.text.Trim();
        var commandManager = Commands.GetComponent<FloatingCommands>();

        if (inputText == "상위" && commandManager.currentCommand.ParentCommand != null)
        {
            // 상위 명령어로 이동
            commandManager.currentCommand = commandManager.currentCommand.ParentCommand;
            commandManager.UpdateCommandUI();
        }
        else
        {
            // 하위 명령어 탐색
            var selectedCommand = commandManager.currentCommand.SubCommands
                .Find(cmd => string.Equals(cmd.CommandText, inputText, StringComparison.OrdinalIgnoreCase));


            if (selectedCommand != null)
            {
                commandManager.currentCommand = selectedCommand;
                commandManager.UpdateCommandUI();

                ExpManager.ret.Value = "5";

                MyPlayer.instance.SendCommand(inputText);
                Debug.Log($"입력된 명령어: {inputText}");
            }
            else if (inputText == "상위" && commandManager.currentCommand.ParentCommand != null)
            {
                commandManager.currentCommand = commandManager.currentCommand.ParentCommand;
                commandManager.UpdateCommandUI();

                MyPlayer.instance.SendCommand(inputText);
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
}
