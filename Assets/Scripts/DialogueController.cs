using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class AvatarController : MonoBehaviour
    {

        private Action callbackAfter;

        public void Speak(string message, Action callback = null)
        {

            if (fila == null)
                fila = new ArrayList();
            // Calls the function
            Dictionary<string, object> action = new Dictionary<string, object>();

            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("first_name", userData.first_name);
            variables.Add("full_name", userData.full_name);
            variables.Add("last_name", userData.last_name);

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

        public void FirstTime()
        {

            Speak("Olá, sou sua assistente virtual criada para ter conversas com você sobre a sua saúde.");

            Speak("Que tal a gente começar com você me dando um nome?", () =>
            {
                // PlayAnimationOnce(3);
                getText("Digite um nome para mim:", (text) =>
                {
                    PlayerPrefs.SetString("nickname", text.Trim());
                    Speak(text + "? Adorei meu novo nome!");
                    PlayAnimationOnce(2);
                    Speak("Agora vamos mudar meu estilo.", () =>
                    {
                        Speak("Para fazer isso, toque na cômoda do lado esquerdo da tela.", () =>
                        {
                            WardRobe.callbacks.Add("OnOpenOnce", () =>
                            {
                                Speak("Isso mesmo!");
                                Speak("Agora clique nos ícones abaixo para alterar o meu estilo, depois clique no X para fechar.");
                                WardRobe.callbacks.Add("OnCloseOnce", () =>
                                {
                                    Speak("Agora vamos ao que importa! Vamos fazer sua primeira medição de glicemia.", () =>
                                    {
                                        getGlycemia((glycemia) =>
                                        {
                                            Speak("Muito bem!", () =>
                                            {
                                                Speak("Agora vamos ver o que você comeu hoje", () =>
                                                {
                                                    selectFood();
                                                });
                                            });

                                        });
                                    });

                                });

                            });
                        });

                    });

                });

            });

        }

        public void ClearQueue()
        {
            this.fila.Clear();
            this.callbackAfter = null;
            ViewNext();
        }

    }

}