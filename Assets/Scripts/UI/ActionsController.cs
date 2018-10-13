using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public partial class CanvasController : MonoBehaviour
    {

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

        public void getGlycemia(Action<double> action = null)
        {
            GetDouble("Qual o seu nível de glicemia nesse momento?", 0, 200, (glycemia) =>
            {

                API.UpdateGlyc(glycemia.ToString());

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

    }

}