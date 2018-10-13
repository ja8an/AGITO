using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    public Text loginStatus;
    public InputField username, password;
    public Button login;

    public void Start()
    {

        // Política de privacidade
        int privacy = PlayerPrefs.GetInt("pprivacy", 0);

        if (privacy == 0)
        {
            SceneManager.LoadScene(GameController.PrivacyPolicy);
            return;
        }

        // Usuário
        User user = GameController.LoadGameData();
        if (user != null)
        {
            SceneManager.LoadScene(GameController.Bedroom);
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetFormEnabled(bool enabled = true)
    {
        username.gameObject.SetActive(enabled);
        password.gameObject.SetActive(enabled);
    }

    public void GoLogin()
    {

        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);

        loginStatus.text = "Carregando...";

        SetFormEnabled(false);

        StartCoroutine(API.Post("login.json", form, (response) =>
        {
            Debug.Log(response.ToString());
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);
            GameController.SaveGameData(User.CreateFromJSON(response));
            SceneManager.LoadScene(GameController.Bedroom);

        }, (err, errText) =>
        {
            SetFormEnabled(true);
            Debug.Log(err + " - " + errText);
            loginStatus.text = "Usuário e/ou senha incorretos. Por favor, tente novamente!";
        }));
    }

}