using System;
using System.IO;
using System.Xml.Linq;
using VNyanInterface;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {

    public class VNyan_Handlers : IVNyanPluginManifest, IButtonClickedHandler, ITriggerHandler {
        public string PluginName { get; } = "VNyan FollowCam";
        public string Version { get; } = "0.1";
        public string Title => PluginName + " " + Version;
        public string Author { get; } = "LumKitty";
        public string Website { get; } = "https://lum.uk/";

        internal static void Log(string Message) {
            UnityEngine.Debug.Log($"[FollowCam] {Message}");
        }

        public void InitializePlugin() {
            Settings.BaseBone = UnityEngine.HumanBodyBones.Hips;
            Settings.LookAtBone = UnityEngine.HumanBodyBones.Head;
            FollowCam.SetActive(false);
            SettingsFile.Load();
            GUI.SetActive(false);
            VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton("FollowCam", this);
            VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(this);
        }

        public void triggerCalled(string name, int int1, int int2, int int3, string text1, string text2, string text3) {
            try {
                if (name == GUI.CloseTriggerName && text1 != GUI.CloseTriggerValue) { GUI.SetActive(false); }
                if (name.Length > 16) {
                    name = name.ToLower();
                    if (name.Substring(0, 15) == "_lum_followcam_") {
                        Log("Detected trigger: " + name);
                        name = name.Substring(14);
                    } else {
                        return;
                    }
                    switch (name) {
                        case "_enable": FollowCam.SetActive(true); break;
                        case "_disable": FollowCam.SetActive(false); break;
                        case "_offsetoff": Settings.OffsetMode = CameraPosMode.Off; break;
                        case "_offsetabs": Settings.OffsetMode = CameraPosMode.Absolute; break;
                        case "_offsetrel": Settings.OffsetMode = CameraPosMode.Relative; break;
                        case "_rotationoff": Settings.RotationMode = CameraPosMode.Off; break;
                        case "_rotationabs": Settings.RotationMode = CameraPosMode.Absolute; break;
                        case "_rotationrel": Settings.RotationMode = CameraPosMode.Relative; break;
                        case "_load":
                            if (File.Exists(text1)) {
                                SettingsFile.Load(text1);
                                if (int1 == 1) {
                                    FollowCam.SetActive(true);
                                }
                            }
                            break;
                    }
                }
            } catch (Exception ex) {
                Log("ERR: " + ex.ToString());
            }
        }

        public void pluginButtonClicked() {
            GUI.ToggleActive();
        }
    }
}
