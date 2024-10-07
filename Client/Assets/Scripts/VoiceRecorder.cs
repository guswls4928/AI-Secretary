using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{
    AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();
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
}
