using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam.Functions;

namespace VNyan_FollowCam {

    //internal SpoutCamera(string SettingsFileName, string SourceName, int Width = -1, int Height = -1, float FocalLength = -1, bool ShowAvatar = true, bool ShowCObject = true, bool ShowWorld = true, bool ShowSkyBox = false) {
    internal abstract class BasicCamera {
        internal CameraWrangler Wrangler;
        internal abstract void DoUpdate(float DeltaTime, double Now);
        internal abstract void Enable();
        internal abstract void Disable();
        internal bool Enabled = false;

        internal int Width;
        internal int Height;
        internal float FocalLength;
        internal bool ShowAvatar = true;
        internal bool ShowCObject = true;
        internal bool ShowWorld = true;
        internal bool ShowSkyBox = false;
    }
    
    internal class MainCamera : BasicCamera {
        new internal int Width => Screen.width;
        new internal int Height => Screen.height;
        new internal float FocalLength => Camera.main.fieldOfView;
        new internal bool ShowAvatar => true;
        new internal bool ShowCObject => true;
        new internal bool ShowWorld => true;
        new internal bool ShowSkyBox => false;
        internal GameObject DummyCamera = new GameObject();
        ulong Sequence = 0;

        internal MainCamera(string SettingsFileName) {
            DummyCamera.transform.position = Camera.main.transform.position;
            DummyCamera.transform.rotation = Camera.main.transform.rotation;
            Wrangler = new CameraWrangler(DummyCamera.transform, SettingsFileName, "Main Camera");
        }

        internal override void DoUpdate(float DeltaTime, double Now) {
            Wrangler.DoUpdate(DeltaTime);
            VRnyan_Handlers.UpdateMMF(Wrangler.CurrentCamera.transform.position, Wrangler.CurrentCamera.transform.rotation);
            VRnyan_Handlers.UpdateVRnyanCameraPos(Wrangler.CurrentCamera.transform.position, Wrangler.CurrentCamera.transform.rotation, Sequence++, Now);
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

        internal SpoutCamera(string SettingsFileName, string SourceName, int Width = -1, int Height = -1, float FocalLength = -1, bool ShowAvatar = true, bool ShowCObject = true, bool ShowWorld = true, bool ShowSkyBox = false) {
            if (FocalLength == -1) { FocalLength = Camera.main.fieldOfView; }
            if (Width == -1) { Width = Screen.width; }
            if (Height == -1) { Height = Screen.height; }
            //VNCamera = VNyanInterface.VNyanInterface.VNyanRender.createSpout2Camera(Width, Height, SourceName, false, new VNyanInterface.VNyanVector3(), new VNyanInterface.VNyanQuaternion(), FocalLength, ShowAvatar, ShowWorld, ShowCObject, ShowSkyBox);
            TempPosition = new VNyanInterface.VNyanVector3();
            TempRotation = new VNyanInterface.VNyanQuaternion();
            DummyCamera.transform.position = new Vector3();
            DummyCamera.transform.rotation = new Quaternion();
            Wrangler = new CameraWrangler(DummyCamera.transform, SettingsFileName, SourceName);
            this.Width = Width;
            this.Height = Height;
            this.FocalLength = FocalLength;
            this.ShowAvatar = ShowAvatar;
            this.ShowCObject = ShowCObject;
            this.ShowWorld = ShowWorld;
            this.ShowSkyBox = ShowSkyBox;
        }

        internal override void Enable() {
            Wrangler.Enable();
            Wrangler.DoUpdate(-1);
            VNCamera = VNyanInterface.VNyanInterface.VNyanRender.createSpout2Camera(Width, Height, Wrangler.Name, false, FromVector3(Wrangler.CurrentCamera.position), FromQuaternion(Wrangler.CurrentCamera.rotation), FocalLength, ShowAvatar, ShowWorld, ShowCObject, ShowSkyBox);
            TempPosition = VNCamera.getPosition();
            TempRotation = VNCamera.getRotation();
            DummyCamera.transform.position = new Vector3(TempPosition.X, TempPosition.Y, TempPosition.Z);
            DummyCamera.transform.rotation = new Quaternion(TempRotation.X, TempRotation.Y, TempRotation.Z, TempRotation.W);
            
            Enabled = true;
            if (!FollowCam.HighResTimer.IsRunning) { FollowCam.HighResTimer.Start(); }

        }
        internal override void Disable() {
            Wrangler.Disable();
            Enabled = false;
            if (!FollowCam.IsAnyFollowCamActive()) { FollowCam.HighResTimer.Stop(); }
            VNyanInterface.VNyanInterface.VNyanRender.removeSpout2Camera(VNCamera);
            VNCamera = null;
        }

        internal override void DoUpdate(float DeltaTime, double Now) {
            Wrangler.DoUpdate(DeltaTime);

            TempPosition = FromVector3(Wrangler.CurrentCamera.position);
            TempRotation = FromQuaternion(Wrangler.CurrentCamera.rotation);
            /*
            TempPosition.X = Wrangler.CurrentCamera.position.x;
            TempPosition.Y = Wrangler.CurrentCamera.position.y;
            TempPosition.Z = Wrangler.CurrentCamera.position.z;
            TempRotation.W = Wrangler.CurrentCamera.rotation.w;
            TempRotation.X = Wrangler.CurrentCamera.rotation.x;
            TempRotation.Y = Wrangler.CurrentCamera.rotation.y;
            TempRotation.Z = Wrangler.CurrentCamera.rotation.z;
            */
            
            //VNCamera = FollowCam.FindVNyanCamera(Wrangler.Name);
            if (VNCamera != null) {
                VNCamera.setPosition(TempPosition);
                VNCamera.setRotation(TempRotation);
            }
        }
    }
}
