using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class Quadro : ClickHandler
{

    private AvatarController avatar;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("avatar");
        avatar = (AvatarController)go.GetComponent(typeof(AvatarController));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == transform.name)
                    {
                        Debug.Log("Set Mood");
                        Open();
                    }
                }
            }
        }
    }

    public void Open()
    {
        Debug.Log("Clicou no humor");
        avatar.SetMood((opcao) =>
        {
            avatar.Speak("Humor registrado!");
        });
    }

}