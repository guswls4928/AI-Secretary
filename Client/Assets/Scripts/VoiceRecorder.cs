using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;
using UnityEditor.VersionControl;
using Newtonsoft.Json;
using System.Collections.Generic;

public class VoiceRecorder : MonoBehaviour
{
    AudioSource aud;
    public GameObject text;
    private TMP_Text _text;
    static public string ret = "test";



    void Start()
    {
        aud = GetComponent<AudioSource>();

        _text = text.GetComponent<TMP_Text>();

        this.UpdateAsObservable()
            .Select(_ => ret)
            .DistinctUntilChanged()
            .Subscribe(_ => _text.text = preprocessing());
    }

    public void StartRecording()
    {
        aud.clip = Microphone.Start(Microphone.devices[0].ToString(), false, 3, 44100);

        while (!(Microphone.GetPosition(Microphone.devices[0].ToString()) > 0)) { }

        aud.Play();

        Debug.Log("Recording started");

        StartCoroutine(StopRecording(3));
    }

    IEnumerator StopRecording(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        Microphone.End(Microphone.devices[0].ToString());

        Debug.Log("Recording stopped");

        APIController.instance.SendAudioToAPI(aud.clip);
    }

    string preprocessing()
    {
        Dictionary<string, string> message = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);

        foreach (KeyValuePair<string, string> kvp in message)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }

        return message["text"];
    }
}
