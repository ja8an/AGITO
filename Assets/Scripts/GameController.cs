using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        // User Data
        private static User _user;
        public static Canvas[] canvas;

        private void Start()
        {

        }


        public static void setUser(User user)
        {
            _user = user;
        }
        public static User getUser()
        {
            return _user;
        }

        public static bool IsUIOpen()
        {
            foreach (Canvas obj in canvas)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }
            return false;
        }

    }
}