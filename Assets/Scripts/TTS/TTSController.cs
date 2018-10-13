using UnityEngine;

namespace Assets.Scripts
{
    public partial class AvatarController : MonoBehaviour
    {
        public TextToSpeech tts;
        private bool soundEnabled = true;

        public void SpeakLoud(string message)
        {

            if (message == null || message.Length == 0 || Application.platform != RuntimePlatform.Android || !soundEnabled)
            {
                return;
            }

            tts.Speak(message);
        }

    }
}
