using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class WardRobe : ClickHandler
    {

        public Canvas clothing_panel;
        private bool open = false;
        private AvatarController avatar;
        private new CameraController camera;
        public Camera _camera;
        public GameObject panel;

        public static Dictionary<string, Action> callbacks = new Dictionary<string, Action>();

        void Start()
        {
            GameObject go = GameObject.Find("avatar");
            avatar = (AvatarController)go.GetComponent(typeof(AvatarController));
            camera = (CameraController)_camera.GetComponent(typeof(CameraController));
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.name == name)
                        {
                            Open();
                        }
                    }
                }
            }
        }

        public void Open()
        {
            Action OnOpenOnce;
            callbacks.TryGetValue("OnOpenOnce", out OnOpenOnce);
            if (OnOpenOnce != null)
            {
                OnOpenOnce();
                callbacks.Remove("OnOpenOnce");
            }

            avatar.playAnimation(3);
            clothing_panel.gameObject.SetActive(true);
            camera.changeTo(0);
            open = true;
        }

        public void Close()
        {
            if (open)
            {

                Action OnCloseOnce;
                callbacks.TryGetValue("OnCloseOnce", out OnCloseOnce);
                if (OnCloseOnce != null)
                {
                    OnCloseOnce();
                    callbacks.Remove("OnCloseOnce");
                }
                avatar.playAnimation(3);
                clothing_panel.gameObject.SetActive(false);
                camera.changeTo(-1);
                open = false;
            }
        }

    }
}