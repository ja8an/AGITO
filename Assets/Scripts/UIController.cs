using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public partial class AvatarController : MonoBehaviour
    {

        [Header("UI Controller")]
        public GameObject[] CanvasGroup;

        public bool HasCanvasOpen()
        {
            for (int i = 0; i < CanvasGroup.Length; i++)
            {
                if (CanvasGroup[i].activeSelf)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetMood(Action<Gallery> callback = null)
        {
            string title = "Como você está se sentindo?";
            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "Feliz", image = "Emoji/happy", value = "feliz" },
                new Gallery { text = "Triste", image = "Emoji/sad", value = "triste" },
                new Gallery { text = "Bravo", image = "Emoji/angry", value = "bravo" },
                new Gallery { text = "Envergonhado", image = "Emoji/embarrassed", value = "envergonhado" },
                new Gallery { text = "Surpreso", image = "Emoji/surprised", value = "surpreso" },
                new Gallery { text = "Pensativo", image = "Emoji/thinking", value = "pensativo" }
            };
            GetGallery(title, options, (res) =>
            {
                gameObject.GetComponent<API>().UpdateMood(res.value);
                callback(res);
            }, true);
        }

        private void SelectBreakfast(Action<Gallery> callback = null)
        {
            Debug.Log("Breakfast");
            string title = "O que você comeu no café da manhã?";
            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "Pão", image = "Food/Breakfast/pao", value = "pao" },
                new Gallery { text = "Cereal", image = "Food/Breakfast/cereal", value = "cereal" },
                new Gallery { text = "Frutas", image = "Food/Breakfast/frutas", value = "frutas" }
            };
            GetGallery(title, options, (res) =>
            {
                gameObject.GetComponent<API>().UpdateFood(res.value);
                callback(res);
            }, true);
        }

        private void SelectLunch(Action<Gallery> callback = null)
        {
            Debug.Log("Lunch");
            string title = "O que você comeu no almoço?";
            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "Lasanha", image = "Food/Breakfast/pao", value = "lasanha" },
                new Gallery { text = "Pizza", image = "Food/Breakfast/cereal", value = "pizza" },
                new Gallery { text = "Bife e Fritas", image = "Food/Breakfast/frutas", value = "bife" }
            };
            GetGallery(title, options, (res) =>
            {
                gameObject.GetComponent<API>().UpdateFood(res.value);
                callback(res);
            }, true);
        }

        private void SelectDinner(Action<Gallery> callback = null)
        {
            Debug.Log("Dinner");
            string title = "O que você comeu no jantar?";
            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "A", image = "Food/Breakfast/cereal", value = "a" },
                new Gallery { text = "B", image = "Food/Breakfast/cereal", value = "b" },
                new Gallery { text = "C", image = "Food/Breakfast/cereal", value = "c" }
            };
            GetGallery(title, options, (res) =>
            {
                gameObject.GetComponent<API>().UpdateFood(res.value);
                callback(res);
            }, true);
        }

        public void ToggleSound()
        {
            soundEnabled = !soundEnabled;
        }

        private void GetDouble(string label, Action<double> action)
        {
            GetDouble(label, -1, -1, action);
        }

        private void getInt(string label, Action<int> action)
        {
            GetInt(label, -1, -1, action);
        }

        public void OpenSettings()
        {
            List<Gallery> options = new List<Gallery>();
            if (soundEnabled)
            {
                options.Add(new Gallery { text = "Sem Som", image = "Ui/mute_sound", value = "sound" });
            }
            else
            {
                options.Add(new Gallery { text = "Som", image = "Ui/play_sound", value = "sound" });
            }
            options.Add(new Gallery { text = "Tutorial", image = "Ui/book", value = "tutorial" });
            options.Add(new Gallery { text = "Sair", image = "Ui/exit", value = "exit" });
            GetGallery(null, options, (option) =>
            {
                Debug.Log("Chosen" + option.value);
                if (option.value == "exit")
                {
                    exitApp();
                }
                else if (option.value == "sound")
                {
                    ToggleSound();
                }
                else if (option.value == "tutorial")
                {
                    FirstTime();
                }
            });
        }

        private void GetInt(string label, int min, int max, Action<int> action)
        {
            // Instances the canvas
            textCanvas.gameObject.SetActive(true);
            Text placeHolder = textInput.GetComponentInChildren<Text>();

            placeHolder.text = label;

            // Focus on the input field
            textInput.Select();
            textInput.ActivateInputField();

            textInput.keyboardType = TouchScreenKeyboardType.NumberPad;
            // Attaches the listener
            textInput.onEndEdit.RemoveAllListeners();
            textInput.onEndEdit.AddListener(delegate
            {
                int text;
                try
                {
                    text = Convert.ToInt32(textInput.text);
                }
                catch
                {
                    text = 1;
                    min = 0;
                }
                if (min < 0 || (text >= min && text <= max))
                {
                    action(text);
                    textInput.text = "";
                    textCanvas.enabled = false;
                    textButton.onClick.RemoveAllListeners();
                    textCanvas.gameObject.SetActive(true);
                }
                else
                {
                    // Focus the input field again
                    textInput.text = "0";
                    textInput.Select();
                    textInput.ActivateInputField();
                }
            });
        }

        private void GetDouble(string label, int min, int max, Action<double> action)
        {
            textCanvas.gameObject.SetActive(true);

            Text placeHolder = textInput.GetComponentInChildren<Text>();
            placeHolder.text = label;

            // Focus on the input field
            textInput.Select();
            textInput.ActivateInputField();

            textInput.keyboardType = TouchScreenKeyboardType.NumberPad;
            // Attaches the listener
            textButton.onClick.RemoveAllListeners();
            textButton.onClick.AddListener(delegate
            {
                double text;
                try
                {
                    text = Convert.ToDouble(textInput.text);
                }
                catch
                {
                    text = -1;
                }
                if (min < 0 || (text >= min && text <= max))
                {
                    action(text);
                    textInput.text = "";
                    textButton.onClick.RemoveAllListeners();
                    textCanvas.gameObject.SetActive(false);
                }
                else
                {
                    // Focus the input field again
                    textInput.text = "0";
                    textInput.Select();
                    textInput.ActivateInputField();
                }
            });
        }

    }
}