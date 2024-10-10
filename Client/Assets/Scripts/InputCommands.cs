using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
using TMPro;

public class InputCommands : MonoBehaviour
{
    public Button button { get; set; }
    public TMP_InputField text { get; set; }
    public string inputText;

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
        Debug.Log($"입력된 명령어: {text.text}");
        text.text = "";
        button.interactable = !button.interactable;
        Invoke("changeButton", 3);

    }

}
