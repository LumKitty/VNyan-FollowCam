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

namespace VNyan_FollowCam {

    public class VNyan_Handlers : IVNyanPluginManifest, IButtonClickedHandler, ITriggerHandler {
        public string PluginName { get; } = "VNyan FollowCam";
        public string Version { get; } = "0.5-beta";
        public string Title => PluginName + " " + Version;
        public string Author { get; } = "LumKitty";
        public string Website { get; } = "https://lum.uk/";

        internal static System.Reflection.MethodInfo? _VRnyan_EnableFollowCam;
        internal static System.Reflection.MethodInfo? _VRnyan_DisableFollowCam;
        internal static System.Reflection.MethodInfo? _VRnyan_GetMMF;
        internal static System.Reflection.MethodInfo? _VRnyan_UpdateCursedCamera;
        //internal delegate void __VRnyan_UpdateMMF(Vector3 CamPos, Quaternion CamRot);
        internal static MemoryMappedViewAccessor? mmfAccess = null;
        internal static int VRnyanConnectionAttempts = 0;
        internal static bool VRnyanConnectionSucceeded = false;
        internal static bool VRnyanConnectionActive = false;
        //internal static GameObject DummyMainCamera = new GameObject();

        internal static void Log(string Message, int LogLevel = 1) {
            if (LogLevel <= _Settings.GlobalSettings.LogLevel) {
                UnityEngine.Debug.Log($"[FollowCam] {Message}");
            }
        }

        internal static void ConnectVRnyan() {
            try {
                if (!VRnyanConnectionSucceeded && VRnyanConnectionAttempts < 5) {
                    VRnyanConnectionAttempts++;
                    Log("Looking for VRnyan");
                    var type = Type.GetType("VRnyan.VRnyan, VRnyan", throwOnError: false);
                    if (type != null) {
                        Log("Found VRnyan, getting methods");
                        Log("Connecting: EnableFollowCam");
                        _VRnyan_EnableFollowCam = type.GetMethod("EnableFollowCam", BindingFlags.Static | BindingFlags.Public);
                        Log("Connecting: DisableFollowCam");
                        _VRnyan_DisableFollowCam = type.GetMethod("DisableFollowCam", BindingFlags.Static | BindingFlags.Public);
                        Log("Connecting: UpdateMMF");
                        _VRnyan_GetMMF = type.GetMethod("GetMMF", BindingFlags.Static|BindingFlags.Public);
                        Log("Connecting: UpdateCursedCamera");
                        _VRnyan_UpdateCursedCamera = type.GetMethod("UpdateCursedCamera", BindingFlags.Static | BindingFlags.Public);
                        if (_VRnyan_EnableFollowCam == null || _VRnyan_DisableFollowCam == null || _VRnyan_GetMMF == null || _VRnyan_UpdateCursedCamera == null) {
                            Log("Couldn't find position methods");
                            Log("EnableFollowCam: "+_VRnyan_EnableFollowCam.ToString());
                            Log("DisableFollowCam: " + _VRnyan_DisableFollowCam.ToString());
                            Log("GetMMF: " + _VRnyan_GetMMF.ToString());
                            Log("UpdateCursedCamera: " + _VRnyan_UpdateCursedCamera.ToString());
                        } else {
                            Log("Got methods - Grabbing MMF");
                            mmfAccess = (MemoryMappedViewAccessor)_VRnyan_GetMMF?.Invoke(null, null);
                            VRnyanConnectionSucceeded = true;
                        }
                    } else {
                        Log("Did not find followcam assembly");
                    }
                }
            } catch (Exception ex) {
                Log("ERR: " + ex.ToString());
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
            //FollowCam.RunTimerAsync(new System.Threading.CancellationToken());
            FollowCam.objFollowCam.SetActive(true);
        }

        internal static void VRnyan_EnableFollowCam() {
            Log("Followcam Enabled");
            _VRnyan_EnableFollowCam?.Invoke(null, null);
            //CursedCamera = TempCursedCamera;
            //FollowCam.objCameras[0].Wrangler.CurrentCamera = DummyMainCamera.transform;
            VRnyanConnectionActive = true;
        }
        internal static void VRnyan_DisableFollowCam() {
            Log("Followcam Disabled");
            _VRnyan_DisableFollowCam?.Invoke(null, null);
            VRnyanConnectionActive = false;
            //FollowCam.objCameras[0].Wrangler.CurrentCamera = Camera.main.transform;
        }

        internal static void VRnyan_UpdateCursedCamera(Vector3 CamPos, Quaternion CamRot) {
            _VRnyan_UpdateCursedCamera?.Invoke(null, new object[] {CamPos, CamRot});
        }

        internal static void UpdateMMF(Vector3 CamPos, Quaternion CamRot) {
            
            if (mmfAccess != null) {
                Log("Remote UpdateMMF called",4);
                mmfAccess.Write(SharedValues.MMFPos_CamPosX, CamPos.x);
                mmfAccess.Write(SharedValues.MMFPos_CamPosY, CamPos.y);
                mmfAccess.Write(SharedValues.MMFPos_CamPosZ, CamPos.z);
                mmfAccess.Write(SharedValues.MMFPos_CamRotW, CamRot.w);
                mmfAccess.Write(SharedValues.MMFPos_CamRotX, CamRot.x);
                mmfAccess.Write(SharedValues.MMFPos_CamRotY, CamRot.y);
                mmfAccess.Write(SharedValues.MMFPos_CamRotZ, CamRot.z);
                mmfAccess.Write(SharedValues.MMFPos_CamFOV, Camera.main.fieldOfView);
            } else {
                Log("Remote UpdateMMF called - but we didn't have a connection");
            }
            //VRnyan_UpdateMMF(CamPos.x, CamPos.y, CamPos.z, CamRot.w, CamRot.x, CamRot.y, CamRot.z);
            //_VRnyan_UpdateMMF?.Invoke(null, new object[]{CamPos, CamRot});
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