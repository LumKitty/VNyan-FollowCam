using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using static VNyan_FollowCam._Settings;
using static VNyan_FollowCam.Functions;

namespace VNyan_FollowCam {
    
    internal class FollowCam : MonoBehaviour {
        internal static GameObject objFollowCam = new GameObject("FollowCam", typeof(FollowCam));
        //internal static CameraWrangler objMainCamera = new CameraWrangler(Camera.main.transform, Settings);
        public static List<BasicCamera> objCameras = new List<BasicCamera>();
        internal static double PrevTime = Time.realtimeSinceStartupAsDouble;
        internal static HighResTimer HighResTimer = new HighResTimer(GlobalSettings.CalculationFPS);
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
                    Log($"Attempted to attach to already handled Spout2 camera: {CameraName}");
                    return -1;
                }
            }

            VNyanInterface.ISpout2Camera? Camera = FindVNyanCamera(CameraName);

            if (Camera != null) {
                objCameras.Add(new SpoutCamera(Camera, SettingsFileName));
                Log($"Attached Spout2 camera {CameraName}");
                return objCameras.Count - 1;
            }
            Log($"Couldn't attach Spout2 camera {CameraName} as it doesn't appear to exist");
            return 0;
        }

        public void Awake() {
            try {
                PrevTime = Time.realtimeSinceStartupAsDouble;
                if (GlobalSettings.CalculationFPS > 0) {
                    HighResTimer.SetFrequency(GlobalSettings.CalculationFPS);
                    if (IsAnyFollowCamActive()) { HighResTimer.Start(); }
                }
                //InvokeRepeating("UpdateCamera", 0, 1f/GlobalSettings.CalculationFPS);
                Log("Enabled followcam");
                //objMainCamera.Enable();
                //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 1f);
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }

        private void _NewFPS() {
            try {
                CancelInvoke();
                if (GlobalSettings.CalculationFPS > 0) {
                    HighResTimer.Stop();
                    HighResTimer.SetFrequency(GlobalSettings.CalculationFPS);
                    
                    HighResTimer.Start();
                    //InvokeRepeating("UpdateCamera", 0, 1f / GlobalSettings.CalculationFPS);
                    Log($"FPS updated to {GlobalSettings.CalculationFPS}");
                } else {
                    HighResTimer.Stop();
                    Log("FPS will follow VNyan FPS");
                }
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }

        public static void NewFPS() {
            objFollowCam.GetComponent<FollowCam>()._NewFPS();
        }

        public static bool IsAnyFollowCamActive() {
            foreach (var objCamera in objCameras) {
                if (objCamera.Enabled) {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void OnDisable() {
            try { 
                CancelInvoke();
                Log("Disabled followcam");
                //objMainCamera.Disable();
                //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 0f);
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }

        public static void UpdateCamera() {
            try {
                double Now = Time.realtimeSinceStartupAsDouble;
                double TimeDelta = Now - PrevTime;
                Log($"Called at: {Now}, {TimeDelta} since previous call",69);
                foreach (var objCamera in objCameras) {
                    if (objCamera.Enabled) { objCamera.DoUpdate((float)TimeDelta); }
                }
                PrevTime = Now;
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }
        
        public void LateUpdate() { 
            try {

                /*TODO:
                    If VRNyan CursedCamera is not enabled, we need to override the VNyan camera here.
                    If VRnyan is connected, but not active, we probably need to override the camera here
                    VRnyan should probably flag to FollowCam when it's not handling things

                    Functions in FollowCam "I need to do things" / "I don't need to do things"
                    Call from VRnyan whenever enable/disable is requested (after checking Cursed Camera)
                */
                if (GlobalSettings.CalculationFPS <= 0) { UpdateCamera(); }
                if (objCameras[0].Wrangler.Enabled &&!VRnyan_Handlers.VRNyanControllingCamera) {
                    Camera.main.transform.position = objCameras[0].Wrangler.CurrentCamera.position;
                    Camera.main.transform.rotation = objCameras[0].Wrangler.CurrentCamera.rotation;
                }
                //Log($"VRNyanControllingCamera: {VRnyan_Handlers.VRNyanControllingCamera}, MainFollowCamActive: {VRnyan_Handlers.MainFollowCamActive}");
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }

        public void OnApplicationQuit() {
            HighResTimer.Stop();
        }
    }
}