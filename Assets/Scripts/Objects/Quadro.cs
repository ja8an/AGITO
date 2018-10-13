using Assets.Scripts;
using UnityEngine;

public class Quadro : ClickHandler
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
        CanvasController._gameObject.SetMood((opcao) =>
        {
            CanvasController._gameObject.Speak("Humor registrado!");
        });
    }

}