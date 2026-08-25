using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNyan_FollowCam {
    internal abstract class BasicCamera {
        internal CameraWrangler Wrangler;
        internal abstract void DoUpdate(float DeltaTime);
    }
    
    internal class MainCamera : BasicCamera {
        internal MainCamera(string SettingsFileName) {
            Wrangler = new CameraWrangler(Camera.main.transform, SettingsFileName, "Main Camera");
        }
        internal override void DoUpdate(float DeltaTime) {
            Wrangler.DoUpdate(DeltaTime);
        }
    }

    internal class SpoutCamera : BasicCamera {
        internal GameObject GameObject;
        internal VNyanInterface.ISpout2Camera? VNCamera;

        private VNyanInterface.VNyanVector3 TempPosition;
        private VNyanInterface.VNyanQuaternion TempRotation;

        internal SpoutCamera(VNyanInterface.ISpout2Camera _VNCamera, string SettingsFileName) {
            GameObject = new GameObject();
            TempPosition = _VNCamera.getPosition();
            TempRotation = _VNCamera.getRotation();
            GameObject.transform.position = new Vector3(TempPosition.X, TempPosition.Y, TempPosition.Z);
            GameObject.transform.rotation = new Quaternion(TempRotation.X, TempRotation.Y, TempRotation.Z, TempRotation.W);
            VNCamera = _VNCamera;
            Wrangler = new CameraWrangler(GameObject.transform, SettingsFileName, _VNCamera.getSourceName());
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
