using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InputCommands : MonoBehaviour
{
    public Button button { get; set; }
    public TMP_InputField text { get; set; }
    public string inputText;
    public float exp;

    public void changeButton()
    {
        button.interactable = !button.interactable;
    }

    void Start()
    {
        button = GameObject.FindGameObjectWithTag("InputCommandButton").GetComponent<Button>();
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();
        exp = 0;
    }

    public void SendCommand()
    {
        exp += 1;
        Debug.Log($"입력된 명령어: {text.text}");
        text.text = "";
        button.interactable = !button.interactable;
        Invoke("changeButton", 3);

    }

}
