using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{

    public Text version;

    // Use this for initialization
    void Start()
    {
        version.text = "Version: " + Application.version.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
