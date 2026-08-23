using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
//using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    internal class GUI : MonoBehaviour {
        internal const string CloseTriggerName = "____bottom_right_gui";
        internal const string CloseTriggerValue = "uk.lum.followcam";
        private const int MinWidth = 426;
        private const int MaxWidth = 1920;
        private const int MinHeight = 400;
        private static int DWidth = MinWidth;
        private static int DHeight = MinHeight;
        internal static GameObject objGUI = new GameObject("FollowCam_GUI", typeof(GUI));

        internal static CameraWrangler CurrentWrangler = new CameraWrangler(Camera.main.transform, _Settings.Settings);

        internal static bool IsActive => objGUI.activeSelf;
        internal static void SetActive(bool Active) { objGUI.SetActive(Active); }
        internal static void ToggleActive() { objGUI.SetActive(!objGUI.activeSelf); }

        internal static int BoneSelector = 0;

        internal string OffsetX = "";
        internal string OffsetY = "";
        internal string OffsetZ = "";
        internal string OffsetLerp = "";
        internal string OffsetMin = "";

        internal string LookAtOffsetX = "";
        internal string LookAtOffsetY = "";
        internal string LookAtOffsetZ = "";
        internal string LookAtLerp = "";
        internal string LookAtMin = "";

        private static GUIStyle ActivateButtonStyle = new GUIStyle();
        private static GUIStyle DeactivateButtonStyle = new GUIStyle();

        private readonly HumanBodyBones[] BoneSelectorList = {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.UpperChest,
            HumanBodyBones.Head,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightHand,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightFoot,
        };
        
        void OnDisable() {
            SettingsFile.Save();
        }
        void OnEnable() {
            BoneSelector = 0;
            ReloadTempStrings();
            VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger(CloseTriggerName, 0, 0, 0, CloseTriggerValue, "", "");
        }

        void ReloadTempStrings() {
            OffsetX = CurrentWrangler.Settings.OffsetPosition.x.ToString();
            OffsetY = CurrentWrangler.Settings.OffsetPosition.y.ToString();
            OffsetZ = CurrentWrangler.Settings.OffsetPosition.z.ToString();
            OffsetLerp = CurrentWrangler.Settings.MaxMovementDistance.ToString();
            OffsetMin = CurrentWrangler.Settings.MinMovementThreshold.ToString();
            LookAtOffsetX = CurrentWrangler.Settings.LookAtOffsetPosition.x.ToString();
            LookAtOffsetY = CurrentWrangler.Settings.LookAtOffsetPosition.y.ToString();
            LookAtOffsetZ = CurrentWrangler.Settings.LookAtOffsetPosition.z.ToString();
            LookAtLerp = CurrentWrangler.Settings.MaxRotation.ToString();
            LookAtMin = CurrentWrangler.Settings.MinRotationThreshold.ToString();
        }
        
        static string FloatTextField(string Input, out float RealValue) {
            string Result = GUILayout.TextField(Input, 12, GUILayout.Width(90));
            float.TryParse(Result, out RealValue);
            return Result;
        }
        static void FloatSlider(ref string Input, ref float RealValue, float Min, float Max) {
            float TempFloat = GUILayout.HorizontalSlider(RealValue, Min, Max);
            if (TempFloat != RealValue) {
                RealValue = TempFloat;
                Input = RealValue.ToString();
            }
        }

        void OnGUI() {
            try { 
                GUILayout.BeginArea(new Rect(Screen.width - DWidth, Screen.height - DHeight, DWidth, DHeight));
                GUILayout.FlexibleSpace(); // Force bottom alignment
                if (BoneSelector == 0) {

                    GameObject AvatarObject = (GameObject)VNyanInterface.VNyanInterface.VNyanAvatar.getAvatarObject();
                    Animator AvatarAnimator = AvatarObject.GetComponent<Animator>();
                    Transform BaseBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)CurrentWrangler.Settings.BaseBone);
                    //Transform LookAtBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.LookAtBone);

                    GUILayout.BeginHorizontal();
                    if (String.IsNullOrEmpty(_Settings.GlobalSettings.LastProfileName)) {
                        GUILayout.Label("FollowCam config");
                    } else {
                        GUILayout.Label(System.IO.Path.GetFileName(_Settings.GlobalSettings.LastProfileName));
                    }
                    GUILayout.FlexibleSpace();
                    DWidth = (int)GUILayout.HorizontalSlider((float)DWidth, MinWidth, MaxWidth, GUILayout.MaxWidth(200));
                    if (GUILayout.Button(" X ")) { SetActive(false); }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    ActivateButtonStyle = new GUIStyle("button");
                    DeactivateButtonStyle = new GUIStyle("button");
                    if (CurrentWrangler.Enabled) { 
                        ActivateButtonStyle.normal.textColor  = new Color(1, 0.5f, 0.5f);
                        ActivateButtonStyle.hover.textColor   = ActivateButtonStyle.normal.textColor;
                        ActivateButtonStyle.active.textColor  = ActivateButtonStyle.normal.textColor;
                        ActivateButtonStyle.focused.textColor = ActivateButtonStyle.normal.textColor;
                    } else { 
                        DeactivateButtonStyle.normal.textColor  = new Color(1, 0.5f, 0.5f);
                        DeactivateButtonStyle.hover.textColor   = DeactivateButtonStyle.normal.textColor;
                        DeactivateButtonStyle.active.textColor  = DeactivateButtonStyle.normal.textColor;
                        DeactivateButtonStyle.focused.textColor = DeactivateButtonStyle.normal.textColor;
                    }
                    if (GUILayout.Button("Activate",   ActivateButtonStyle))   { CurrentWrangler.Enable(); }
                    if (GUILayout.Button("Deactivate", DeactivateButtonStyle)) { CurrentWrangler.Disable(); }
                    GUILayout.FlexibleSpace();
                    if (!String.IsNullOrEmpty(_Settings.GlobalSettings.LastProfileName)) {
                        if (GUILayout.Button("QLoad")) {
                            SettingsFile.Load(_Settings.GlobalSettings.LastProfileName, CurrentWrangler);
                            ReloadTempStrings();
                        }
                        if (GUILayout.Button("QSave")) {
                            SettingsFile.Save(_Settings.GlobalSettings.LastProfileName, CurrentWrangler);
                            ReloadTempStrings();
                        }
                    }

                    if (GUILayout.Button("Load")) {
                        string Result = VNyanInterface.VNyanInterface.VNyanUI.openLoadFileDialog("Load Follow Cam profile", new[] { "json" });
                        if (!string.IsNullOrEmpty(Result)) {
                            SettingsFile.Load(Result, CurrentWrangler);
                            ReloadTempStrings();
                        }
                    }

                    if (GUILayout.Button("Save")) {
                        string Result = VNyanInterface.VNyanInterface.VNyanUI.openSaveFileDialog("Save Follow Cam profile as...", new[] { "json" });
                        if (!string.IsNullOrEmpty(Result)) {
                            SettingsFile.Save(Result, CurrentWrangler);
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(); {
                        GUILayout.BeginVertical(); {
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label("Camera Offset");
                                if (GUILayout.Button(CurrentWrangler.Settings.BaseBone.ToString())) { BoneSelector = 1; }
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"X: ");
                                OffsetX = FloatTextField(OffsetX, out CurrentWrangler.Settings.OffsetPosition.x);
                                CurrentWrangler.Settings.StaticX = GUILayout.Toggle(CurrentWrangler.Settings.StaticX, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetX, ref CurrentWrangler.Settings.OffsetPosition.x, -10, 10);
                            //Settings.OffsetPosition.x = GUILayout.HorizontalSlider(Settings.OffsetPosition.x, -10, 10);
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Y: ");
                                OffsetY = FloatTextField(OffsetY, out CurrentWrangler.Settings.OffsetPosition.y);
                                CurrentWrangler.Settings.StaticY = GUILayout.Toggle(CurrentWrangler.Settings.StaticY, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetY, ref CurrentWrangler.Settings.OffsetPosition.y, -10, 10);
                            //Settings.OffsetPosition.y = GUILayout.HorizontalSlider(Settings.OffsetPosition.y, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Z: ");
                                OffsetZ = FloatTextField(OffsetZ, out CurrentWrangler.Settings.OffsetPosition.z);
                                CurrentWrangler.Settings.StaticZ = GUILayout.Toggle(CurrentWrangler.Settings.StaticZ, "Static");
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetZ, ref CurrentWrangler.Settings.OffsetPosition.z, -10, 10);
                            //Settings.OffsetPosition.z = GUILayout.HorizontalSlider(Settings.OffsetPosition.z, -10, 10);

                            GUILayout.BeginHorizontal(); {
                                if (GUILayout.Toggle((CurrentWrangler.Settings.OffsetMode == CameraPosMode.Off),      "Off"))      { CurrentWrangler.Settings.OffsetMode = CameraPosMode.Off; }
                                if (GUILayout.Toggle((CurrentWrangler.Settings.OffsetMode == CameraPosMode.Absolute), "Absolute")) { CurrentWrangler.Settings.OffsetMode = CameraPosMode.Absolute; }
                                if (GUILayout.Toggle((CurrentWrangler.Settings.OffsetMode == CameraPosMode.Relative), "Relative")) { CurrentWrangler.Settings.OffsetMode = CameraPosMode.Relative; }
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Movement Lerp: ");
                                OffsetLerp = FloatTextField(OffsetLerp, out CurrentWrangler.Settings.MaxMovementDistance);
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();

                            FloatSlider(ref OffsetLerp, ref CurrentWrangler.Settings.MaxMovementDistance, 0, 0.05f);
                            //Settings.MaxMovementDistance = GUILayout.HorizontalSlider(Settings.MaxMovementDistance, 0, 0.01f);

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Min Move Thresh: ");
                                OffsetMin = FloatTextField(OffsetMin, out CurrentWrangler.Settings.MinMovementThreshold);
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetMin, ref CurrentWrangler.Settings.MinMovementThreshold, 0, 1);
                            //Settings.MinMovementThreshold = GUILayout.HorizontalSlider(Settings.MinMovementThreshold, 0, 1);

                            GUILayout.Label($"FCam: {CurrentWrangler.CurrentCamera.position.ToString()}");
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"VCam: {CurrentWrangler.CurrentCamera.position.ToString()}");
                                if (!FollowCam.objMainCamera.Enabled) {
                                    if (GUILayout.Button("Copy")) {
                                        Transform CopyBaseBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)CurrentWrangler.Settings.BaseBone);
                                        if (CurrentWrangler.Settings.StaticX) {
                                            CurrentWrangler.Settings.Offset_X = CurrentWrangler.CurrentCamera.position.x; 
                                        } else {
                                            CurrentWrangler.Settings.Offset_X = CurrentWrangler.CurrentCamera.position.x - CopyBaseBoneTransform.position.x;
                                        }
                                        if (CurrentWrangler.Settings.StaticY) {
                                            CurrentWrangler.Settings.Offset_Y = CurrentWrangler.CurrentCamera.position.y;
                                        } else {
                                            CurrentWrangler.Settings.Offset_Y = CurrentWrangler.CurrentCamera.position.y - CopyBaseBoneTransform.position.y;
                                        }
                                        if (CurrentWrangler.Settings.StaticZ) {
                                            CurrentWrangler.Settings.Offset_Z = CurrentWrangler.CurrentCamera.position.z;
                                        } else {
                                            CurrentWrangler.Settings.Offset_Z = CurrentWrangler.CurrentCamera.position.z - CopyBaseBoneTransform.position.z;
                                        }
                                        ReloadTempStrings();
                                    }
                                }
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            GUILayout.Label($"Trg: {CurrentWrangler.CurrentCamera.position.ToString()}");
                            GUILayout.Label($"Bone: {BaseBoneTransform.position.ToString()}");
                        }
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(); {
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label("Look at bone");
                                if (GUILayout.Button(CurrentWrangler.Settings.LookAtBone.ToString())) { BoneSelector = 2; }
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"X: ");
                                LookAtOffsetX = FloatTextField(LookAtOffsetX, out CurrentWrangler.Settings.LookAtOffsetPosition.x);
                                CurrentWrangler.Settings.LookAtStaticX = GUILayout.Toggle(CurrentWrangler.Settings.LookAtStaticX, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetX, ref CurrentWrangler.Settings.LookAtOffsetPosition.x, -10, 10);
                            //Settings.LookAtOffsetPosition.x = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.x, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Y: ");
                                LookAtOffsetY = FloatTextField(LookAtOffsetY, out CurrentWrangler.Settings.LookAtOffsetPosition.y);
                                CurrentWrangler.Settings.LookAtStaticY = GUILayout.Toggle(CurrentWrangler.Settings.LookAtStaticY, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetY, ref CurrentWrangler.Settings.LookAtOffsetPosition.y, -10, 10);
                            //Settings.LookAtOffsetPosition.y = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.y, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Z: ");
                                LookAtOffsetZ = FloatTextField(LookAtOffsetZ, out CurrentWrangler.Settings.LookAtOffsetPosition.z);
                                CurrentWrangler.Settings.LookAtStaticZ = GUILayout.Toggle(CurrentWrangler.Settings.LookAtStaticZ, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetZ, ref CurrentWrangler.Settings.LookAtOffsetPosition.z, -10, 10);
                            //Settings.LookAtOffsetPosition.z = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.z, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Toggle((CurrentWrangler.Settings.RotationMode == CameraPosMode.Off),      "Off"))      { CurrentWrangler.Settings.RotationMode = CameraPosMode.Off; }
                                if (GUILayout.Toggle((CurrentWrangler.Settings.RotationMode == CameraPosMode.Absolute), "Absolute")) { CurrentWrangler.Settings.RotationMode = CameraPosMode.Absolute; }
                                if (GUILayout.Toggle((CurrentWrangler.Settings.RotationMode == CameraPosMode.Relative), "Relative")) { CurrentWrangler.Settings.RotationMode = CameraPosMode.Relative; }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Rotation Lerp: ");
                                LookAtLerp = FloatTextField(LookAtLerp, out CurrentWrangler.Settings.MaxRotation);
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtLerp, ref CurrentWrangler.Settings.MaxRotation, 0, 0.1f);
                            //Settings.MaxRotation = GUILayout.HorizontalSlider(Settings.MaxRotation, 0, 1);

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Min rotation thresh: ");
                                LookAtMin = FloatTextField(LookAtMin, out CurrentWrangler.Settings.MinRotationThreshold);
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtMin, ref CurrentWrangler.Settings.MinRotationThreshold, 0, 360);
                            //Settings.MinRotationThreshold = GUILayout.HorizontalSlider(Settings.MinRotationThreshold, 0, 360);

                            GUILayout.Label(CurrentWrangler.CurrentCamera.rotation.eulerAngles.ToString());
                            GUILayout.BeginHorizontal(); {

                                GUILayout.Label(CurrentWrangler.CurrentCamera.rotation.eulerAngles.ToString());
                                if (!FollowCam.objMainCamera.Enabled) {
                                    if (GUILayout.Button("Copy")) {
                                        Transform LookAtBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)CurrentWrangler.Settings.LookAtBone);
                                        float BoneZ = LookAtBoneTransform.position.z;
                                        float HitPoint;
                                        Ray CameraRay = new Ray(CurrentWrangler.CurrentCamera.position, CurrentWrangler.CurrentCamera.forward);
                                        Plane CollisionPlane = new Plane(new Vector3(0, 0, BoneZ), new Vector3(1, 1, BoneZ), new Vector3(0, 1, BoneZ));
                                        CollisionPlane.Raycast(CameraRay, out HitPoint);
                                        Vector3 GlobalLookAtPos = CameraRay.GetPoint(HitPoint);
                                        if (CurrentWrangler.Settings.LookAtStaticX) {
                                            CurrentWrangler.Settings.LookAtOffset_X = GlobalLookAtPos.x;
                                        } else {
                                            CurrentWrangler.Settings.LookAtOffset_X = GlobalLookAtPos.x - LookAtBoneTransform.position.x;
                                        }
                                        if (CurrentWrangler.Settings.LookAtStaticY) {
                                            CurrentWrangler.Settings.LookAtOffset_Y = GlobalLookAtPos.y;
                                        } else {
                                            CurrentWrangler.Settings.LookAtOffset_Y = GlobalLookAtPos.y - LookAtBoneTransform.position.y;
                                        }
                                        ReloadTempStrings();
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                            } GUILayout.EndHorizontal();
                            GUILayout.Label("");
                            GUILayout.Label(CurrentWrangler.Persist_LookAtTrgPos.ToString());
                            
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();

                } else {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select a bone");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(" X ")) { BoneSelector = 0; }
                    GUILayout.EndHorizontal();

                    foreach (HumanBodyBones Bone in BoneSelectorList) {
                        if (GUILayout.Button(Bone.ToString())) {
                            switch (BoneSelector) {
                                case 1:
                                    CurrentWrangler.Settings.BaseBone = Bone;
                                    break;
                                case 2:
                                    CurrentWrangler.Settings.LookAtBone = Bone;
                                    break;
                            }
                            BoneSelector = 0;
                        }
                    }
                }

                    GUILayout.EndArea();
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
    }
}
