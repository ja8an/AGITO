using Assets.Scripts;
using UnityEngine;

public class NotebookController : ClickHandler
{ 

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
                        CanvasController._gameObject.Speak("Certo! Vamos ver como está sua glicemia.", () =>
                        {
                            CanvasController._gameObject.getGlycemia();
                        });
                    }
                }
            }
        }
    }
}
