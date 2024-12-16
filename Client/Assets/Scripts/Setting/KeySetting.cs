using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;

public enum KeyActions
{
    VoiceRecord,
    InputCommand
}

public static class KeyDict
{
    public static Dictionary<KeyActions, KeyCode> key = new Dictionary<KeyActions, KeyCode>();
}

public class KeySetting : MonoBehaviour
{
    private KeyCode[] defaultKey = new KeyCode[]
    {
        KeyCode.Return,
        KeyCode.V
    };

    TMP_InputField text;
    Button inputButton;
    Button micButton;

    void Start()
    {
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();
        inputButton = GameObject.FindGameObjectWithTag("InputCommandButton").GetComponent<Button>();
        micButton = GameObject.Find("MicButton").GetComponent<Button>();

        // 키보드 엔터키 입력 시 채팅창 활성화
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(defaultKey[0])&& !text.isFocused && inputButton.interactable)
            .Subscribe(_ =>
            {
                text.Select();
            })
            .AddTo(this);

        // 채팅창이 활성화되어 있는 상태에서 엔터키 입력 시 text 전송
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(defaultKey[0]) && !string.IsNullOrWhiteSpace(text.text)&& inputButton.interactable)
            .Subscribe(_ =>
            {
                inputButton.onClick.Invoke();
                text.text = "";
            })
            .AddTo(this);


        // 키보드 V키 입력 시 음성녹음 활성화
        Observable.EveryUpdate()
            .Where(_ => !text.isFocused && Input.GetKeyDown(defaultKey[1]) && inputButton.interactable)
            .Subscribe(_ =>
            {
                micButton.onClick.Invoke();
            })
            .AddTo(this);
    }
}