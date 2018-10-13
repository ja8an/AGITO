using Assets.SimpleAndroidNotifications;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public partial class CanvasController : MonoBehaviour
    {

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
                API.UpdateMood(res.value);
                callback(res);
            }, true);
        }

        private  void SelectBreakfast(Action<Gallery> callback = null)
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
                API.UpdateFood(res.value);
                callback(res);
            }, true);
        }

        private  void SelectLunch(Action<Gallery> callback = null)
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
                API.UpdateFood(res.value);
                callback(res);
            }, true);
        }

        private  void SelectDinner(Action<Gallery> callback = null)
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
                API.UpdateFood(res.value);
                callback(res);
            }, true);
        }

        public  void GetNotification()
        {
            Debug.Log("Notification");
            string title = "Notificações";

            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "Manhã", value = "m" },
                new Gallery { text = "Tarde", value = "t" },
                new Gallery { text = "Noite", value = "n" }
            };

            GetGallery(title, options, (res) =>
            {
                UpdateNotification(res, (hora, minuto, days) =>
                {
                    for (int i = 0; i < days.Length; i++)
                    {

                        Debug.Log("Dia" + i);

                        // Se o dia não estiver marcado, pula para o próximo laço
                        if (days[i] == false)
                        {
                            continue;
                        }

                        // Cria uma data
                        DateTime dt = DateTime.Now;

                        Debug.Log("Hoje é " + dt.ToLongDateString());

                        while ((int)dt.DayOfWeek == i)
                        {
                            dt = dt.AddDays(1);
                            Debug.Log("Próximo :" + dt.DayOfWeek + "-" + i);
                        }

                        Debug.Log("NextFN:" + GetNextWeekday(DayOfWeek.Monday, DateTime.Today));

                        TimeSpan ts = new TimeSpan(hora, minuto, 0);
                        dt = dt + ts;

                        int nid = res.value.ToCharArray()[0];

                        NotificationIdHandler.RemoveScheduledNotificaion(nid);
                        Debug.Log("Repetir: " + TimeSpan.FromDays(7));
                        NotificationManager.SendCustom(new NotificationParams
                        {
                            Delay = dt.TimeOfDay,
                            RepeatInterval = TimeSpan.FromDays(7),
                            Repeat = true,
                            Multiline = true,
                            Message = "Hey!\nEstá na hora de medir sua glicemia!\n",
                            Title = "AGITO",
                            Id = nid,
                            Ticker = "Vamos lá!",
                            GroupName = "AGITO",
                            GroupSummary = "{0} novas notificações",
                            ChannelId = "br.pucpr.ppgia.lasin.AGITO." + res.value,
                            ChannelName = "Glicemia",
                            LargeIcon = "app_icon",
                            Light = true,
                            ExecuteMode = NotificationExecuteMode.ExactAndAllowWhileIdle,
                            SmallIcon = NotificationIcon.Clock,
                            SmallIconColor = Color.blue,
                            Vibrate = true,
                            Sound = true,
                            LightColor = Color.blue,
                            CallbackData = "glicemia",
                        });

                        Speak("Feito", () =>
                        {
                            Speak("Vou te lembrar de medir a glicemia às " + hora + ":" + minuto + " nos dias que você definiu!");
                        });

                    }
                });
            });
        }

        public  DateTime GetNextWeekday(DayOfWeek day, DateTime start)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }


        private void getText(string message, Action<string> action)
        {
            textCanvas.gameObject.SetActive(true);

            Text placeHolder = textInput.GetComponentInChildren<Text>();

            // Changes the field label
            placeHolder.text = message;

            // Focus on the input field
            textInput.Select();
            textInput.ActivateInputField();

            // Attaches the listener
            textButton.onClick.RemoveAllListeners();
            textButton.onClick.AddListener(delegate
            {
                string text = textInput.text;
                if (text.Length > 0)
                {
                    action(text);
                    textInput.text = "";
                    textButton.onClick.RemoveAllListeners();
                    textCanvas.gameObject.SetActive(false);
                }
                else
                {
                    // Focus the input field again
                    textInput.Select();
                    textInput.ActivateInputField();
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

        private  void GetDouble(string label, int min, int max, Action<double> action)
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

        private void GetDouble(string label, Action<double> action)
        {
            GetDouble(label, -1, -1, action);
        }

        private void getInt(string label, Action<int> action)
        {
            GetInt(label, -1, -1, action);
        }

        public void CloseGallery()
        {
            galleryCanvas.gameObject.SetActive(false);
        }


    }
}