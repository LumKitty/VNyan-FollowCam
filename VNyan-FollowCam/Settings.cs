using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VNyan_FollowCam {

    public enum CameraPosMode {
        Off = 0,
        Absolute = 1,
        Relative = 2
    }

    internal class __GlobalSettings {
        internal static string SettingsFileName = VNyanInterface.VNyanInterface.VNyanSettings.getProfilePath() + "\\FollowCam.json";
        public string MainCameraSettingsFile = VNyanInterface.VNyanInterface.VNyanSettings.getProfilePath() + "\\FollowCam-MainCam.json";
        internal int LogLevel = 4;

    }
    
    public class __Settings {
        [JsonIgnore] public HumanBodyBones BaseBone;
        [JsonIgnore] public HumanBodyBones LookAtBone;
        public string CameraBoneBase { get { return BaseBone.ToString();   } set { Enum.TryParse<HumanBodyBones>(value, out BaseBone); } }
        public string LookAt         { get { return LookAtBone.ToString(); } set { Enum.TryParse<HumanBodyBones>(value, out LookAtBone); } }

        [JsonIgnore] public Vector3 OffsetPosition = new Vector3();
        public float Offset_X { get { return OffsetPosition.x; } set { OffsetPosition.x = value; } }
        public float Offset_Y { get { return OffsetPosition.y; } set { OffsetPosition.y = value; } }
        public float Offset_Z { get { return OffsetPosition.z; } set { OffsetPosition.z = value; } }
        public bool StaticX;
        public bool StaticY;
        public bool StaticZ;
        public CameraPosMode OffsetMode = CameraPosMode.Relative;

        public float MovementLerp;
        
        public float MinMovementThreshold;

        [JsonIgnore] public Vector3 LookAtOffsetPosition = new Vector3();
        public float LookAtOffset_X { get { return LookAtOffsetPosition.x; } set { LookAtOffsetPosition.x = value; } }
        public float LookAtOffset_Y { get { return LookAtOffsetPosition.y; } set { LookAtOffsetPosition.y = value; } }
        public float LookAtOffset_Z { get { return LookAtOffsetPosition.z; } set { LookAtOffsetPosition.z = value; } }
        public bool LookAtStaticX;
        public bool LookAtStaticY;
        public bool LookAtStaticZ;
        public CameraPosMode RotationMode = CameraPosMode.Relative;

        public float RotationLerp;
        public float MinRotationThreshold;

        public float MaxMovementDistance; // Obsolete, remove before final release
        public float MaxRotation; // Obsolete, remove before final release
    }

    internal static class _Settings {
        //internal static __Settings Settings = new __Settings();
        internal static __GlobalSettings GlobalSettings = new __GlobalSettings();
    }

    internal static class SettingsFile {

        internal static bool Load(string FileName, CameraWrangler CurrentWrangler, bool UpdateLastProfile = true) {
            try {
                if (File.Exists(FileName)) {
                    VNyan_Handlers.Log($"Loading {FileName}");
                    __Settings? TempSettings = JsonConvert.DeserializeObject<__Settings>(File.ReadAllText(FileName));
                    if (TempSettings != null) {
                        bool GUIStatus = GUI.IsActive;
                        if (GUIStatus) { GUI.SetActive(false); }
                        if (TempSettings.MovementLerp == 0) { TempSettings.MovementLerp = TempSettings.MaxMovementDistance * 60; } // Obsolete, remove before final release
                        if (TempSettings.RotationLerp == 0) { TempSettings.RotationLerp = TempSettings.MaxRotation * 60; } // Obsolete, remove before final release
                        CurrentWrangler.Settings = TempSettings;
                        if (UpdateLastProfile) { CurrentWrangler.SettingsFileName = FileName; }
                        GUI.SetActive(GUIStatus);
                        return true;
                    } else {
                        VNyan_Handlers.Log($"Invalid settings file: {FileName}");
                    }
                } else {
                    VNyan_Handlers.Log($"Could not find {FileName}");
                }
                return false;
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
                return false;
            }
        }
        internal static void Save(string FileName, CameraWrangler CurrentWrangler, bool UpdateLastProfile = true) {
            try {
                VNyan_Handlers.Log($"Saving to {FileName}");
                File.WriteAllText(FileName, JsonConvert.SerializeObject(CurrentWrangler.Settings, Formatting.Indented));
                if (UpdateLastProfile) { CurrentWrangler.SettingsFileName = FileName; }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        internal static void SaveGlobal() {
            try {
                VNyan_Handlers.Log($"Saving to {__GlobalSettings.SettingsFileName}");
                File.WriteAllText(__GlobalSettings.SettingsFileName, JsonConvert.SerializeObject(_Settings.GlobalSettings, Formatting.Indented));
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        internal static bool LoadGlobal() {
            try {
                if (File.Exists(__GlobalSettings.SettingsFileName)) {
                    VNyan_Handlers.Log($"Loading {__GlobalSettings.SettingsFileName}");
                    __GlobalSettings? TempSettings = JsonConvert.DeserializeObject<__GlobalSettings>(File.ReadAllText(__GlobalSettings.SettingsFileName));
                    if (TempSettings != null) {
                        _Settings.GlobalSettings = TempSettings;
                    } else {
                        VNyan_Handlers.Log($"Invalid settings file: {__GlobalSettings.SettingsFileName}");
                    }
                } else {
                    VNyan_Handlers.Log($"Could not find {__GlobalSettings.SettingsFileName}");
                }
                return false;
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
                return false;
            }
        }
    }
}
