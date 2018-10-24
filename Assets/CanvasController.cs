using Assets.SimpleAndroidNotifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public partial class CanvasController : MonoBehaviour
    {

        // Public variables

        [Header("Canvas Button")]
        public Button kitchen_view_button;
        public Button office_view_button;
        public Button bedroom_view_button;
        public Button settings_button;
        public Button dialogue_cancel_button;

        public Canvas settings_canvas;

        [Header("UI Controller")]
        public GameObject[] CanvasGroup;
        public GameObject actions_canvas;

        // Chat canvas
        [Header("Dialogue Canvas")]
        // [Tooltip("Canvas helper")]
        public GameObject dialogueCanvas;
        public Button dialogueBox;
        public Text dialogueText;

        [Header("Time Canvas")]
        public GameObject timeCanvas;
        public Dropdown hour;
        public Dropdown minutes;
        public Toggle[] daysOfWeek;
        public Button timeButton;

        // Gallery
        [Header("Gallery Picker")]
        public Canvas galleryCanvas;
        public GameObject galleryContainer;
        public Button galleryItem;
        public Text galleryTitle;
        public Image galleryImage;

        private User userData;

        [Header("Actions")]
        public GameObject[] actions;

        // Text
        [Header("Input Canvas")]
        public Canvas textCanvas;
        public InputField textInput;
        public Button textButton;

        private ArrayList fila;
        private TextToSpeech tts;

        public static CanvasController _gameObject;

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

        // Private Variables
        private Action callbackAfter;

        // Use this for initialization
        void Start()
        {
            // Inicia a fila do dialogo
            fila = new ArrayList();

            _gameObject = this;

            foreach (Text t in GetComponents<Text>())
            {
                Debug.Log("text name: " + t.name);
            }
            foreach (Button b in GetComponents<Button>())
            {
                Debug.Log("button name: " + b.name);
            }

            // Se estiver no android, inicia o TTS
            if (Application.platform == RuntimePlatform.Android)
            {
                tts = new TextToSpeech();
            }

            userData = GameController.LoadGameData();

            dialogueBox.onClick.AddListener(delegate
            {
                ViewNext();
            });

            int first_time = PlayerPrefs.GetInt("first_time");

            if (first_time == 0)
            {
                FirstTime();
                PlayerPrefs.SetInt("first_time", 1);
            }

            /* var notificationParams = new NotificationParams {
                Id = NotificationIdHandler.GetNotificationId (),
                    Delay = TimeSpan.FromSeconds (5),
                    Title = PlayerPrefs.getString ("nickname") + " do Agito",
                    Message = "A partir de agora te enviarei notificações",
                    Ticker = "para que você se lembre de interagir comigo!",
                    Multiline = true
            };
            NotificationManager.SendCustom (notificationParams); */

            /* NotificationManager.SendWithAppIcon(
                TimeSpan.FromSeconds(5),
                "AGITO",
                "Hora de medir sua glicemia!",
                new Color(0, 0.6f, 1),
                NotificationIcon.Star
                ); */

            NotificationCallback nb = NotificationManager.GetNotificationCallback();
            Debug.Log("Notification Callback " + nb);
            if (nb != null)
            {
                switch (nb.Data)
                {
                    case "glicemia":
                        getTips();
                        break;
                    default:
                        break;
                }
            }

            // Clear Notifications
            NotificationManager.CancelAllDisplayed();

        }

        public static void UpdateCanvas(int target)
        {
            AvatarController.avatar.current_target = target;
            _gameObject.HideActions(target);

        }

        public void ToggleSettings()
        {
            settings_canvas.gameObject.SetActive(!settings_canvas.gameObject.activeInHierarchy);
        }

        public void HideActions(int a = -1)
        {
            foreach (GameObject action in actions)
            {
                action.gameObject.SetActive(false);
            }
            if (a != -1)
            {
                actions[a].gameObject.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {

            settings_button.gameObject.SetActive(!HasCanvasOpen());

            if (dialogueText != null && dialogueText.text != null && dialogueText.text.Length > 0)
            {
                ShowText();
                actions_canvas.gameObject.SetActive(false);
            }
            else
            {
                if (fila != null && fila.Count > 0)
                {

                    ViewNext();
                    actions_canvas.gameObject.SetActive(false);
                }
                else
                {
                    HideText();
                    actions_canvas.gameObject.SetActive(true);
                }
            }

        }

        void OnApplicationFocus(bool hasFocus)
        {

            NotificationManager.CancelAllDisplayed();
            NotificationCallback nb = NotificationManager.GetNotificationCallback();
            if (nb != null)
            {
                switch (nb.Data)
                {
                    case "glicemia":
                        getTips();
                        break;
                    default:
                        break;
                }
                return;
            }

            if (
                (fila == null || (fila.Count == 0 && dialogueText.text.Length == 0)) &&
                GameObject.FindGameObjectsWithTag("created_canvas").Length == 0 &&
                !HasCanvasOpen()

            )
            {

                if (hasFocus)
                {

                    int sysHour = System.DateTime.Now.Hour;

                    string[] manha = {
                        "Bom dia!",
                        "Oi {first_name}! Que bom ver você!",
                        "Ei, {first_name}! Tenha um bom dia!",
                        "O que vamos fazer hoje, {first_name}?"
                    };
                    string[] tarde = {
                        "Boa tarde, {first_name}!",
                        "Oi! Que bom ver você, {first_name}!",
                        "Como está o tempo lá fora, {first_name}?"
                    };
                    string[] noite = {
                        "Boa noite, {first_name}!",
                        "Oi! Que bom ver você, {first_name}!",
                        "As vezes eu tenho medo do escuro, {first_name}!"
                    };

                    if (sysHour > 13)
                    {
                        Speak(tarde[new System.Random().Next(0, tarde.Length - 1)]);
                    }
                    else if (sysHour > 8)
                    {
                        Speak(manha[new System.Random().Next(0, manha.Length - 1)]);
                    }
                    else
                    {
                        Speak(noite[new System.Random().Next(0, noite.Length - 1)]);
                    }


                }
            }



        }

        public void ToggleSound()
        {
            GameController.sound_enabled = !GameController.sound_enabled;
        }



        public void OpenSettings()
        {
            List<Gallery> options = new List<Gallery>();
            if (GameController.sound_enabled)
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
                    GameController.ExitApp();
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

        private void GetGallery(string message, List<Gallery> options, Action<Gallery> action, bool shuffle = false)
        {

            if (shuffle)
            {
                System.Random rand = new System.Random();
                options = options.OrderBy(o => rand.Next()).ToList<Gallery>();
            }

            Debug.Log("Showing gallery " + message);
            galleryCanvas.gameObject.SetActive(true);
            galleryTitle.text = message;

            if (message != null && message.Length > 0)
            {
                SpeakLoud(message);
            }

            Button[] buttons = new Button[options.Count];
            foreach (Transform b in galleryContainer.transform)
            {
                Destroy(b.gameObject);
            }

            for (int i = 0; i < options.Count; i++)
            {

                Gallery temp = options[i];

                Button btn = Instantiate(galleryItem);
                buttons[i] = btn;
                btn.transform.SetParent(galleryContainer.transform);

                if (temp.image != null)
                {
                    Image img = Instantiate(galleryImage);
                    img.sprite = Resources.Load<Sprite>(temp.image);
                    img.transform.SetParent(btn.transform);
                }

                string txt = temp.text;

                if (txt.Length > 0)
                {
                    Text btnText = btn.GetComponentInChildren<Text>();
                    btnText.text = txt;
                    // btnText.fontSize = 60;
                    if (temp.image == null)
                    {
                        btnText.alignment = TextAnchor.MiddleCenter;
                    }
                }

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(delegate
                {
                    SpeakLoud("Você selecionou: " + temp.text);
                    galleryCanvas.gameObject.SetActive(false);
                    if (action != null)
                    {
                        action(temp);
                    }
                });

            }
        }

        public void selectFood(Action<Gallery> callback = null)
        {
            int sysHour = DateTime.Now.Hour;

            if (sysHour > 13)
            {
                SelectLunch(callback);
            }
            else if (sysHour > 8)
            {
                SelectBreakfast(callback);
            }
            else
            {
                SelectDinner(callback);
            }
        }

        public void ClearQueue()
        {
            fila.Clear();
            callbackAfter = null;
            ViewNext();
        }

        public void SetNotification()
        {
            string title = "Alarmes";
            List<Gallery> options = new List<Gallery>
            {
                new Gallery { text = "Café da Manhã", value = "cmanha" },
                new Gallery { text = "Almoço", value = "cmanha" },
                new Gallery { text = "Lanche da Tarde", value = "cmanha" },
                new Gallery { text = "Café da Tarde", value = "cmanha" },
                new Gallery { text = "Janta", value = "cmanha" }
            };
            GetGallery(title, options, (res) =>
            {
            });
        }

    }


}