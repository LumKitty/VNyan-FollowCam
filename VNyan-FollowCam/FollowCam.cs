using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    
    internal class FollowCam : MonoBehaviour {
        internal static GameObject objFollowCam = new GameObject("FollowCam", typeof(FollowCam));
        //internal static CameraWrangler objMainCamera = new CameraWrangler(Camera.main.transform, Settings);
        public static List<BasicCamera> objCameras = new List<BasicCamera>();
        internal static DateTime PrevTime = DateTime.UtcNow;
        //internal static CameraWrangler objMainCamera => objCameras[0].Wrangler;
        //internal static bool IsActive => objFollowCam.activeSelf;
        /*internal static void SetActive(bool Active) {
            if (Active && !objFollowCam.activeSelf) {
                objFollowCam.SetActive(true);
            } else if (!Active && objFollowCam.activeSelf) {
                objFollowCam.SetActive(false);
            }
        }*/

        public static Transform GetFollowCamTransform(int Camera = 0) {
            return objCameras[Camera].Wrangler.CurrentCamera;
        }

        internal static VNyanInterface.ISpout2Camera? FindVNyanCamera(string CameraName) {
            foreach (var Camera in VNyanInterface.VNyanInterface.VNyanRender.getSpout2Cameras()) {
                if (Camera.getSourceName() == CameraName) {
                    return Camera;
                }
            }
            return null;
        }
        
        public static int AttachSpoutCamera(string CameraName, string SettingsFileName) {
            foreach (BasicCamera ExistingCamera in objCameras) {
                if (ExistingCamera.Wrangler.Name == CameraName) {
                    VNyan_Handlers.Log($"Attempted to attach to already handled Spout2 camera: {CameraName}");
                    return -1;
                }
            }

            VNyanInterface.ISpout2Camera? Camera = FindVNyanCamera(CameraName);

            if (Camera != null) {
                objCameras.Add(new SpoutCamera(Camera, SettingsFileName));
                VNyan_Handlers.Log($"Attached Spout2 camera {CameraName}");
                return objCameras.Count - 1;
            }
            VNyan_Handlers.Log($"Couldn't attach Spout2 camera {CameraName} as it doesn't appear to exist");
            return 0;
        }

        public void OnEnable() {
            try { 
                PrevTime = DateTime.UtcNow;
                InvokeRepeating("UpdateCamera", 0, 1f/GlobalSettings.CalculationFPS);
                VNyan_Handlers.Log("Enabled followcam");
                //objMainCamera.Enable();
                //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 1f);
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        private void _NewFPS() {
            try {
                CancelInvoke();
                InvokeRepeating("UpdateCamera", 0, 1f / GlobalSettings.CalculationFPS);
                VNyan_Handlers.Log($"FPS updated to {GlobalSettings.CalculationFPS}");
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        public static void NewFPS() {
            objFollowCam.GetComponent<FollowCam>()._NewFPS();
        }

        public void OnDisable() {
            try { 
                CancelInvoke();
                VNyan_Handlers.Log("Disabled followcam");
                //objMainCamera.Disable();
                //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 0f);
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        public void UpdateCamera() {
            try {
                DateTime Now = DateTime.UtcNow;
                float TimeDelta = (float)((Now - PrevTime).TotalSeconds);
                VNyan_Handlers.Log($"Called at: {Now}, {TimeDelta} since previous call",69);
                foreach (var objCamera in objCameras) {
                    objCamera.DoUpdate(TimeDelta);
                }
                PrevTime = Now;
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
        
        public void LateUpdate() { 
            try {
                /*foreach (var objCamera in objCameras) {
                    objCamera.DoUpdate(Time.deltaTime);
                }*/
                if (!VNyan_Handlers.VRnyanConnectionActive && objCameras[0].Wrangler.Enabled) {
                    Camera.main.transform.position = objCameras[0].Wrangler.CurrentCamera.position;
                    Camera.main.transform.rotation = objCameras[0].Wrangler.CurrentCamera.rotation;
                }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}