using UnityEngine;

namespace Assets.Scripts
{
    public class FoodController : ClickHandler
    {


        // Use this for initialization
        void Start()
        {

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
                            Debug.Log("Select Food");
                            Open();
                        }
                    }
                }
            }
        }

        public void Open()
        {
            CanvasController._gameObject.selectFood((food) =>
            {
                CanvasController._gameObject.Speak("Alimentação registrada!");
            });
        }

    }
}