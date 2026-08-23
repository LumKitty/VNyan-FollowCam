using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNyan_FollowCam {
    public class CameraWrangler {
        internal Vector3 Persist_TrgPos;       // For reporting in the GUI
        internal Vector3 Persist_LookAtTrgPos; //
        internal Vector3 Persist_PrevPos { get; set; }
        internal Quaternion Persist_PrevRot { get; set; }
        internal float Persist_MinMovementThreshold;
        internal float Persist_MinRotationThreshold;

        public __Settings Settings;
        public Transform CurrentCamera;

        internal GameObject Temp_Camera = new GameObject(); // Mainly to avoid creating and destroying these every frame!
        internal GameObject Temp_CameraLookAt = new GameObject(); //

        internal bool _Enabled = false;

        public bool Enabled { get { return _Enabled; } }

        public CameraWrangler(Transform _CurrentCamera, __Settings _Settings) {
            Settings = _Settings;
            CurrentCamera = _CurrentCamera;
            Persist_PrevPos = CurrentCamera.position;
            Persist_PrevRot = CurrentCamera.rotation;
        }
        
        public void Enable() {
            _Enabled = true;
            Persist_PrevPos = CurrentCamera.position;
            Persist_PrevRot = CurrentCamera.rotation;
            Persist_MinMovementThreshold = Settings.MinMovementThreshold;
            Persist_MinRotationThreshold = Settings.MinRotationThreshold;
        }

        public void Disable() {
            _Enabled = false;
        }

        public void DoUpdate() {
            try {
                GameObject AvatarObject = (GameObject)VNyanInterface.VNyanInterface.VNyanAvatar.getAvatarObject();
                Animator AvatarAnimator = AvatarObject.GetComponent<Animator>();
                Transform BaseBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.BaseBone);
                Transform LookAtBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.LookAtBone);
                Vector3 BonePos = BaseBoneTransform.position;
                Vector3 LookAtBonePos = LookAtBoneTransform.position;
                Quaternion BoneRot = BaseBoneTransform.rotation;

                float TempFloat;
                Vector3 TempVector3;

                if (Settings.StaticX) { BonePos.x = 0; }
                if (Settings.StaticY) { BonePos.y = 0; }
                if (Settings.StaticZ) { BonePos.z = 0; }
                if (Settings.LookAtStaticX) { LookAtBonePos.x = 0; }
                if (Settings.LookAtStaticY) { LookAtBonePos.y = 0; }
                if (Settings.LookAtStaticZ) { LookAtBonePos.z = 0; }
                Temp_CameraLookAt.transform.position = LookAtBonePos;

                switch (Settings.OffsetMode) {
                    case CameraPosMode.Off:
                        Temp_Camera.transform.position = CurrentCamera.transform.position;
                        break;
                    case CameraPosMode.Absolute:
                        Temp_Camera.transform.position = BonePos + Settings.OffsetPosition;
                        break;
                    case CameraPosMode.Relative:
                        Temp_Camera.transform.position = BonePos + (BoneRot * Settings.OffsetPosition);
                        break;
                }
                Persist_TrgPos = Temp_Camera.transform.position;
                // Handle movement distance limit
                if ((Persist_PrevPos - Temp_Camera.transform.position).magnitude > Persist_MinMovementThreshold) {
                    CurrentCamera.position = Vector3.Lerp(Persist_PrevPos, Temp_Camera.transform.position, Settings.MaxMovementDistance);
                    Persist_PrevPos = CurrentCamera.position;
                    Persist_MinMovementThreshold = Settings.MinMovementThreshold / 10;
                } else {
                    CurrentCamera.position = Persist_PrevPos;
                    Persist_MinMovementThreshold = Settings.MinMovementThreshold;
                }

                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camx", Persist_PrevPos.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camy", Persist_PrevPos.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camz", Persist_PrevPos.z);



                // Get target lookat angle
                switch (Settings.RotationMode) {
                    case CameraPosMode.Off:
                        Temp_Camera.transform.rotation = CurrentCamera.transform.rotation;
                        break;
                    case CameraPosMode.Absolute:
                        //Temp.Pos = Persist.PrevPos;
                        //Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        Temp_CameraLookAt.transform.position += Settings.LookAtOffsetPosition;
                        Temp_Camera.transform.LookAt(Temp_CameraLookAt.transform);
                        break;
                    case CameraPosMode.Relative:
                        //Temp.Pos = Persist.PrevPos;
                        //Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        Temp_CameraLookAt.transform.position += (LookAtBoneTransform.rotation * Settings.LookAtOffsetPosition);
                        Temp_Camera.transform.LookAt(Temp_CameraLookAt.transform);
                        break;
                }
                Persist_LookAtTrgPos = Temp_CameraLookAt.transform.position;
                // Handle rotation distance limit
                (Persist_PrevRot * Quaternion.Inverse(Temp_Camera.transform.rotation)).ToAngleAxis(out TempFloat, out TempVector3);
                if (TempFloat > Persist_MinRotationThreshold) {
                    CurrentCamera.transform.rotation = Quaternion.Lerp(Persist_PrevRot, Temp_Camera.transform.rotation, Settings.MaxRotation);
                    Persist_PrevRot = CurrentCamera.rotation;
                    Persist_MinRotationThreshold = Settings.MinRotationThreshold / 10;
                } else {
                    CurrentCamera.transform.rotation = Persist_PrevRot;
                    Persist_MinRotationThreshold = Settings.MinRotationThreshold;
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotw", Persist_PrevRot.w);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotx", Persist_PrevRot.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_roty", Persist_PrevRot.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotz", Persist_PrevRot.z);

            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

    }
}
