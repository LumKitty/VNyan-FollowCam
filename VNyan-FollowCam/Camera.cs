using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNyan_FollowCam {
    internal abstract class BasicCamera {
        internal CameraWrangler Wrangler;
        internal abstract void DoUpdate(float DeltaTime);
        internal abstract void Enable();
        internal abstract void Disable();
        internal bool Enabled = false;
    }
    
    internal class MainCamera : BasicCamera {
        internal GameObject DummyCamera = new GameObject();
        internal MainCamera(string SettingsFileName) {
            DummyCamera.transform.position = Camera.main.transform.position;
            DummyCamera.transform.rotation = Camera.main.transform.rotation;
            Wrangler = new CameraWrangler(DummyCamera.transform, SettingsFileName, "Main Camera");
        }
        internal override void DoUpdate(float DeltaTime) {
            Wrangler.DoUpdate(DeltaTime);
            VRnyan_Handlers.UpdateVRnyanCameraPos(Wrangler.CurrentCamera.transform.position, Wrangler.CurrentCamera.transform.rotation);
        }
        internal override void Enable() {
            //VRnyan_Handlers.ConnectVRnyan();
            DummyCamera.transform.position = Camera.main.transform.position;
            DummyCamera.transform.rotation = Camera.main.transform.rotation;
            Wrangler.Enable();
            VRnyan_Handlers.MainFollowCamActive = true;
            Enabled = true;
            if (!FollowCam.HighResTimer.IsRunning) { FollowCam.HighResTimer.Start(); }
        }
        internal override void Disable() {
            Wrangler.Disable();
            VRnyan_Handlers.MainFollowCamActive = false;
            Enabled = false;
            if (!FollowCam.IsAnyFollowCamActive()) { FollowCam.HighResTimer.Stop(); }
        }
    }

    internal class SpoutCamera : BasicCamera {
        internal GameObject DummyCamera = new GameObject();
        internal VNyanInterface.ISpout2Camera? VNCamera;

        private VNyanInterface.VNyanVector3 TempPosition;
        private VNyanInterface.VNyanQuaternion TempRotation;

        internal SpoutCamera(VNyanInterface.ISpout2Camera _VNCamera, string SettingsFileName) {
            TempPosition = _VNCamera.getPosition();
            TempRotation = _VNCamera.getRotation();
            DummyCamera.transform.position = new Vector3(TempPosition.X, TempPosition.Y, TempPosition.Z);
            DummyCamera.transform.rotation = new Quaternion(TempRotation.X, TempRotation.Y, TempRotation.Z, TempRotation.W);
            VNCamera = _VNCamera;
            Wrangler = new CameraWrangler(DummyCamera.transform, SettingsFileName, _VNCamera.getSourceName());
        }

        internal override void Enable() {
            TempPosition = VNCamera.getPosition();
            TempRotation = VNCamera.getRotation();
            DummyCamera.transform.position = new Vector3(TempPosition.X, TempPosition.Y, TempPosition.Z);
            DummyCamera.transform.rotation = new Quaternion(TempRotation.X, TempRotation.Y, TempRotation.Z, TempRotation.W);
            Wrangler.Enable();
            if (!FollowCam.HighResTimer.IsRunning) { FollowCam.HighResTimer.Start(); }

        }
        internal override void Disable() {
            Wrangler.Disable();
            if (!FollowCam.IsAnyFollowCamActive()) { FollowCam.HighResTimer.Stop(); }
        }

        internal override void DoUpdate(float DeltaTime) {
            Wrangler.DoUpdate(DeltaTime);
            TempPosition.X = Wrangler.CurrentCamera.position.x;
            TempPosition.Y = Wrangler.CurrentCamera.position.y;
            TempPosition.Z = Wrangler.CurrentCamera.position.z;
            TempRotation.W = Wrangler.CurrentCamera.rotation.w;
            TempRotation.X = Wrangler.CurrentCamera.rotation.x;
            TempRotation.Y = Wrangler.CurrentCamera.rotation.y;
            TempRotation.Z = Wrangler.CurrentCamera.rotation.z;
            
            VNCamera = FollowCam.FindVNyanCamera(Wrangler.Name);
            if (VNCamera != null) {
                VNCamera.setPosition(TempPosition);
                VNCamera.setRotation(TempRotation);
            }
        }
    }
}
