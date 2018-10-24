using System;
using System.Collections;
using System.Timers;
using UnityEngine;

namespace Assets.Scripts
{

    public partial class AvatarController : MonoBehaviour
    {

        // Avatar
        [Header("Avatar")]
        public GameObject ed_targetAvatar;

        public Transform[] targets;
        public int current_target;
        public float speed;

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

        public static AvatarController avatar;

        public static void SetTarget(int index)
        {
            avatar.current_target = (index + 1) % avatar.targets.Length;
        }

        void Start()
        {

            avatar = this;
            animationNames = new string[ed_targetAvatar.GetComponent<Animation>().GetClipCount()];

            int i = 0;
            foreach (AnimationState animState in ed_targetAvatar.GetComponent<Animation>())
            {
                // animState.layer = i < 6 ? 0 : 1;
                animationNames[i] = animState.clip.name;
                i++;
            }

            setHair(PlayerPrefs.GetInt("hair", 0));
            setGlasses(PlayerPrefs.GetInt("glasses", 0));
            setOutfit(PlayerPrefs.GetInt("outfit"));

            playAnimation(13);

        }

        void PlayAnimationOnce(int index)
        {
            // int duration = Convert.ToInt32 (ed_targetAvatar.GetComponent<Animation> () [animationNames [animIndex]].clip.length) * 1000;
            // playAnimation (index);
            // SetTimeOut (() => {
            // 	playAnimation (index);
            // }, duration);
        }

        public void Position(Vector3 pos)
        {
            gameObject.transform.position = pos;
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

            if (transform.position != targets[current_target].position)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, targets[current_target].position, speed);
                transform.LookAt(pos);
                GetComponent<Rigidbody>().MovePosition(pos);

                if (animIndex != 13)
                {
                    playAnimation(13);
                }
            }
            else
            {

                Vector3 cam_pos = GameObject.Find("main_camera").gameObject.transform.position;
                cam_pos.y = 0f;
                transform.LookAt(cam_pos);
                if (animIndex != 0)
                {
                    playAnimation(0);
                }
                CanvasController.UpdateCanvas(current_target);
            }
        }


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
            SynchAnimation();
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
                SynchAnimation();
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
                SynchAnimation();
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
                SynchAnimation();
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

        public void SynchAnimation()
        {
            if (animIndex == -1)
            {
                return;
            }

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

            StartCoroutine(WaitThenSynch());
        }

        IEnumerator WaitThenSynch()
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