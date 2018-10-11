using UnityEngine;
using UnityEngine.SceneManagement;

public class PrivacyPolicy : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Read()
    {
        Application.OpenURL("http://192.168.1.6/agito/privacy-policy");
    }

    public void Agree()
    {
        PlayerPrefs.SetInt("ppolicy", 1);
        SceneManager.LoadScene("FemaleAvatar");
    }

    public void Disagree()
    {
        Application.Quit();
    }

}
