using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class CanvasController : MonoBehaviour
    {

        public void Speak(string message, Action callback = null)
        {

            if (fila == null)
            {
                fila = new ArrayList();
            }
            // Calls the function
            Dictionary<string, object> action = new Dictionary<string, object>();

            Dictionary<string, string> variables = new Dictionary<string, string>
        {
            { "first_name", userData.first_name },
            { "full_name", userData.full_name },
            { "last_name", userData.last_name }
        };

            foreach (string key in variables.Keys)
            {
                string v;
                variables.TryGetValue(key, out v);
                message = message.Replace("{" + key + "}", v);
            }

            action.Add("message", message);
            action.Add("callback", callback);
            fila.Add(action);

        }

        public void HideText()
        {
            dialogueText.text = null;
            dialogueCanvas.gameObject.SetActive(false);
        }

        public void ShowText()
        {
            dialogueCanvas.gameObject.SetActive(true);
        }


        public void ViewNext()
        {
            if (callbackAfter != null)
            {
                callbackAfter();
                callbackAfter = null;
            }

            if (fila.Count > 0)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)fila[0];
                fila.RemoveAt(0);

                string message = (string)temp["message"];
                callbackAfter = (Action)temp["callback"];

                Debug.Log("Avatar:" + message);

                dialogueText.text = message;

                SpeakLoud(message);

                ShowText();
            }
            else
            {
                HideText();
            }

        }

        public void SpeakLoud(string text)
        {
            if (tts == null)
            {
                tts = new TextToSpeech();
            }

            tts.Speak(text);
        }

    }

}