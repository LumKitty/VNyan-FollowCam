using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {
    internal class GUI : MonoBehaviour {
        internal const string CloseTriggerName = "____bottom_right_gui";
        internal const string CloseTriggerValue = "uk.lum.followcam";
        private const int MinWidth = 426;
        private const int MaxWidth = 1920;
        private const int MinHeight = 400;
        private static int DWidth = MinWidth;
        private static int DHeight = MinHeight;
        private static GameObject objGUI = new GameObject("FollowCam_GUI", typeof(GUI));

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
            OffsetX = Settings.OffsetPosition.x.ToString();
            OffsetY = Settings.OffsetPosition.y.ToString();
            OffsetZ = Settings.OffsetPosition.z.ToString();
            OffsetLerp = Settings.MaxMovementDistance.ToString();
            OffsetMin = Settings.MinMovementThreshold.ToString();
            LookAtOffsetX = Settings.LookAtOffsetPosition.x.ToString();
            LookAtOffsetY = Settings.LookAtOffsetPosition.y.ToString();
            LookAtOffsetZ = Settings.LookAtOffsetPosition.z.ToString();
            LookAtLerp = Settings.MaxRotation.ToString();
            LookAtMin = Settings.MinRotationThreshold.ToString();
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
                    Transform BaseBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.BaseBone);
                    //Transform LookAtBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.LookAtBone);

                    GUILayout.BeginHorizontal();
                    if (String.IsNullOrEmpty(GlobalSettings.LastProfileName)) {
                        GUILayout.Label("FollowCam config");
                    } else {
                        GUILayout.Label(System.IO.Path.GetFileName(GlobalSettings.LastProfileName));
                    }
                    GUILayout.FlexibleSpace();
                    DWidth = (int)GUILayout.HorizontalSlider((float)DWidth, MinWidth, MaxWidth, GUILayout.MaxWidth(200));
                    if (GUILayout.Button(" X ")) { SetActive(false); }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    ActivateButtonStyle = new GUIStyle("button");
                    DeactivateButtonStyle = new GUIStyle("button");
                    if (FollowCam.IsActive) { 
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
                    if (GUILayout.Button("Activate",   ActivateButtonStyle))   { FollowCam.SetActive(true); }
                    if (GUILayout.Button("Deactivate", DeactivateButtonStyle)) { FollowCam.SetActive(false); }
                    GUILayout.FlexibleSpace();
                    if (!String.IsNullOrEmpty(GlobalSettings.LastProfileName)) {
                        if (GUILayout.Button("QLoad")) {
                            SettingsFile.Load(GlobalSettings.LastProfileName);
                            ReloadTempStrings();
                        }
                        if (GUILayout.Button("QSave")) {
                            SettingsFile.Save(GlobalSettings.LastProfileName);
                            ReloadTempStrings();
                        }
                    }

                    if (GUILayout.Button("Load")) {
                        string Result = VNyanInterface.VNyanInterface.VNyanUI.openLoadFileDialog("Load Follow Cam profile", new[] { "json" });
                        if (!string.IsNullOrEmpty(Result)) {
                            SettingsFile.Load(Result);
                            ReloadTempStrings();
                        }
                    }

                    if (GUILayout.Button("Save")) {
                        string Result = VNyanInterface.VNyanInterface.VNyanUI.openSaveFileDialog("Save Follow Cam profile as...", new[] { "json" });
                        if (!string.IsNullOrEmpty(Result)) {
                            SettingsFile.Save(Result);
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(); {
                        GUILayout.BeginVertical(); {
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label("Camera Offset");
                                if (GUILayout.Button(Settings.BaseBone.ToString())) { BoneSelector = 1; }
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"X: ");
                                OffsetX = FloatTextField(OffsetX, out Settings.OffsetPosition.x);
                                Settings.StaticX = GUILayout.Toggle(Settings.StaticX, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetX, ref Settings.OffsetPosition.x, -10, 10);
                            //Settings.OffsetPosition.x = GUILayout.HorizontalSlider(Settings.OffsetPosition.x, -10, 10);
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Y: ");
                                OffsetY = FloatTextField(OffsetY, out Settings.OffsetPosition.y);
                                Settings.StaticY = GUILayout.Toggle(Settings.StaticY, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetY, ref Settings.OffsetPosition.y, -10, 10);
                            //Settings.OffsetPosition.y = GUILayout.HorizontalSlider(Settings.OffsetPosition.y, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Z: ");
                                OffsetZ = FloatTextField(OffsetZ, out Settings.OffsetPosition.z);
                                Settings.StaticZ = GUILayout.Toggle(Settings.StaticZ, "Static");
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetZ, ref Settings.OffsetPosition.z, -10, 10);
                            //Settings.OffsetPosition.z = GUILayout.HorizontalSlider(Settings.OffsetPosition.z, -10, 10);

                            GUILayout.BeginHorizontal(); {
                                if (GUILayout.Toggle((Settings.OffsetMode == CameraPosMode.Off),      "Off"))      { Settings.OffsetMode = CameraPosMode.Off; }
                                if (GUILayout.Toggle((Settings.OffsetMode == CameraPosMode.Absolute), "Absolute")) { Settings.OffsetMode = CameraPosMode.Absolute; }
                                if (GUILayout.Toggle((Settings.OffsetMode == CameraPosMode.Relative), "Relative")) { Settings.OffsetMode = CameraPosMode.Relative; }
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Movement Lerp: ");
                                OffsetLerp = FloatTextField(OffsetLerp, out Settings.MaxMovementDistance);
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();

                            FloatSlider(ref OffsetLerp, ref Settings.MaxMovementDistance, 0, 0.05f);
                            //Settings.MaxMovementDistance = GUILayout.HorizontalSlider(Settings.MaxMovementDistance, 0, 0.01f);

                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"Min Move Thresh: ");
                                OffsetMin = FloatTextField(OffsetMin, out Settings.MinMovementThreshold);
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            FloatSlider(ref OffsetMin, ref Settings.MinMovementThreshold, 0, 1);
                            //Settings.MinMovementThreshold = GUILayout.HorizontalSlider(Settings.MinMovementThreshold, 0, 1);

                            GUILayout.Label($"FCam: {Persist.PrevPos.ToString()}");
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label($"VCam: {Camera.main.transform.position.ToString()}");
                                if (!FollowCam.IsActive) {
                                    if (GUILayout.Button("Copy")) {
                                        Transform CopyBaseBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.BaseBone);
                                        if (Settings.StaticX) {
                                            Settings.Offset_X = Camera.main.transform.position.x; 
                                        } else {
                                            Settings.Offset_X = Camera.main.transform.position.x - CopyBaseBoneTransform.position.x;
                                        }
                                        if (Settings.StaticY) {
                                            Settings.Offset_Y = Camera.main.transform.position.y;
                                        } else {
                                            Settings.Offset_Y = Camera.main.transform.position.y - CopyBaseBoneTransform.position.y;
                                        }
                                        if (Settings.StaticZ) {
                                            Settings.Offset_Z = Camera.main.transform.position.z;
                                        } else {
                                            Settings.Offset_Z = Camera.main.transform.position.z - CopyBaseBoneTransform.position.z;
                                        }
                                        ReloadTempStrings();
                                    }
                                }
                                GUILayout.FlexibleSpace();
                            } GUILayout.EndHorizontal();
                            GUILayout.Label($"Trg: {Persist.TrgPos.ToString()}");
                            GUILayout.Label($"Bone: {BaseBoneTransform.position.ToString()}");
                        }
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(); {
                            GUILayout.BeginHorizontal(); {
                                GUILayout.Label("Look at bone");
                                if (GUILayout.Button(Settings.LookAtBone.ToString())) { BoneSelector = 2; }
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"X: ");
                                LookAtOffsetX = FloatTextField(LookAtOffsetX, out Settings.LookAtOffsetPosition.x);
                                Settings.LookAtStaticX = GUILayout.Toggle(Settings.LookAtStaticX, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetX, ref Settings.LookAtOffsetPosition.x, -10, 10);
                            //Settings.LookAtOffsetPosition.x = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.x, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Y: ");
                                LookAtOffsetY = FloatTextField(LookAtOffsetY, out Settings.LookAtOffsetPosition.y);
                                Settings.LookAtStaticY = GUILayout.Toggle(Settings.LookAtStaticY, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetY, ref Settings.LookAtOffsetPosition.y, -10, 10);
                            //Settings.LookAtOffsetPosition.y = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.y, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Z: ");
                                LookAtOffsetZ = FloatTextField(LookAtOffsetZ, out Settings.LookAtOffsetPosition.z);
                                Settings.LookAtStaticZ = GUILayout.Toggle(Settings.LookAtStaticZ, "Static");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtOffsetZ, ref Settings.LookAtOffsetPosition.z, -10, 10);
                            //Settings.LookAtOffsetPosition.z = GUILayout.HorizontalSlider(Settings.LookAtOffsetPosition.z, -10, 10);
                            GUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Toggle((Settings.RotationMode == CameraPosMode.Off),      "Off"))      { Settings.RotationMode = CameraPosMode.Off; }
                                if (GUILayout.Toggle((Settings.RotationMode == CameraPosMode.Absolute), "Absolute")) { Settings.RotationMode = CameraPosMode.Absolute; }
                                if (GUILayout.Toggle((Settings.RotationMode == CameraPosMode.Relative), "Relative")) { Settings.RotationMode = CameraPosMode.Relative; }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Rotation Lerp: ");
                                LookAtLerp = FloatTextField(LookAtLerp, out Settings.MaxRotation);
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtLerp, ref Settings.MaxRotation, 0, 0.1f);
                            //Settings.MaxRotation = GUILayout.HorizontalSlider(Settings.MaxRotation, 0, 1);

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label($"Min rotation thresh: ");
                                LookAtMin = FloatTextField(LookAtMin, out Settings.MinRotationThreshold);
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            FloatSlider(ref LookAtMin, ref Settings.MinRotationThreshold, 0, 360);
                            //Settings.MinRotationThreshold = GUILayout.HorizontalSlider(Settings.MinRotationThreshold, 0, 360);

                            GUILayout.Label(Persist.PrevRot.eulerAngles.ToString());
                            GUILayout.BeginHorizontal(); {

                                GUILayout.Label(Camera.main.transform.rotation.eulerAngles.ToString());
                                if (!FollowCam.IsActive) {
                                    if (GUILayout.Button("Copy")) {
                                        Transform LookAtBoneTransform = AvatarAnimator.GetBoneTransform((HumanBodyBones)Settings.LookAtBone);
                                        float BoneZ = LookAtBoneTransform.position.z;
                                        float HitPoint;
                                        Ray CameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                                        Plane CollisionPlane = new Plane(new Vector3(0, 0, BoneZ), new Vector3(1, 1, BoneZ), new Vector3(0, 1, BoneZ));
                                        CollisionPlane.Raycast(CameraRay, out HitPoint);
                                        Vector3 GlobalLookAtPos = CameraRay.GetPoint(HitPoint);
                                        if (Settings.LookAtStaticX) {
                                            Settings.LookAtOffset_X = GlobalLookAtPos.x;
                                        } else {
                                            Settings.LookAtOffset_X = GlobalLookAtPos.x - LookAtBoneTransform.position.x;
                                        }
                                        if (Settings.LookAtStaticY) {
                                            Settings.LookAtOffset_Y = GlobalLookAtPos.y;
                                        } else {
                                            Settings.LookAtOffset_Y = GlobalLookAtPos.y - LookAtBoneTransform.position.y;
                                        }
                                        ReloadTempStrings();
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                            } GUILayout.EndHorizontal();
                            GUILayout.Label("");
                            GUILayout.Label(Persist.LookAtTrgPos.ToString());
                            
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
                                    Settings.BaseBone = Bone;
                                    break;
                                case 2:
                                    Settings.LookAtBone = Bone;
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
