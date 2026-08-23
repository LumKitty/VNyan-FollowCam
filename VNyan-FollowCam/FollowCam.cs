using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    internal class FollowCam : MonoBehaviour {
        internal static GameObject objFollowCam = new GameObject("FollowCam", typeof(FollowCam));
        internal static CameraWrangler objMainCamera = new CameraWrangler(Camera.main.transform, Settings);
        internal static bool IsActive => objFollowCam.activeSelf;
        internal static void SetActive(bool Active) {
            if (Active && !objFollowCam.activeSelf) {
                objFollowCam.SetActive(true);
            } else if (!Active && objFollowCam.activeSelf) {
                objFollowCam.SetActive(false);
            }
        }

        public void OnEnable() {
            objMainCamera.Enable();
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 1f);
        }

        public void OnDisable() {
            objMainCamera.Disable();
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 0f);
        }

        public void LateUpdate() { 
            try {
                objMainCamera.DoUpdate();
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camx", objMainCamera.CurrentCamera.position.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camy", objMainCamera.CurrentCamera.position.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camz", objMainCamera.CurrentCamera.position.z);

                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotw", objMainCamera.CurrentCamera.rotation.w);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotx", objMainCamera.CurrentCamera.rotation.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_roty", objMainCamera.CurrentCamera.rotation.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotz", objMainCamera.CurrentCamera.rotation.z);
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}