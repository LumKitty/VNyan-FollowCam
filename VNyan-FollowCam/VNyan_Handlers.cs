using System;
using System.IO;
using System.Xml.Linq;
using VNyanInterface;
using static VNyan_FollowCam._Settings;

namespace VNyan_FollowCam {

    public class VNyan_Handlers : IVNyanPluginManifest, IButtonClickedHandler, ITriggerHandler {
        public string PluginName { get; } = "VNyan FollowCam";
        public string Version { get; } = "0.4-beta";
        public string Title => PluginName + " " + Version;
        public string Author { get; } = "LumKitty";
        public string Website { get; } = "https://lum.uk/";

        internal static System.Reflection.MethodInfo? _VRnyan_EnableFollowCam;
        internal static System.Reflection.MethodInfo? _VRnyan_DisableFollowCam;
        internal static bool VRnyanConnectionAttempted = false;

        internal static void Log(string Message, int LogLevel = 1) {
            if (LogLevel <= _Settings.GlobalSettings.LogLevel) {
                UnityEngine.Debug.Log($"[FollowCam] {Message}");
            }
        }

        internal static void ConnectVRnyan() {
            Log("Looking for VRnyan");
            var type = Type.GetType("VRnyan.VRnyan, VRnyan", throwOnError: false);
            if (type != null) {
                Log("Found VRnyan, getting methods");
                _VRnyan_EnableFollowCam = type.GetMethod("EnableFollowCam");
                _VRnyan_DisableFollowCam = type.GetMethod("DisableFollowCam");
                if (_VRnyan_EnableFollowCam == null || _VRnyan_DisableFollowCam == null) {
                    Log("Couldn't find position methods");
                } else {
                    Log("Got methods");
                }
            } else {
                Log("Did not find followcam assembly");
            }
        }

        public void InitializePlugin() {
            SettingsFile.LoadGlobal();
            GUI.SetActive(false);
            FollowCam.objCameras.Add(new MainCamera(_Settings.GlobalSettings.MainCameraSettingsFile));
            //GUI.CurrentWrangler = FollowCam.objCameras[0].Wrangler;
            VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton("FollowCam", this);
            VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(this);
            Log($"Assembly name: {typeof(FollowCam).AssemblyQualifiedName}");
            ConnectVRnyan();
        }

        internal void VRnyan_EnableFollowCam() {
            _VRnyan_EnableFollowCam?.Invoke(null, null);
        }
        internal void VRnyan_DisableFollowCam() {
            _VRnyan_EnableFollowCam?.Invoke(null, null);
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
                        case "_enable": FollowCam.objCameras[0].Wrangler.Enable(); break;
                        case "_disable": FollowCam.objCameras[0].Wrangler.Disable(); break;
                        case "_offsetoff": FollowCam.objCameras[0].Wrangler.Settings.OffsetMode = CameraPosMode.Off; break;
                        case "_offsetabs": FollowCam.objCameras[0].Wrangler.Settings.OffsetMode = CameraPosMode.Absolute; break;
                        case "_offsetrel": FollowCam.objCameras[0].Wrangler.Settings.OffsetMode = CameraPosMode.Relative; break;
                        case "_rotationoff": FollowCam.objCameras[0].Wrangler.Settings.RotationMode = CameraPosMode.Off; break;
                        case "_rotationabs": FollowCam.objCameras[0].Wrangler.Settings.RotationMode = CameraPosMode.Absolute; break;
                        case "_rotationrel": FollowCam.objCameras[0].Wrangler.Settings.RotationMode = CameraPosMode.Relative; break;
                        case "_load":
                            if (File.Exists(text1)) {
                                SettingsFile.Load(text1, FollowCam.objCameras[0].Wrangler);
                                if (int2 == 1) {
                                    FollowCam.objCameras[0].Wrangler.Enable();
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
