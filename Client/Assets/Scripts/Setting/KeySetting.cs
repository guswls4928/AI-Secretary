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

        // 키보드 엔터키 입력 시 text ui 활성화
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(defaultKey[0])&& !text.isFocused)
            .Subscribe(_ =>
            {
                text.Select();
            })
            .AddTo(this);

        // text ui 활성화 상태에서 키보드 엔터키 입력 시 text 전송
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(defaultKey[0])
                && !string.IsNullOrWhiteSpace(text.text))
            .Subscribe(_ =>
            {
                if (text.isFocused) inputButton.onClick.Invoke();
            })
            .AddTo(this);


        // 키보드 V키 입력 시 음성녹음 활성화
        Observable.EveryUpdate()
            .Where(_ => !text.isFocused && Input.GetKeyDown(defaultKey[1]))
            .Subscribe(_ =>
            {
                micButton.onClick.Invoke();
            })
            .AddTo(this);
    }
}
