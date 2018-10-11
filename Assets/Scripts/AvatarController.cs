using Assets.SimpleAndroidNotifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public partial class AvatarController : MonoBehaviour
    {

        // Avatar
        [Header("Avatar")]
        public GameObject ed_targetAvatar;

        public GameObject[] hairs;
        public GameObject[] glasses;
        public GameObject[] outfits;

        public GameObject[] hideObjects;

        private string[] animationNames;

        private GameObject wornHair = null;
        private GameObject wornGlasses = null;
        private GameObject wornOutfit = null;

        private int animIndex = -1;
        private int hairIndex = -1;
        private int glassesIndex = -1;
        private int outfitIndex = -1;

        // Chat canvas
        [Header("Dialogue Canvas")]
        // [Tooltip("Canvas helper")]
        public Canvas dialogueCanvas;
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

        // Text
        [Header("Input Canvas")]
        public Canvas textCanvas;
        public InputField textInput;
        public Button textButton;

        [Header("Settings Button")]
        public Button settingsButton;

        public ArrayList fila;

        void Start()
        {
            // Inicia a fila do dialogo
            fila = new ArrayList();

            // Se estiver no android, inicia o TTS
            if (Application.platform == RuntimePlatform.Android)
            {
                tts = new TextToSpeech();
            }

            animationNames = new string[ed_targetAvatar.GetComponent<Animation>().GetClipCount()];

            int i = 0;
            foreach (AnimationState animState in ed_targetAvatar.GetComponent<Animation>())
            {
                animationNames[i] = animState.clip.name;
                i++;
            }

            playAnimation(0);

            userData = API.LoadGameData();

            int h, o, g;

            // Hair
            try
            {
                h = PlayerPrefs.GetInt("hair");
            }
            catch
            {
                h = 0;
            }
            setHair(h);

            // Glasses
            try
            {
                g = PlayerPrefs.GetInt("glasses");
            }
            catch
            {
                g = 0;
            }
            setGlasses(g);

            // Outfit
            try
            {
                o = PlayerPrefs.GetInt("outfit");
            }
            catch
            {
                o = 0;
            }
            setOutfit(o);

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

            // SetNotification();
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
            }
        }

        // Ao reabrir o aplicativo
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

        public void getGlycemia(Action<double> action = null)
        {
            GetDouble("Qual o seu nível de glicemia nesse momento?", 0, 200, (glycemia) =>
            {

                gameObject.GetComponent<API>().UpdateGlyc(glycemia.ToString());

                if (glycemia > 180)
                {
                    Speak("Sua glicemia está um pouco alta!");
                }
                else if (glycemia < 70)
                {
                    Speak("Sua glicemmia está muito baixa!");
                }
                else
                {
                    Speak("Tudo bem por aqui!", () =>
                    {
                        Speak("Continue assim!");
                    });
                }

                if (action != null)
                {
                    action(glycemia);
                }
            });

        }

        public void getTips()
        {

            int sysHour = DateTime.Now.Hour;

            getGlycemia(glycemia =>
            {

                if (sysHour > 13)
                {
                    getGlycemia();
                }
                else if (sysHour > 8)
                {
                    TipGlycemiaMorning(glycemia);
                }
                else
                {
                    getGlycemia();
                }

            });

        }

        public void TipGlycemiaMorning(double glycemia)
        {
            if (glycemia > 180)
            {
                List<Gallery> itens = new List<Gallery>
                {
                    new Gallery { text = "Esqueci de aplicar a insulina basal ontem", value = "1" },
                    new Gallery { text = "Dormi com a glicose alta", value = "2" },
                    new Gallery { text = "Comi muito carboidrato antes de dormir", value = "3" },
                    new Gallery { text = "Estou doente", value = "4" },
                    new Gallery { text = "Eu não sei (o que você acha que pode estar acontecendo?)", value = "5" }
                };

                GetGallery("Essa glicemia está mesmo alta... o que você acha que pode ser?", itens, (response) =>
                {

                    switch (response.value)
                    {
                        case "1":
                            Speak("Sabia que a insulina basal é importante para o nosso corpo, porque ela age nele durante a noite?", () =>
                            {
                                Speak("Pois é! Então, não esquece de aplicá-la antes de dormir, viu?", () =>
                                {
                                    Speak("Vou te mostrar um video...", () =>
                                    {
                                        Application.OpenURL("https://youtu.be/84c3Ipy5Lis");
                                    });
                                });
                            });
                            break;
                        case "2":
                            Speak("Fica ligado na sua alimentação, ok?", () =>
                            {
                                Speak("Assista o video com algumas dicas", () =>
                                {
                                    Application.OpenURL("https://www.youtube.com/watch?v=AFRbY60I9cw");
                                });
                            });
                            break;
                        case "3":
                            Speak("Sabia que tudo o que a gente come antes de dormir é absorvido mais devagar pelo nosso corpo?", () =>
                            {
                                Speak("Aí, o pico de efeito da insulina pode acabar antes que o alimento que a gente comeu seja totalmente absorvido.", () =>
                                {
                                    Speak("Isso pode aumentar o nível de açúcar no sangue, acredita?", () =>
                                    {
                                        Speak("Por isso, coma menos carboidratos no jantar, tá?");

                                    });
                                });
                            });
                            break;
                        case "4":
                            Speak("Sabia que quando a gente está doente a nossa glicemia pode ficar mais alta?", () =>
                            {
                                Speak("Pois é! Isso acontece porque o nosso corpo está tentando combater os vírus/ bactérias que estão deixando a gente doente.", () =>
                                {
                                    Speak("Por isso, ajude o seu corpo nessa tarefa e não esquece de aplicar a insulina, ok?", () =>
                                    {
                                        Speak("Vou te mostrar um vídeo com orientações de como aplicar a insulina.", () =>
                                        {
                                            Application.OpenURL("https://www.youtube.com/watch?v=M6EUnDGA5uU&t=3s");
                                        });
                                    });
                                });
                            });
                            break;
                        case "5":
                            Speak("Você já ouviu falar do tal fenômeno do amanhecer?", () =>
                            {
                                Speak("Sabe o que é?", () =>
                                {
                                    Speak("É quando você acorda, mede a glicemia e ela já está alta mesmo sem você ter comido nada ainda.", () =>
                                    {
                                        Speak("Isso acontece porque você está em fase de crescimento", () =>
                                        {
                                            Speak("e nessa fase o nosso corpo produz ainda mais cortisol", () =>
                                            {
                                                Speak("sem falar no hormônio do crescimento também, né?", () =>
                                                {
                                                    Speak("Assiste isso aqui...", () =>
                                                    {
                                                        Application.OpenURL("https://www.youtube.com/watch?v=9vzRzeUr1V8&feature=youtu.be");
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                            break;
                    }

                }, true);

            }
            else if (glycemia > 70)
            {
                Speak("Hmm.. parece que está tudo certo por aqui!", () =>
                {
                    Speak("Continue assim durante todo o dia!");
                });
            }
            else
            {
                Speak("Glicemia abaixo de 70. O que você acha que pode ser?!", () =>
                {

                    List<Gallery> itens = new List<Gallery>
                    {
                        new Gallery { text = "Fiz jejum prolongado", value = "1" },
                        new Gallery { text = "Apliquei insulina de ação rápida de noite/madrugada", value = "2" },
                        new Gallery { text = "Comi menos carboidratos que o programado", value = "3" },
                        new Gallery { text = "Fiz atividade física de noite", value = "4" },
                        new Gallery { text = "Estou doente", value = "5" },
                        new Gallery { text = "Não sei  (O que você acha que pode estar acontecendo?)", value = "6" }
                    };
                    GetGallery("", itens, (response) =>
                    {
                        switch (response.value)
                        {
                            case "1":
                                Speak("Sabia porque ao acordar a nossa glicose está baixa?", () =>
                                {
                                    Speak("É porque mesmo dormindo a gente gasta energia!", () =>
                                    {
                                        Speak("Por isso que a gente não pode pular o café da manhã!", () =>
                                        {
                                            Speak("É ele que vai dar a energia pra gente começar o dia!");
                                        });
                                    });
                                });
                                break;
                            case "2":
                                Speak("Entendi, mas espera aí, não desanima não!", () =>
                                {
                                    Speak("Dá uma olhada neste vídeo aqui...", () =>
                                    {
                                        Application.OpenURL("https://www.youtube.com/watch?v=y_GuzvlT3LY");
                                    });
                                });
                                break;
                            case "3":
                                Speak("Que tal comer uma fruta?", () =>
                                {
                                    Speak("Sabia que elas também são ricas em açúcar?", () =>
                                    {
                                        Speak("Elas também têm mais fibras que o suco", () =>
                                        {
                                            Speak("então, fica a dica: escolha sempre a fruta, ok?");
                                        });
                                    });

                                });
                                break;
                            case "4":
                                Speak("Sabe aquele exercício físico/esporte/aula de educação física que a gente faz?", () =>
                                {
                                    Speak("tudo isso faz muito bem para o nosso corpo, mas também pode deixar a gente mais sensível à insulina", () =>
                                    {
                                        Speak("Por isso a gente não pode esquecer de fazer aquele lanchinho antes e depois dessas atividades físicas", () =>
                                        {
                                            Speak("e quando você tiver bem animado e empolgar na atividade física, não esquece de medir a glicemia às 3h da manhã também", () =>
                                            {
                                                Speak("Veja esse video...", () =>
                                                {
                                                    Application.OpenURL("https://www.youtube.com/watch?v=vdepP0C50hU");
                                                });
                                            });
                                        });
                                    });
                                });
                                break;
                            case "5":
                                Speak("Quando você ficar doente, não esquece de conferir a sua glicemia mais vezes durante o dia", () =>
                                {
                                    Speak("e sempre seguindo as orientações do seu médico, beleza?");
                                });
                                break;
                            case "6":
                                Speak("Lembra das correções que você combinou com o seu médico?", () =>
                                {
                                    Speak("Então, vamos coloca-las em prática?", () =>
                                    {
                                        Speak("Mãos à obra!");
                                    });
                                });
                                break;
                        }
                    });

                });
            }
        }

        public void GetNotification()
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

        public static DateTime GetNextWeekday(DayOfWeek day, DateTime start)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public void CloseGallery()
        {
            galleryCanvas.gameObject.SetActive(false);
        }

        public void UpdateNotification(Gallery res, Action<int, int, bool[]> callback)
        {
            timeCanvas.gameObject.SetActive(true);

            Text placeHolder = textInput.GetComponentInChildren<Text>();

            // Changes the field label
            placeHolder.text = res.text;

            // Attaches the listener
            timeButton.onClick.RemoveAllListeners();
            timeButton.onClick.AddListener(delegate
            {
                bool[] days = new bool[7];
                int i = 0;
                foreach (Toggle t in daysOfWeek)
                {
                    Debug.Log("Day " + i + " is " + t.isOn);
                    days[i] = t.isOn;
                    i++;
                }
                callback(Convert.ToInt32(hour.value), Convert.ToInt32(minutes.value), days);
                timeCanvas.gameObject.SetActive(false);
            });
        }

        public void CancelNotificationUpdate()
        {
            timeCanvas.gameObject.SetActive(false);
        }

        void PlayAnimationOnce(int index)
        {
            // int duration = Convert.ToInt32 (ed_targetAvatar.GetComponent<Animation> () [animationNames [animIndex]].clip.length) * 1000;
            // playAnimation (index);
            // SetTimeOut (() => {
            // 	playAnimation (index);
            // }, duration);
        }

        void SetTimeOut(Action action, int seconds)
        {
            Timer tmr = new Timer
            {
                Interval = seconds
            };
            tmr.Elapsed += delegate
            {
                action();
                tmr.Stop();
            };
            tmr.Start();
        }

        void OnRecognizeText(string[] input)
        {
            Debug.Log(input.ToString());
        }

        void Update()
        {

            settingsButton.gameObject.SetActive(!HasCanvasOpen());

            CameraController _camera = (CameraController)GetComponent(typeof(CameraController));
            if (dialogueText.text != null && dialogueText.text.Length > 0)
            {
                if (_camera != null)
                {
                    _camera.changeTo(1);
                }

                ShowText();
            }
            else
            {
                if (fila != null && fila.Count > 0)
                {
                    if (_camera != null)
                    {
                        _camera.changeTo(1);
                    }

                    ViewNext();
                }
                else
                {
                    if (_camera != null)
                    {
                        _camera.changeTo(0);
                    }

                    HideText();
                }
            }
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

        public void exitApp()
        {
            Application.Quit();
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





        /*
		 * 0 - Idle
		 * 1 - Fear
		 * 2 - Happy
		 * 3 - Better Idle
		 * 4 - Scared
		 * 5 - Running
		 * 6 - Weird
		 * 7 - Walking
		 */

        public void playAnimation(string index)
        {
            playAnimation(Convert.ToInt32(index));
        }

        public void playAnimation(int val)
        {
            if (animIndex != -1)
            {
                if (ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]].speed == 0)
                {
                    pauseAnimation(true);
                }
            }

            animIndex = (val) % animationNames.Length;
            ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]].wrapMode = WrapMode.Loop;
            ed_targetAvatar.GetComponent<Animation>().Play(animationNames[animIndex]);
            synchAnimation();
        }

        public void pauseAnimation(bool value)
        {
            if (!value)
            {
                ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]].speed = 0;

                if (wornHair != null)
                {
                    wornHair.GetComponent<Animation>()[animationNames[animIndex]].speed = 0;
                }

                if (wornGlasses != null)
                {
                    wornGlasses.GetComponent<Animation>()[animationNames[animIndex]].speed = 0;
                }

                if (wornOutfit != null)
                {
                    wornOutfit.GetComponent<Animation>()[animationNames[animIndex]].speed = 0;
                }
            }
            else
            {
                ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]].speed = 1;
                if (wornHair != null)
                {
                    wornHair.GetComponent<Animation>()[animationNames[animIndex]].speed = 1;
                }

                if (wornGlasses != null)
                {
                    wornGlasses.GetComponent<Animation>()[animationNames[animIndex]].speed = 1;
                }

                if (wornOutfit != null)
                {
                    wornOutfit.GetComponent<Animation>()[animationNames[animIndex]].speed = 1;
                }
            }
        }

        public void playNextAnimation()
        {
            playAnimation(animIndex + 1);
        }

        public void changeNextHair()
        {
            setHair(hairIndex + 1);
        }

        public void setHair(int index, bool skipAnim = true)
        {
            hairIndex = index % hairs.Length;
            Destroy(wornHair);
            if (hairs[hairIndex] != null)
            {
                wornHair = Instantiate(hairs[hairIndex]) as GameObject;
                wornHair.transform.parent = transform;
                wornHair.transform.localScale = Vector3.one;
                wornHair.transform.localRotation = Quaternion.identity;
                wornHair.transform.localPosition = Vector3.zero;
                synchAnimation();
                PlayerPrefs.SetInt("hair", hairIndex);
                if (!skipAnim)
                {
                    playAnimation(3);
                }
            }
        }

        public void changeNextGlasses()
        {
            setGlasses(glassesIndex + 1);
        }

        public void setGlasses(int index, bool skipAnim = true)
        {
            glassesIndex = index % glasses.Length;
            Destroy(wornGlasses);
            if (glasses[glassesIndex] != null)
            {
                wornGlasses = Instantiate(glasses[glassesIndex]) as GameObject;
                wornGlasses.transform.parent = transform;
                wornGlasses.transform.localScale = Vector3.one;
                wornGlasses.transform.localRotation = Quaternion.identity;
                wornGlasses.transform.localPosition = Vector3.zero;
                synchAnimation();
                PlayerPrefs.SetInt("glasses", hairIndex);
                if (!skipAnim)
                {
                    playAnimation(10);
                }
            }
        }

        public void changeNextOutfit()
        {
            setOutfit(outfitIndex + 1);
        }

        public void setOutfit(int index, bool skipAnim = true)
        {
            outfitIndex = index % outfits.Length;

            Destroy(wornOutfit);

            if (outfits[outfitIndex] != null)
            {
                wornOutfit = Instantiate(outfits[outfitIndex]) as GameObject;
                wornOutfit.transform.parent = transform;
                wornOutfit.transform.localScale = Vector3.one;
                wornOutfit.transform.localRotation = Quaternion.identity;
                wornOutfit.transform.localPosition = Vector3.zero;

                foreach (GameObject go in hideObjects)
                {
                    go.SetActive(false);
                }

                PlayerPrefs.SetInt("outfit", hairIndex);
                synchAnimation();
                if (!skipAnim)
                {
                    playAnimation(11);
                }
            }
            else
            {
                foreach (GameObject go in hideObjects)
                {
                    go.SetActive(true);
                }
            }
        }

        public void synchAnimation()
        {
            AnimationClip clip = ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]].clip;
            clip.wrapMode = WrapMode.Loop;
            if (wornHair != null)
            {
                wornHair.GetComponent<Animation>().AddClip(clip, clip.name);
                wornHair.GetComponent<Animation>().Play(clip.name);
            }
            if (wornGlasses != null)
            {
                wornGlasses.GetComponent<Animation>().AddClip(clip, clip.name);
                wornGlasses.GetComponent<Animation>().Play(clip.name);
            }
            if (wornOutfit != null)
            {
                wornOutfit.GetComponent<Animation>().AddClip(clip, clip.name);
                wornOutfit.GetComponent<Animation>().Play(clip.name);
            }

            StartCoroutine(waitThenSynch());
        }

        IEnumerator waitThenSynch()
        {
            yield return new WaitForEndOfFrame();

            AnimationState animState = ed_targetAvatar.GetComponent<Animation>()[animationNames[animIndex]];

            if (wornHair != null)
            {
                wornHair.GetComponent<Animation>()[animState.clip.name].time = animState.time;
                wornHair.GetComponent<Animation>()[animState.clip.name].speed = animState.speed;
            }
            if (wornGlasses != null)
            {
                wornGlasses.GetComponent<Animation>()[animState.clip.name].time = animState.time;
                wornGlasses.GetComponent<Animation>()[animState.clip.name].speed = animState.speed;
            }
            if (wornOutfit != null)
            {
                wornOutfit.GetComponent<Animation>()[animState.clip.name].time = animState.time;
                wornOutfit.GetComponent<Animation>()[animState.clip.name].speed = animState.speed;
            }
        }
    }

}