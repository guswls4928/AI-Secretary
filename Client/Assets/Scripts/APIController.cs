using System.Net;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections;

public class APIController : MonoBehaviour
{
    public static APIController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SendAudioToAPI(AudioClip audioClip)
    {
        StartCoroutine(PostRequest(audioClip));
    }

    IEnumerator PostRequest(AudioClip audioClip)
    {
        string url = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor";

        byte[] audioData = ConvertAudioClipToByteArray(audioClip);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "g5iq8uefms");
        request.Headers.Add("X-NCP-APIGW-API-KEY", "aEPmNbDWDKRptmvOE7PRbrHVcJoYrEolA7ZJV8dQ");
        request.ContentType = "application/octet-stream";
        request.ContentLength = audioData.Length;
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(audioData, 0, audioData.Length);
            requestStream.Close();
        }
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (Stream stream = response.GetResponseStream())
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                Debug.Log(reader.ReadToEnd());
            }
        }

        yield return null;
    }

    byte[] ConvertAudioClipToByteArray(AudioClip audioClip)
    {
        SavWav.Save("audio", audioClip);

        string path = Application.persistentDataPath + "/audio.wav";

        return File.ReadAllBytes(path);
    }
}
