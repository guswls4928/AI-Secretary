using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

// 음악 재생을 위한 임시 스크립트입니다. . .

public class MusicPlay : MonoBehaviour
{
    public AudioSource _curMusic;
    public List<AudioClip> _list = new List<AudioClip>();

    TMP_InputField text;
    public GameObject Commands;

    void Start()
    {
        text = GameObject.FindGameObjectWithTag("InputCommandText").GetComponent<TMP_InputField>();
        var commandManager = Commands.GetComponent<FloatingCommands>();

        // 재생
        Observable.EveryUpdate()
            .Where(_ => commandManager.currentCommand.CommandText == "재생")
            .First()
            .Subscribe(_ =>
            {
                PlayRandomMusic();
                text.text = ""; // 입력 필드 초기화
            })
            .AddTo(this);

        // 정지
        Observable.EveryUpdate()
            .Where(_ => text.text == "정지")
            .First()
            .Subscribe(_ =>
            {
                _curMusic.Pause();
                text.text = ""; // 입력 필드 초기화
            })
            .AddTo(this);
    }

    void PlayRandomMusic()
    {
        if (_list.Count == 0) return; // 리스트가 비어 있는 경우 처리
        Debug.Log(_list.Count);
        // 랜덤으로 AudioClip 선택
        int randomIndex = Random.Range(0, _list.Count);
        _curMusic.clip = _list[randomIndex];

        // 음악 재생
        _curMusic.Play();
        Debug.Log($"재생 중: {_curMusic.clip.name}");
    }
}
