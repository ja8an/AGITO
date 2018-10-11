using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class API : MonoBehaviour
{

    private const string DEV = "http://10.32.1.99/api/";
    private const string PROD = "http://200.192.112.148:1433/api/";
    private const string LOCAL = "http://192.168.1.6/agito/api/";

    public Text loginStatus;
    public InputField username, password;
    public static User gameData;
    public GameObject panel;
    private bool IsRequesting = false;

    private static string gameDataProjectFilePath = "data.json";

    public void Start()
    {
        // Usuário
        User user = LoadGameData();
        if (user.api_key != null && gameObject.name != "avatar")
        {
            SceneManager.LoadScene("FemaleAvatar");
        }
        // Política de privacidade
        int privacy = PlayerPrefs.GetInt("pprivacy", 0);

        if (privacy == 0)
        {
            SceneManager.LoadScene("PrivacyPolicy");
        }

    }

    public void Update()
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(!IsRequesting);
        }
    }

    public static void SaveGameData(User user)
    {
        string dataAsJson = JsonUtility.ToJson(user);
        string filePath = Application.persistentDataPath + gameDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
    }

    public static User LoadGameData()
    {
        string filePath = Application.persistentDataPath + gameDataProjectFilePath;
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            gameData = User.CreateFromJSON(dataAsJson);
            return gameData;
        }
        else
        {
            return null;
        }
    }

    public void Login()
    {
        WWWForm form = new WWWForm();

        username.gameObject.SetActive(false);
        password.gameObject.SetActive(false);

        form.AddField("username", username.text);
        form.AddField("password", password.text);

        loginStatus.text = "Carregando...";

        StartCoroutine(Post("login.json", form, (response) =>
        {
            Debug.Log(response.ToString());
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);
            SaveGameData(User.CreateFromJSON(response));
            SceneManager.LoadScene("FemaleAvatar");

        }, (err, errText) =>
        {
            username.gameObject.SetActive(true);
            password.gameObject.SetActive(true);
            Debug.Log(err);
            Debug.Log(errText);
            loginStatus.text = "Usuário e/ou senha incorretos. Por favor, tente novamente!";
        }));
    }

    public void UpdateGlyc(string glyc)
    {
        WWWForm form = new WWWForm();
        form.AddField("level", glyc);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        StartCoroutine(Post("glycemia/add.xml", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    public void UpdateFood(string food)
    {
        WWWForm form = new WWWForm();
        form.AddField("food", food);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        StartCoroutine(Post("foods/add.xml", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    public void UpdateMood(string mood)
    {
        WWWForm form = new WWWForm();
        form.AddField("mood", mood);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("password", PlayerPrefs.GetString("password"));
        StartCoroutine(Post("moods/add.json", form, (data) =>
        {
            Debug.Log(data);
        }, (err, errText) =>
        {
            Debug.Log(err);
            Debug.Log(errText);
        }));
    }

    static IEnumerator Post(string url, WWWForm form = null, Action<string> success = null, Action<long, string> error = null)
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