using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    internal static class Persist {
        internal static Vector3 TrgPos;       // For reporting in the GUI
        internal static Vector3 LookAtTrgPos; //
        internal static Vector3 PrevPos { get; set; } = UnityEngine.Camera.main.transform.position; // These actually need to persist
        internal static Quaternion PrevRot { get; set; } = UnityEngine.Camera.main.transform.rotation; //
        internal static float MinMovementThreshold;
        internal static float MinRotationThreshold;
    }
    static class Temp {
        internal static GameObject Camera       = new GameObject(); // Mainly to avoid creating and destroying these every frame!
        internal static GameObject CameraLookAt = new GameObject(); //
        //internal static Vector3    Pos { get { return Camera.transform.position; } set { Camera.transform.position = value; } } // I'm lazy
        //internal static Quaternion Rot { get { return Camera.transform.rotation; } set { Camera.transform.rotation = value; } } //
    }

    [DefaultExecutionOrder(14000)]
    internal class FollowCam : MonoBehaviour {
        private static GameObject objFollowCam = new GameObject("FollowCam", typeof(FollowCam));
        internal static bool IsActive => objFollowCam.activeSelf;
        internal static void SetActive(bool Active) {
            if (Active && !objFollowCam.activeSelf) {
                objFollowCam.SetActive(true);
            } else if (!Active && objFollowCam.activeSelf) {
                objFollowCam.SetActive(false);
            }
        }

        public void OnEnable() {
            Persist.PrevPos = Camera.main.transform.position;
            Persist.PrevRot = Camera.main.transform.rotation;
            Persist.MinMovementThreshold = Settings.MinMovementThreshold;
            Persist.MinRotationThreshold = Settings.MinRotationThreshold;
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 1f);
        }

        public void OnDisable() {
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_enabled", 0f);
        }

        public void LateUpdate() {
            try {
                //Vector3 CameraPos = Camera.main.transform.position;
                //Quaternion CameraRot = Camera.main.transform.rotation;
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
                Temp.CameraLookAt.transform.position = LookAtBonePos;

                switch (Settings.OffsetMode) {
                    case CameraPosMode.Off:
                        Temp.Camera.transform.position = Camera.main.transform.position;
                        break;
                    case CameraPosMode.Absolute:
                        Temp.Camera.transform.position = BonePos + Settings.OffsetPosition;
                        break;
                    case CameraPosMode.Relative:
                        Temp.Camera.transform.position = BonePos + (BoneRot * Settings.OffsetPosition);
                        break;
                }
                Persist.TrgPos = Temp.Camera.transform.position;
                // Handle movement distance limit
                if ((Persist.PrevPos - Temp.Camera.transform.position).magnitude > Persist.MinMovementThreshold) {
                    Camera.main.transform.position = Vector3.Lerp(Persist.PrevPos, Temp.Camera.transform.position, Settings.MaxMovementDistance);
                    Persist.PrevPos = Camera.main.transform.position;
                    Persist.MinMovementThreshold = Settings.MinMovementThreshold / 10;
                } else {
                    Camera.main.transform.position = Persist.PrevPos;
                    Persist.MinMovementThreshold = Settings.MinMovementThreshold;
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camx", Persist.PrevPos.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camy", Persist.PrevPos.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camz", Persist.PrevPos.z);



                // Get target lookat angle
                switch (Settings.RotationMode) {
                    case CameraPosMode.Off:
                        Temp.Camera.transform.rotation = Camera.main.transform.rotation;
                        break;
                    case CameraPosMode.Absolute:
                        //Temp.Pos = Persist.PrevPos;
                        //Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        Temp.CameraLookAt.transform.position += Settings.LookAtOffsetPosition;
                        Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        break;
                    case CameraPosMode.Relative:
                        //Temp.Pos = Persist.PrevPos;
                        //Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        Temp.CameraLookAt.transform.position += (LookAtBoneTransform.rotation * Settings.LookAtOffsetPosition);
                        Temp.Camera.transform.LookAt(Temp.CameraLookAt.transform);
                        break;
                }
                Persist.LookAtTrgPos = Temp.CameraLookAt.transform.position;
                // Handle rotation distance limit
                (Persist.PrevRot * Quaternion.Inverse(Temp.Camera.transform.rotation)).ToAngleAxis(out TempFloat, out TempVector3);
                if (TempFloat > Persist.MinRotationThreshold) {
                    Camera.main.transform.rotation = Quaternion.Lerp(Persist.PrevRot, Temp.Camera.transform.rotation, Settings.MaxRotation);
                    Persist.PrevRot = Camera.main.transform.rotation;
                    Persist.MinRotationThreshold = Settings.MinRotationThreshold / 10;
                } else {
                    Camera.main.transform.rotation = Persist.PrevRot;
                    Persist.MinRotationThreshold = Settings.MinRotationThreshold;
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotw", Persist.PrevRot.w);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotx", Persist.PrevRot.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_roty", Persist.PrevRot.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotz", Persist.PrevRot.z);

            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}
