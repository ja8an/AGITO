using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.EventSystems;

public class NotebookController : ClickHandler
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
                        Debug.Log(hit.transform.name + '-' + transform.name);
                        avatar.Speak("Certo! Vamos ver como está sua glicemia.", () =>
                        {
                            avatar.getGlycemia();
                        });
                    }
                }
            }
        }
    }
}
