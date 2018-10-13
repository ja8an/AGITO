using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        // User Data
        private static User _user;
        public static Canvas[] canvas;

        // Scenes
        public static readonly string Bedroom = "Bedroom";
        public static readonly string Kitchen = "Kitchen";
        public static readonly string Login = "Login";
        public static readonly string Register = "Register";
        public static readonly string PrivacyPolicy = "PrivacyPolicy";

        public static bool sound_enabled = true;

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

        private static string gameDataProjectFilePath = "data.json";

        public static void SaveGameData(User user)
        {
            string dataAsJson = JsonUtility.ToJson(user);
            string filePath = Application.persistentDataPath + gameDataProjectFilePath;
            File.WriteAllText(filePath, dataAsJson);
        }

        public static User LoadGameData()
        {
            string filePath = Application.persistentDataPath + gameDataProjectFilePath;
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                setUser(User.CreateFromJSON(dataAsJson));
                return getUser();
            }
            else
            {
                return null;
            }
        }

        public static void ExitApp()
        {
            Application.Quit();
        }

    }



}