using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{

    private const string DEV = "http://10.32.1.99/api/";
    private const string PROD = "http://200.192.112.148:1433/api/";
    private const string LOCAL = "http://192.168.1.6/agito/api/";

    public static User gameData;
    public GameObject panel;

    private static API api;

    private void Start()
    {
        api = this;
    }

    public static void UpdateGlyc(string glyc)
    {
        WWWForm form = new WWWForm();
        form.AddField("level", glyc);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        api.StartCoroutine(Post("glycemia/add.xml", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    public static void UpdateFood(string food)
    {
        WWWForm form = new WWWForm();
        form.AddField("food", food);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        api.StartCoroutine(Post("foods/add.xml", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    public static void UpdateMood(string mood)
    {
        WWWForm form = new WWWForm();
        form.AddField("mood", mood);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        api.StartCoroutine(Post("moods/add.json", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    public static IEnumerator Post(string url, WWWForm form = null, Action<string> success = null, Action<long, string> error = null)
    {
        Debug.Log("Iniciando requisição para " + LOCAL + url);
        using (UnityWebRequest www = UnityWebRequest.Post(LOCAL + url, form))
        {
            Debug.Log("Aguardando retorno...");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (error != null)
                {
                    error.Invoke(www.responseCode, www.downloadHandler.text);
                }
            }
            else
            {
                if (success != null)
                {
                    success.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}