using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    static class FollowCamPersist {
        
        internal static Vector3 PrevPos { get; set; } = Camera.main.transform.position;
        internal static Quaternion PrevRot { get; set; } = Camera.main.transform.rotation;
    }

    static class Persist {
        internal static GameObject Camera = new GameObject();
        internal static Vector3 Pos { get { return Camera.transform.position; } set { Camera.transform.position = value; } }
        internal static Quaternion Rot { get { return Camera.transform.rotation; } set { Camera.transform.rotation = value; } }
        internal static float MinMovementThreshold;
        internal static float MinRotationThreshold;
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
            FollowCamPersist.PrevPos = Camera.main.transform.position;
            FollowCamPersist.PrevRot = Camera.main.transform.rotation;
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
                Quaternion BoneRot = BaseBoneTransform.rotation;

                float TempFloat;
                Vector3 TempVector3;

                switch (Settings.OffsetMode) {
                    case CameraPosMode.Off:
                        Persist.Pos = Camera.main.transform.position;
                        break;
                    case CameraPosMode.Absolute:
                        Persist.Pos = BonePos + Settings.OffsetPosition;
                        break;
                    case CameraPosMode.Relative:
                        Persist.Pos = BonePos + (BoneRot * Settings.OffsetPosition);
                        break;
                }
                // Handle movement distance limit
                if ((FollowCamPersist.PrevPos - Persist.Pos).magnitude > Persist.MinMovementThreshold) {
                    Camera.main.transform.position = Vector3.Lerp(FollowCamPersist.PrevPos, Persist.Pos, Settings.MaxMovementDistance);
                    FollowCamPersist.PrevPos = Camera.main.transform.position;
                    Persist.MinMovementThreshold = Settings.MinMovementThreshold / 10;
                } else {
                    Camera.main.transform.position = FollowCamPersist.PrevPos;
                    Persist.MinMovementThreshold = Settings.MinMovementThreshold;
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camx", FollowCamPersist.PrevPos.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camy", FollowCamPersist.PrevPos.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_camz", FollowCamPersist.PrevPos.z);



                // Get target lookat angle
                switch (Settings.RotationMode) {
                    case CameraPosMode.Off:
                        Persist.Rot = Camera.main.transform.rotation;
                        break;
                    case CameraPosMode.Absolute:
                        Persist.Pos = FollowCamPersist.PrevPos;
                        Persist.Camera.transform.LookAt(LookAtBoneTransform);
                        Persist.Pos += Settings.LookAtOffsetPosition;
                        Persist.Camera.transform.LookAt(LookAtBoneTransform);
                        break;
                    case CameraPosMode.Relative:
                        Persist.Pos = FollowCamPersist.PrevPos;
                        Persist.Camera.transform.LookAt(LookAtBoneTransform);
                        Persist.Pos += (Persist.Rot * Settings.LookAtOffsetPosition);
                        Persist.Camera.transform.LookAt(LookAtBoneTransform);
                        break;
                }

                // Handle rotation distance limit
                (FollowCamPersist.PrevRot * Quaternion.Inverse(Persist.Rot)).ToAngleAxis(out TempFloat, out TempVector3);
                if (TempFloat > Persist.MinRotationThreshold) {
                    Camera.main.transform.rotation = Quaternion.Lerp(FollowCamPersist.PrevRot, Persist.Rot, Settings.MaxRotation);
                    FollowCamPersist.PrevRot = Camera.main.transform.rotation;
                    Persist.MinRotationThreshold = Settings.MinRotationThreshold / 10;
                } else {
                    Camera.main.transform.rotation = FollowCamPersist.PrevRot;
                    Persist.MinRotationThreshold = Settings.MinRotationThreshold;
                }
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotw", FollowCamPersist.PrevRot.w);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotx", FollowCamPersist.PrevRot.x);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_roty", FollowCamPersist.PrevRot.y);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_followcam_rotz", FollowCamPersist.PrevRot.z);

            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}
