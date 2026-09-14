using System;
using System.IO;
using System.Xml.Linq;
using VNyanInterface;
using UnityEngine;
using System.Collections.Generic;
using static VNyan_FollowCam._Settings;
using System.Runtime.CompilerServices;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using static VNyan_FollowCam.Functions;

namespace VNyan_FollowCam {

    public class VNyan_Handlers : IVNyanPluginManifest, IButtonClickedHandler, ITriggerHandler {
        public string PluginName { get; } = "VNyan FollowCam";
        public string Version { get; } = "0.9-beta";
        public string Title => PluginName + " " + Version;
        public string Author { get; } = "LumKitty";
        public string Website { get; } = "https://lum.uk/";

        //internal static MemoryMappedViewAccessor? mmfAccess = null;

        public void InitializePlugin() {
            SettingsFile.LoadGlobal();
            GUI.SetActive(false);
            FollowCam.objCameras.Add(new MainCamera(_Settings.GlobalSettings.MainCameraSettingsFile));
            SettingsFile.CreateCamerasFromGlobal();
            //GUI.CurrentWrangler = FollowCam.objCameras[0].Wrangler;
            VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton("FollowCam", this);
            VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(this);
            Log($"Assembly name: {typeof(FollowCam).AssemblyQualifiedName}");
            VRnyan_Handlers.ConnectVRnyan();
            //FollowCam.RunTimerAsync(new System.Threading.CancellationToken());
            FollowCam.objFollowCam.SetActive(true);
            FollowCam.NewFPS();
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
                        case "_enable": FollowCam.objCameras[0].Enable(); break;
                        case "_disable": FollowCam.objCameras[0].Disable(); break;
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
                                    FollowCam.objCameras[0].Enable();
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