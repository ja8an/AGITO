using UnityEngine;

namespace Assets.Scripts
{
    public class FoodController : ClickHandler
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
                            Debug.Log("Select Food");
                            Open();
                        }
                    }
                }
            }
        }

        public void Open()
        {
            avatar.selectFood((food) =>
            {
                avatar.Speak("Alimentação registrada!");
            });
        }

    }
}