using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VNyan_FollowCam {

    enum CameraPosMode {
        Off = 0,
        Absolute = 1,
        Relative = 2
    }

    internal class __Settings {
        internal HumanBodyBones BaseBone;
        internal HumanBodyBones LookAtBone;
        public string CameraBoneBase { get { return BaseBone.ToString(); } set { Enum.TryParse<HumanBodyBones>(value, out BaseBone); } }
        public string LookAt { get { return LookAtBone.ToString(); } set { Enum.TryParse<HumanBodyBones>(value, out LookAtBone); } }

        internal Vector3 OffsetPosition = new Vector3();
        public float Offset_X { get { return OffsetPosition.x; } set { OffsetPosition.x = value; } }
        public float Offset_Y { get { return OffsetPosition.y; } set { OffsetPosition.y = value; } }
        public float Offset_Z { get { return OffsetPosition.z; } set { OffsetPosition.z = value; } }
        public CameraPosMode OffsetMode = CameraPosMode.Relative;

        public float MaxMovementDistance;
        public float MinMovementThreshold;

        internal Vector3 LookAtOffsetPosition = new Vector3();
        public float LookAtOffset_X { get { return LookAtOffsetPosition.x; } set { LookAtOffsetPosition.x = value; } }
        public float LookAtOffset_Y { get { return LookAtOffsetPosition.y; } set { LookAtOffsetPosition.y = value; } }
        public float LookAtOffset_Z { get { return LookAtOffsetPosition.z; } set { LookAtOffsetPosition.z = value; } }
        public CameraPosMode RotationMode = CameraPosMode.Relative;

        public float MaxRotation;
        public float MinRotationThreshold;
    }

    internal static class _Settings {
        internal static __Settings Settings = new __Settings();
    }

    internal static class SettingsFile {
        private static readonly String SettingsFilename = VNyanInterface.VNyanInterface.VNyanSettings.getProfilePath() + "\\FollowCam.json";
        internal static void Load(string FileName) {
            try {
                if (File.Exists(FileName)) {
                    VNyan_Handlers.Log($"Loading {FileName}");
                    _Settings.Settings = JsonConvert.DeserializeObject<__Settings>(File.ReadAllText(FileName));
                } else {
                    VNyan_Handlers.Log($"Could not find {FileName}");
                }
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }
        internal static void Save(string FileName) {
            try {
                VNyan_Handlers.Log($"Saving to {FileName}");
                File.WriteAllText(FileName, JsonConvert.SerializeObject(_Settings.Settings));
            } catch (Exception ex) {
                VNyan_Handlers.Log(ex.ToString());
            }
        }

        internal static void Load() { Load(SettingsFilename); }
        internal static void Save() { Save(SettingsFilename); }
    }
}
