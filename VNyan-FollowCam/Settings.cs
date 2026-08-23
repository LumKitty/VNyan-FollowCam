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
        public string LastProfileName = "";
    }
    
    public class __Settings {
        internal HumanBodyBones BaseBone;
        internal HumanBodyBones LookAtBone;
        public string CameraBoneBase { get { return BaseBone.ToString(); } set { Enum.TryParse<HumanBodyBones>(value, out BaseBone); } }
        public string LookAt { get { return LookAtBone.ToString(); } set { Enum.TryParse<HumanBodyBones>(value, out LookAtBone); } }

        internal Vector3 OffsetPosition = new Vector3();
        public float Offset_X { get { return OffsetPosition.x; } set { OffsetPosition.x = value; } }
        public float Offset_Y { get { return OffsetPosition.y; } set { OffsetPosition.y = value; } }
        public float Offset_Z { get { return OffsetPosition.z; } set { OffsetPosition.z = value; } }
        public bool StaticX;
        public bool StaticY;
        public bool StaticZ;
        public CameraPosMode OffsetMode = CameraPosMode.Relative;

        public float MaxMovementDistance;
        public float MinMovementThreshold;

        internal Vector3 LookAtOffsetPosition = new Vector3();
        public float LookAtOffset_X { get { return LookAtOffsetPosition.x; } set { LookAtOffsetPosition.x = value; } }
        public float LookAtOffset_Y { get { return LookAtOffsetPosition.y; } set { LookAtOffsetPosition.y = value; } }
        public float LookAtOffset_Z { get { return LookAtOffsetPosition.z; } set { LookAtOffsetPosition.z = value; } }
        public bool LookAtStaticX;
        public bool LookAtStaticY;
        public bool LookAtStaticZ;
        public CameraPosMode RotationMode = CameraPosMode.Relative;

        public float MaxRotation;
        public float MinRotationThreshold;
    }

    internal static class _Settings {
        internal static __Settings Settings = new __Settings();
        internal static __GlobalSettings GlobalSettings = new __GlobalSettings();
    }

    internal static class SettingsFile {
        internal static readonly String SettingsFilename = VNyanInterface.VNyanInterface.VNyanSettings.getProfilePath() + "\\FollowCam.json";
        
        internal static void Load(string FileName, CameraWrangler CurrentCameraWrangler, bool UpdateLastProfile = true) {
            try {
                if (File.Exists(FileName)) {
                    VNyan_Handlers.Log($"Loading {FileName}");
                    __Settings? TempSettings = JsonConvert.DeserializeObject<__Settings>(File.ReadAllText(FileName));
                    if (TempSettings != null) {
                        bool GUIStatus = GUI.IsActive;
                        if (GUIStatus) { GUI.SetActive(false); }
                        CurrentCameraWrangler.Settings = TempSettings;
                        if (UpdateLastProfile) { _Settings.GlobalSettings.LastProfileName = FileName; }
                        GUI.SetActive(GUIStatus);
                    }
                } else {
                    VNyan_Handlers.Log($"Could not find {FileName}");
                }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
        internal static void Save(string FileName, CameraWrangler CurrentCameraWrangler, bool UpdateLastProfile = true) {
            try {
                VNyan_Handlers.Log($"Saving to {FileName}");
                File.WriteAllText(FileName, JsonConvert.SerializeObject(CurrentCameraWrangler.Settings,Formatting.Indented));
                if (UpdateLastProfile) { _Settings.GlobalSettings.LastProfileName = FileName; }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        internal static void Load() { Load(SettingsFilename, FollowCam.objMainCamera, false); }
        internal static void Save() { Save(SettingsFilename, FollowCam.objMainCamera); }
    }
}
