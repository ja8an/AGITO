using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrivacyPolicy : MonoBehaviour
{

    public void Read()
    {
        Application.OpenURL("http://192.168.1.6/agito/privacy-policy");
    }

    public void Agree()
    {
        PlayerPrefs.SetInt("ppolicy", 1);
        SceneManager.LoadScene(GameController.Bedroom);
    }

    public void Disagree()
    {
        Application.Quit();
    }

}
