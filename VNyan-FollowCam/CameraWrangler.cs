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

        public string SettingsFileName;
        public __Settings Settings;
        public Transform CurrentCamera;
        public string Name { get; }

        internal GameObject Temp_Camera = new GameObject(); // Mainly to avoid creating and destroying these every frame!
        internal GameObject Temp_CameraLookAt = new GameObject(); //

        internal bool _Enabled = false;

        public bool Enabled { get { return _Enabled; } }

        public CameraWrangler(Transform _CurrentCamera, string _SettingsFileName, string _Name) {
            SettingsFileName = _SettingsFileName;
            if (!SettingsFile.Load(_SettingsFileName, this)) {
                Settings = new __Settings();
            }
            CurrentCamera = _CurrentCamera;
            Persist_PrevPos = CurrentCamera.position;
            Persist_PrevRot = CurrentCamera.rotation;
            Name = _Name;
        }
        
        public void Enable() {
            if (!_Enabled) {
                _Enabled = true;
                Persist_PrevPos = CurrentCamera.position;
                Persist_PrevRot = CurrentCamera.rotation;
                Persist_MinMovementThreshold = Settings.MinMovementThreshold;
                Persist_MinRotationThreshold = Settings.MinRotationThreshold;
            }
        }

        public void Disable() {
            if (_Enabled) {
                _Enabled = false;
            }
        }

        public void DoUpdate(float DeltaTime) {
            try {
                if (_Enabled) {
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
                            Persist_TrgPos = CurrentCamera.transform.position;
                            break;
                        case CameraPosMode.Absolute:
                            Persist_TrgPos = BonePos + Settings.OffsetPosition;
                            break;
                        case CameraPosMode.Relative:
                            Persist_TrgPos = BonePos + (BoneRot * Settings.OffsetPosition);
                            break;
                    }
                    if (Settings.StaticX) { Persist_TrgPos.x = Settings.OffsetPosition.x; }
                    if (Settings.StaticY) { Persist_TrgPos.y = Settings.OffsetPosition.y; }
                    if (Settings.StaticZ) { Persist_TrgPos.z = Settings.OffsetPosition.z; }

                    Temp_Camera.transform.position = Persist_TrgPos;
                    // Handle movement distance limit
                    if ((Persist_PrevPos - Temp_Camera.transform.position).magnitude > Persist_MinMovementThreshold) {
                        CurrentCamera.position = Vector3.Lerp(Persist_PrevPos, Temp_Camera.transform.position, Settings.MovementLerp*DeltaTime);
                        Persist_PrevPos = CurrentCamera.position;
                        Persist_MinMovementThreshold = Settings.MinMovementThreshold / 10;
                    } else {
                        CurrentCamera.position = Persist_PrevPos;
                        Persist_MinMovementThreshold = Settings.MinMovementThreshold;
                    }

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
                        CurrentCamera.transform.rotation = Quaternion.Lerp(Persist_PrevRot, Temp_Camera.transform.rotation, Settings.RotationLerp * DeltaTime);
                        Persist_PrevRot = CurrentCamera.rotation;
                        Persist_MinRotationThreshold = Settings.MinRotationThreshold / 10;
                    } else {
                        CurrentCamera.transform.rotation = Persist_PrevRot;
                        Persist_MinRotationThreshold = Settings.MinRotationThreshold;
                    }
                }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

    }
}
