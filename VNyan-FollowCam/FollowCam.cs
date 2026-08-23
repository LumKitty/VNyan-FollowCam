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
        //internal static CameraWrangler objMainCamera => objCameras[0].Wrangler;
        //internal static bool IsActive => objFollowCam.activeSelf;
        /*internal static void SetActive(bool Active) {
            if (Active && !objFollowCam.activeSelf) {
                objFollowCam.SetActive(true);
            } else if (!Active && objFollowCam.activeSelf) {
                objFollowCam.SetActive(false);
            }
        }*/

        public static Vector3 GetMainCameraPos() {
            return objCameras[0].Wrangler.CurrentCamera.position;
        }
        public static Quaternion GetMainCameraRot() {
            return objCameras[0].Wrangler.CurrentCamera.rotation;
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
            //objMainCamera.Enable();
            //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 1f);
        }

        public void OnDisable() {
            //objMainCamera.Disable();
            //VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 0f);
        }

        public void LateUpdate() { 
            try {
                foreach (var objCamera in objCameras) {
                    objCamera.DoUpdate();
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camx", objCameras[0].Wrangler.CurrentCamera.position.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camy", objCameras[0].Wrangler.CurrentCamera.position.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camz", objCameras[0].Wrangler.CurrentCamera.position.z);

                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotw", objCameras[0].Wrangler.CurrentCamera.rotation.w);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotx", objCameras[0].Wrangler.CurrentCamera.rotation.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_roty", objCameras[0].Wrangler.CurrentCamera.rotation.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotz", objCameras[0].Wrangler.CurrentCamera.rotation.z);
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}