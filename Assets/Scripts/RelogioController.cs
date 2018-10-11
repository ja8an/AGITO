using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class RelogioController : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        if (hit.transform.name == name)
                        {
                            GameObject go = GameObject.Find("avatar");
                            AvatarController avatar = (AvatarController)go.GetComponent(typeof(AvatarController));
                            avatar.GetNotification();
                        }
                    }
                }
            }

        }

    }
}