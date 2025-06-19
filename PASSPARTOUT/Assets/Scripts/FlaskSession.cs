using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public struct ImagePayload
{
    public string theme;
    public string image;
    public int width, height;

    public ImagePayload(string t, string i, int w, int h)
    {
        theme = t;
        image = i;
        width = w;
        height = h;
    }
}
[Serializable]
public class TextPayload
{
    public string theme;

    public TextPayload(string s) { theme = s; }
}
public class RequestText
{
    public string request_text;

    public RequestText(string s) {  request_text = s; }
}

public class FlaskSession : MonoBehaviour
{
    private static int imgNum;
    private string fileName;



    private void Start()
    {
        imgNum = 0;
        fileName = DateTime.Now.ToString("yyMMddHHmmss_");
    }

    public void SendImgToCNN(Texture2D tex, string theme, int w, int h)
    {
        string datastream = System.Convert.ToBase64String(tex.EncodeToPNG());
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName + imgNum.ToString());
        System.IO.File.WriteAllText(@path, datastream);

        ImagePayload payload = new ImagePayload(theme, datastream, w, h);

        Debug.Log("send to flask server");
        // StartCoroutine(uploadToServer("http://127.0.0.1:5000/predict", payload));
    }
    private IEnumerator uploadToServer(string url, ImagePayload payload)
    {
        string json = JsonUtility.ToJson(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
    if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError($"Upload Failed: {request.error}");
        }
        else
        {
            Debug.Log($"Server Response: {request.downloadHandler.text}");
        }
    }
}
